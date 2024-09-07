using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Teste2Play : MonoBehaviour
{
    // Variaveis do player
    public Animator _anim;
    public float _speed;
    Rigidbody2D _rb;
    [SerializeField] Vector2 _move;
    private Vector2 _lastMoveDirection; // armazena a última direção válida
    public bool _andando;

    // Variaveis da flecha
    public GameObject _arrowPrefab; // Prefab da flecha
    public Transform _arrowSpawnPoint; // ponto de spanw da flecha
    public float _arrowSpeed = 10f; // velocidade da flecha (acho que tá obvio pelo nome, mas né...)

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Debug.Log("Special thanks to ChatGPT ;)");
        Debug.Log("Botão esquerdo do mouse para disparar flechas.");
    }

    // Update is called once per frame
    void Update()
    {
        _andando = (_move.x != 0 || _move.y != 0);

        if(_andando)
        {
            _lastMoveDirection = _move.normalized; // atualiza a última direção válida
            _anim.SetFloat("Horizontal", _move.x);
            _anim.SetFloat("Vertical", _move.y);
        }
        _anim.SetBool("Andando", _andando);
    }

    void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _move * _speed * Time.fixedDeltaTime);
    }

    public void SetMove(InputAction.CallbackContext value)
    {
        _move = value.ReadValue<Vector3>();
    }

    public void SetAttack(InputAction.CallbackContext value)
    {
        if(value.performed)
        {
            _anim.SetTrigger("Attack");
            ShootArrow();
        }
    }

    void ShootArrow()
    {
        // Instancia a flecha na posição do ponto de spawn
        GameObject arrow = Instantiate(_arrowPrefab, _arrowSpawnPoint.position, Quaternion.identity);

        // Calcula a direção da flecha com base no movimento atual do player
        Vector2 shootDirection = _move == Vector2.zero ? _lastMoveDirection : _move.normalized;

        // remover disparos na diagonal
        if (Mathf.Abs(shootDirection.x) > Mathf.Abs(shootDirection.y))
        {
            // Se o movimento no eixo X for maior que no Y, travamos no eixo X
            shootDirection = new Vector2(Mathf.Sign(shootDirection.x), 0); // dispara para os lados
        }
        else
        {
            // Se o movimento no eixo Y for maior ou igual, travamos no eixo Y
            shootDirection = new Vector2(0, Mathf.Sign(shootDirection.y)); // dispara pra cima ou baixo
        }
        

        // Rotaciona a flecha na direção do movimento
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        // ajuste de rotação da flecha
        if (shootDirection.x == 0 && shootDirection.y > 0) // Flecha apontando para cima
        {
            angle = 0;
        }
        else if(shootDirection.x == 0 && shootDirection.y < 0) // Flecha apontando para baixo
        {
            angle = 180;
        }
        else if(shootDirection.x > 0 && shootDirection.y == 0) // Flecha apontando para a direita
        {
            angle = -90;
        }
        else if(shootDirection.x < 0 && shootDirection.y == 0) // Flecha apontando para a esquerda
        {
            angle = 90;
        }

        arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Aplica a força para mover a flecha
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        arrowRb.velocity = shootDirection * _arrowSpeed;
    }
}
