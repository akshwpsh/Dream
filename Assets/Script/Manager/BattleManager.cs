using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public Image FadeImg;
    public GameObject DamagePopup;
    public Image TurnIcon;
    public TMP_Text TurnText;
    public Image RoundIcon;
    public TMP_Text RoundText;
    
    private GameObject AttackUnit;

    public List<Unit> enemyUnits;
    public List<Unit> myUnits;
    public List<Unit> allUnits;
    
    public int Cur_turn;
    public int Cur_Round;
    
    //캐릭터 정보
    public GameObject infoPanel;
    public TMP_Text Nameinfo_Text;
    public TMP_Text Hpinfo_Text;
    public TMP_Text Mpinfo_Text;
    public TMP_Text Skillinfo_Text;
    public TMP_Text Evasioninfo_Text;
    public TMP_Text Criticalinfo_Text;
    public Image Typeinfo;
    [HideInInspector]public bool isinfo;
    private float infoTime = 0f;
    private Transform hitinfoUnit;
    
    //승리창
    public GameObject WinPanel;
    public Image EXP_Bar;
    public TMP_Text Lv_Text;
    public TMP_Text EXP_Text;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue);
            if (Physics.Raycast(ray, out hit))
            {
                hitinfoUnit = hit.transform;
            }
        }
        
        if (Input.GetMouseButton(0)) // 마우스 좌클릭을 했을시
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == hitinfoUnit)
                {
                    if (infoTime < 1f)
                        infoTime += Time.deltaTime;
                    else
                    {
                        isinfo = true;
                        Setinfo(hitinfoUnit.GetComponent<Unit>());
                    }
                        
                }
                else
                {
                    isinfo = false;
                    infoTime = 0;
                    infoPanel.SetActive(false);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isinfo)
            {
                isinfo = false;
                infoTime = 0;
                UnSetinfo();
            }
        }
    }
    
    public void Setinfo(Unit unit)
    {
        
        Nameinfo_Text.text = unit.unitdata.name;
        Hpinfo_Text.text = unit.Cur_Hp + "/" + unit.Max_Hp;
        Mpinfo_Text.text = unit.Cur_Mp + "/" + unit.Max_Mp;
        Evasioninfo_Text.text = unit.Evasion + "%";
        Criticalinfo_Text.text = unit.Critical + "%";
        Skillinfo_Text.text = unit.info;

        if (unit.offensPower != 0 && unit.magicPower == 0)
        {
            Color color = new Color(250 / 255f, 140 / 255f, 60 / 255f);
            Typeinfo.color = color;
        }
        else if(unit.offensPower == 0 && unit.magicPower != 0)
        {
            Color color = new Color(135 / 255f, 60 / 255f, 250 / 255f);
            Typeinfo.color = color;
        }

        RectTransform panelRect = infoPanel.GetComponent<RectTransform>();
        float startXPosition = Screen.width + panelRect.rect.width;
        float endXPosition = Screen.width/2f - (panelRect.rect.width/2f + 10);
        panelRect.anchoredPosition = new Vector2(startXPosition, panelRect.anchoredPosition.y);
        
        infoPanel.SetActive(true);
        panelRect.DOAnchorPosX(endXPosition, 0.5f);
    }

    public void UnSetinfo()
    {
        RectTransform panelRect = infoPanel.GetComponent<RectTransform>();
        float startXPosition = Screen.width + panelRect.rect.width;
        panelRect.DOAnchorPosX(startXPosition, 0.5f).OnComplete(() => infoPanel.SetActive(false));
    }
    
    public void SetEnemy()
    {
        GetComponent<UnitManager>().EnemyUnitList.Clear();
        //GetComponent<UnitManager>().AddUnit(false, "어비스", 0, 20, 30, 0, 0,30, 10);
        //GetComponent<UnitManager>().AddUnit(false, "장웅",10,0,70, 0, 0,10, 10);
        GetComponent<UnitManager>().AddUnit(false, "양",20,0,50, 0, 0,10, 10); //임시로 3마리 그냥 넣어둠. 이제 랜덤으로 들어가게 해야겠죠? 넹

    }
    
    public void StartBattle()
    {
        FadeImg.DOFade(0.6f, 0.5f);// 배경 어두워지기
        RoundIcon.DOFade(1, 0.5f);
        RoundText.DOFade(1, 0.5f);
        
        SetEnemy();//적 구선을 설정하는것.
        GetComponent<UnitManager>().SpawnUnits();// 이제 적 구성에 맞게 유닛을 소환해줍니다. 방금 이거 하나 설명함.
        SetCounts();// 이제 그 공격 순서를 정해주는것. 처음 시작할때 한번 정해줘야겠죠.
        Cur_turn = 1;// 현재 공격해야하는 순서를 1이라고 지정.
        Cur_Round = 1;
        DOVirtual.DelayedCall(1.0f, () => startNextTurn(Cur_turn));//딜레이드 콜이라고 딜레이를 준다음 늦게 해당 함수를 실행하게 하는것. 그냥 기능 2개 붙인거네 넹
    }

    public void SetCounts()
    {
        allUnits.Clear();
        // Create a list of available unit numbers
        List<int> availableNumbers = new List<int>(); // 수를 저장할 리스트 생성
        for (int i = 1; i <= 6; i++)
        {
            availableNumbers.Add(i); // 1부터 6까지 그 리스트에 각각 저장. 지금 이 리스트는 1,2,34, ... 순서대로 저장돼있음
            allUnits.Add(null);
        }

        // Assign random numbers to enemy units
        foreach (Unit enemyUnit in enemyUnits)
        {
            int randomIndex = Random.Range(0, availableNumbers.Count);//이제 1부터 6까지 저장한 리스트에서 랜덤으로 하나 고름.
            int randomNumber = availableNumbers[randomIndex];// 랜덤넘버라는 변수에 그 고른 수를 저장.
            enemyUnit.Count.Add(randomNumber);// 그리고 그 유닛에게 랜덤으로 봅은 순서를 지정.
            availableNumbers.RemoveAt(randomIndex);// 그리고 남은 수에서 다시 지정해야하기 때문에 리스트에서 뽑은건 제거.확인
            allUnits[randomNumber -1] = enemyUnit;
        }

        // Assign random numbers to my units
        foreach (Unit myUnit in myUnits)// 이건 똑같은데 적.
        {
            int randomIndex = Random.Range(0, availableNumbers.Count);
            int randomNumber = availableNumbers[randomIndex];
            myUnit.Count.Add(randomNumber);
            availableNumbers.RemoveAt(randomIndex);
            allUnits[randomNumber-1] = myUnit;

        }

        // If there are less than 3 units on either team, assign remaining numbers to the team with fewer units
        if (enemyUnits.Count < 3)// 이제 만약에 유닛 수가 3마리보다 적다면 공격권을 더 가지기 때문에 만든 조건.
        {
            for (int i = 0; i < 3 - enemyUnits.Count; i++)//유닛 수가 부족한 수만큼 반복.지금 전투로 인해 유닛이 죽었다가 다시 이 걸 실행하면 빈수가 생긴다는 소리.
            {
                int randomIndex = Random.Range(0, enemyUnits.Count);// 남은 유닛중에서 수를 추가로 줄 유닛을 랜덤으로 뽑음.
                int randomUnmder_Index = Random.Range(0, availableNumbers.Count);
                int randomNumber = availableNumbers[randomUnmder_Index]; // 이제 남은수의 처음껄 저장. 이걸 랜덤으로 뽑는걸로 바꿔야할듯. 
                enemyUnits[randomIndex].GetComponent<Unit>().Count.Add(randomNumber); // 그 저장한걸 랜덤으로 봅은 유닛에게 줌.
                availableNumbers.RemoveAt(randomUnmder_Index); // 그리고 남은수의 처음껄 지움. 
                allUnits[randomNumber-1] = enemyUnits[randomIndex];
            }
        }
        if (myUnits.Count < 3)
        {
            for (int i = 0; i < 3 - myUnits.Count; i++)
            {
                int randomIndex = Random.Range(0, myUnits.Count);
                int randomUnmder_Index = Random.Range(0, availableNumbers.Count);
                int randomNumber = availableNumbers[randomUnmder_Index]; // 이제 남은수의 처음껄 저장. 이걸 랜덤으로 뽑는걸로 바꿔야할듯. 
                myUnits[randomIndex].GetComponent<Unit>().Count.Add(randomNumber);
                availableNumbers.RemoveAt(randomUnmder_Index);
                allUnits[randomNumber-1] = myUnits[randomIndex];
            }
        }
        Debug.Log("List Count:" + allUnits.Count);
        Debug.Log("List contents: " + string.Join(",", allUnits));
    }

    void SetWinPanel()
    {
        InGameManager gm = GetComponent<InGameManager>();
        Lv_Text.text = gm.Lv.ToString();
        EXP_Text.text = gm.Cur_Exp + "/" + gm.Max_Exp;
        EXP_Bar.fillAmount = gm.Cur_Exp / gm.Max_Exp;
        RectTransform panelts = WinPanel.GetComponent<RectTransform>();
        panelts.position = new Vector3(0, 10, 0);

        WinPanel.SetActive(true);
        panelts.DOMove(new Vector3(0, 0, 0), 0.5f);
        
    }
    public void startNextTurn(int turn) // 이건 유닛이 공격을 마치고 다음 턴 하라고 실행해줄꺼임.
    {
        if (enemyUnits.Count == 0 || myUnits.Count == 0)
        {
            if (enemyUnits.Count == 0)
            {
                UnitManager um = GetComponent<UnitManager>();
                foreach (GameObject unitObject in um.MyUnitObjects)
                {
                    unitObject.transform.DOMove(new Vector2(unitObject.transform.position.x, -10),0.5f).SetDelay(1f);
                }

                RoundIcon.DOFade(0, 0.5f);
                RoundText.DOFade(0, 0.5f);
                Invoke("SetWinPanel", 2f);
            }
            return;
        }
        
        if (turn == 7)// 마지막 순서였다면 다음 라운드로 새로 수를 지정하게 할꺼임.
        {
            DOVirtual.DelayedCall(2.0f, NextRound);

            return;//리턴으로 아래 내용은 실행하지 못하도록 반환.
        }
        Cur_turn = turn; // 다음턴으로 현재 턴을 변경.
        if (allUnits[turn - 1].isDie)//현재 턴 유닛이 죽어서 없다면 다음턴으로 바로 넘기기.
        {
            Cur_turn++;
            if (Cur_turn >= 7)
            {
                DOVirtual.DelayedCall(2.0f, NextRound);

                return;
            }
        }
        DOVirtual.DelayedCall(1f, PopupTurn);//빨간 칼로 숫자 뜨는거 실행시킨느거임. 가운데 뜨는거.
        Unit unit = allUnits[Cur_turn - 1];
        
        if (unit.CompareTag("Player"))
        {
            DOVirtual.DelayedCall(3.0f, ()=> unit.EnableMyAtk());//공격하게 허용.
        }
        else if(unit.CompareTag("Enemy"))
        {
            DOVirtual.DelayedCall(3.0f, ()=> unit.DoEnemyAttack(Cur_turn));
        }
        /*
        for (int i = 0; i < myUnits.Count; i++)//이건 나중에 수정할껀데 일단 설명해줌. 내 유닛수만큼 계속 돎.
        {
            Unit unit = myUnits[i].GetComponent<Unit>();
            for (int j = 0; j < unit.Count.Count; j++)// 유닛의 카운트도 리스트로 해놨는데 여러개 가쥘수도 있기 때문임. 그래서 가지고 있는 수만큼 반복하는거.
            {
                if (unit.Count[j] == Cur_turn)//만약 그 유닛이 지금 턴의 수를 가지고 있다면
                {
                    DOVirtual.DelayedCall(3.0f, ()=> unit.EnableMyAtk());//공격하게 허용.
                    return;// 이미 찾았으니 밑에껀 샐행 안해도 되니 반환.
                }
            }
        }
        
        for (int i = 0; i < enemyUnits.Count; i++)//이건 적 유닛
        {
            Unit unit = enemyUnits[i].GetComponent<Unit>();
            for (int j = 0; j < unit.Count.Count; j++)
            {
                if (unit.Count[j] == Cur_turn)
                {
                    DOVirtual.DelayedCall(3.0f, ()=> unit.DoEnemyAttack(j));
                    return;
                }
            }
        }// 근데 이 방식은 매우 비효율적인 검사 방식이라. 리스트를 한개 더만들어서 좀더 쉽게 턴 지정하게 만드는게 좋을듯. 그냥 턴 순서대로 리스트를 넣는 방식이 더 편할듯.
        //나중에 같이 수정하자구.
        */
    }

    public void NextRound()
    {
        ClearCounts();
        SetCounts();
        Cur_turn = 1;
        Cur_Round++;
        RoundText.text = Cur_Round.ToString();
        RefreshRound();
        RefreshTexts();
        DOVirtual.DelayedCall(1.0f, () => startNextTurn(Cur_turn));
    }
    // 이 밑은 나중에.
    public void SetDamaged(bool isoff, int Damage, GameObject target, bool isCri = false)
    {
        var unit = target.GetComponent<Unit>();
        if (Damage != 0)
            unit.DoShake();
        if (unit.Shield != 0)
        {
            if (unit.Shield >= Damage)
                unit.Shield -= Damage;
            else
            {
                unit.Shield = 0;
                unit.Cur_Hp -= Damage - unit.Shield;
            }
        }
        else
        {
            unit.Cur_Hp -= Damage;
        }
            
        unit.SetText();
        
        GameObject popup = Instantiate(DamagePopup, target.transform.position, Quaternion.identity);
        popup.GetComponent<DamagePopup>().SetDamage(Damage, isCri);
        
        if (unit.Cur_Hp <= 0)
        {
            unit.OnDie();
        }
    }

    public void PopupTurn()
    {
        TurnText.text = Cur_turn.ToString();
        
        TurnText.color = new Color(1f, 1f, 1f, 0f);
        TurnIcon.color = new Color(210/255f, 50/255f, 50/255f, 0f);
        // Animate the popup text
        Sequence popupSequence = DOTween.Sequence();
        popupSequence.Append(TurnText.DOColor(new Color(1f, 1f, 1f, 1f), 0.3f));
        popupSequence.Join(TurnIcon.DOColor(new Color(210/255f, 50/255f, 50/255f, 1f), 0.3f));
        popupSequence.AppendInterval(0.7f);
        popupSequence.Append(TurnText.DOColor(new Color(1f, 1f, 1f, 0f), 0.3f));
        popupSequence.Join(TurnIcon.DOColor(new Color(210/255f, 50/255f, 50/255f, 0f), 0.3f));

    }

    public void RefreshRound()
    {
        RoundIcon.transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f, 1, 0);// 이게 그 애니 
        RoundText.transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f, 1, 0);// 이게 그 애니
    }
    
    public void RefreshTexts()
    {
        foreach (var OB in GetComponent<UnitManager>().EnemyUnitObjects)
        {
            OB.GetComponent<Unit>().SetCountText(true);
        }

        foreach (var OB in GetComponent<UnitManager>().MyUnitObjects)
        {
            OB.GetComponent<Unit>().SetCountText(true);
        }
    }

    public void ClearCounts()
    {
        foreach (var OB in GetComponent<UnitManager>().EnemyUnitObjects)
        {
            OB.GetComponent<Unit>().Count.Clear();
        }

        foreach (var OB in GetComponent<UnitManager>().MyUnitObjects)
        {
            OB.GetComponent<Unit>().Count.Clear();
        }
    }
}
