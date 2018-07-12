using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Speed = 1;
    public float LeftBorder;
    public float RightBorder;

    Vector2 axis;
    Rigidbody body;
    Vector3 velocity;

    bool jumping;

    private void Start ()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update ()
    {
        Movement();
    }


    void Movement ()
    {
        if(jumping)
            return;

        axis.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        velocity = Vector3.zero;
        velocity.x = axis.x * Speed;
        body.velocity = velocity;

        if(transform.position.x > RightBorder)
            transform.position = new Vector3(LeftBorder, transform.position.y, transform.position.z);

        if(transform.position.x < LeftBorder)
            transform.position = new Vector3(RightBorder, transform.position.y, transform.position.z);

        if(axis.y > 0)
        {
            Jump();
        }
    }


    void Jump ()
    {
        if(Physics.Linecast(transform.position, transform.position + Vector3.up * 1.2f))
        {
            StartCoroutine(VerticalMovement(transform.position, transform.position + Vector3.up * 1.2f));
        }
        else
        {
            StartCoroutine(Fail());
        }
    }

    IEnumerator Fail ()
    {
        yield return StartCoroutine(VerticalMovement(transform.position, transform.position + Vector3.up * 0.35f));
        yield return StartCoroutine(VerticalMovement(transform.position, transform.position + Vector3.up * -0.35f));
        jumping = true;
        yield return new WaitForSeconds(2);
        jumping = false;
    }


    IEnumerator VerticalMovement (Vector3 from, Vector3 to)
    {
        jumping = true;

        float t = 0;
        while(t < 1)
        {
            transform.position = Vector3.Lerp(from, to, t);
            t += Time.deltaTime * 4;
            yield return null;
        }

        transform.position = to;
        jumping = false;
    }

    private void OnTriggerEnter (Collider other)
    {
        if(!jumping)
        {
            StartCoroutine(VerticalMovement(transform.position, transform.position + Vector3.up * -1.2f));
        }
    }



}
