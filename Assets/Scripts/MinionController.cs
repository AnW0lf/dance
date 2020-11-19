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

    private bool _hasEnd = false;
    private Dance _currentDance = null;
    private Coroutine _bonusMovesProcessing = null;

    public UnityAction<Dance> OnMiss { get; set; } = null;
    public UnityAction<Dance> OnGood { get; set; } = null;
    public UnityAction<Dance> OnPerfect { get; set; } = null;
    public UnityAction<Dance> OnTooSlow { get; set; } = null;
    public UnityAction<Dance> OnSetDance { get; set; } = null;

    private void Start()
    {
        _eventListener.OnBegin += OnBegin;
        _eventListener.OnEnd += OnEnd;

        _eventListener.OnMissBegin += OnMissBegin;
        _eventListener.OnMissEnd += OnMissEnd;

        SetNextState(StateType.STRETCH);
        BeginNextStage();
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
        _currentDance = dance;
        OnSetDance?.Invoke(_currentDance);

        if (CurrentAnimationTag == MinionAnimationTag.DANCE)
        {
            float currentProgress = CurrentAnimationProgress;
            if (currentProgress < _maxMiss)
            {
                OnMiss?.Invoke(_currentDance);

                SetNextState(StateType.MISSTAKE);
                OnEnd(_currentDance.AnimationID);
                SetNextState(StateType.DANCE, _currentDance.AnimationID);
            }
            else
            {
                if (currentProgress < _maxGood)
                {
                    OnGood?.Invoke(_currentDance);
                }
                else if (currentProgress < _maxPerfect)
                {
                    OnPerfect?.Invoke(_currentDance);
                    Instantiate(_perfectEffect, _animator.transform);
                }

                SetNextState(StateType.DANCE, _currentDance.AnimationID);
            }
        }
        else if (CurrentAnimationTag == MinionAnimationTag.MISSTAKE)
        {
            OnGood?.Invoke(_currentDance);
            SetNextState(StateType.DANCE, _currentDance.AnimationID);
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

            OnGood?.Invoke(_currentDance);
            SetNextState(StateType.DANCE, _currentDance.AnimationID);
            BeginNextStage();
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

    public void EndGame(bool win)
    {
        _currentDance = null;

        if (win)
        {
            foreach (var winEffect in _winEffects)
                if (!winEffect.activeSelf) winEffect.SetActive(true);
        }
        else
        {

            SetNextState(StateType.DEFEAT);
            BeginNextStage();
        }
    }

    private void Update()
    {
        MinionAnimationTag animationTag = CurrentAnimationTag;

        if (animationTag != MinionAnimationTag.DANCE)
        {
            if (_progress.Visible)
            {
                _progress.Visible = false;
                _progress.Progress = 0f;
            }

            if (animationTag == MinionAnimationTag.IDLE)
            {
                if (_timer.TimeOver && _currentState != StateType.VICTORY)
                {
                    SetNextState(StateType.VICTORY);
                    BeginNextStage();
                }
                else if (_hasNextState)
                {
                    OnGood?.Invoke(_currentDance);
                    BeginNextStage();
                }
            }
        }
        else
        {
            _progress.Progress = CurrentAnimationProgress;
        }
    }

    #region NewDanceSystem
    private StateType _currentState = StateType.IDLE;
    private int _currentDanceId = 0;

    private StateType _nextState = StateType.IDLE;
    private int _nextDanceId = 0;

    private bool _hasNextState = false;

    private void BeginState(string stateName)
    {
        if (CurrentAnimatorStateIsName(stateName))
            _animator.CrossFade($"{stateName} 0", 0.1f);
        else
            _animator.CrossFade(stateName, 0.1f);
    }
    private void BeginState(StateType stateType)
    {
        string stateName = string.Empty;

        switch (stateType)
        {
            case StateType.IDLE:
                stateName = "Idle";
                break;
            case StateType.STRETCH:
                stateName = "Stretch";
                break;
            case StateType.DANCE:
                stateName = $"Dance {_currentDanceId}";
                break;
            case StateType.MISSTAKE:
                stateName = "Misstake";
                break;
            case StateType.VICTORY:
                stateName = "Victory";
                break;
            case StateType.DEFEAT:
                stateName = "Defeat";
                break;
        }

        BeginState(stateName);
    }

    public void SetNextState(StateType nextState)
    {
        _nextState = nextState;
        _hasNextState = true;
    }

    public void SetNextState(StateType nextState, int nextDanceId)
    {
        _nextState = nextState;
        _nextDanceId = nextDanceId;
        _hasNextState = true;
    }

    private void BeginNextStage()
    {
        _hasNextState = false;
        _currentState = _nextState;
        _currentDanceId = _nextDanceId;
        BeginState(_currentState);
    }

    private bool TryBeginNextStage()
    {
        if (_hasNextState)
        {
            BeginNextStage();
            return true;
        }
        else return false;
    }

    private void OnBegin(int id)
    {
        print($"Begin {id}");

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

        _hasEnd = false;
    }

    private void OnEnd(int id)
    {
        print($"End {id}");
        if (_hasEnd) return;

        if (_timer.TimeOver)
        {
            SetNextState(StateType.VICTORY);
            BeginNextStage();
        }
        else
        {
            bool fail = !TryBeginNextStage();
            if (fail)
            {
                _currentDance = null;
                SetNextState(StateType.MISSTAKE);
                BeginNextStage();
            }
        }

        _hasEnd = true;
        _progress.Visible = false;
        _progress.Clear();
    }

    private void OnMissBegin()
    {
        print($"Begin miss");

        StartCoroutine(SetZeroPosition());

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

        if (_timer.TimeOver)
        {
            SetNextState(StateType.VICTORY);
            BeginNextStage();
        }
        else
        {
            bool fail = !TryBeginNextStage();
            if (fail)
            {
                SetNextState(StateType.IDLE);
                BeginNextStage();
            }
        }
    }
    #endregion NewDanceSystem

    #region AnimationStateInfo
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

    public bool CurrentAnimatorStateIsName(string stateName) => CurrentAnimatorStateInfo.IsName(stateName);

    public float CurrentAnimationProgress
    {
        get
        {
            AnimatorStateInfo stateInfo = CurrentAnimatorStateInfo;
            return stateInfo.normalizedTime * 5f / 4f - 1f / 8f;
        }
    }

    public enum MinionAnimationTag { IDLE, DANCE, MISSTAKE, END, UNTAGGED }
    public MinionAnimationTag CurrentAnimationTag
    {
        get
        {
            AnimatorStateInfo stateInfo = CurrentAnimatorStateInfo;
            if (stateInfo.IsTag("Dance")) return MinionAnimationTag.DANCE;
            if (stateInfo.IsTag("Idle")) return MinionAnimationTag.IDLE;
            if (stateInfo.IsTag("Misstake")) return MinionAnimationTag.MISSTAKE;
            if (stateInfo.IsTag("End")) return MinionAnimationTag.END;
            return MinionAnimationTag.UNTAGGED;
        }
    }
    #endregion AnimationStateInfo

    #region BonusMove
    public void BonusMove()
    {
        OnGood?.Invoke(_currentDance);
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
    #endregion BonusMove
}

public enum StateType { IDLE, STRETCH, DANCE, MISSTAKE, VICTORY, DEFEAT }
