using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movecontrol : MonoBehaviour


{


    [SerializeField] Vector3 _Move;
    [SerializeField] Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();


    }

    // Update is called once per frame
    void Update()
    {
        _rb.velocity = new Vector2(_Move.x, _rb.velocity.y);



    }

    public void SetMove(InputAction.CallbackContext value)
    {
        _Move = value.ReadValue<Vector3>();


    }



}