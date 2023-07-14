using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class SoldierBehavior : MonoBehaviour
{

    public void Hit()
    {
    }

    private IEnumerator ToDestroy()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(other.transform.tag);
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("player met");
            Hit();
        }
    }
}
