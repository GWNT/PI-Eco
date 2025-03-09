using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayControl : MonoBehaviour
{
    [Header("Player variables")]
    public float moveSpeed = 5f;
    public Vector2 moveInput;
    public bool canMove;
    public bool isWalking = false;
    public bool canShoot = true;
    public bool isShooting = false;

    [Header("References")]
    public Rigidbody2D rb;
    public Animator animator;
    public Teste2Play oldScript;

    [Header("Arrow variables")]
    public Transform firePoint;
    public GameObject arrowPrefab;
    public float arrowSpeed = 10f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        oldScript = GetComponent<Teste2Play>();
        animator = GetComponent<Animator>();

        canShoot = true;
        canMove = true;
    }


    void Update()
    {
        oldScript.enabled = false;
        AnimationController();
    }

    void FixedUpdate()
    {
        Move();
    }

    public void SetMove(InputAction.CallbackContext value)
    {
        moveInput = value.ReadValue<Vector2>();
    }

    void Move()
    {
        if (!canMove) return;

        rb.velocity = moveInput.normalized * moveSpeed;
        //rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    public void SetAttack(InputAction.CallbackContext value)
    {
        if (value.performed && canShoot)
        {
            ShootArrow();
            StartCoroutine(ShootCooldown());
        }
    }

    void AnimationController()
    {
        isWalking = moveInput.x != 0 || moveInput.y != 0;
        if (isWalking)
        {
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
        }
        animator.SetBool("Andando", isWalking);

        animator.SetBool("Atacando", isShooting);
    }

    void ShootArrow()
    {
        Debug.Log("disparokkkk");
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
        arrow.GetComponent<Projectile>().SetSpeed(arrowSpeed);
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;
        isShooting = true;
        canMove = false;
        rb.velocity = new Vector3(0f, 0f, 0f);
        yield return new WaitForSeconds(0.5f);
        canMove = true;
        isShooting = false;
        canShoot = true;
    }
}
