using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class DanceProgressBar : MonoBehaviour
{
    [SerializeField] private bool _visible = false;
    [SerializeField] private Slider _slider = null;

    public bool Visible
    {
        get => _visible;
        set
        {
            if (_visible != value)
            {
                _visible = value;
                _slider.gameObject.SetActive(_visible);
            }
        }
    }

    public UnityAction OnBegin { get; set; } = null;
    public UnityAction OnEnd { get; set; } = null;

    public float Progress => _slider.value / _slider.maxValue;

    public void Begin(float duration)
    {
        Visible = true;
        _slider.value = 0f;
        _slider.minValue = 0f;
        _slider.maxValue = duration;
        OnBegin?.Invoke();
    }

    private void End()
    {
        Visible = false;
        OnEnd?.Invoke();
    }

    private void Start()
    {
        _slider.gameObject.SetActive(_visible);
    }

    private void Update()
    {
        if (Visible && _slider.value < _slider.maxValue)
        {
            _slider.value = Mathf.Clamp(_slider.value + Time.deltaTime, _slider.minValue, _slider.maxValue);
            if (_slider.value == _slider.maxValue) End();
        }
    }
}
