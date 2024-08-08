using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveControl : MonoBehaviour


{


    [SerializeField] Vector3 _Move;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] float speed;
    bool _facingRight;


    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();


    }

    // Update is called once per frame
    void Update()
    {
        _rb.velocity = new Vector2(_Move.x * speed, _rb.velocity.y);

        if (_Move.x > 0 && _facingRight == true)
        {
            flip();

        }

        else if (_Move.x < 0 && _facingRight == false)
        {

            flip();
        }

    }

    public void SetMove(InputAction.CallbackContext value)
    {
        _Move = value.ReadValue<Vector3>();


    }

    void flip()
    {

        _facingRight = !_facingRight;
        float x = transform.localScale.x;
        x *= -1;

        transform.localScale = new Vector2(x, transform.localScale.y);


    }



}
