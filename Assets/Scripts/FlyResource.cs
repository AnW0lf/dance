using UnityEngine;
using TMPro;

public class FlyResource : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer = null;
    [SerializeField] private TextMeshPro _counter = null;
    [SerializeField] private Sprite _normal = null;
    [SerializeField] private Sprite _powerUpped = null;
    [SerializeField] private float _sizeUp = 1.5f;

    private int _count = 1;
    public int Count
    {
        get => _count;
        set
        {
            _count = Mathf.Max(value, 1);
            if (_count > 1)
            {
                _counter.text = $"{_count}";
                transform.localScale *= _sizeUp;
                _spriteRenderer.sprite = _powerUpped;
            }
            else
            {
                _counter.text = string.Empty;
                _spriteRenderer.sprite = _normal;
            }
        }
    }
}
