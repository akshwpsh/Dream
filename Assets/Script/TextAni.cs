using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoreanTyper;
using TMPro;

public class TextAni : MonoBehaviour
{
    private TMP_Text Boxtext;

    private void Start()
    {
        Boxtext = GetComponent<TMP_Text>();
    }
    
    public void SetText(String text)
    {
        StartCoroutine(TypingText(text));
    }

    public IEnumerator TypingText(String text)
    {
        yield return new WaitForSeconds(1);
        //=======================================================================================================
        // Initializing | 초기화
        //=======================================================================================================
        string intext = text;
        
        //=======================================================================================================


        //=======================================================================================================
        //  Typing effect | 타이핑 효과
        //=======================================================================================================
       
            int strTypingLength = intext.GetTypingLength();

            for (int i = 0; i <= strTypingLength; i++)
            {
                Boxtext.text = intext.Typing(i);
                yield return new WaitForSeconds(0.03f);
            }

    }
}
