using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveInimigo : MonoBehaviour
{
    [Header("Variáveis de controle personalizáveis")]
    [SerializeField] private int HP = 3;
    [SerializeField] private float _speed = 3;
    [SerializeField] private float _distanSeguir = 7f;
    [SerializeField] private float viewRadius = 7f;  // Alcance da visão
    [SerializeField] private float viewAngle = 135f;  // Ângulo da visão
    [SerializeField] private bool Boss = false;
    [SerializeField] private bool BossAtivo = false;

    [Header("Variáveis de controle gerais")]
    public Transform _direcaoSeguindo;
    public GameObject _player;  
    public float _displayer;
    public GameObject lifePrefab;  // Prefab da vida a ser dropada
    public GameObject treePrefab;  // Prefab da árvore a ser spawnada
    public Transform[] _pos;
    public Transform visionOrigin; // O GameObject filho
    public Transform BossStart;  // local onde o boss vai aparecer

    // Demais variáveis
    private Rigidbody2D _rig2d;
    private Animator _anim;
    private bool _andando;
    private Vector2 movimento;
    private bool _seguindoPlayer = false;
    private int listPos = 0;
    private bool PlayerAlive = true;
    private CapsuleCollider2D _collider;
    private GameController gameController;


    void Start()
    {
        _rig2d = GetComponent<Rigidbody2D>();
        _direcaoSeguindo = _pos[0];
        _anim = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider2D>();
        gameController = Camera.main.GetComponent<GameController>();

        if (Boss)
        {
            viewRadius = _distanSeguir = 12;
            HP = 10;
            _collider.enabled = false;
        }
    }

    void Update()
    {
        if (_player.gameObject.activeSelf == false)
        {
            PlayerAlive = false;
        }

        SelectTarget();  // Define se o inimigo vai seguir o jogador ou um dos waypoints

        if(Boss && !BossAtivo)
        {
            _speed = 0;
            if(gameController.purificouTodosInimigos)
            {
                _speed = 3;
                _direcaoSeguindo = BossStart;
            }
        }

        // Se não houver obstáculo, mover na direção normal
        if (_direcaoSeguindo != null)
        {
            movimento = (_direcaoSeguindo.position - transform.position).normalized;
        }

        // Controle de animação
        AnimationController();

        // if(Boss && !BossAtivo) return;  
        StartCoroutine(VerificaPreso());  // 
    }

    void FixedUpdate()
    {
        _rig2d.MovePosition(_rig2d.position + movimento * _speed * Time.fixedDeltaTime);
    }

    private void AnimationController()
    {
        _andando = movimento.x != 0 || movimento.y != 0;
        if (_andando)
        {
            _anim.SetFloat("Horizontal", movimento.x);
            _anim.SetFloat("Vertical", movimento.y);
        }
        _anim.SetBool("Andando", _andando);
    }

    private void SelectTarget()
    {
        // Detecta se o player está no campo de visão
        if (PlayerInSight() && PlayerAlive)
        {
            _displayer = Vector2.Distance(transform.position, _player.transform.position);
            if (_displayer <= _distanSeguir && !gameController.pacifico)
            {
                _direcaoSeguindo = _player.transform;
                _seguindoPlayer = true;
            }
        }
        else if (_seguindoPlayer)
        {
            _seguindoPlayer = false;
            _direcaoSeguindo = _pos[0];
            listPos = 0;
        }
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
        
        if(Boss && collision.gameObject.name == "BossStart")
        {
            Debug.Log("Boss chegou.");
            BossAtivo = true;
            _collider.enabled = true;
            listPos = 0;
            MudaDirecao();
        }
    }

    void MudaDirecao()
    {
        if (!_seguindoPlayer)
        {
            if (_direcaoSeguindo == _pos[_pos.Length - 1])
            {
                listPos = 0;
                _direcaoSeguindo = _pos[0];
            }
            else
            {
                listPos += 1;
                _direcaoSeguindo = _pos[listPos];
            }
        }
    }

    // Controle de hp simples
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            HP -= 1;

            _displayer = Vector2.Distance(transform.position, _player.transform.position);
            if (!_seguindoPlayer && (_displayer <= _distanSeguir))
            {
                if (!gameController.pacifico)
                {
                    _direcaoSeguindo = _player.transform;
                    _seguindoPlayer = true;
                }
                
            }

            if (HP == 0)
            {
                gameController.inimigosDerrotados += 1;
                gameController.inimigoPurificado.Play();

                if(Boss)
                {
                    gameController.bossDerrotado = true;
                }

                _anim.SetBool("Morto", true);
                Destroy(gameObject);

                Instantiate(treePrefab, transform.position, Quaternion.identity);

                if (!gameController.pacifico)
                {
                    Instantiate(lifePrefab, transform.position, Quaternion.identity);
                }
            }
        }
    }

    // Função para verificar se o player está no campo de visão
    bool PlayerInSight()
    {
        Vector2 directionToPlayer = (_player.transform.position - transform.position).normalized;

        // Verifica se o player está dentro do ângulo de visão com base na direção atual
        if (Vector2.Angle(movimento, directionToPlayer) < viewAngle / 2)
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
            angleInDegrees += Mathf.Atan2(movimento.y, movimento.x) * Mathf.Rad2Deg;
        }
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad));
    }

    IEnumerator VerificaPreso()  // função pra verificar se o mob está "preso" em alguma parte do cenário
    {
        Vector2 firstPosition = transform.position;
        yield return new WaitForSeconds(2);
        Vector2 atualPosition = transform.position;

        if (firstPosition == atualPosition)
        {
            MudaDirecao();
        }

        firstPosition = atualPosition = Vector2.zero;
        yield return new WaitForSeconds(1);
    }
}
