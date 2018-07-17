using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holes : MonoBehaviour
{
    public Rigidbody PartA;
    public Rigidbody PartB;

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
    int direction;
    int currentFloor;
    public int CurrentFloor
    {
        get
        {
            return currentFloor;
        }
    }

    LevelManager manager;

    Vector3 velocity;

    Rigidbody body;

    private void Awake ()
    {
        body = PartA;
    }

    private void Update ()
    {
        velocity.x = speed * direction;
        body.velocity = velocity;

        if(body.transform.position.x > RightBorder)
            GoToNextFloor(LeftBorder);

        if(body.transform.position.x < LeftBorder)
            GoToNextFloor(RightBorder);
    }

    public void Set (int floor, Vector3 pos, int dir, LevelManager m)
    {
        currentFloor = floor;
        direction = dir;
        body.transform.position = pos;
        manager = m;

        if(dir == 1)
            RightBorder -= 1.4f;
        else
            LeftBorder += 1.4f;
    }


    void GoToNextFloor (float xPos)
    {

        if(body == PartA)
            body = PartB;
        else
            body = PartA;

        if(direction == 1)
        {
            if(currentFloor == 0)
                currentFloor = manager.FloorsAmount - 1;
            else
                currentFloor--;
        }
        else
        {
            if(currentFloor == manager.FloorsAmount - 1)
                currentFloor = 0;
            else
                currentFloor++;
        }

        body.transform.position = new Vector3(xPos, manager.InitialFloorPosition + currentFloor * manager.FloorSpace);
    }
}