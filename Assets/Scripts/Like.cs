using UnityEngine;
using TMPro;

public class Like : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _likeRenderer = null;
    [SerializeField] private TextMeshPro _counter = null;
    [SerializeField] private Sprite _normal = null;
    [SerializeField] private Sprite _powerUpped = null;

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
                transform.localScale *= 1.5f;
                _likeRenderer.sprite = _powerUpped;
            }
            else
            {
                _counter.text = string.Empty;
                _likeRenderer.sprite = _normal;
            }
        }
    }
}
