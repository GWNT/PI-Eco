using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveControl : MonoBehaviour


{


    [SerializeField] Vector3 _Move;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] float _speed;
    bool _facingRight;
    bool _facingUp;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();


    }

    // Update is called once per frame
    void Update()
    {
        _rb.velocity = new Vector2(_Move.x , _Move.y) *_speed;

        if (_Move.x > 0 && _facingRight == true)
        {
            flip();

        }

        else if (_Move.x < 0 && _facingRight == false)
        {

            flip();
        }

        if (_Move.y < 0 && _facingUp == true)
        {
            flipY();

        }

        else if (_Move.y > 0 && _facingUp == false)
        {

            flipY();
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
    void flipY()
    {

        _facingUp = !_facingUp;
        float y = transform.localScale.y;
        y *= -1;

        transform.localScale = new Vector2(transform.localScale.x,  y);


    }


}
