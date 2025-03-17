using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayControl : MonoBehaviour
{
    [Header("Player variables")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private bool canMove;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool canShoot = true;
    [SerializeField] private bool isShooting = false;
    [SerializeField] private Vector2 shootDirection;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Teste2Play oldScript;

    [Header("Arrow variables")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed = 10f;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        oldScript = GetComponent<Teste2Play>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        canShoot = true;
        canMove = true;
    }

    void Update()
    {
        oldScript.enabled = false;
        AnimationController();
        UpdateShootDirection();
        
    }

    void FixedUpdate()
    {
        Move();
    }

    public void SetMove(InputAction.CallbackContext value)
    {
        if (canMove && value.performed)
        {
            moveInput = value.ReadValue<Vector2>().normalized;
        }
        if (value.canceled)
        {
            moveInput = Vector2.zero;
        }
    }

    void Move()
    {
        if (!canMove) return;

        rb.velocity = moveInput * moveSpeed;
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

    void UpdateShootDirection()
    {
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                shootDirection = new Vector2(moveInput.x, 0).normalized;
            }
            else
            {
                shootDirection = new Vector2(0, moveInput.y).normalized;
            }
            UpdateFirePointRotation();
        }
    }

    void UpdateFirePointRotation()
    {
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}
