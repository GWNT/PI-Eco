using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class Teste2Play : MonoBehaviour
{
    // Variáveis do player
    public Animator _anim;
    public float _speed;
    Rigidbody2D _rb;
    [SerializeField] Vector2 _move;
    [SerializeField] private Vector2 _lastMoveDirection; // armazena a última direção válida
    public bool _andando;
    private bool _canMove = true; // controla se o player pode se mover
    private Vector2 _storedMove; // armazena a entrada de movimento enquanto o movimento está bloqueado
    public float _attackAnimationDuration = 0.4f; // Tempo fixo para duração da animação de ataque
    public float knockbackForce = 10f;  // Ajuste a força do impulso como preferir
    public bool contato = false; // verifica se o player colide com enemies
    public Collision2D colisao; // teste 

    // Life
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
        Debug.Log("Special thanks to ChatGPT ;)");
        Debug.Log("Botão esquerdo do mouse para disparar flechas.");
    }

    void Update()
    {
        // Se puder se mover, atualiza as animações de movimento
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

        if (contato)
        {
            Knockback(colisao);
        }
        

    }

    void FixedUpdate()
    {
        if (_canMove)
        {
            // Move o player se ele puder se mover
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
        if (value.performed && _canShoot && !IsAnimationPlaying("Attack"))
        {
            _anim.SetTrigger("Attack");
            _anim.SetBool("Atacando", true); // define Atacando como true
            _move = Vector2.zero; // força o movimento a zero ao iniciar o ataque
            ShootArrow();
            StartCoroutine(ShootCooldown()); // inicia o cooldown
            StartCoroutine(DisableMovementDuringAttack()); // desativa o movimento durante o ataque
        }
    }

    void ShootArrow()
    {
        // GameObject arrow = Instantiate(_arrowPrefab, _arrowSpawnPoint.position, Quaternion.identity);
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
        //Debug.Log("Movimento bloqueado");

        yield return new WaitForSeconds(_attackAnimationDuration);

        _canMove = true;
        _move = _storedMove; // restaura o movimento armazenado
        //Debug.Log("Movimento liberado");
        _anim.SetBool("Atacando", false);
    }
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LifeScript.PerderVida();
            Debug.Log("Tomou dano");

            // Calcula a direção oposta à do inimigo
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;

            Debug.Log(knockbackDirection * knockbackForce);

            // Aplica uma força na direção oposta
            //_rb.velocity = knockbackDirection * knockbackForce;
            _rb.MovePosition(_rb.position + knockbackDirection * knockbackForce);
        }
    } */
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LifeScript.PerderVida();
            Debug.Log("Tomou dano");
            knockbackForce = 10f;
            contato = true;

            colisao = collision;
        }
    } 

    void Knockback(Collision2D collision)
    {
        // Pega o ponto de contato da colisão
        Vector2 contactPoint = collision.contacts[0].point;
        Vector2 playerPosition = transform.position;

        // Calcula a direção oposta ao ponto de contato
        Vector2 knockbackDirection = (playerPosition - contactPoint).normalized;

        Debug.Log(knockbackDirection * knockbackForce);

        // Aplica uma força na direção oposta à colisão
        //_rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        _rb.velocity = knockbackDirection * knockbackForce;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            knockbackForce = 0f;
        }
        contato = false;
    }
}
