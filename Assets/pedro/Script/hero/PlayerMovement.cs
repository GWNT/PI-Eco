using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]    private     Rigidbody2D rb;
    public              float       moveSpeed = 5f;
    public              Animator    animator;

    private Vector2 movement;

    void Update()
    {
        // Captura o input de movimento
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Atualiza os parâmetros do Animator para movimento
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Atualiza os parâmetros para a última direção de movimento
        if (movement.x != 0 || movement.y != 0)
        {
            animator.SetFloat("LastHorizontal", movement.x);
            animator.SetFloat("LastVertical", movement.y);
        }

        movement = movement.normalized;

        // Verifica se o botão de ataque foi pressionado
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }

        Debug.Log($"Movement: {movement}, Speed: {movement.sqrMagnitude}");
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void Attack()
    {
        // Ativa o trigger de ataque
        animator.SetTrigger("Attack");
    }
}


