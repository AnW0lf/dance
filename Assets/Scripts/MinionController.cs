using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MinionController : MonoBehaviour
{
    [SerializeField] private float _maxMiss = 0.7f;
    [SerializeField] private float _maxGood = 0.9f;
    [SerializeField] private float _maxPerfect = 1f;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private ProgressBar _progress = null;

    private bool _beginDance = false;
    private bool _tooSlow = false;

    public UnityAction OnMiss { get; set; } = null;
    public UnityAction OnGood { get; set; } = null;
    public UnityAction OnPerfect { get; set; } = null;
    public UnityAction OnTooSlow { get; set; } = null;

    private void Start()
    {
        OnMiss += DoMiss;
        OnTooSlow += DoMiss;
    }

    public void SetDance(Dance dance)
    {
        if (CurrentAnimationTag == AnimationTag.DANCE)
        {
            float currentProgress = CurrentAnimationProgress;
            if (currentProgress < _maxMiss) OnMiss?.Invoke();
            else if (currentProgress < _maxGood) OnGood?.Invoke();
            else if (currentProgress < _maxPerfect) OnPerfect?.Invoke();
            else OnTooSlow?.Invoke();
        }
        DanceId = dance.AnimationID;
        _beginDance = true;
    }

    public int DanceId
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
        print("begin");
        _beginDance = false;
        _progress.Progress = 0f;
        _animator.SetTrigger("Dance");
    }

    private void Update()
    {
        //print($"Animation tag {CurrentAnimationTag}");
        switch (CurrentAnimationTag)
        {
            case AnimationTag.DANCE:
                {
                    if (!_progress.Visible)
                        _progress.Visible = true;
                    _progress.Progress = CurrentAnimationProgress;
                    if (_progress.Progress == 1f) _tooSlow = true;
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

                    if (_tooSlow)
                    {
                        _tooSlow = false;
                        DoMiss();
                    }
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
