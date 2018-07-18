using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    //start hp and current hp
    public int StartHp;
    static int hp = -1;

    //Screen borders to appear on the opposite side
    public float LeftBorder;
    public float RightBorder;

    //graphic player
    public Animator Model;

    //movement values
    float speed;
    Vector2 axis;
    Vector3 velocity;

    //components
    Rigidbody body;
    Collider collision;

    //player states
    bool jumping;
    bool stun;
    float stunTime;

    //external entities
    LevelManager manager;
    HUDManager hud;

    //Id of the standing floor 
    int currentFloor;

    //audio components
    AudioSource source;
    public AudioClip JumpClip;
    public AudioClip HitClip;

    //get all references
    private void Awake ()
    {
        body = GetComponent<Rigidbody>();
        manager = FindObjectOfType<LevelManager>();
        hud = FindObjectOfType<HUDManager>();
        source = GetComponent<AudioSource>();
        collision = GetComponent<Collider>();
        LevelManager.SpeedChange += SetSpeed;
    }

    //if hp have its default value set it to the start hp
    private void Start ()
    {
        if(hp == -1)
            hp = StartHp;
        hud.UpdateHp(hp);
    }

    //update its speed acording to game manager
    void SetSpeed (float s)
    {
        speed = s;
    }

    private void Update ()
    {
        Movement();
    }


    void Movement ()
    {
        //is player is jumping or stuned its velocity is zero
        if(jumping || stun)
        {
            velocity = Vector3.zero;
            body.velocity = velocity;
            return;
        }

        //take the values from the inputs
        axis.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //move according to inputs
        velocity.x = axis.x * speed;
        body.velocity = velocity;

        //check if is out of border to respawn on the opposite side of the screen
        if(transform.position.x > RightBorder)
            transform.position = new Vector3(LeftBorder, transform.position.y, transform.position.z);

        if(transform.position.x < LeftBorder)
            transform.position = new Vector3(RightBorder, transform.position.y, transform.position.z);

        //rotate the model depending on the movement direction
        if(axis.x > 0)
            Model.transform.localEulerAngles = Vector3.up * 90;
        else if(axis.x < 0)
            Model.transform.localEulerAngles = Vector3.up * -90;
        else
            Model.transform.localEulerAngles = Vector3.up * 180;

        //update the animator
        Model.SetFloat("Move", Mathf.Abs(axis.x));

        //jump
        if(axis.y > 0)
        {
            Model.SetFloat("Move", 0);
            Model.transform.localEulerAngles = Vector3.up * 180;
            Jump();
        }
    }

    void Jump ()
    {
        //play sound
        source.PlayOneShot(JumpClip);

        //check if are holes above, else run a fail animation
        if(Physics.Linecast(transform.position, transform.position + Vector3.up * manager.FloorSpace))
        {
            currentFloor++;
            StartCoroutine(VerticalMovement(transform.position, GetNextPos()));
            manager.AddScore();
        }
        else
        {
            StartCoroutine(Fail());
        }
    }

    //fail animation
    IEnumerator Fail ()
    {
        yield return StartCoroutine(VerticalMovement(transform.position, transform.position + Vector3.up * 0.35f, true));
        yield return StartCoroutine(VerticalMovement(transform.position, transform.position + Vector3.up * -0.35f));
        StartCoroutine(Stun());
    }

    //execute all vertical movement, jumping, falling and fail
    IEnumerator VerticalMovement (Vector3 from, Vector3 to, bool fakeMove = false)
    {

        jumping = from.y < to.y && !fakeMove;

        jumping = true;
        collision.enabled = false;
        float t = 0;
        while(t < 1)
        {
            transform.position = Vector3.Lerp(from, to, t);
            t += Time.deltaTime * speed;
            yield return null;
        }

        transform.position = to;
        collision.enabled = true;
        jumping = false;

        if(currentFloor == manager.FloorsAmount)
            manager.Win();
        if(!fakeMove)
        {
            if(currentFloor == 0)//if the movement ends on the first floor a life is lost
            {
                hp--;
                hud.UpdateHp(hp);
                if(hp <= 0)//if there is no lifes, game over
                {
                    manager.GameOver();
                }
            }
        }
    }

    //check when touching enemies or holes
    private void OnTriggerEnter (Collider other)
    {
        if(!jumping)
        {
            if(other.CompareTag("Hole"))
            {
                currentFloor--;
                if(currentFloor < 0)
                    currentFloor = 0;

                StartCoroutine(VerticalMovement(transform.position, GetNextPos()));

                StartCoroutine(Stun());
            }
            else if(other.CompareTag("Enemy"))
            {
                StartCoroutine(Stun());
            }
        }
    }

    //stun the player and play its respective animation
    IEnumerator Stun ()
    {
        stunTime = 2;
        source.PlayOneShot(HitClip);
        Model.Rebind();
        Model.Play("Death");
        if(!stun)
        {
            stun = true;
            while(stunTime > 0)
            {
                stunTime -= Time.deltaTime;
                yield return null;
            }
            Model.SetTrigger("Revive");
            stun = false;
        }
    }

    public void AddLife ()
    {
        hp++;
        hud.UpdateHp(hp);
    }

    Vector3 GetNextPos ()
    {
        return new Vector3(transform.position.x, manager.InitialFloorPosition + ((currentFloor - 1) * manager.FloorSpace) + 0.075f, 0);
    }
}