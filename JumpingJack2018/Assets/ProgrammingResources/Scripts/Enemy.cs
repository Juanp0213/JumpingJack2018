using UnityEngine;

public class Enemy : MonoBehaviour
{

    //Screen borders to appear on the opposite side
    public float LeftBorder;
    public float RightBorder;

    //Id of the standing floor 
    int currentFloor;

    Rigidbody body;
    Vector3 velocity;

    //Scene values to calculate positions
    float floorStartPos;
    float floorsSpacing;
    int floorsAmount;


    private void Awake ()
    {
        //gets its own rigidbody
        body = GetComponent<Rigidbody>();

        //suscribe to know when game speed change
        LevelManager.SpeedChange += SetSpeed;
    }

    //update its speed acording to game manager
    void SetSpeed (float s)
    {
        velocity.x = -s;
        body.velocity = velocity;
    }

    private void Update ()
    {
        //check if is out of border to respawn on the opposite side of the screen
        if(transform.position.x < LeftBorder)
            GoToNextFloor(RightBorder);
    }

    //receive values from the manager to set its position and scene values
    public void Set (int startFloor, float xPos, GameObject model, float startPos, float spacing, int floors)
    {
        floorStartPos = startPos;
        floorsSpacing = spacing;
        floorsAmount = floors;

        currentFloor = startFloor;
        //if is on the top floor take a position off screen
        if(currentFloor == floorsAmount - 1)
            transform.position = new Vector3(xPos, floorStartPos + (currentFloor * floorsSpacing) + 5f);
        else
            transform.position = new Vector3(xPos, floorStartPos + (currentFloor * floorsSpacing) + 0.075f);

        //set the model inside
        model = Instantiate(model);
        model.transform.SetParent(transform);
        model.transform.localPosition = Vector3.zero;
    }

    //decide whats the next floor to go
    void GoToNextFloor (float xPos)
    {
        if(currentFloor == floorsAmount - 1)
            currentFloor = 0;
        else
            currentFloor++;

        //if is on the top floor take a position off screen
        if(currentFloor == floorsAmount - 1)
            transform.position = new Vector3(xPos, floorStartPos + (currentFloor * floorsSpacing) + 5f);
        else
            transform.position = new Vector3(xPos, floorStartPos + (currentFloor * floorsSpacing) + 0.075f);

    }
}
