using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNPlayer : MonoBehaviour
{
    BossManager bossManager;
    private Animator animator;

    //public Transform dustPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        bossManager = GameObject.Find("BossManager").GetComponent<BossManager>();
    }

    private void Update()
    {
        if (bossManager.isstop)
        {
            SetAnimationSpeed(0.3f);
        }
        else
        {
            SetAnimationSpeed(1);
        }
    }

    public void PlayAnimation(string animationState)
    {
        animator.Play(animationState);
    }

    // 애니메이션 속도를 조절한다.
    public void SetAnimationSpeed(float speed)
    {
        animator.speed = speed;
    }
}
