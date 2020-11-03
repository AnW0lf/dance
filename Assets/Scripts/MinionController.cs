using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MinionController : MonoBehaviour
{
    [SerializeField] private MusicPlayer _musicPlayer = null;
    [SerializeField] private Timer _timer = null;
    [Space(20)]
    [Header("Animation controller")]
    [SerializeField] private float _maxMiss = 0.7f;
    [SerializeField] private float _maxGood = 0.9f;
    [SerializeField] private float _maxPerfect = 1f;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private ProgressBar _progress = null;

    private bool _beginDance = false;

    public UnityAction OnMiss { get; set; } = null;
    public UnityAction OnGood { get; set; } = null;
    public UnityAction OnPerfect { get; set; } = null;
    public UnityAction OnTooSlow { get; set; } = null;

    private ReadOnlyCollection<BonusMove> _bonusMoves = null;

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
        else
        {
            if (!_timer.Active)
                _timer.Active = true;
            if (!_musicPlayer.Active)
                _musicPlayer.Play();
        }
        _bonusMoves = dance.BonusMoves;
        DanceId = dance.AnimationID;
        _beginDance = true;
    }

    public int DanceId
    {
        get => _animator.GetInteger("DanceId");
        private set => _animator.SetInteger("DanceId", value);
    }

    public bool TooSlow
    {
        get => _animator.GetBool("TooSlow");
        private set => _animator.SetBool("TooSlow", value);
    }

    private void DoMiss()
    {
        _animator.SetTrigger("Miss");
    }

    private void BeginDance()
    {
        _beginDance = false;
        _progress.Progress = 0f;
        if (_bonusMoves != null && _bonusMoves.Count > 0)
            _progress.SetBonusMoves(_bonusMoves);
        _animator.SetTrigger("Dance");
    }

    private void Update()
    {
        //print($"Animation tag {CurrentAnimationTag}");
        //print($"Animation duration {CurrentAnimationDuration} sec.");
        switch (CurrentAnimationTag)
        {
            case AnimationTag.DANCE:
                {
                    if (!_progress.Visible)
                        _progress.Visible = true;
                    _progress.Progress = CurrentAnimationProgress;

                    if (_progress.Progress >= 0.9f && _beginDance)
                        BeginDance();

                    if (_progress.Progress == 1f)
                    {
                        if (!_timer.TimeOver)
                            TooSlow = true;
                    }
                }
                break;
            case AnimationTag.IDLE:
                {
                    if (_progress.Visible)
                    {
                        _progress.Visible = false;
                        _progress.Progress = 0f;
                    }

                    if (_timer.TimeOver)
                    {
                        if (_musicPlayer.Active)
                            _musicPlayer.Stop();
                    }
                    else
                    {
                        if (TooSlow)
                        {
                            TooSlow = false;
                            OnTooSlow?.Invoke();
                        }
                        else if (_beginDance)
                            BeginDance();
                    }
                }
                break;
            case AnimationTag.MISS:
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
    public float CurrentAnimationDuration
    {
        get
        {
            var clip = _animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            return clip.length;
        }
    }

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

    private IEnumerator DelayedAction(float delay, UnityAction action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
