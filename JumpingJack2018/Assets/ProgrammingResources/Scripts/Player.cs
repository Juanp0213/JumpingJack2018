using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int StartHp;
    static int hp = -1;
    public float LeftBorder;
    public float RightBorder;

    public Animator Model;

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
        if(hp == -1)
            hp = StartHp;
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

        if(axis.x > 0)
            Model.transform.localEulerAngles = Vector3.up * 90;
        else if(axis.x < 0)
            Model.transform.localEulerAngles = Vector3.up * -90;
        else
            Model.transform.localEulerAngles = Vector3.up * 180;

        Model.SetFloat("Move", Mathf.Abs(axis.x));


        if(axis.y > 0)
        {
            Model.SetFloat("Move", 0);
            Model.transform.localEulerAngles = Vector3.up * 180;
            Jump();
        }

    }


    void Jump ()
    {
        if(Physics.Linecast(transform.position, transform.position + Vector3.up * manager.FloorSpace))
        {
            StartCoroutine(VerticalMovement(transform.position, transform.position + Vector3.up * manager.FloorSpace));
            currentFloor++;

            manager.AddScore();
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
            {
                Debug.Log("Hp-");
                hp--;
                if(hp <= 0)
                {
                    manager.GameOver();
                }
            }
        }
    }

    private void OnTriggerEnter (Collider other)
    {
        if(other.CompareTag("Hole"))
        {
            if(!jumping)
            {
                StartCoroutine(VerticalMovement(transform.position, transform.position + Vector3.up * -manager.FloorSpace));
                currentFloor--;
                if(currentFloor < 0)
                    currentFloor = 0;
                StartCoroutine(Stun());
            }
        }
        else if(other.CompareTag("Enemy"))
        {
            StartCoroutine(Stun());
        }
    }

    IEnumerator Stun ()
    {
        Model.Play("Death");
        stun = true;
        yield return new WaitForSeconds(2);
        Model.SetTrigger("Revive");
        stun = false;
    }



}
