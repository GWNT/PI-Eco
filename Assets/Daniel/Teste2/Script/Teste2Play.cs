using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Teste2Play : MonoBehaviour
{
    public Animator _anim;
    public float _speed;
    Rigidbody2D _rb;
    [SerializeField] Vector2 _move;
    public bool _andando;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _andando = (_move.x != 0 || _move.y != 0);

        if(_andando)
        {
            _anim.SetFloat("Horizontal", _move.x);
            _anim.SetFloat("Vertical", _move.y);
        }
        _anim.SetBool("Andando", _andando);
    }

    void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _move * _speed * Time.fixedDeltaTime);
    }

    public void SetMove(InputAction.CallbackContext value)
    {
        _move = value.ReadValue<Vector3>();
    }
}
