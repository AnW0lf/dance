using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class LikeCounter : MonoBehaviour
{
    [SerializeField] private int _count = 0;
    [SerializeField] private TextMeshProUGUI _counter = null;
    [SerializeField] private Image _icon = null;
    [SerializeField] private float _iconPulseScale = 1.25f;
    [SerializeField] private float _iconPulseDuration = 0.3f;
    [SerializeField] private LeanTweenType _ltt = LeanTweenType.linear;

    private bool _doPulse = false;
    private bool _pulsing = false;
    private LTDescr _pulse = null;

    public UnityAction<int> OnCountChanged { get; set; } = null;
 
    public int Count
    {
        get => _count;
        set
        {
            if(_count != value)
            {
                _doPulse = true;
                OnCountChanged?.Invoke(value);
            }
            _count = value;
            _counter.text = _count.ToString();
        }
    }

    private void Start()
    {
        Count = _count;
    }

    private void Update()
    {
        if (_doPulse && !_pulsing)
        {
            _doPulse = false;
            if (_pulse != null) LeanTween.cancel(_pulse.uniqueId);
            _pulsing = true;
            _pulse = LeanTween.scale(_icon.gameObject, Vector3.one * _iconPulseScale, _iconPulseDuration / 2f)
                .setEase(_ltt)
                .setLoopPingPong(1)
                .setOnComplete(() => _pulsing = false);
        }
    }
}
