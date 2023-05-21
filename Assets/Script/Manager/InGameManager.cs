using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEditor;

public class InGameManager : MonoBehaviour
{
    public Image TextBox;
    public TMP_Text TextBox_text;

    public bool isoffens;

    public int Lv = 1;
    public int Max_Exp = 2;
    public int Cur_Exp = 0;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void ContinueButtonClick()
    {
        Camera.main.GetComponent<Animator>().SetTrigger("Start");
        StartCoroutine(OpenTextBox("양장웅이 습격했다.",true));
        
    }

    public IEnumerator OpenTextBox(string text, bool isBattleStart)
    {
        TextBox_text.text = "";
        yield return new WaitForSeconds(2);
        
        TextBox.DOFade(0.7f, 0.5f);
        
        yield return new WaitForSeconds(0.5f);
        TextBox_text.color = new Color(1, 1, 1, 1);
        TextBox_text.GetComponent<TextAni>().SetText(text);
        
        yield return new WaitForSeconds(3f);
        if (isBattleStart)
        {
            CloseTextBox();
            GetComponent<BattleManager>().StartBattle();
        }
    }

    public void CloseTextBox()
    {
        TextBox.DOFade(0f, 0.5f);
        TextBox_text.DOFade(0, 0.5f);
    }

    public bool GetExp(int exp)
    {
        Cur_Exp += exp;
        if (Cur_Exp >= Max_Exp)
        {
            Lv++;
            Cur_Exp -= Max_Exp;
            Max_Exp = Lv * 2;
            return true;
        }

        return false;
    }

    
}
