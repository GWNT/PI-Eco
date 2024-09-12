using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f; // Velocidade do movimento
    [SerializeField] private float moveDistance = 5.0f; // Dist�ncia total que o inimigo percorrer�

    private Vector2 startPosition; // Posi��o inicial do inimigo
    private Vector2 targetPosition; // Posi��o alvo para o movimento
    private bool movingRight = true; // Dire��o atual do movimento horizontal

    void Start()
    {
        // Armazena a posi��o inicial
        startPosition = transform.position;
        // Define a posi��o alvo inicial para a direita
        targetPosition = startPosition + Vector2.right * moveDistance;
    }

    void Update()
    {
        // Move o inimigo em dire��o � posi��o alvo
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Verifica se o inimigo chegou � posi��o alvo
        if ((Vector2)transform.position == targetPosition)
        {
            // Inverte a dire��o do movimento horizontal
            movingRight = !movingRight;
            // Atualiza a posi��o alvo
            targetPosition = movingRight
                ? startPosition + Vector2.right * moveDistance
                : startPosition - Vector2.right * moveDistance;
        }
    }
}
