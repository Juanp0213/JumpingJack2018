using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holes : MonoBehaviour {

    public float Speed = 1;
    public float LeftBorder;
    public float RightBorder;

    Rigidbody body;
    Vector3 velocity;

    private void Start ()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update ()
    {
        velocity.x = Speed;
        body.velocity = velocity;

        if(transform.position.x > RightBorder)
            transform.position = new Vector3(LeftBorder, transform.position.y, transform.position.z);

        if(transform.position.x < LeftBorder)
            transform.position = new Vector3(RightBorder, transform.position.y, transform.position.z);
    }
}
