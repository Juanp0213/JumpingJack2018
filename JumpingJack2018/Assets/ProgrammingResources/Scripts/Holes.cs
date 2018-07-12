using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holes : MonoBehaviour
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

    Rigidbody body;
    Vector3 velocity;


    private void Start ()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update ()
    {
        velocity.x = speed * direction;
        body.velocity = velocity;

        if(transform.position.x > RightBorder)
            GoToNextFloor(LeftBorder);

        if(transform.position.x < LeftBorder)
            GoToNextFloor(RightBorder);
    }

    public void Set (int floor, Vector3 pos, int dir, LevelManager m)
    {
        currentFloor = floor;
        direction = dir;
        transform.position = pos;
        manager = m;
    }


    void GoToNextFloor (float xPos)
    {
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
        transform.position = new Vector3(xPos, manager.InitialFloorPosition + currentFloor * manager.FloorSpace);
    }
}