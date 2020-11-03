using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class BonusMoveCircle : MonoBehaviour
{
    [SerializeField] private Image _image = null;
    [SerializeField] private Image _childImage = null;
    [SerializeField] private RectTransform _childRect = null;
    [SerializeField] private Button _button = null;

    public float Progress
    {
        get => 1f - Mathf.Sqrt(_childRect.localScale.x);
        set
        {
            float progress = Mathf.Clamp(value, -1f, 1f);
            if (progress == 1f) Destroy(gameObject);
            _childRect.localScale = Vector2.one * Mathf.Pow(1f - value, 2f);

            Color color = _image.color;
            color.a = 1f + progress;
            _image.color = color;

            color = _childImage.color;
            color.a = 1f + progress;
            _childImage.color = color;

            if (progress > -0.75f)
            {
                if (!_button.interactable) _button.interactable = true;
            }
            else
            {
                if (_button.interactable) _button.interactable = false;
            }
        }
    }

    public UnityAction<float> OnClick { get; set; } = null;

    private void Start()
    {
        _button.onClick.AddListener(Click);
    }

    public void Click()
    {
        OnClick?.Invoke(Progress);
        Destroy(gameObject);
    }
}
