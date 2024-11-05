using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class Teste2Play : MonoBehaviour
{
    // Variáveis do player
    Animator _anim;
    [SerializeField] float _speed;
    Rigidbody2D _rb;
    [SerializeField] Vector2 _move;
    [SerializeField] private Vector2 _lastMoveDirection; // armazena a última direção válida
    public bool _andando;
    private bool _canMove = true; // controla se o player pode se mover
    private Vector2 _storedMove; // armazena a entrada de movimento enquanto o movimento está bloqueado
    public float _attackAnimationDuration = 0.4f; // Tempo fixo para duração da animação de ataque
    public float knockbackForce = 10f;  // Ajuste a força do impulso como preferir
    public bool isInvulnerable = false;
    public float invulnerableDuration = 0.5f;
    public int numberOfFlashes = 4; // Número de piscadas
    private SpriteRenderer spriteRenderer;

    // Life na HUD
    public LifeControl LifeScript;

    // Variáveis da flecha
    public GameObject _arrowPrefab; // Prefab da flecha
    public Transform _arrowSpawnPoint; // ponto de spawn da flecha
    public float _arrowSpeed = 10f; // velocidade da flecha
    public float _shootCooldown = 0.5f; // intervalo de tempo entre os disparos
    private bool _canShoot = true; // controla se o player pode disparar
    [SerializeField] Vector2 shootDirection; // direção do disparo

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        //Debug.Log("Special thanks to ChatGPT ;)");
        //Debug.Log("Botão esquerdo do mouse ou espaço no teclado para disparar flechas.");
        //Debug.Log("Odeio calvos");
    }

    void Update()
    {
        if (_canMove)
        {
            _andando = (_move.x != 0 || _move.y != 0);

            if (_andando)
            {
                _lastMoveDirection = _move.normalized; // atualiza a última direção válida
                _anim.SetFloat("Horizontal", _move.x);
                _anim.SetFloat("Vertical", _move.y);
            }
            _anim.SetBool("Andando", _andando);
        }

        LifeScript.CheckMorte();
        // se quiser deixar o player imortal para testes, descomentar a linha abaixo
        // LifeScript.GanharVida();  // deixando player regenerar vida continuamente para fazer testes de colisão com inimigos
    }

    void FixedUpdate()
    {
        if (_canMove)
        {
            _rb.MovePosition(_rb.position + _move.normalized * _speed * Time.fixedDeltaTime);
        }
    }

    public void SetMove(InputAction.CallbackContext value)
    {
        // Verifica se o input é válido e se está no estado de "performed"
        if (value.action != null && value.performed)
        {
            _storedMove = value.ReadValue<Vector3>(); // Armazena o movimento temporariamente

            if (_canMove)
            {
                _move = _storedMove; // Atualiza o movimento somente se permitido
            }
        }

        // Quando o input for "canceled", paramos o movimento
        if (value.canceled)
        {
            _move = Vector2.zero;
        }
    }



    public void SetAttack(InputAction.CallbackContext value)
    {
        if (PauseMenu.GameIsPaused == true || TutorialController.GameStarted == false) 
            return;
        
        if (value.performed && _canShoot && !IsAnimationPlaying("Attack"))
        {
            _anim.SetBool("Atacando", true); // define Atacando como true
            _move = Vector2.zero; // força o movimento a zero ao iniciar o ataque
            ShootArrow();
            StartCoroutine(ShootCooldown()); // inicia o cooldown
            StartCoroutine(DisableMovementDuringAttack()); // desativa o movimento durante o ataque
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

        yield return new WaitForSeconds(_attackAnimationDuration);

        _canMove = true;
        _move = _storedMove; // restaura o movimento armazenado
        _anim.SetBool("Atacando", false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Colidiu com " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Enemy"))
        {
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
                knockbackDirection = Vector2.up; // Define uma direção padrão
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

        yield return new WaitForSeconds(0.1f);  // Ajuste o tempo conforme necessário

        _canMove = true;   // Libera o movimento novamente
        _rb.velocity = Vector2.zero;  // Reseta a velocidade para evitar movimento indesejado
        _move = _storedMove;  // restaura o movimento armazenado
    }

    private IEnumerator BecomeTemporarilyInvulnerable()
    {
        isInvulnerable = true;

        // Faz o player ficar piscando
        for (int i = 0; i < numberOfFlashes; i++)
        {
            // Deixa o player parcialmente transparente (50%)
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(invulnerableDuration / (numberOfFlashes * 2));

            // Volta o player para opaco
            spriteRenderer.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(invulnerableDuration / (numberOfFlashes * 2));
        }
        
        isInvulnerable = false;
    }
}
