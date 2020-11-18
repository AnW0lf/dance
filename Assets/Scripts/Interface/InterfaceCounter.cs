using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class InterfaceCounter : MonoBehaviour
{
    [SerializeField] protected int _count = 0;
    [SerializeField] protected TextMeshProUGUI _counter = null;
    [SerializeField] protected Image _icon = null;
    [SerializeField] protected float _iconPulseScale = 1.25f;
    [SerializeField] protected float _iconPulseDuration = 0.3f;
    [SerializeField] protected LeanTweenType _ltt = LeanTweenType.linear;

    protected bool _doPulse = false;
    protected bool _pulsing = false;
    protected LTDescr _pulse = null;

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
            Pulse();
        }
    }

    protected void Pulse()
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
