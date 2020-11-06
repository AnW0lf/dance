using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Like : MonoBehaviour
{
    [SerializeField] private RectTransform _rect = null;
    [SerializeField] private Image _likeImage = null;
    [SerializeField] private TextMeshProUGUI _counter = null;
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
                _rect.localScale = Vector3.one * 1.5f;
                _likeImage.sprite = _powerUpped;
            }
            else
            {
                _counter.text = string.Empty;
                _rect.localScale = Vector3.one;
                _likeImage.sprite = _normal;
            }
        }
    }
}
