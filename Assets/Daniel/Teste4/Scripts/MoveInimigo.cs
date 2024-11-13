using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveInimigo : MonoBehaviour
{
    [Header("Variáveis de controle personalizáveis")]
    [SerializeField] int HP = 3;
    [SerializeField] float _speed = 3;
    [SerializeField] float _distanSeguir;
    [SerializeField] float viewRadius = 7f;  // Alcance da visão
    [SerializeField] float viewAngle = 135f;  // Ângulo da visão
    [SerializeField] bool Boss = false;

    [Header("Variáveis de controle gerais")]
    public Transform _direcao;
    public GameObject _player;  
    public float _displayer;
    public GameObject lifePrefab;  // Prefab da vida a ser dropada
    public GameObject treePrefab;  // Prefab da árvore a ser spawnada
    public Transform[] _pos;
    public Transform visionOrigin; // O GameObject filho
    


    // Demais variáveis
    Rigidbody2D _rig2d;
    Animator _anim;
    bool _andando;
    Vector2 direcao;
    bool _seguindoPlayer = false;
    int listPos = 0;
    bool PlayerAlive = true;
    CapsuleCollider2D _collider;
    GameController gameController;


    void Start()
    {
        _rig2d = GetComponent<Rigidbody2D>();
        _direcao = _pos[0];
        _anim = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider2D>();
        gameController = Camera.main.GetComponent<GameController>();

        if (Boss)
        {
            viewRadius = _distanSeguir = 12;
            HP = 10;
        }
    }

    void Update()
    {
        if (!_player.gameObject.activeSelf)
        {
            PlayerAlive = false;
        }

        // Detecta se o player está no campo de visão
        if (PlayerInSight() && PlayerAlive)
        {
            _displayer = Vector2.Distance(transform.position, _player.transform.position);
            if (_displayer <= _distanSeguir)
            {
                _direcao = _player.transform;
                _seguindoPlayer = true;
            }
        }
        else if (_seguindoPlayer)
        {
            _seguindoPlayer = false;
            _direcao = _pos[0];
            listPos = 0;
        }

        // Se não houver obstáculo, mover na direção normal
        if (_direcao != null)
        {
            direcao = (_direcao.position - transform.position).normalized;
        }

        // Controle de animação
        _andando = (direcao.x != 0 || direcao.y != 0);
        if (_andando)
        {
            _anim.SetFloat("Horizontal", direcao.x);
            _anim.SetFloat("Vertical", direcao.y);
        }
        _anim.SetBool("Andando", _andando);
    }

    void FixedUpdate()
    {
        _rig2d.MovePosition(_rig2d.position + direcao * _speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Waypoint"))
        {
            MudaDirecao();
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            MudaDirecao();
        }
    }

    void MudaDirecao()
    {
        if (!_seguindoPlayer)
        {
            if (_direcao == _pos[_pos.Length - 1])
            {
                listPos = 0;
                _direcao = _pos[0];
            }
            else
            {
                listPos += 1;
                _direcao = _pos[listPos];
            }
        }
    }

    // Controle de hp simples
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            HP -= 1;
            if (HP == 0)
            {
                gameController.inimigosDerrotados += 1;

                _anim.SetBool("Morto", true);
                Destroy(gameObject);
                
                Instantiate(lifePrefab, transform.position, Quaternion.identity);
                Instantiate(treePrefab, transform.position, Quaternion.identity);
            }
        }
    }

    // Função para verificar se o player está no campo de visão
    bool PlayerInSight()
    {
        Vector2 directionToPlayer = (_player.transform.position - transform.position).normalized;

        // Verifica se o player está dentro do ângulo de visão com base na direção atual
        if (Vector2.Angle(direcao, directionToPlayer) < viewAngle / 2)
        {
            // Usa uma camada específica para garantir que só o player seja detectado
            LayerMask mask = LayerMask.GetMask("Player");  

            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, viewRadius, mask);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                return true;  // Player detectado
            }
        }
        return false;  // Player está fora do campo de visão
    }

    void OnDrawGizmos()
    {
        // Define a cor do círculo de visão
        Gizmos.color = Color.yellow;

        // Desenha o círculo de visão
        DrawCircle(visionOrigin.position, viewRadius);

        // Define a cor do cone de visão
        Gizmos.color = Color.red; 

        // Calcula a direção dos limites do campo de visão com base na direção do inimigo
        Vector2 rightBoundary = DirFromAngle(-viewAngle / 2, false);  // false para usar a direção local (não global)
        Vector2 leftBoundary = DirFromAngle(viewAngle / 2, false);

        // Desenha as linhas que formam o cone de visão
        Gizmos.DrawLine(visionOrigin.position, (Vector2)visionOrigin.position + rightBoundary * viewRadius);
        Gizmos.DrawLine(visionOrigin.position, (Vector2)visionOrigin.position + leftBoundary * viewRadius);
    }

    void DrawCircle(Vector2 center, float radius)
    {
        int segments = 50; // Número de segmentos para o círculo
        float angle = 0f;

        Vector2 lastPoint = center + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        for (int i = 1; i <= segments; i++)
        {
            angle += 2 * Mathf.PI / segments;
            Vector2 newPoint = center + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
        }
    }


    Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            // Adiciona o ângulo da direção atual (onde o inimigo está olhando)
            angleInDegrees += Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        }
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad));
    }
}
