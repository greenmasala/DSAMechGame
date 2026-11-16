using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public float dashForce = 2f;

    private Rigidbody rb;

    private Vector3 _previousPos;

    private Vector3 _currentPos;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _previousPos = _currentPos;

        _currentPos = transform.position;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Cast();
        }
    }

    public Vector3 moveDirection
    {
        get
        {
            return (_currentPos - _previousPos).normalized;
        }
    }

    void Cast()
    {
        rb.AddForce(moveDirection * dashForce, ForceMode.Impulse);
    }
}

