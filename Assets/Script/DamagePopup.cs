using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float duration = 1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float startYOffset = 1f;
    [SerializeField] private Vector3 endScale = Vector3.one;

    private TMP_Text damageText;

    private void Awake()
    {
        damageText = GetComponent<TMP_Text>();
    }

    public void Start()
    {
        PlayAnimation();
    }

    public void SetDamage(int damageAmount, bool isCri = false)
    {
        if (isCri)
            damageText.color = Color.red;
        else
            damageText.color = Color.white;
            
        if(damageAmount == 0)
            damageText.text = "Miss";
        else
            damageText.text = "-"+damageAmount.ToString();
    }

    public void SetHeal(int healAmount)
    {
        damageText.color = Color.green;
        damageText.text = "+" + healAmount.ToString();
    }
    public void PlayAnimation()
    {
        transform.localScale = Vector3.zero;
        transform.position += new Vector3(0, startYOffset, 0);

       transform.DOScale(endScale, duration).SetEase(Ease.OutElastic);
       transform.DOMoveY(transform.position.y - startYOffset, duration).SetEase(Ease.OutCubic).OnComplete(() => DestroyAfterFade());
       damageText.DOFade(0f, fadeDuration).SetEase(Ease.InQuad).SetDelay(duration - fadeDuration);
    }

    private void DestroyAfterFade()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
}
