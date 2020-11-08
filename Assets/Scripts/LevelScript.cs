﻿using UnityEngine;
using System.Collections;

public class LevelScript : MonoBehaviour
{
    [Header("Start level")]
    [SerializeField] private MoveCamera _moveCamera = null;
    [SerializeField] private LevelName _levelName = null;
    [SerializeField] private MovePanel[] _panels = null;

    [Space(20)]
    [Header("End level")]
    [SerializeField] private MinionController _minion = null;
    [SerializeField] private Timer _timer = null;
    [SerializeField] private MovePanel _bottomPanel = null;

    private bool _levelEnded = false;

    private void Start()
    {
        StartLevel();
    }

    private void LateUpdate()
    {
        if (_timer.TimeOver)
        {
            if (_bottomPanel.Visible) _bottomPanel.Visible = false;

            if (!_levelEnded && _minion.CurrentAnimationTag == MinionController.MinionAnimationTag.IDLE)
            {
                _levelEnded = true;
                EndLevel();
            }
        }
    }

    private void StartLevel()
    {
        _moveCamera.OnBegin += () => LeanTween.delayedCall(0.3f, () => _levelName.Show());
        _moveCamera.OnEnd += () =>
        {
            foreach (var panel in _panels) panel.Visible = true;
        };

        _moveCamera.Begin();
    }

    private void EndLevel()
    {
        FanController[] fans = FindObjectsOfType<FanController>();
        foreach (var fan in fans)
        {
            fan.LoopClamp = true;
            fan.CreateLike();
        }
    }
}
