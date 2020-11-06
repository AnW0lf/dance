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
    [SerializeField] private MinionEventListener _eventListener = null;
    [SerializeField] private CardSpawner _cardSpawner = null;
    [SerializeField] private BonusButton _bonusButton = null;
    [SerializeField] private GameObject _perfectEffect, _bonusEffect;

    private bool _beginDance = false;
    private bool _hasEnd = true;
    private Dance _currentDance = null;
    private Coroutine _bonusMovesProcessing = null;

    public UnityAction OnMiss { get; set; } = null;
    public UnityAction OnGood { get; set; } = null;
    public UnityAction OnPerfect { get; set; } = null;
    public UnityAction OnTooSlow { get; set; } = null;
    public UnityAction<Dance> OnSetDance { get; set; } = null;

    private void Start()
    {
        _eventListener.OnBegin += OnDanceBegin;
        _eventListener.OnEnd += OnDanceEnd;

        _eventListener.OnMissBegin += OnMissBegin;
        _eventListener.OnMissEnd += OnMissEnd;
    }

    private void OnDanceBegin(int id)
    {
        print($"Begin id {id} in progress {CurrentAnimationProgress}");

        _progress.Clear();
        List<BonusMove> bonusMoves = new List<BonusMove>();
        if (_currentDance.BonusMoves != null)
        {
            foreach (var bonusMove in _currentDance.BonusMoves)
                bonusMoves.Add(bonusMove);
        }
        _progress.SetBonusMoves(bonusMoves);

        if(_bonusMovesProcessing != null)
            StopCoroutine(_bonusMovesProcessing);
        _bonusMovesProcessing = StartCoroutine(BonusMovesProcessor(bonusMoves));

        _progress.Visible = true;
    }

    private void OnDanceEnd(int id)
    {
        print($"End id {id} in progress {CurrentAnimationProgress}");

        _hasEnd = true;
        _progress.Visible = false;
        _progress.Clear();
        HasNextDance = false;
    }

    private void OnMissBegin()
    {
        print($"Begin miss");

        _progress.Visible = false;
        _progress.Clear();

        if (_bonusMovesProcessing != null)
        {
            StopCoroutine(_bonusMovesProcessing);
            _bonusMovesProcessing = null;
        }
    }

    private void OnMissEnd()
    {
        print($"End miss");
    }

    public void SetDance(Dance dance)
    {
        if (!_hasEnd && _currentDance != null)
            OnDanceEnd(_currentDance.AnimationID);

        _currentDance = dance;
        DanceId = _currentDance.AnimationID;
        _hasEnd = false;
        OnSetDance?.Invoke(_currentDance);

        if (CurrentAnimationTag == AnimationTag.DANCE)
        {
            float currentProgress = CurrentAnimationProgress;
            if (currentProgress < _maxMiss)
            {
                OnMiss?.Invoke();
                DoMiss();
                HasNextDance = true;
            }
            else if (currentProgress < _maxGood)
            {
                OnGood?.Invoke();
                HasNextDance = true;
            }
            else if (currentProgress < _maxPerfect)
            {
                OnPerfect?.Invoke();
                Instantiate(_perfectEffect, gameObject.transform);
                HasNextDance = true;
            }
            else
            {
                OnTooSlow?.Invoke();
                DoTooSlow();
            }
        }
        else
        {
            if (!_timer.Active)
                _timer.Active = true;
            if (!_musicPlayer.Active)
                _musicPlayer.Play();
            if (CurrentAnimationTag == AnimationTag.IDLE)
                HasNextDance = true;
        }
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

    private void DoTooSlow()
    {
        _animator.SetTrigger("Miss");
        DanceId = 0;
    }

    private bool _hasNextDance = false;
    public bool HasNextDance
    {
        get => _hasNextDance;
        set
        {
            _hasNextDance = value;
            if (_hasNextDance) _animator.SetTrigger("Dance");
        }
    }

    private void Update()
    {
        switch (CurrentAnimationTag)
        {
            case AnimationTag.DANCE:
                {
                    _progress.Progress = CurrentAnimationProgress;

                    if (_progress.Progress >= 1f)
                    {
                        if (_timer.TimeOver)
                        {
                            DanceId = 0;
                            _currentDance = null;
                            _animator.SetTrigger("TimeOver");
                        }
                        else if (!HasNextDance)
                        {
                            TooSlow = true;
                        }
                    }
                }
                break;
            case AnimationTag.IDLE:
                {
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
                            DoTooSlow();
                        }
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

    public float CurrentAnimationProgress
    {
        get
        {
            float progress = _animator.GetNextAnimatorStateInfo(0).normalizedTime;
            if (progress == 0)
                progress = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            progress = progress * 5f / 4f - 1f / 8f;
            return progress;
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

    public void BonusMove()
    {
        OnGood?.Invoke();
        _bonusHasClick = true;
    }

    private bool _bonusHasClick = false;

    private IEnumerator BonusMovesProcessor(List<BonusMove> bonusMoves)
    {
        _cardSpawner.Visible = false;
        _bonusButton.Visible = true;
        _bonusButton.OnClick = BonusMove;
        while (bonusMoves.Count > 0)
        {
            float progress = _progress.Progress;
            for (int i = bonusMoves.Count - 1; i >= 0; i--)
            {
                BonusMove bonus = bonusMoves[i];

                if (_bonusHasClick)
                {
                    if (bonus.Start <= progress)
                    {
                        _bonusButton.Interactable = false;
                        bonusMoves.RemoveAt(i);
                        _bonusHasClick = false;
                    }
                }
                else
                {
                    if (_bonusButton.Interactable)
                    {
                        if (bonus.End < progress)
                        {
                            _bonusButton.Interactable = false;
                            bonusMoves.RemoveAt(i);
                        }
                    }
                    else
                    {
                        if (bonus.Start <= progress && !_bonusButton.Interactable)
                        {
                            _bonusButton.Interactable = true;
                        }
                    }
                }
            }
            yield return null;
        }

        _bonusButton.Visible = false;
        _cardSpawner.Spawn(3);
        _cardSpawner.Visible = true;
    }
}
