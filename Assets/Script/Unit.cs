using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour
{
    public Unitdata unitdata;
    [SerializeField] private TMP_Text PowerText;
    [SerializeField] private TMP_Text HpText;
    [SerializeField] private TMP_Text CountText;
    [SerializeField] private TMP_Text MpText;
    [SerializeField] private TMP_Text ShieldText;
    

    public int offensPower = 0;
    public int magicPower = 0;
    public int Max_Hp = 0;
    public int Cur_Hp = 0;
    public int Max_Mp = 0;
    public int Cur_Mp = 0;
    public int Evasion = 0;
    public int Critical = 0;
    public int Shield = 0;
    public List<int> Count = new List<int>();
    
    [SerializeField] private float Shakeduration = 0.5f;
    [SerializeField] private float strength = 0.5f;
    [SerializeField] private int vibrato = 10;
    [SerializeField] private float randomness = 90f;

    [HideInInspector]public Transform Movetarget; // 타겟 오브젝트
    [HideInInspector]public float Moveduration = 0.2f; // 이동하는 데 걸리는 시간
    [HideInInspector]public float delay = 0f; // 시작하기 전 대기 시간
    [HideInInspector]public Ease easeType = Ease.Linear; // 동작 방식

    private Vector3 originalPosition; // 이동하기 전 위치
    
    [HideInInspector] public GameObject Aim;
    [HideInInspector] public LineRenderer LR;
    [HideInInspector] public BattleManager BM;
    [HideInInspector] public EffectManager EM;
    [HideInInspector] public UnitManager UM;

    [HideInInspector]public bool isMyAtk;
    [HideInInspector]public bool isDie;

    private Tween outlineTween;
    private Tween moveTween;

    [TextArea]
    public string info;//유닛 설명
    

    // Start is called before the first frame update
    void Start() //각각 지정하는거임.
    {
        Aim = GameObject.Find("Aim"); // 캐릭터 공격하려고 적에 두면 그 적위치에 에임이미지 보이는거. 이거 임.
        LR = GameObject.Find("GameManager").GetComponent<LineRenderer>();//걍 각각 그 요소를 접근하게 편하게 하려고 한거임.
        BM = GameObject.Find("GameManager").GetComponent<BattleManager>();
        EM = GameObject.Find("GameManager").GetComponent<EffectManager>();
        UM = GameObject.Find("GameManager").GetComponent<UnitManager>();
        
        SetText();
    }

    private void Update()
    {
        

    }

    

    public void SetText()//이제 내 공격력이랑 체력이 잘 나와야하기때문에 텍스트를 갱신해주는거.
    {
       
        if (Cur_Hp < Max_Hp)// 피해를 받아서 풀피가 아닌 상태라면 체력텍스트 색이 노란색으로 나옴.
        {
            Color color = new Color(230 / 255f, 230 / 255f, 30 / 255f);
            HpText.color = color;
        }
        else if(Cur_Hp == Max_Hp)// 풀피인 상태라면 흰색 텍스트로 나옴.
        {
            Color color = new Color(1, 1, 1);
            HpText.color = color;
        }

        if (Cur_Mp == Max_Mp)//마나가 전부 채워진 상태라면 초록색으로 변경
        {
            Color color = new Color(30 / 255f, 230 / 255f, 30 / 255f);
            MpText.color = color;
        }
        else if(Cur_Mp < Max_Mp)
        {
            Color color = new Color(1, 1, 1);
            MpText.color = color;
        }

        if (Max_Mp == 0)//마나가 없는 캐릭이면 마나 정보가 안뜸.
        {
            Color color = new Color(1, 1, 1, 0);
            SpriteRenderer mp = transform.Find("MpImg").GetComponent<SpriteRenderer>();
            mp.color = color;
            MpText.color = color;
        }

        if (offensPower == 0)
        {
            if (magicPower != 0)
            {
                Color color = new Color(135 / 255f, 60 / 255f, 250 / 255f);
                SpriteRenderer power = transform.Find("PowerImg").GetComponent<SpriteRenderer>();
                power.color = color;
                PowerText.text = magicPower.ToString();
            }
        }
        else if(offensPower != 0)
        {
            Color color = new Color(250 / 255f, 140 / 255f, 60 / 255f);
            SpriteRenderer power = transform.Find("PowerImg").GetComponent<SpriteRenderer>();
            power.color = color;
            PowerText.text = offensPower.ToString();
        }

        if (Shield == 0)
        {
            Color color = new Color(1, 1, 1, 0);
            SpriteRenderer shield = transform.Find("ShieldImg").GetComponent<SpriteRenderer>();
            shield.color = color;
            ShieldText.color = color;
        }
        else
        {
            Color color = new Color(140/255f, 140/255f, 140/255f, 1);
            SpriteRenderer shield = transform.Find("ShieldImg").GetComponent<SpriteRenderer>();
            shield.color = color;
            color = Color.white;
            ShieldText.color = color;
            //shield.transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f, 1, 0);//애니
            //ShieldText.transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f, 1, 0);//애니
        }
        
        HpText.text = Cur_Hp.ToString();
        MpText.text = Cur_Mp + "/" + Max_Mp;
        SetCountText(false);
    }

    public void SetCountText(bool isAni)// 공격순서가 한번씩 다 돌아서 새로 지정해줄경우 띠용하는 애니를 넣기위해 카운트 텍스트만 새로 뺐음.
    {
        string count_text = "";
        Count.Sort();
        for (int i = 0; i < Count.Count; i++)
        {
            if (i == 0)
                count_text += Count[i];
            else
            {
                count_text += "|" + Count[i];
            }
        }
        CountText.text = count_text;
        if(isAni)
            CountText.transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f, 1, 0);// 이게 그 애니 
    }

    public void MoveTransform(Vector3 pos, float dotweenTime = 0, float delay = 0)
    {
       moveTween = transform.DOMove(pos, dotweenTime).SetEase(Ease.OutBack).SetDelay(delay).OnComplete(() => originalPosition = transform.position);
    }
    
    public void DoShake()// 맞으면 흔들게 하는 함수
    {
        Vector3 originpos = transform.position;
        transform.DOShakePosition(Shakeduration, strength, vibrato, randomness).OnComplete(() => transform.DOMove(originpos,0.2f));
    }
    
    public void MoveToTarget()// 때릴때 그 적 위치로 이동하는 함수.
    {
        moveTween.Complete();
        transform.DOMove(Movetarget.position, Moveduration).SetEase(easeType).SetDelay(delay).OnComplete(ReturnToOriginalPosition);// OnComplete가 이동할거 전부 이동할경우 다음걸 실횅
    }

    private void ReturnToOriginalPosition()//다시 내 원래 위치로 돌아오는것.
    {
        transform.DOMove(originalPosition, Moveduration).SetEase(easeType);
    }

    public void DoEnemyAttack(int turn)//이건 적 유닛이라면 우리 유닛을 공격하게 하는 함수.
    {
        if (CompareTag("Enemy"))//태그로 적일경우를 구분.
        {
            int target_num = Random.Range(0, UM.MyUnitObjects.Count);// 우리 아군 유닛중 랜덤으로 지정.
            GameObject target = UM.MyUnitObjects[target_num];// 그새끼 타겟으로 지정하고 아래 방식으로 공격.
    
            Movetarget = target.transform;
            MoveToTarget();
            EM.CreateSlashEffect(target.transform.position);

            int Misspercentage = Random.Range(0, 100);
            if(Misspercentage < target.GetComponent<Unit>().Evasion)
                BM.SetDamaged(true, 0, target.transform.gameObject);
            else//미스가 아닐경우
            {
                int Damage = 0;
                bool isoff = false;
                if (offensPower != 0)
                {
                    Damage = offensPower;
                    isoff = true;
                }
                else
                {
                    Damage = magicPower;
                    isoff = false;
                }
                
                int Criticalpercentage = Random.Range(0, 100);
                if (Criticalpercentage < Critical)  //치명타 확률 10퍼
                {
                    BM.SetDamaged(isoff, (int)(Damage * 1.5f), target.transform.gameObject, true);
                }
                else// 치명타가 아닐경우
                {
                    BM.SetDamaged(isoff, Damage, target.transform.gameObject);
                }
            }
            
            if(Cur_Mp < Max_Mp)
                Cur_Mp++;
            SetText();
            BM.startNextTurn(turn + 1);
        }
    }

    public void EnableMyAtk()
    {
        isMyAtk = true;
        OnOutLine();
    }

    public void disableMyAtk()
    {
        isMyAtk = false;
        OffOutLine();
    }

    public void OnOutLine()
    {
        if (isMyAtk)
        {
            outlineTween.Complete();
            outlineTween = DOTween.To(() => transform.Find("model").GetComponent<SpriteOutline>().outlineSize, x => transform.Find("model").GetComponent<SpriteOutline>().outlineSize = x, 12, 0.3f);
        }
    }

    public void OffOutLine()
    {
        outlineTween.Complete();
        outlineTween =  DOTween.To(() => transform.Find("model").GetComponent<SpriteOutline>().outlineSize, x => transform.Find("model").GetComponent<SpriteOutline>().outlineSize = x, 0, 0.3f);
    }
    
    public void OnDie()
    {
        isDie = true;//넥스트턴 함수 실행할때 죽은 유닛인지 판단하기 위함.
        DestroyOnBattle();
         
        SpriteRenderer model = transform.Find("model").GetComponent<SpriteRenderer>();
        SpriteRenderer frame = transform.Find("frame").GetComponent<SpriteRenderer>();
        SpriteRenderer power = transform.Find("PowerImg").GetComponent<SpriteRenderer>();
        SpriteRenderer hp = transform.Find("HpImg").GetComponent<SpriteRenderer>();
        SpriteRenderer mp = transform.Find("MpImg").GetComponent<SpriteRenderer>();

        float fadedur = 0.7f;
        power.DOFade(0, fadedur);
        hp.DOFade(0,fadedur);
        mp.DOFade(0, fadedur);
        PowerText.DOFade(0, fadedur);
        HpText.DOFade(0, fadedur);
        MpText.DOFade(0, fadedur);
        CountText.DOFade(0, fadedur);
        model.DOFade(0f, fadedur);
        frame.DOFade(0f, fadedur).OnComplete(() => Destroy(gameObject));
        
    }

    public void DestroyOnBattle()
    {
        if (CompareTag("Player"))
        {
            for (int i = 0; i < BM.myUnits.Count; i++) 
            {
                if (BM.myUnits[i] == this)
                {
                    BM.myUnits.RemoveAt(i);
                }
            }

            for (int i = 0; i < UM.MyUnitObjects.Count; i++)
            {
                if (UM.MyUnitObjects[i] == gameObject)
                {
                    UM.MyUnitObjects.RemoveAt(i);
                }
            }

            for (int i = 0; i < UM.MyUnitList.Count; i++) 
            {
                if (UM.MyUnitList[i] == unitdata)
                {
                    UM.MyUnitList.RemoveAt(i);
                }
            }
        }

        if (CompareTag("Enemy"))
        {
            for (int i = 0; i < BM.enemyUnits.Count; i++) 
            {
                if (BM.enemyUnits[i] == this)
                {
                    BM.enemyUnits.RemoveAt(i);
                }
            }

            for (int i = 0; i < UM.EnemyUnitObjects.Count; i++)
            {
                if (UM.EnemyUnitObjects[i] == gameObject)
                {
                    UM.EnemyUnitObjects.RemoveAt(i);
                }
            }

            for (int i = 0; i < UM.EnemyUnitList.Count; i++) 
            {
                if (UM.EnemyUnitList[i] == unitdata)
                {
                    UM.EnemyUnitList.RemoveAt(i);
                }
            }
        }
    }

    void OnDestroy()
    {
        UM.UnitPosRefresh(CompareTag("Player"));
    }
}
