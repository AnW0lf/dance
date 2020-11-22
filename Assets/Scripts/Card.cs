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
    [SerializeField] private GameObject[] _stars = null;
    [SerializeField] private GameObject[] _starsFade = null;

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
                    FadeOut();
                    _button.interactable = true;
                    break;
                case CardState.GLOWED:
                    _glow.SetActive(true);
                    FadeOut();
                    _button.interactable = false;
                    break;
                case CardState.FADED:
                    _glow.SetActive(false);
                    FadeIn();
                    _button.interactable = false;
                    break;
            }
        }
    }

    public void SetCard(Dance dance)
    {
        _background.sprite = dance.BackgroundSprite;
        _label.text = dance.LabelText;
        _icon.sprite = dance.IconSprite;
        _icon.enabled = _icon.sprite != null;
        for (int i = 0; i < _stars.Length; i++)
            _stars[i].SetActive(i < dance.Level);
    }

    public void SetAction(UnityAction action)
    {
        _button.onClick.AddListener(action);
    }

    private void FadeIn()
    {
        _fade.SetActive(true);
        for (int i = 0; i < _starsFade.Length; i++)
            _starsFade[i].SetActive(_stars[i].activeSelf);
    }

    private void FadeOut()
    {
        _fade.SetActive(false);
        for (int i = 0; i < _starsFade.Length; i++)
            _starsFade[i].SetActive(false);
    }
}

public enum CardState { NORMAL, GLOWED, FADED }
