using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtraLifeText : MonoBehaviour
{
    public Color C1;
    public Color C2;

    public Image Background;
    public Text Message;

    private void Start ()
    {
        StartCoroutine(ColorSwap());
    }
    
    //swap colors between text and its containing box
    IEnumerator ColorSwap ()
    {
        while(true)
        {
            Background.color = C1;
            Message.color = C2;
            yield return new WaitForSeconds(0.2f);
            Background.color = C2;
            Message.color = C1;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
