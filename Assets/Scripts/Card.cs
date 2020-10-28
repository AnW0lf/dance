using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

public class Card : MonoBehaviour
{
    [SerializeField] private Image _background = null;
    [SerializeField] private TextMeshProUGUI _label = null;
    [SerializeField] private Image _icon = null;
    [SerializeField] private Button _button = null;

    public int AnimationID { get; private set; }

    public void SetCard(Dance dance)
    {
        _background.sprite = dance.BackgroundSprite;
        _label.text = dance.LabelText;
        _icon.sprite = dance.IconSprite;
        _icon.enabled = _icon.sprite != null;
    }

    public void SetAction(UnityAction action)
    {
        _button.onClick.AddListener(action);
    }
}
