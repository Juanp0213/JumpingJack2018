using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    //gui elements
    public Image TitleMask;
    public Text StartMessage;
    AudioSource source;
    public AudioClip Clip;

    private void Start ()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine(Animate());
    }

    //logo and press start animation 
    IEnumerator Animate ()
    {
        float t = 0;
        WaitForSeconds wait = new WaitForSeconds(0.4f);
        StartMessage.enabled = false;
        while(t < 1)
        {
            TitleMask.fillAmount = Mathf.Lerp(0, 1, t);
            t += Time.deltaTime * 0.5f;
            yield return null;
        }

        TitleMask.fillAmount = 1;
        StartCoroutine(WaitForButton());
        while(true)
        {
            StartMessage.enabled = true;
            yield return wait;
            StartMessage.enabled = false;
            yield return wait;
        }
    }

    //wait for the player to press start to load the game scene
    IEnumerator WaitForButton ()
    {
        while(true)
        {
            if(Input.GetButtonDown("Submit"))
            {
                source.PlayOneShot(Clip);
                yield return new WaitForSeconds(0.2f);
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            }
            yield return null;
        }
    }

}
