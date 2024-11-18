using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollision : MonoBehaviour
{
    BoxCollider2D collider;

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Flecha colidiu com : " + collision.gameObject.name);
        if(collision.gameObject.CompareTag("ArrowCollision") || collision.gameObject.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);
        }
    }
    
    public void AtivarColisao()
    {
        collider.enabled = true;
    }
}
