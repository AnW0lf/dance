using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionController : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;
    [SerializeField] private ProgressBar _progress = null;

    public void SetDance(Dance dance)
    {
        DanceId = dance.AnimationID;
        _progress.Visible = true;
        _progress.Progress = 0f;
    }

    private int DanceId
    {
        get => _animator.GetInteger("DanceId");
        set
        {
            _animator.SetInteger("DanceId", value);
            _animator.StartRecording(0);
        }
    }

    private void Update()
    {
        if (_progress.Progress == 1f)
            DanceId = 0;

        if (DanceId != 0)
        {
            if (!_progress.Visible)
                _progress.Visible = true;
            _progress.Progress = CurrentAnimationProgress;

            print($"{_progress.Progress}");
        }
        else if (_progress.Visible)
        {
            _progress.Visible = false;
            _progress.Progress = 0f;
        }
    }

    public float CurrentAnimationProgress => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
}
