using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Image[] Lifes;
    public Text ScoreText;
    public Text HiScoreText;

    public GameObject NextLevelScreen;
    public Text NextLevelText;
    public Text LevelMessage;
    public GameObject ExtraLife;


    public void UpdateHp (int lifes)
    {
        for(int i = 0; i < Lifes.Length; i++)
        {
            if(i < lifes)
                Lifes[i].enabled = true;
            else
                Lifes[i].enabled = false;
        }
    }

    public void UpdateScore (int score)
    {
        ScoreText.text = System.String.Format("{0:00000}", score);
    }

    public void SetHiScore (int score)
    {
        HiScoreText.text = String.Format("{0:00000}", score);
    }

    public void ShowNextLevel (string message, int hazards, bool extraLife)
    {
        NextLevelScreen.SetActive(true);
        NextLevelText.text = string.Format("Next Level -{0} Hazard", hazards);
        StartCoroutine(WriteText(message));
        ExtraLife.SetActive(extraLife);
    }

    IEnumerator WriteText (string msg)
    {
        string str = string.Empty;
        WaitForSeconds wfs = new WaitForSeconds(0.075f);
        for(int i = 0; i < msg.Length; i++)
        {
            str = string.Concat(str, msg[i]);
            LevelMessage.text = str;
            yield return wfs;
        }
    }

}
