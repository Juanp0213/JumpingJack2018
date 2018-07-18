using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//general manager of the UI
public class HUDManager : MonoBehaviour
{
    //On screen information
    public Image[] Lifes;
    public Text ScoreText;
    public Text HiScoreText;

    //Elements for "win/next level" screen
    public GameObject NextLevelScreen;
    public GameObject NextLevelBox;
    public Text NextLevelText;
    public Text LevelMessage;
    public GameObject ExtraLife;

    //Elements for "game over" screen
    public GameObject GameOverScreen;
    public GameObject NewHighBox;
    public Text ResultText;
    public Text ReplayText;

    //info values
    int currentScore;
    int currentHiScore;
    int enemies;

    //updates life icons when the player is damaged or win lifes
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
    
    //update the score on screen and save the value
    public void UpdateScore (int score)
    {
        ScoreText.text = String.Format("{0:00000}", score);
        currentScore = score;
    }

    //update the hi-score on screen and save the value
    public void SetHiScore (int score)
    {
        HiScoreText.text = String.Format("{0:00000}", score);
        currentHiScore = score;
    }

    //save how many enemies are on scene
    public void SetEnemies (int e)
    {
        enemies = e;
    }

    //display the next level screen and the correspondent info
    public void ShowNextLevel (string message, int hazards, bool extraLife)
    {
        NextLevelScreen.SetActive(true);
        if(hazards == -1)
            NextLevelBox.SetActive(false);
        else
            NextLevelText.text = string.Format("Next Level -{0} Hazard", hazards);
        StartCoroutine(NextLevelWait(message, hazards == -1));
        //show extra life box
        ExtraLife.SetActive(extraLife);
    }

    //wait while the text is being displayed
    IEnumerator NextLevelWait (string msg, bool isLastLevel)
    {
        yield return StartCoroutine(WriteText(LevelMessage, msg));

        yield return new WaitForSeconds(4);
        //if it is the last level, shows the "game over" screen, if is not load the next level
        if(!isLastLevel)
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        else
            ShowGameOver(currentScore, currentScore > currentHiScore, enemies);
    }

    //shows the "game over" screen
    public void ShowGameOver (int score, bool newHigh, int hazards)
    {
        GameOverScreen.SetActive(true);
        NewHighBox.SetActive(newHigh);
        StartCoroutine(GameOverWait(score, hazards));
    }

    //wait while the text is being displayed and later for the player to press start
    IEnumerator GameOverWait (int score, int hazards)
    {
        ResultText.text = string.Format("Final Score {0}\nWith {1} hazards", String.Format("{0:00000}", score), hazards);

        yield return StartCoroutine(WriteText(ReplayText, "Press ENTER to replay"));

        while(true)
        {
            if(Input.GetButtonDown("Submit"))
                break;
            yield return null;
        }
        //reload the game
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    //helpet method for the writing effect
    IEnumerator WriteText (Text t, string msg)
    {
        string str = string.Empty;
        WaitForSeconds wfs = new WaitForSeconds(0.075f);
        for(int i = 0; i < msg.Length; i++)
        {
            str = string.Concat(str, msg[i]);
            t.text = str;
            yield return wfs;
        }
    }
}
