using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveInimigo : MonoBehaviour
{
    Rigidbody2D _rig2d;
    [SerializeField] float _speed;
    public Transform _direcao;
    public GameObject _player;  // alterado de Transform
    public float _displayer;
    public float _distanSeguir;
    public Transform[] _pos;
    Animator _anim;
    public bool _andando;
    [SerializeField] Vector2 direcao;

    // teste hp
    [SerializeField] int HP = 3;
    // controle waypoint por list
    [SerializeField] int listPos = 0;
    [SerializeField] bool _seguindoPlayer = false;

    // teste -> parar de seguir player se o player morrer
    [SerializeField] bool PlayerAlive = true;

    // teste: drop de hp
    public GameObject lifePrefab;  // Prefab da vida a ser dropada

    // Campo de visão
    [SerializeField] float viewRadius = 5f;  // Alcance da visão
    [SerializeField] float viewAngle = 90f;  // Ângulo da visão
    public Transform visionOrigin; // O GameObject filho
    
    NavMeshAgent agent;  // teste: seguir player desviando automaticamente dos obstáculos

    void Start()
    {
        _rig2d = GetComponent<Rigidbody2D>();
        _direcao = _pos[0];
        _anim = GetComponent<Animator>();
        /*
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false; */
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
                //agent.SetDestination(_direcao.position);
                _seguindoPlayer = true;
            }
        }
        else if (_seguindoPlayer)
        {
            _seguindoPlayer = false;
            _direcao = _pos[0];
            //agent.SetDestination(_pos[0].position);
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

        //agent.SetDestination(direcao);
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
                _anim.SetBool("Morto", true);
                Destroy(gameObject);
                // Dropar o item de vida na posição do inimigo
                Instantiate(lifePrefab, transform.position, Quaternion.identity);
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
                //Debug.Log("Player detectado!");
                return true; 
            }
        }
        return false; // Player está fora do campo de visão
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
