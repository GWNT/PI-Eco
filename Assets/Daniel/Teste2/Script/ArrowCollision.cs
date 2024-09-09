using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Flecha colidiu com : " + collision.gameObject.name);
        if(collision.gameObject.CompareTag("Player") == false)
        {
            Destroy(gameObject);
        }
    }
}
