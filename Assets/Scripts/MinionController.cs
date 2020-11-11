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
    [SerializeField] private float _switchButtonsDelay = 0.2f;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private ProgressBar _progress = null;
    [SerializeField] private MinionEventListener _eventListener = null;
    [SerializeField] private CardSpawner _cardSpawner = null;
    [SerializeField] private BonusButton _bonusButton = null;
    [SerializeField] private GameObject _perfectEffect, _bonusEffect;
    [SerializeField] private LevelProgress _levelProgress = null;
    [SerializeField] private GameObject[] _winEffects = null;

    private bool _missing = false;
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

        if (_bonusMovesProcessing != null)
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

        StartCoroutine(SetZeroPosition());

        _missing = false;
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

    private IEnumerator SetZeroPosition()
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        while (_animator.transform.position != transform.position)
        {
            _animator.transform.position = transform.position;
            yield return wait;
        }
    }

    public void SetDance(Dance dance)
    {
        if (!_hasEnd && _currentDance != null)
            OnDanceEnd(_currentDance.AnimationID);

        _currentDance = dance;
        DanceId = _currentDance.AnimationID;
        _hasEnd = false;
        OnSetDance?.Invoke(_currentDance);

        if (CurrentAnimationTag == MinionAnimationTag.DANCE)
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
                Instantiate(_perfectEffect, _animator.transform);
                HasNextDance = true;
            }
            else
            {
                //OnTooSlow?.Invoke();
                //DoTooSlow();
            }
        }
        else
        {
            if (!_timer.Active)
                _timer.Active = true;
            if (!_musicPlayer.Active)
            {
                _musicPlayer.Play();
                _musicPlayer.FadeSound(0f, 1f);
            }
            HasNextDance = true;
        }

        _cardSpawner.Visible = false;

        StartCoroutine(DelayedAction(_switchButtonsDelay, () => _bonusButton.Visible = true));

        _bonusButton.OnClick = BonusMove;
    }

    private IEnumerator DelayedAction(float delay, UnityAction action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
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
        print("Do miss");
        _animator.SetTrigger("Miss");
    }

    private void DoTooSlow()
    {
        DoMiss();
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

    public void EndGame(bool win)
    {
        DanceId = 0;
        _currentDance = null;
        _animator.SetBool("Win", win);
        _animator.SetTrigger("TimeOver");

        if (win)
        {
            foreach (var winEffect in _winEffects)
                if (!winEffect.activeSelf) winEffect.SetActive(true);
        }
    }

    private void Update()
    {
        switch (CurrentAnimationTag)
        {
            case MinionAnimationTag.DANCE:
                {
                    _progress.Progress = CurrentAnimationProgress;

                    if (_progress.Progress >= 1f)
                    {
                        if (_timer.TimeOver)
                        {
                            DanceId = 0;
                            _currentDance = null;
                            _animator.SetTrigger("TimeOver");

                            if (!_musicPlayer.IsFading)
                                _musicPlayer.FadeSound(1f, 0.3f);
                        }
                        else if (!HasNextDance && !_missing)
                        {
                            _missing = true;
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

    public AnimatorStateInfo CurrentAnimatorStateInfo
    {
        get
        {
            AnimatorStateInfo stateInfo = _animator.GetNextAnimatorStateInfo(0);
            if (stateInfo.normalizedTime == 0)
                stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo;
        }
    }

    public float CurrentAnimationProgress
    {
        get
        {
            AnimatorStateInfo stateInfo = CurrentAnimatorStateInfo;
            return stateInfo.normalizedTime * 5f / 4f - 1f / 8f;
        }
    }

    public enum MinionAnimationTag { IDLE, DANCE, MISS, COMPLETE, UNTAGGED }
    public MinionAnimationTag CurrentAnimationTag
    {
        get
        {
            AnimatorStateInfo stateInfo = CurrentAnimatorStateInfo;
            if (stateInfo.IsTag("Dance")) return MinionAnimationTag.DANCE;
            if (stateInfo.IsTag("Idle")) return MinionAnimationTag.IDLE;
            if (stateInfo.IsTag("Miss")) return MinionAnimationTag.MISS;
            if (stateInfo.IsTag("Complete")) return MinionAnimationTag.COMPLETE;
            return MinionAnimationTag.UNTAGGED;
        }
    }

    public void BonusMove()
    {
        OnGood?.Invoke();
        Instantiate(_bonusEffect, _animator.transform);
        _bonusHasClick = true;
    }

    private bool _bonusHasClick = false;

    private IEnumerator BonusMovesProcessor(List<BonusMove> bonusMoves)
    {
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

        yield return StartCoroutine(DelayedAction(_switchButtonsDelay, () => _cardSpawner.Visible = true));
    }
}
