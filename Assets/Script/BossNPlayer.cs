using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNPlayer : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
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
