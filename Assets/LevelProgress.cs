using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgress : MonoBehaviour
{
    [SerializeField] private LikeCounter _likeCounter;
    [SerializeField] private Slider _slider;
    [SerializeField] private int _needLikes;

    // Start is called before the first frame update
    void Start()
    {
        _slider.maxValue = _needLikes;
    }

    // Update is called once per frame
    void Update()
    {
        _slider.value = _likeCounter.Count;
    }
}
