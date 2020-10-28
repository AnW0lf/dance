using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class ProgressBar : MonoBehaviour
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
    public float Progress
    {
        get => _slider.value;
        set => _slider.value = value;
    }

    private void Start()
    {
        _slider.gameObject.SetActive(_visible);
    }
}
