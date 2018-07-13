using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float LeftBorder;
    public float RightBorder;

    float speed
    {
        get
        {
            if(manager == null)
                return 0;
            return -manager.OverallSpeed;
        }
    }
    int currentFloor;

    LevelManager manager;

    Rigidbody body;
    Vector3 velocity;


    private void Start ()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update ()
    {
        velocity.x = speed;
        body.velocity = velocity;

        if(transform.position.x < LeftBorder)
            GoToNextFloor(RightBorder);
    }

    public void Set (EnemyData data, LevelManager m)
    {
        manager = m;
        currentFloor = data.StartFloor;
        transform.position = new Vector3(data.StartXPos, manager.InitialFloorPosition + currentFloor * manager.FloorSpace + 0.45f);
    }


    void GoToNextFloor (float xPos)
    {

        if(currentFloor == manager.FloorsAmount - 1)
            currentFloor = 0;
        else
            currentFloor++;

        if(currentFloor == manager.FloorsAmount - 1)
            transform.position = new Vector3(xPos, manager.InitialFloorPosition + currentFloor * manager.FloorSpace + 5f);
        else
            transform.position = new Vector3(xPos, manager.InitialFloorPosition + currentFloor * manager.FloorSpace + 0.45f);

    }
}
