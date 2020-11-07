using UnityEngine;
using System.Collections;

public class MovePanel : MonoBehaviour
{
    [SerializeField] private bool _visible = false;
    [SerializeField] private RectTransform _panel = null;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private Vector2 _visiblePosition = Vector2.zero;
    [SerializeField] private Vector2 _invisiblePosition = Vector2.zero;

    private Coroutine _moving = null;

    public bool Visible
    {
        get => _visible;
        set
        {
            _visible = value;
            if (_moving != null) StopCoroutine(_moving);
            _moving = StartCoroutine(Moving());
        }
    }

    private IEnumerator Moving()
    {
        Vector2 startPostion = _panel.anchoredPosition;
        Vector2 endPosition = _visible ? _visiblePosition : _invisiblePosition;
        float timer = 0f;

        while (timer <= _duration)
        {
            timer += Time.deltaTime;
            _panel.anchoredPosition = Vector2.Lerp(startPostion, endPosition, timer / _duration);
            yield return null;
        }

        _moving = null;
    }

    private void Start()
    {
        _panel.anchoredPosition = _visible ? _visiblePosition : _invisiblePosition;
    }
}
