using UnityEngine;
using System.Collections;

public class Wish : MonoBehaviour
{
    [SerializeField] private bool _visible = false;
    [SerializeField] private float _scalingDuration = 0.25f;
    [SerializeField] private Transform _background = null;
    [SerializeField] private SpriteRenderer _spriteRenderer = null;

    private Coroutine _scaling = null;
    public bool Visible
    {
        get => _visible;
        set
        {
            _visible = value;
            if (_scaling != null) StopCoroutine(_scaling);
            _scaling = StartCoroutine(Scaling(_visible ? Vector3.one : Vector3.zero, _scalingDuration));
        }
    }

    public Sprite Sprite
    {
        get => _spriteRenderer.sprite;
        set => _spriteRenderer.sprite = value;
    }

    public void SetWish(Sprite sprite)
    {
        Sprite = sprite;
        Visible = true;
    }

    private IEnumerator Scaling(Vector3 scale, float duration)
    {
        float timer = 0f;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            _background.localScale = Vector3.Lerp(_background.localScale, scale, timer / duration);
            yield return null;
        }
        _scaling = null;
    }

    private void Start()
    {
        Visible = _visible;
    }
}
