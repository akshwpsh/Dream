using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEditor;

public class selectdata
{
    public int code;
    public string name;
    public string info;
    public Sprite icon;

    public selectdata(int code, string name, string info, Sprite icon)
    {
        this.code = code;
        this.name = name;
        this.info = info;
        this.icon = icon;
    }
}


public class InGameManager : MonoBehaviour
{
    public Image TextBox;
    public TMP_Text TextBox_text;

    public bool isoffens;

    public int Lv = 1;
    public int Max_Exp = 2;
    public int Cur_Exp = 0;

    public GameObject[] selectionItem;

    public Sprite[] Icons;

    public List<selectdata> Selectdatas = new List<selectdata>();
    
    public Image EXP_Bar;
    public TMP_Text Lv_Text;
    public TMP_Text EXP_Text;

    // Start is called before the first frame update
    void Start()
    {
        AddSelectdatas();
        SetScelections();
        SetExp();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void AddSelectdatas()
    {
        Selectdatas.Add(new selectdata(0, "전투", "적과 조우하여 싸웁니다. \n보다 많은 경험치와 \n보상을 받을 수 있습니다.", Icons[0])); 
        Selectdatas.Add(new selectdata(1, "보물상자", "무엇이 들어있을지\n 모릅니다.", Icons[1]));
        Selectdatas.Add(new selectdata(2, "물약", "아군의 체력을\n 회복시킵니다.", Icons[2]));
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

    public void SetExp()
    {
        Lv_Text.text ="Lv." + Lv;
        EXP_Text.text = Cur_Exp + "/" + Max_Exp;
        EXP_Bar.fillAmount = Cur_Exp / Max_Exp;
    }
    

    public void SetScelections()
    {
        List<int> numlist = new List<int>();
        for (int i = 0; i < Selectdatas.Count; i++)
        {
            numlist.Add(i);
        }

        foreach (var item in selectionItem)
        {
            int RandomNum = Random.Range(0, numlist.Count);
            int Num = numlist[RandomNum];
            numlist.RemoveAt(RandomNum);

            TMP_Text name = item.transform.Find("NameText").GetComponent<TMP_Text>();
            TMP_Text infoText = item.transform.Find("info").Find("Text").GetComponent<TMP_Text>();
            Image icon = item.transform.Find("Icon").GetComponent<Image>();

            name.text = Selectdatas[Num].name;
            infoText.text = Selectdatas[Num].info;
            icon.sprite = Selectdatas[Num].icon;
        }
    }
    
}
