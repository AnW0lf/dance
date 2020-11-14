using UnityEngine;
using System.Collections;
using System;

namespace Assets.Scripts.Test
{
    public class TestMinion : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;
        [SerializeField] private MinionEventListener _minionEventListener = null;

        private StateType _currentState = StateType.IDLE;
        private int _currentDanceId = 0;

        private StateType _nextState = StateType.IDLE;
        private int _nextDanceId = 0;

        private bool _hasNextState = false;

        private void BeginState(string stateName)
        {
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
        }

        private void OnEnd(int id)
        {
            print($"End {id}");
            bool fail = !TryBeginNextStage();
            if (fail)
            {
                SetNextState(StateType.MISSTAKE);
                BeginNextStage();
            }
        }

        private void OnMissBegin()
        {
            print($"Begin miss");
        }

        private void OnMissEnd()
        {
            print($"End miss");
            bool fail = !TryBeginNextStage();
            if (fail)
            {
                SetNextState(StateType.IDLE);
                BeginNextStage();
            }
        }

        private void Start()
        {
            _minionEventListener.OnBegin += OnBegin;
            _minionEventListener.OnEnd += OnEnd;
            _minionEventListener.OnMissBegin += OnMissBegin;
            _minionEventListener.OnMissEnd += OnMissEnd;

            StartCoroutine(Test());
        }

        private IEnumerator Test()
        {
            WaitForSeconds delay = new WaitForSeconds(4f);

            yield return delay;
            SetNextState(StateType.STRETCH);
            BeginNextStage();

            yield return delay;
            SetNextState(StateType.DANCE, 1);
            BeginNextStage();

            yield return delay;
            SetNextState(StateType.DANCE, 2);

            yield return delay;
            SetNextState(StateType.DANCE, 3);

            yield return delay;
            SetNextState(StateType.DANCE, 4);

            yield return delay;
            SetNextState(StateType.DANCE, 5);

            yield return delay;
            SetNextState(StateType.DANCE, 6);

            yield return delay;
            SetNextState(StateType.DANCE, 7);

            yield return delay;
            SetNextState(StateType.DANCE, 8);

            yield return new WaitForSeconds(10f);
            SetNextState(StateType.VICTORY);
        }
    }

    public enum StateType { IDLE, STRETCH, DANCE, MISSTAKE, VICTORY, DEFEAT }
}