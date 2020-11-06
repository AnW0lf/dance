using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class BonusButton : MonoBehaviour
{
    [SerializeField] private bool _visible = false;
    [SerializeField] private Vector2 _visiblePosition = Vector2.zero;
    [SerializeField] private Vector2 _invisiblePosition = Vector2.zero;
    [SerializeField] private RectTransform _rect = null;
    [SerializeField] private Button _button = null;

    private Coroutine _moveTo = null;
    private Coroutine _scaling = null;

    public UnityAction OnClick { get; set; } = null;

    public bool Visible
    {
        get => _visible;
        set
        {
            _visible = value;
            if (_moveTo != null) StopCoroutine(_moveTo);
            _moveTo = StartCoroutine(MoveTo());
        }
    }

    private IEnumerator MoveTo()
    {
        float timer = 0f;
        float speed = 1500f;
        Vector2 endPosition = Visible ? _visiblePosition : _invisiblePosition;
        float duration = Vector2.Distance(_rect.anchoredPosition, endPosition) / speed;

        while (timer <= duration)
        {
            timer += Time.deltaTime;
            _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, endPosition, timer / duration);
            yield return null;
        }

        _moveTo = null;
    }

    public bool Interactable
    {
        get => _button.interactable;
        set
        {
            _button.interactable = value;
            if (_scaling != null) StopCoroutine(_scaling);
            _scaling = StartCoroutine(Scaling());
        }
    }

    private IEnumerator Scaling()
    {
        float timer = 0f;
        float speed = 1.5f;
        Vector2 endScale = Vector2.one * (Interactable ? 1.2f : 1f);
        float duration = Vector2.Distance(_rect.localScale, endScale) / speed;

        while (timer <= duration)
        {
            timer += Time.deltaTime;
            _rect.localScale = Vector2.Lerp(_rect.localScale, endScale, timer / duration);
            yield return null;
        }

        _scaling = null;
    }

    public void Click()
    {
        OnClick?.Invoke();
    }

    private void Start()
    {
        Visible = _visible;
        Interactable = _button.interactable;
    }
}
