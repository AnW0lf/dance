using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgress : MonoBehaviour
{
    [SerializeField] private LikeCounter _likeCounter;
    [SerializeField] private Slider _slider;
    [SerializeField] private int _needLikes;

    void Start()
    {
        _slider.maxValue = _needLikes;
        _likeCounter.OnCountChanged += OnCountChanged;
    }

    private void OnCountChanged(int count)
    {
        _slider.value = count;
    }
}
