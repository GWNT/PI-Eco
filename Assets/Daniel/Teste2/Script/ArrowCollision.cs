using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollision : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private Coroutine activationCoroutine;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false; // Sempre inicia desativado.
    }

    void OnEnable()
    {
        // Quando o objeto é ativado no pool, desativa o collider por padrão.
        boxCollider.enabled = false;
    }

    public void ActivateCollisionWithDelay(float delay)
    {
        // Cancela qualquer coroutine anterior.
        if (activationCoroutine != null)
        {
            StopCoroutine(activationCoroutine);
        }
        // Inicia uma coroutine para ativar a colisão após o delay.
        activationCoroutine = StartCoroutine(ActivateCollisionCoroutine(delay));
    }

    private IEnumerator ActivateCollisionCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        boxCollider.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Flecha colidiu com : " + collision.gameObject.name);
        if(collision.gameObject.CompareTag("ArrowCollision") || collision.gameObject.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);
        }
    }
}
