using UnityEngine;
using System.Collections;
using System;

public class MinionDancePreview : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;
    [SerializeField] private MinionEventListener _eventListener = null;

    private void Start()
    {
        _eventListener.OnBegin += OnBegin;
        _eventListener.OnEnd += OnEnd;
    }

    public void ShowDancePreview(Dance dance)
    {
        _animator.transform.localPosition = Vector3.zero;
        _animator.transform.localEulerAngles = Vector3.zero;
        string stateName = $"Dance {dance.AnimationID}";

        _animator.Play(stateName, 0, 0f);
        //if (_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        //    _animator.Play(stateName, 0, 0f);
        //else
        //    _animator.CrossFade(stateName, 0.1f);
    }

    private void OnBegin(int arg0)
    {

    }

    private void OnEnd(int arg0)
    {
        _animator.CrossFade("Idle", 0.1f);
    }
}
