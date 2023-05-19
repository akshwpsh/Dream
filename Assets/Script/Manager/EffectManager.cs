using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EffectManager : MonoBehaviour
{

    public GameObject[] effects;
    // Start is called before the first frame update
    
    public void CreateHitEffect(Vector3 position)
    {
        GameObject hitEffect = Instantiate(effects[0], position, Quaternion.identity);
        Destroy(hitEffect, 2.0f); // Destroy the hit effect after 1 second
    }
    
    public void CreateSlashEffect(Vector3 position)
    {
        GameObject hitEffect = Instantiate(effects[1], position, Quaternion.identity);
        Destroy(hitEffect, 2.0f); // Destroy the hit effect after 1 second
    }
}
