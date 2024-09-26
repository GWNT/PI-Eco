using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInimigo : MonoBehaviour
{
    Rigidbody2D _rig2d;
    [SerializeField] float _speed;
    public Transform _direcao;
    public Transform _player;
    public float _displayer;
    public float _distanSeguir;
    public Transform[] _pos;
    Animator _anim;
    public bool _andando;
    [SerializeField] Vector2 direcao;

    // teste hp
    [SerializeField] int HP = 3;

    bool _checkLoop;
    void Start()
    {
        _rig2d = GetComponent<Rigidbody2D>();
        _direcao = _pos[0];
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _displayer = Vector2.Distance(transform.position, _player.position);
        if (_displayer <= _distanSeguir)
        {
            _direcao = _player;
            _checkLoop = true;
        }
        else if (_checkLoop == true)
        {
            _checkLoop = false;
            _direcao = _pos[1];
        }

        direcao = (_direcao.position - transform.position).normalized;

        //teste controle animator
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
        if (collision.gameObject.name == "Pos1")
        {
            Debug.Log("Pos1");
            _direcao = _pos[1];
        }
        else if (collision.gameObject.name == "Pos2")
        {
            Debug.Log("Pos2");
            _direcao = _pos[0];
        }
    }
    // teste hp simples
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            Debug.Log("hit");
            HP -= 1;
            if (HP == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
