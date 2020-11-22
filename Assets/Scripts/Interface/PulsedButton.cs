using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PulsedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private float _normalScale = 1f;
    [SerializeField] private float _pressedScale = 0.95f;
    [SerializeField] private float _maxScale = 1.05f;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private UnityEvent _onClick = null;

    public UnityEvent OnClick => _onClick;

    private Coroutine _scaling = null;

    private bool _pressed = false;

    public bool Pressed
    {
        get => _pressed;
        private set
        {
            if(_pressed != value)
            {
                _pressed = value;
                if (_scaling != null) StopCoroutine(_scaling);
                _scaling = StartCoroutine(_pressed ? Press() : Unpress());
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _onClick?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }

    private IEnumerator Press()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one * _pressedScale;
        float timer = 0f;
        float duration = Vector3.Distance(startScale, endScale) / _speed;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, timer / duration);
            yield return null;
        }

        _scaling = null;
    }

    private IEnumerator Unpress()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one * _maxScale;
        float timer = 0f;
        float duration = Vector3.Distance(startScale, endScale) / _speed;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, timer / duration);
            yield return null;
        }

        startScale = transform.localScale;
        endScale = Vector3.one * _normalScale;
        timer = 0f;
        duration = Vector3.Distance(startScale, endScale) / _speed;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, timer / duration);
            yield return null;
        }

        _scaling = null;
    }
}
