using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetSpeed(float speed)
    {
        rb.velocity = transform.right * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Flecha colidiu com: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("ArrowCollision") || collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);    
        }
    }
}
