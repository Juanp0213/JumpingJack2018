using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float LeftBorder;
    public float RightBorder;

    float speed
    {
        get
        {
            if(manager == null)
                return 0;
            return manager.OverallSpeed;
        }
    }
    Vector2 axis;
    Rigidbody body;
    Vector3 velocity;

    bool jumping;
    bool stun;

    LevelManager manager;

    int currentFloor;

    private void Start ()
    {
        body = GetComponent<Rigidbody>();
        manager = FindObjectOfType<LevelManager>();
    }

    private void Update ()
    {
        Movement();
    }


    void Movement ()
    {
        if(jumping || stun)
        {
            velocity = Vector3.zero;
            body.velocity = velocity;
            return;
        }

        axis.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        velocity.x = axis.x * speed;
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
            currentFloor++;

            manager.SetHole();
        }
        else
        {
            StartCoroutine(Fail());
        }
    }

    IEnumerator Fail ()
    {
        yield return StartCoroutine(VerticalMovement(transform.position, transform.position + Vector3.up * 0.35f, false));
        yield return StartCoroutine(VerticalMovement(transform.position, transform.position + Vector3.up * -0.35f));
        StartCoroutine(Stun());
    }


    IEnumerator VerticalMovement (Vector3 from, Vector3 to, bool checkHp = true)
    {
        jumping = true;

        float t = 0;
        while(t < 1)
        {
            transform.position = Vector3.Lerp(from, to, t);
            t += Time.deltaTime * speed;
            yield return null;
        }

        transform.position = to;
        jumping = false;

        if(currentFloor == manager.FloorsAmount)
            manager.Win();
        if(checkHp)
        {
            if(currentFloor == 0)
                Debug.Log("Hp-");
        }
    }

    private void OnTriggerEnter (Collider other)
    {
        if(!jumping)
        {
            StartCoroutine(VerticalMovement(transform.position, transform.position + Vector3.up * -1.2f));
            currentFloor--;
            if(currentFloor < 0)
                currentFloor = 0;
            StartCoroutine(Stun());
        }
    }

    IEnumerator Stun ()
    {
        stun = true;
        yield return new WaitForSeconds(2);
        stun = false;
    }



}
