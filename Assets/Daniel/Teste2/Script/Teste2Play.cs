using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class Teste2Play : MonoBehaviour
{
    [Header("Variáveis do Player")]
    [SerializeField] private float _speed;
    [SerializeField] private Vector2 _move;
    [SerializeField] private Vector2 _lastMoveDirection; // armazena a última direção válida
    [SerializeField] private Vector2 _storedMove; // armazena a entrada de movimento enquanto o movimento está bloqueado
    [SerializeField] private float knockbackForce = 20f;

    private Animator _anim;
    private Rigidbody2D _rb;
    private SpriteRenderer spriteRenderer;
    private bool _andando;
    private bool _canMove = true;
    private bool _canShoot = true; 
    private float _attackAnimationDuration = 0.4f; 
    private bool isInvulnerable = false;
    private float invulnerableDuration = 1f;
    private int numberOfFlashes = 10;
    private string MoveInput;  // evitar que o player se mova sozinho através de uma verificação

    // Script que controla a Life na HUD
    LifeControl LifeScript;
    // Game Controller
    GameController gameController;

    ArrowCollision arrowCollisionScript;

    [Header("Variáveis da Flecha")]
    [SerializeField] private GameObject _arrowPrefab; 
    [SerializeField] private Transform _arrowSpawnPoint; 
    [SerializeField] private float _arrowSpeed = 10f; 
    [SerializeField] private float _shootCooldown = 0.5f; 
    [SerializeField] private Vector2 shootDirection;


    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();

        LifeScript = Camera.main.GetComponent<LifeControl>();
        gameController = Camera.main.GetComponent<GameController>();
        arrowCollisionScript = GameObject.FindObjectOfType<ArrowCollision>();
    }

    void Update()
    {
        AnimationController();  // Controle de animação
        
        if (gameController.pacifico == false)
        {
            LifeScript.CheckMorte();  // Verifica se o player morreu
        }
    }

    void FixedUpdate()
    {
        if (_canMove)
        {
            _rb.MovePosition(_rb.position + _move.normalized * _speed * Time.fixedDeltaTime);  // move o personagem
        }
    }

    private void AnimationController()
    {
        if (_canMove)
        { 
            _andando = _move.x != 0 || _move.y != 0;

            if (_andando)
            {
                _lastMoveDirection = _move.normalized; // atualiza a última direção válida
                _anim.SetFloat("Horizontal", _move.x);
                _anim.SetFloat("Vertical", _move.y);
            }
            _anim.SetBool("Andando", _andando);
        }
    }

    public void SetMove(InputAction.CallbackContext value)
    {
        // Verifica se o input é válido e se está no estado de "performed"
        if (value.action != null && value.performed)
        {
            MoveInput = "performed";
            _storedMove = value.ReadValue<Vector3>().normalized; // Armazena o movimento temporariamente

            if (_canMove)
            {
                _move = _storedMove; // Atualiza o movimento somente se permitido
            }
        }

        // Quando o input for "canceled", paramos o movimento
        if (value.canceled)
        {
            MoveInput = "canceled";
            _move = Vector2.zero;
        }
    }

    public void SetAttack(InputAction.CallbackContext value)
    {
        if (PauseMenu.GameIsPaused == true || gameController.GameStarted == false) 
            return;
        
        if (value.performed && _canShoot && !IsAnimationPlaying("Attack"))
        {
            _anim.SetBool("Atacando", true); 
            _move = Vector2.zero; // força o movimento a zero ao iniciar o ataque
            ShootArrow();
            StartCoroutine(ShootCooldown()); 
            StartCoroutine(DisableMovementDuringAttack());
            gameController.disparoFlecha.Play();
        }
    }

    void ShootArrow()
    {
        GameObject arrow = BalaPool.SharedInstance.GetPooledObject();
        if (arrow != null)
        {
            arrow.transform.position = _arrowSpawnPoint.position;
            arrow.transform.rotation = Quaternion.identity;
            arrow.SetActive(true);
        }
        

        shootDirection = _move == Vector2.zero ? _lastMoveDirection : _move.normalized;

        if (shootDirection == Vector2.zero)
        {
            shootDirection = new Vector2(0, -1);
        }

        if (Mathf.Abs(shootDirection.x) > Mathf.Abs(shootDirection.y))
        {
            shootDirection = new Vector2(Mathf.Sign(shootDirection.x), 0); // dispara para os lados
        }
        else
        {
            shootDirection = new Vector2(0, Mathf.Sign(shootDirection.y)); // dispara para cima ou baixo
        }


        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        if (shootDirection.x == 0 && shootDirection.y > 0)
        {
            angle = 0;
        }
        else if (shootDirection.x == 0 && shootDirection.y < 0)
        {
            angle = 180;
        }
        else if (shootDirection.x > 0 && shootDirection.y == 0)
        {
            angle = -90;
        }
        else if (shootDirection.x < 0 && shootDirection.y == 0)
        {
            angle = 90;
        }

        arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        arrowRb.velocity = shootDirection * _arrowSpeed;
    }

    private IEnumerator ShootCooldown()
    {
        _canShoot = false;
        yield return new WaitForSeconds(_shootCooldown);
        _canShoot = true;
    }

    private bool IsAnimationPlaying(string animationName)
    {
        return _anim.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    private IEnumerator DisableMovementDuringAttack()
    {
        _canMove = false;
        _rb.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(_attackAnimationDuration);
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _canMove = true;
        _anim.SetBool("Atacando", false);

        if(MoveInput == "performed")
        {
            _move = _storedMove;
        } else if (MoveInput == "canceled")
        {
            _move = Vector2.zero;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Colidiu com " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (gameController.pacifico) return;

            if (!isInvulnerable)
            {
                LifeScript.PerderVida(); 
            }

            // Pega as posições do player e do inimigo
            Vector2 playerPosition = transform.position;
            Vector2 enemyPosition = collision.transform.position;

            // Calcula a direção oposta à do inimigo
            Vector2 knockbackDirection = (playerPosition - enemyPosition).normalized;

            // Garante que o knockbackDirection não seja zero
            if (knockbackDirection == Vector2.zero)
            {
                knockbackDirection = Vector2.up;
            }

            // Aplica a força de knockback imediatamente
            _rb.velocity = knockbackDirection * knockbackForce;

            // Bloqueia temporariamente o movimento após o knockback
            StartCoroutine(TemporaryBlockMovement());  // aparentemente precisa disso para knockback funcionar corretamente
            // Deixa o player temporariamente invulnerável
            StartCoroutine(BecomeTemporarilyInvulnerable());
        }
    }

    private IEnumerator TemporaryBlockMovement()
    {
        _canMove = false;  // Impede o movimento temporariamente

        yield return new WaitForSeconds(0.1f);

        _canMove = true;   // Libera o movimento novamente
        _rb.velocity = Vector2.zero;  // Reseta a velocidade para evitar movimento indesejado

        if (MoveInput == "performed")
        {
            _move = _storedMove;
        }
        else if (MoveInput == "canceled")
        {
            _move = Vector2.zero;
        }
    }

    private IEnumerator BecomeTemporarilyInvulnerable()
    {
        isInvulnerable = true;

        // Faz o player ficar piscando
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(invulnerableDuration / (numberOfFlashes * 2));

            spriteRenderer.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(invulnerableDuration / (numberOfFlashes * 2));
        }
        
        isInvulnerable = false;
    }
}
