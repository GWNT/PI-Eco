using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeRegen : MonoBehaviour
{
    public LifeControl LifeHUD;

    void Start()
    {
        // Tenta encontrar o script LifeControl na cena e referenciar automaticamente
        LifeHUD = GameObject.FindObjectOfType<LifeControl>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LifeHUD.GanharVida();

            // Destruir o item de vida
            Destroy(gameObject);
        }
    }
}
