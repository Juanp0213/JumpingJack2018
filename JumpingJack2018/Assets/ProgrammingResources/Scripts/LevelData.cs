using System.Collections.Generic;
using UnityEngine;

//class for the storage of each level info
public class LevelData : ScriptableObject
{
    public LevelInfo[] Levels;
    //wich are the models for the enemies
    public GameObject[] EnemyModels;

    int currentEnemy = -1;
    public GameObject GetEnemyModel ()
    {
        currentEnemy++;
        if(currentEnemy >= EnemyModels.Length)
            currentEnemy = 0;
        return EnemyModels[currentEnemy];
    }

    public void ResetEnemyIndex ()
    {
        currentEnemy = -1;
    }
}

[System.Serializable]
public class LevelInfo
{
    //give or not extra life on complete
    public bool ExtraLife;
    //part of the ballad to reveal after complete the level
    public string FinishMessage;
}


static class Helper
{
    //helper to shuffle arrays, used to shuffle enemies model each time the game start
    public static void Shuffle<T> (this IList<T> list)
    {
        int n = list.Count;
        while(n > 1)
        {
            n--;
            int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static class ThreadSafeRandom
    {
        [System.ThreadStatic] static System.Random Local;

        public static System.Random ThisThreadsRandom
        {
            get
            {
                return Local ?? (Local = new System.Random(unchecked(System.Environment.TickCount * 31 + System.Threading.Thread.CurrentThread.ManagedThreadId)));
            }
        }
    }
}