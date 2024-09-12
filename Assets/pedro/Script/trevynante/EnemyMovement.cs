using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f; // Velocidade do movimento
    [SerializeField] private float moveDistance = 5.0f; // Distância total que o inimigo percorrerá

    private Vector2 startPosition; // Posição inicial do inimigo
    private Vector2 targetPosition; // Posição alvo para o movimento
    private bool movingRight = true; // Direção atual do movimento horizontal

    void Start()
    {
        // Armazena a posição inicial
        startPosition = transform.position;
        // Define a posição alvo inicial para a direita
        targetPosition = startPosition + Vector2.right * moveDistance;
    }

    void Update()
    {
        // Move o inimigo em direção à posição alvo
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Verifica se o inimigo chegou à posição alvo
        if ((Vector2)transform.position == targetPosition)
        {
            // Inverte a direção do movimento horizontal
            movingRight = !movingRight;
            // Atualiza a posição alvo
            targetPosition = movingRight
                ? startPosition + Vector2.right * moveDistance
                : startPosition - Vector2.right * moveDistance;
        }
    }
}
