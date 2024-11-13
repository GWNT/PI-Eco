using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSorting : MonoBehaviour
{
    private Transform player;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Encontra o player pela tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player não encontrado na cena!");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Ajusta o sortingOrder com base na posição do jogador
        if (player.position.y > transform.position.y)
        {
            spriteRenderer.sortingOrder = 1;
        }
        else
        {
            spriteRenderer.sortingOrder = -1;
        }
    }
}
