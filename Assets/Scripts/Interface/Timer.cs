using UnityEngine;
using System.Collections;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private bool _active = true;
    [SerializeField] private TextMeshProUGUI _timer = null;
    [SerializeField] private float _duration = 0f;

    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
        }
    }

    public bool TimeOver => _duration == 0f;

    public void Begin(float duration)
    {
        _duration = duration;
        Active = true;
        SetTimer(_duration);
    }

    private void SetTimer(float seconds)
    {
        int i_seconds = Mathf.CeilToInt(seconds);
        int i_minutes = i_seconds / 60;
        i_seconds = i_seconds % 60;
        if (i_seconds < 10)
            _timer.text = $"{i_minutes}:0{i_seconds}";
        else
            _timer.text = $"{i_minutes}:{i_seconds}";
    }

    private void Start()
    {
        SetTimer(_duration);
    }

    private void Update()
    {
        if (!_active) return;

        _duration -= Time.deltaTime;
        _duration = Mathf.Max(0f, _duration);

        SetTimer(_duration);

        if (_duration == 0f) Active = false;
    }
}
