using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgress : MonoBehaviour
{
    [SerializeField] private InterfaceCounter _likeCounter = null;
    [SerializeField] private Slider _slider = null;
    [SerializeField] private int _needLikes = 0;

    void Start()
    {
        _slider.maxValue = _needLikes;
        _likeCounter.OnCountChanged += SetSlider;
    }

    private void SetSlider(int count)
    {
        _slider.value = count;
    }

    public float Progress => _slider.value / _slider.maxValue;
}
