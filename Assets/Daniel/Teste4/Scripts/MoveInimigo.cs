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

    bool _checkLoop;
    void Start()
    {
        _rig2d = GetComponent<Rigidbody2D>();
        _direcao = _pos[0];
    }

    // Update is called once per frame
    void FixedUpdate()
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



        Vector2 direcao = (_direcao.position - transform.position).normalized;

        _rig2d.MovePosition(_rig2d.position + direcao * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "pos1")
        {
            Debug.Log("Pos1");
            _direcao = _pos[1];
        }
        else if (collision.gameObject.name == "pos2")
        {
            Debug.Log("Pos2");
            _direcao = _pos[0];
        }
    }
}
