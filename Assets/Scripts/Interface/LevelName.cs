using UnityEngine;
using System.Collections;
using TMPro;

public class LevelName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name = null;
    [SerializeField] private RectTransform _rect = null;
    [SerializeField] private Vector2 _startPosition = Vector2.zero;
    [SerializeField] private Vector2 _visiblePosition = Vector2.zero;
    [SerializeField] private Vector2 _endPosition = Vector2.zero;
    [SerializeField] private LeanTweenType _easeShow = LeanTweenType.linear;
    [SerializeField] private LeanTweenType _easeHide = LeanTweenType.linear;
    [SerializeField] private float _showDuration = 0.5f;
    [SerializeField] private float _visibleDuration = 2f;
    [SerializeField] private float _hideDuration = 0.5f;

    public string Name
    {
        get => _name.text;
        set => _name.text = value; 
    }

    public void Show()
    {
        LeanTween.cancel(_rect.gameObject);

        _rect.anchoredPosition = _startPosition;

        _rect.LeanMoveLocal(_visiblePosition, _showDuration)
            .setEase(_easeShow);

        _rect.LeanMoveLocal(_endPosition, _hideDuration)
            .setDelay(_showDuration + _visibleDuration)
            .setEase(_easeHide);
    }
}
