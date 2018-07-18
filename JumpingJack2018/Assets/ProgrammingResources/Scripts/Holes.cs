using UnityEngine;

public class Holes : MonoBehaviour
{
    //Each hole has 2 parts to make the effect of exit by one side and entering for the other
    public Rigidbody PartA;
    public Rigidbody PartB;

    Rigidbody body;
    Vector3 velocity;

    //Screen borders to appear on the opposite side
    public float LeftBorder;
    public float RightBorder;

    //movement values
    float speed;
    int direction;

    //Id of the standing floor 
    int currentFloor;
    public int CurrentFloor
    {
        get
        {
            return currentFloor;
        }
    }

    //Scene values to calculate positions
    float floorStartPos;
    float floorSpacing;
    int floorsAmount;


    private void Awake ()
    {
        //assings partA as default part
        body = PartA;
        LevelManager.SpeedChange += SetSpeed;
    }

    //update its speed acording to game manager
    void SetSpeed (float s)
    {
        speed = s;
        velocity.x = speed * direction;
        body.velocity = velocity;
    }


    //check if is out of border to respawn on the opposite side of the screen
    private void Update ()
    {
        if(body.transform.position.x > RightBorder)
            GoToNextFloor(LeftBorder);

        if(body.transform.position.x < LeftBorder)
            GoToNextFloor(RightBorder);
    }

    //receive values from the manager to set its position, scene and movement values
    public void Set (int floor, Vector3 pos, int dir, float startPos, float spacing, int floors)
    {
        currentFloor = floor;
        direction = dir;
        body.transform.position = pos;

        velocity.x = speed * direction;
        body.velocity = velocity;

        floorStartPos = startPos;
        floorSpacing = spacing;
        floorsAmount = floors;

        //one of the borders is reduced to enter on the screen while exiting
        if(dir == 1)
            RightBorder -= 1.4f;
        else
            LeftBorder += 1.4f;
    }


    //decide whats the next floor to go
    void GoToNextFloor (float xPos)
    {
        //when changing the floor both parts change places
        if(body == PartA)
            body = PartB;
        else
            body = PartA;

        //depending on its direction decides to go up or down
        if(direction == 1)
        {
            if(currentFloor == 0)
                currentFloor = floorsAmount - 1;
            else
                currentFloor--;
        }
        else
        {
            if(currentFloor == floorsAmount - 1)
                currentFloor = 0;
            else
                currentFloor++;
        }

        body.transform.position = new Vector3(xPos, floorStartPos + (currentFloor * floorSpacing));
        SetSpeed(speed);
    }
}