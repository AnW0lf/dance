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
    [SerializeField] private GameObject _glow = null;
    [SerializeField] private GameObject _fade = null;

    public int AnimationID { get; private set; }

    private CardState _state = CardState.NORMAL;
    public CardState State
    {
        get => _state;
        set
        {
            _state = value;
            switch (_state)
            {
                case CardState.NORMAL:
                    _glow.SetActive(false);
                    _fade.SetActive(false);
                    _button.interactable = true;
                    SetScale(Vector3.one);
                    break;
                case CardState.GLOWED:
                    _glow.SetActive(true);
                    _fade.SetActive(false);
                    _button.interactable = false;
                    SetScale(Vector3.one);
                    break;
                case CardState.BONUS:
                    _glow.SetActive(true);
                    _fade.SetActive(false);
                    _button.interactable = true;
                    SetScale(Vector3.one * 1.25f);
                    break;
                case CardState.FADED:
                    _glow.SetActive(false);
                    _fade.SetActive(true);
                    _button.interactable = false;
                    SetScale(Vector3.one);
                    break;
            }
        }
    }

    private void SetScale(Vector3 scale)
    {
        _fade.transform.localScale = scale;
        _background.transform.localScale = scale;
        _glow.transform.localScale = scale;
        _icon.transform.localScale = scale;
        _label.transform.localScale = scale;
    }

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

    public void ClearActions()
    {
        _button.onClick.RemoveAllListeners();
    }
}

public enum CardState { NORMAL, GLOWED, FADED, BONUS }
