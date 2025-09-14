using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotsEllipsisAnimation : MonoBehaviour
{
    [Tooltip("Enter number of Dots that you want to show +1")]
    public int numOfDots = 3;
    public float animTime = 0.5f;
    public Text targetText;

    public static DotsEllipsisAnimation instance;


    private void OnEnable()
    {
        instance = this;

        StopCoroutine("AnimateDots");
        StartCoroutine("AnimateDots");
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    

    public IEnumerator AnimateDots()
    {
      

        for (int i=0;i<numOfDots;i++)
        {
           
            targetText.text += ".";

            if(i==numOfDots-1)
            {
                targetText.text = string.Empty;
                StopAllCoroutines();
                StartCoroutine("AnimateDots");
            }


            yield return new WaitForSeconds(animTime);
        }
    }




}
