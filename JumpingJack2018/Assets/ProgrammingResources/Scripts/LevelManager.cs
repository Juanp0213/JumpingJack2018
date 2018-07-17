using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public float InitialFloorPosition;
    public float FloorSpace;

    public GameObject FloorModel;
    public int FloorsAmount;
    GameObject[] floors;

    public Holes HoleModel;
    public int HolesAmount;
    Holes[] holes;
    int currentHole;

    public Enemy EnemyModel;
    Enemy[] enemies;
    public LevelData Level;


    public float OverallSpeed;

    static int currentLevel;
    static int score;
    static int hiScore;
    const int scoreBase = 5;

    public GameObject GameOverScreen;

    HUDManager hud;

    private void Start ()
    {
        CreateFloors();
        CreateHoles();
        CreateEnemies();
        hud = FindObjectOfType<HUDManager>();
        hud.UpdateScore(score);
        hud.SetHiScore(hiScore);
        Debug.Log(currentLevel);
    }


    void CreateFloors ()
    {
        Vector3 floorPosition = Vector3.zero;
        floorPosition.y = InitialFloorPosition;

        floors = new GameObject[FloorsAmount];
        floors[0] = FloorModel;
        floors[0].transform.position = floorPosition;

        for(int i = 1; i < FloorsAmount; i++)
        {
            floors[i] = Instantiate(FloorModel);
            floors[i].transform.SetParent(FloorModel.transform.parent);
            floorPosition.y += FloorSpace;
            floors[i].transform.position = floorPosition;
            floors[i].name = "Floor " + i;
        }
    }

    void CreateHoles ()
    {
        holes = new Holes[HolesAmount];
        holes[0] = HoleModel;
        for(int i = 1; i < HolesAmount; i++)
        {
            holes[i] = Instantiate(HoleModel);
            holes[i].transform.SetParent(HoleModel.transform.parent);
            holes[i].transform.position = Vector3.up * -10;
            holes[i].name = "Hole " + i;
        }
        SetHole(1);
        SetHole(-1);
    }


    public void AddScore ()
    {
        SetHole();
        score += scoreBase + (scoreBase * currentLevel);
        hud.UpdateScore(score);
    }

    public void SetHole (int d = 0)
    {
        if(currentHole == HolesAmount)
            return;

        int f;
        bool repeatedLine;

        do
        {
            f = Random.Range(0, 8);
            repeatedLine = false;
            for(int i = 0; i < holes.Length; i++)
            {
                if(holes[i].CurrentFloor == f)
                    repeatedLine = true;
            }
        }
        while(repeatedLine);


        if(d == 0)
        {
            if(currentHole < 5)
                d = 1;
            else
                d = -1;
        }

        holes[currentHole].Set(f, new Vector3(Random.Range(-10, 10), InitialFloorPosition + f * FloorSpace, 0), d, this);
        currentHole++;
    }


    void CreateEnemies ()
    {
        int enemiesAmount = Level.Levels[currentLevel].Enemies.Length;
        enemies = new Enemy[enemiesAmount];

        for(int i = 0; i < enemiesAmount; i++)
        {
            if(i == 0)
                enemies[i] = EnemyModel;
            else
            {
                enemies[i] = Instantiate(EnemyModel);
                enemies[i].transform.SetParent(EnemyModel.transform.parent);
            }
            enemies[i].Set(Level.Levels[currentLevel].Enemies[i], this);


        }
    }

    [ContextMenu("asd")]
    public void Win ()
    {
        OverallSpeed = 0;
        currentLevel++;
        hud.ShowNextLevel(Level.Levels[currentLevel - 1].FinishMessage, Level.Levels[currentLevel].Enemies.Length, Level.Levels[currentLevel - 1].ExtraLife);
        StartCoroutine(LoadDelay());
    }


    public void GameOver ()
    {
        OverallSpeed = 0;
        currentLevel = 0;
        GameOverScreen.SetActive(true);
        StartCoroutine(LoadDelay());

        if(score > hiScore)
        {
            hiScore = score;
            hud.SetHiScore(hiScore);
        }
        score = 0;
    }

    IEnumerator LoadDelay ()
    {
        yield return new WaitForSeconds(8);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
