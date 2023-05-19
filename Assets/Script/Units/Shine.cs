using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shine : MonoBehaviour
{
    private Unit unit;
    private bool isLine;
    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 좌클릭을 했을시
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Player")&& hit.transform.gameObject == gameObject)
                {
                    for (int i = 0; i < unit.Count.Count; i++)
                    {
                        if (unit.BM.Cur_turn == unit.Count[i] && unit.isMyAtk)
                        {
                            unit.LR.positionCount = 2;
                            unit.LR.SetPosition(0,hit.transform.position);
                            isLine = true;
                        }   
                    }
                }
            }
        }

        if (Input.GetMouseButton(0)) // 마우스를 누르고 있을시
        {
            
            if (isLine)
            {
                Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 linepos = new Vector3(mousepos.x, mousepos.y, 0);
                unit.LR.SetPosition(1,linepos);
                
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.CompareTag("Enemy") || hit.transform.CompareTag("Player") )
                    {
                        if(hit.transform.gameObject == gameObject)
                            return;
                        unit.Aim.transform.position = hit.transform.position;
                        Color color = unit.Aim.GetComponent<SpriteRenderer>().color;
                        color.a = 1;
                        unit.Aim.GetComponent<SpriteRenderer>().color = color;
                    }
                }
                else
                {
                    Color color = unit.Aim.GetComponent<SpriteRenderer>().color;
                    color.a = 0;
                    unit.Aim.GetComponent<SpriteRenderer>().color = color;
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) // 마우스를 뗏을시 
        {
            if (isLine)
            {
                unit.LR.positionCount = 0; // 선 지우기
            
                Color color = unit.Aim.GetComponent<SpriteRenderer>().color;
                color.a = 0;
                unit.Aim.GetComponent<SpriteRenderer>().color = color;//에임 이미지 투명화

                isLine = false;
            
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.CompareTag("Enemy")) // 공격하기
                    {
                        for (int i = 0; i < unit.Count.Count; i++)
                        {
                            if (unit.BM.Cur_turn == unit.Count[i] && unit.isMyAtk)
                            {
                                GetComponent<Unit>().Movetarget = hit.transform;
                                GetComponent<Unit>().MoveToTarget();
                                if (unit.Cur_Mp == unit.Max_Mp)//스킬 사용(더블어택)
                                {
                                    DoAtk(hit);
                                    DOVirtual.DelayedCall(0.3f, () => DoAtk(hit, true));
                                    unit.Cur_Mp = 0;
                                }
                                else
                                {
                                    DoAtk(hit);
                                    if (unit.Cur_Mp < unit.Max_Mp)
                                        unit.Cur_Mp++;
                                }
                            
                            
                                unit.BM.startNextTurn(unit.Count[i] + 1);
                            
                                unit.SetText();
                                DOVirtual.DelayedCall(1.0f, ()=> unit.disableMyAtk());
                                break;
                            }
                        } 
                    }
                }
            }
            
        }
    }

    void DoAtk(RaycastHit hit, bool isSkill = false)
    {
        unit.EM.CreateSlashEffect(hit.transform.position);
        int Misspercentage = Random.Range(0, 100);
        if(Misspercentage < hit.transform.GetComponent<Unit>().Evasion)
            unit.BM.SetDamaged(true, 0, hit.transform.gameObject);
        else//미스가 아닐경우
        {
            int Damage = 0;
            bool isoff = false;
            if (unit.offensPower != 0)
            {
                Damage = unit.offensPower;
                if (isSkill)
                    Damage /= 2;
                isoff = true;
            }
            else
            {
                Damage = unit.magicPower;
                isoff = false;
            }
                
            int Criticalpercentage = Random.Range(0, 100);
            if (Criticalpercentage < unit.Critical) 
            {
                unit.BM.SetDamaged(isoff, (int)(Damage * 1.5f), hit.transform.gameObject, true);
            }
            else// 치명타가 아닐경우
            {
                unit.BM.SetDamaged(isoff, Damage, hit.transform.gameObject);
            }
        }
    }

 
}
