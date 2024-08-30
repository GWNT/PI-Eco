using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestePlayerController : MonoBehaviour
{
    public Animator anim;
    public float speed;
    Rigidbody2D _rb;
    float _input_x, _input_y;
    [SerializeField] Vector3 _move;
    public bool _andando;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        _input_x = _move.x;
        _input_y = _move.y;
        _andando = (_input_x != 0 || _input_y != 0);

        if(_andando)
        {
            anim.SetFloat("Horizontal", _input_x);
            anim.SetFloat("Vertical", _input_y);
        }
        anim.SetBool("Andando", _andando);
        
    }

    void FixedUpdate()
    {
        _rb.velocity = new Vector2(_move.x, _move.y) * speed * Time.fixedDeltaTime;
    }

    public void SetMove(InputAction.CallbackContext value)
    {
        _move = value.ReadValue<Vector3>(); 
    }

}
