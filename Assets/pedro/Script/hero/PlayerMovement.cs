using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField]    Rigidbody2D rb;

    public float        moveSpeed = 5f;

    public Animator     animator;

    Vector2             movement;
    
    
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if(Input.GetAxisRaw("Horizontal")==1 || Input.GetAxisRaw("Horizontal")==-1 || Input.GetAxisRaw("Vertical")==1 || Input.GetAxisRaw("Vertical")==-1)
        {
            animator.SetFloat("LastHorizontal", Input.GetAxisRaw("Horizontal"));
            animator.SetFloat("LastVertical", Input.GetAxisRaw("Vertical"));
        }

        movement = movement.normalized;

        Debug.Log($"Movement: {movement}, Speed: {movement.sqrMagnitude}");

        
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

    }
}
