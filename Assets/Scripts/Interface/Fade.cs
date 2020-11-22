using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class Fade : MonoBehaviour
{
    [SerializeField] private bool _visible = false;
    [SerializeField] private Image _fade = null;
    [SerializeField] private float _fadingSpeed = 2f;
    private Coroutine _fading = null;

    public UnityAction<bool> OnComplete { get; set; } = null;

    public bool Visible
    {
        get => _visible;
        set
        {
            _visible = value;
            if (_fading != null) StopCoroutine(_fading);
            _fading = StartCoroutine(SetAlpha(_visible ? 1f : 0f));
        }
    }

    private IEnumerator SetAlpha(float alpha)
    {
        yield return new WaitForSeconds(0.2f);

        if (alpha > 0f) _fade.raycastTarget = true;
        float from = _fade.color.a;
        float timer = 0f;
        float duration = Mathf.Abs(alpha - from) / _fadingSpeed;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            Color color = _fade.color;
            color.a = Mathf.Lerp(from, alpha, timer / duration);
            _fade.color = color;
            yield return null;
        }
        if (alpha == 0f) _fade.raycastTarget = false;
        _fading = null;
        OnComplete?.Invoke(Visible);
    }
}
