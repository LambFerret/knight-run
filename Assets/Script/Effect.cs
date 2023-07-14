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
                // ���� 1.5��� Ŀ���ϴ�.
                transform.DOScale(transform.localScale * 1.5f, 0.7f)
                    .OnComplete(() =>
                    {
                        // Ŀ�� �� �ٽ� 0���� �پ��ϴ�.
                        transform.DOScale(0.1f, 1.5f)
                            .OnComplete(() =>
                            {
                                // ũ�Ⱑ 0.2 ���Ϸ� �پ��� �ı��մϴ�.
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
