using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Effect : MonoBehaviour
{
    public enum Case
    {
        Dust, Fight 
    }

    public Case this_case;

    private void Start()
    {
        StartCoroutine(EffectControl());
    }

    IEnumerator EffectControl()
    {
        switch (this_case)
        {
            case Case.Dust:
                // 먼저 1.5배로 커집니다.
                transform.DOScale(transform.localScale * 1.5f, 0.7f)
                    .OnComplete(() =>
                    {
                        // 커진 후 다시 0으로 줄어듭니다.
                        transform.DOScale(0.1f, 1.5f)
                            .OnComplete(() =>
                            {
                                // 크기가 0.2 이하로 줄어들면 파괴합니다.
                                if (transform.localScale.magnitude <= 0.2f)
                                {
                                    Destroy(gameObject);
                                }
                            });
                    });
                break;
            case Case.Fight:
                yield return new WaitForSeconds(0.5f);
                Destroy(gameObject);
                break;
        }
        yield return null;
    }
}
