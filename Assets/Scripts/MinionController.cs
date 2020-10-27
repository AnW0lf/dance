using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;

    public void SetDance(Dance dance)
    {
        _animator.SetInteger("DanceId", dance.AnimationID);
    }

    public float CurrentAnimationDuration => _animator.GetCurrentAnimatorStateInfo(0).length;
    public float CurrentAnimationSpeed => _animator.GetCurrentAnimatorStateInfo(0).speed;
}
