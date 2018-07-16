using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Image TitleMask;
    public Text StartMessage;
    bool animFinish;

    private void Start ()
    {
        StartCoroutine(Animate());
    }

    private void Update ()
    {
        if(animFinish && Input.GetButtonDown("Submit"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }


    IEnumerator Animate ()
    {
        float t = 0;
        WaitForSeconds wait = new WaitForSeconds(0.4f);
        StartMessage.enabled = false;
        while(t < 1)
        {
            TitleMask.fillAmount = Mathf.Lerp(0, 1, t);
            t += Time.deltaTime;
            yield return null;
        }

        animFinish = true;
        while(true)
        {
            StartMessage.enabled = true;
            yield return wait;
            StartMessage.enabled = false;
            yield return wait;
        }

    }

}
