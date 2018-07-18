
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    //position of the first floor
    public float InitialFloorPosition;
    //space between floors
    public float FloorSpace;

    //graphic floor
    public GameObject FloorModel;
    //how many floors are on the game
    public int FloorsAmount;
    //created floors
    GameObject[] floors;

    //graphic hole
    public Holes HoleModel;
    //how many holes are on the game
    public int HolesAmount;
    //created holes
    Holes[] holes;
    //currnet hole being used
    int currentHole;

    //folder to agrupate enemies
    public Transform EnemiesParent;
    //graphic enemy
    public Enemy EnemyModel;
    //created enemies
    Enemy[] enemies;

    //speed of the game, as many entities use the same speed this is handle by the game manager
    public float OverallSpeed;
    //event to change the speed to all entities using it
    public static event UnityAction<float> SpeedChange;

    //game stats
    static int currentLevel;
    static int score;
    static int hiScore;
    const int scoreBase = 5;

    //references to external entities
    public LevelData LevelDatabase;
    HUDManager hud;
    Player player;

    //helper to prevent enemies created over enemies
    struct EnemyPos
    {
        public int Floor;
        public int XPos;
    }


    private void Start ()
    {
        //create game necessary elements
        CreateFloors();
        CreateHoles();
        CreateEnemies();

        //get player reference
        player = FindObjectOfType<Player>();

        //get ui reference and update it
        hud = FindObjectOfType<HUDManager>();
        hud.UpdateScore(score);
        hud.SetHiScore(hiScore);
        hud.SetEnemies(currentLevel);

        //set speed
        if(SpeedChange != null)
            SpeedChange(OverallSpeed);

        //shuffle enemies models
        if(currentLevel == 0)
            LevelDatabase.EnemyModels.Shuffle();
    }

    //cancel all subscription to the event
    private void OnDestroy ()
    {
        SpeedChange = null;
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

    //when score is added adds a new hole and update the gui
    public void AddScore ()
    {
        SetHole();
        score += scoreBase + (scoreBase * currentLevel);
        hud.UpdateScore(score);
    }

    //add holes to the level...
    public void SetHole (int d = 0)
    {
        if(currentHole == HolesAmount)//... if the limit is not yet reached
            return;

        //only create holes in clean floors
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

        //if "d" comes with value different to zero its used as fixed direction,
        //if not then the first 3 holes goes right and the other 3 go left
        if(d == 0)
        {
            if(currentHole < 5)
                d = 1;
            else
                d = -1;
        }

        holes[currentHole].Set(f, new Vector3(Random.Range(-7, 7), InitialFloorPosition + f * FloorSpace, 0), d, InitialFloorPosition, FloorSpace, FloorsAmount);
        currentHole++;
    }

    void CreateEnemies ()
    {
        LevelDatabase.ResetEnemyIndex();
        int enemiesAmount = currentLevel;
        enemies = new Enemy[enemiesAmount];

        //check that there is no enemy being instantiated over other enemy
        EnemyPos currentPos = new EnemyPos();//position assigned to a new enemy
        List<EnemyPos> ep = new List<EnemyPos>();//record of previously created enemies
        bool repeated = false;

        for(int i = 0; i < enemiesAmount; i++)
        {
            enemies[i] = Instantiate(EnemyModel);
            enemies[i].transform.SetParent(EnemiesParent);
            do
            {
                repeated = false;
                currentPos.Floor = Random.Range(0, FloorsAmount);
                currentPos.XPos= Random.Range(-7, 7);
                for(int j = 0; j < ep.Count; j++)
                {
                    if(currentPos.Floor == ep[j].Floor && currentPos.XPos == ep[j].XPos)
                        repeated = true;
                }
            } while(repeated);

            ep.Add(currentPos);
            enemies[i].Set(currentPos.Floor, currentPos.XPos, LevelDatabase.GetEnemyModel(), InitialFloorPosition, FloorSpace, FloorsAmount);
        }
    }

    public void Win ()
    {
        //game speed is set to zero
        if(SpeedChange != null)
            SpeedChange(0);

        currentLevel++;

        //if this is the last level, dont seek next level info
        if(currentLevel >= LevelDatabase.Levels.Length)
            hud.ShowNextLevel(LevelDatabase.Levels[currentLevel - 1].FinishMessage, -1, LevelDatabase.Levels[currentLevel - 1].ExtraLife);
        else
            hud.ShowNextLevel(LevelDatabase.Levels[currentLevel - 1].FinishMessage, currentLevel, LevelDatabase.Levels[currentLevel - 1].ExtraLife);

        //if level has a life give it to the player
        if(LevelDatabase.Levels[currentLevel - 1].ExtraLife)
            player.AddLife();

    }


    public void GameOver ()
    {
        //game speed is set to zero
        if(SpeedChange != null)
            SpeedChange(0);

        if(currentLevel >= LevelDatabase.Levels.Length)
            currentLevel = LevelDatabase.Levels.Length - 1;
        hud.ShowGameOver(score, score > hiScore, currentLevel);

        if(score > hiScore)
        {
            hiScore = score;
            hud.SetHiScore(hiScore);
        }

        //restart info
        currentLevel = 0;
        score = 0;
    }
}
