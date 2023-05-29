using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unitdata
{
    public String name = "";
    public int offensPower = 0;
    public int magicPower = 0;
    public int Max_HP = 0;
    public int Cur_Hp = 0;
    public int Max_Mp = 0;
    public int Cur_Mp = 0;
    public int Evasion = 0;//회피율
    public int Critical = 0;
}

public class UnitManager : MonoBehaviour
{
    public List<Unitdata> MyUnitList = new List<Unitdata>();// 내 유닛의 기본 구성과 핸재 스탯을 저장하는 리스트.
    public List<GameObject> MyUnitObjects = new List<GameObject>(); // 이건 이제 유닛이 생성됐을때 여기에 저장할껍니다. 오브젝트 자체를.
    public List<Unitdata> EnemyUnitList = new List<Unitdata>();//이건 적
    public List<GameObject> EnemyUnitObjects = new List<GameObject>();//적

    public GameObject 다경;//이건 이제 소환할 오브젝트를 설정하는 겁니다. 근데 이제 변수를 바꾸야죠. 한명은 다경 한명은 샤인으로 해놨으니.
    public GameObject 샤인;
    public GameObject 양;
    public GameObject 장웅;
    public GameObject 어비스;
    // Start is called before the first frame update
    void Start()
    {
        AddUnit(true, "다경", 5, 0, 100, 2, 0, 10, 10); // 일단 아군 유닛은 그냥 시작할때 임의로 설정해둠.
        AddUnit(true, "샤인", 20, 0, 80, 2, 0, 10, 10); //이친구는 공30에 체력 70인 캐릭.

        //SpawnUnits();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddUnit(bool isMyUnit, String name, int offens, int magic, int max_hp, int max_mp, int cur_mp, int Evasion, int Critical)
    {
        Unitdata unitdata = new Unitdata(); // 클래스인 유닉 데이터를 새로 만들고 각각 지정한다음.
        unitdata.name = name;
        unitdata.offensPower = offens;
        unitdata.magicPower = magic;
        unitdata.Max_HP = max_hp;
        unitdata.Cur_Hp = max_hp;
        unitdata.Max_Mp = max_mp;
        unitdata.Cur_Mp = cur_mp;
        unitdata.Evasion = Evasion;
        unitdata.Critical = Critical;

        var targetList = isMyUnit ? MyUnitList : EnemyUnitList;
        targetList.Add(unitdata);// 그 유닛데이터를 저장하는거임. 리스트에. 확인
    }

    public void SpawnUnits()
    {// 유닛 데이터를 가지고 오브젝트를 생성하는 함수.
        MyUnitObjects.Clear();// 오브젝트를 새로 쭉 생설할꺼기 때문에 기존 오브젝트 리스트를 초기화 해주는것.
        EnemyUnitObjects.Clear();
        GetComponent<BattleManager>().myUnits.Clear();//배들매니저에도 리스트가 있는데 이것도 초기화.
        GetComponent<BattleManager>().enemyUnits.Clear();
        
        for (int i = 0; i < MyUnitList.Count; i++)// 내 유닛정보가 들어가있는 수만큼 반복. 그럼 리스트 안에 있는 모든 오브젝트를 소환하겠죠
        {
            String name = MyUnitList[i].name;//각각 해당 유닛정보를 가져오는 과정.
            int offensPower = MyUnitList[i].offensPower;
            int magicPower = MyUnitList[i].magicPower;
            int Max_Hp = MyUnitList[i].Max_HP;
            int Cur_Hp = MyUnitList[i].Cur_Hp;
            int Max_Mp = MyUnitList[i].Max_Mp;
            int Cur_Mp = MyUnitList[i].Cur_Mp;
            int Evasion = MyUnitList[i].Evasion;
            int Critical = MyUnitList[i].Critical;
            
            GameObject gameObject = SetUnitPrefab(name);
            GameObject Unit = Instantiate(gameObject, new Vector3((MyUnitList.Count - 1) * -2.75f +i * 5.5f, -10,0), Quaternion.identity);// 이게 좀 중요한데 지정한 오브젝트틑 소환하는 함수.
            Unit.transform.SetParent(GameObject.Find("MyUnits").transform,false);// 이건 그냥 생으로 계속 소환되면 지저분해서 빈 오브젝트 자식으로 소환되게 하는 기능. 걍 정리용.
            Unit.GetComponent<Unit>().offensPower = offensPower;//스탯 설정
            Unit.GetComponent<Unit>().magicPower = magicPower;
            Unit.GetComponent<Unit>().Max_Hp = Max_Hp;
            Unit.GetComponent<Unit>().Cur_Hp = Cur_Hp;
            Unit.GetComponent<Unit>().Max_Mp = Max_Mp;
            Unit.GetComponent<Unit>().Cur_Mp = Cur_Mp;
            Unit.GetComponent<Unit>().Evasion = Evasion;
            Unit.GetComponent<Unit>().Critical = Critical;
            
            Unit.transform.tag = "Player";//해당 오브젝트의 태그를 아군인 플레이어로 설정. 이 태그를 통해 아군 유닛인지 적 유닛인지 구분할것임.
            Unit.GetComponent<Unit>().unitdata = MyUnitList[i];// 이건 나중에 유닛이 죽으면 리스트에서 해당 번호의 오브젝트를 지워야 하기 때문에 지정해둠.
            MyUnitObjects.Add(Unit);// 그리고 소환한 아군 오브젝트를 아군 오브젝트들을 모아두는 리스트에 저장.
            Unit.name = "MyUnit " + i; 
            GetComponent<BattleManager>().myUnits.Add(Unit.GetComponent<Unit>());// 그리고 배틀매니저에 유닛 모아두는 리스트에 저장. 이 리스트에서는 유닛의 공격순서를 지정할때 씀.
        }
        
        for (int i = 0; i < EnemyUnitList.Count; i++)//이건 아군과 똑같은 방식읜데 적일경우를 해둔것.
        {
            String name = EnemyUnitList[i].name;
            int offensPower = EnemyUnitList[i].offensPower;
            int magicPower = EnemyUnitList[i].magicPower;
            int Max_Hp = EnemyUnitList[i].Max_HP;
            int Cur_Hp = EnemyUnitList[i].Cur_Hp;
            int Max_Mp = EnemyUnitList[i].Max_Mp;
            int Cur_Mp = EnemyUnitList[i].Cur_Mp;
            int Evasion = EnemyUnitList[i].Evasion;
            int Critical = EnemyUnitList[i].Critical;

            GameObject gameObject = SetUnitPrefab(name);
            GameObject Unit = Instantiate(gameObject, new Vector3((EnemyUnitList.Count - 1) * -2.75f +i * 5.5f, 10,0), Quaternion.identity);
            Unit.transform.SetParent(GameObject.Find("EnemyUnits").transform,false);
            Unit.GetComponent<Unit>().offensPower = offensPower;
            Unit.GetComponent<Unit>().magicPower = magicPower;
            Unit.GetComponent<Unit>().Max_Hp = Max_Hp;
            Unit.GetComponent<Unit>().Cur_Hp = Cur_Hp;
            Unit.GetComponent<Unit>().Max_Mp = Max_Mp;
            Unit.GetComponent<Unit>().Cur_Mp = Cur_Mp;
            Unit.GetComponent<Unit>().Evasion = Evasion;
            Unit.GetComponent<Unit>().Critical = Critical;
            
            Unit.transform.tag = "Enemy";
            Unit.GetComponent<Unit>().unitdata = EnemyUnitList[i];
            EnemyUnitObjects.Add(Unit);
            Unit.name = "EnemyUnit " + i;
            GetComponent<BattleManager>().enemyUnits.Add(Unit.GetComponent<Unit>());
        }

        
        UnitPosAligment(true);// 유닛정렬 
        UnitPosAligment(false);
    }

    public void UnitPosAligment(bool isMyUnits)
    {
        float targetY = isMyUnits ? -2.7f : 1.7f; // 아군유닛일시 위치, 적 유닛일때 위치를 구분하기 위해 매개변수로 bool을 받음. 
        var targetUnits = isMyUnits ? MyUnitObjects : EnemyUnitObjects; //아군이면 진실, 적이면 거짓 예아

        for (int i = 0; i < targetUnits.Count; i++)
        {
            float targetX = (targetUnits.Count - 1) * -2f +i * 4f;// 정렬하기위한 x값을 지정하는것. 확인

            var targetUnit = targetUnits[i];
            targetUnit.GetComponent<Unit>().MoveTransform(new Vector3(targetX, targetY), 0.5f, i * 0.2f);// 유닛 위치를 서서히 이동.
        }
    }

    public void UnitPosRefresh(bool isMyUnits)
    {
        var targetUnits = isMyUnits ? MyUnitObjects : EnemyUnitObjects; //아군이면 진실, 적이면 거짓 예아
        for (int i = 0; i < targetUnits.Count; i++)
        {
            float targetX = (targetUnits.Count - 1) * -2f +i * 4f;// 정렬하기위한 x값을 지정하는것. 확인

            var targetUnit = targetUnits[i];
            targetUnit.GetComponent<Unit>().MoveTransform(new Vector3(targetX, targetUnit.transform.position.y), 0.5f);
        }
    }

    public GameObject SetUnitPrefab(string name)
    {
        switch (name)
        {
            case "다경":
                return 다경;
            case "샤인":
                return 샤인;
            case "양":
                return 양;
            case "장웅":
                return 장웅;
            case "어비스":
                return 어비스;

        }

        return null;
    }
}
