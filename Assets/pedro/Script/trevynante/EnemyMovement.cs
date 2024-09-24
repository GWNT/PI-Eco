using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rig2d;

    [SerializeField] float       _speed; // Velocidade de movimento do inimigo

    public Transform             _direcao; // Referência ao Transform do jogador

    public Transform[]           _pos;


    void Start()
    {
        _rig2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Calcule a direção para o jogador
        Vector2 direcao = (_direcao.position - transform.position).normalized;


        // Mova o inimigo
        _rig2d.MovePosition(_rig2d.position + direcao * _speed * Time.deltaTime);

            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "pos1")
        {

        }
        else if (collision.gameObject.name == "pos2")
        {
            Debug.Log("pos2");
        }
        
    }



}
