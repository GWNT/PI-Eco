using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        _rig2d = GetComponent<Rigidbody2D>();
        _direcao = _pos[0];
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        
        if (!_player.gameObject.activeSelf)
        {
            PlayerAlive = false;
        }  
        
        _displayer = Vector2.Distance(transform.position, _player.transform.position);
        if (_displayer <= _distanSeguir && PlayerAlive)
        {
            _direcao = _player.transform;
            _seguindoPlayer = true;
        } 
        else if (_seguindoPlayer == true)
        {
            _seguindoPlayer = false;
            _direcao = _pos[0];
            listPos = 0;
            //Debug.Log("Parando de seguir o player - " + gameObject.name);
        } 

        direcao = (_direcao.position - transform.position).normalized;

        // controle animator
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
        else if(collision.gameObject.CompareTag("Enemy"))
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
    
    // controle de hp simples
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            HP -= 1;
            if (HP == 0)
            {
                Destroy(gameObject);
                // Dropar o item de vida na posição do inimigo
                Instantiate(lifePrefab, transform.position, Quaternion.identity);
            }
        } 
    }
}
