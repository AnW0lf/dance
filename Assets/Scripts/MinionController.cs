using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionController : MonoBehaviour
{
    [SerializeField] private float _perfectBorder = 0.75f;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private ProgressBar _progress = null;

    private bool _beginDance = false;

    public void SetDance(Dance dance)
    {
        if (CurrentAnimationTag == AnimationTag.DANCE && CurrentAnimationProgress < _perfectBorder)
            DoMiss();
        DanceId = dance.AnimationID;
        _beginDance = true;
    }

    private int DanceId
    {
        get => _animator.GetInteger("DanceId");
        set => _animator.SetInteger("DanceId", value);
    }

    private void DoMiss()
    {
        _animator.SetTrigger("Miss");
    }

    private void BeginDance()
    {
        _beginDance = false;
        _progress.Progress = 0f;
        _animator.SetTrigger("Dance");
    }

    private void Update()
    {
        print($"Animation tag {CurrentAnimationTag}");
        switch (CurrentAnimationTag)
        {
            case AnimationTag.DANCE:
                {
                    if (!_progress.Visible)
                        _progress.Visible = true;
                    _progress.Progress = CurrentAnimationProgress;
                }
                break;
            case AnimationTag.IDLE:
                {
                    if (_progress.Visible)
                    {
                        _progress.Visible = false;
                        _progress.Progress = 0f;
                    }

                    if (_beginDance)
                        BeginDance();
                }
                break;
            default:
                {
                    if (_progress.Visible)
                    {
                        _progress.Visible = false;
                        _progress.Progress = 0f;
                    }
                }
                break;
        }
    }

    public float CurrentAnimationProgress => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

    private enum AnimationTag { IDLE, DANCE, MISS, UNTAGGED }
    private AnimationTag CurrentAnimationTag
    {
        get
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Dance")) return AnimationTag.DANCE;
            if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle")) return AnimationTag.IDLE;
            if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Miss")) return AnimationTag.MISS;
            return AnimationTag.UNTAGGED;
        }
    }
}
