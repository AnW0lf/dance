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

    private Coroutine _scaling = null;
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
                    SetScale(Vector3.one * 1.2f);
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
        if (_scaling != null) StopCoroutine(_scaling);
        _scaling = StartCoroutine(Scaling(1.5f, scale));
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

    private IEnumerator Scaling(float speed, Vector3 scale)
    {
        float timer = 0f;
        float duration = Vector3.Distance(_background.transform.localScale, scale) / speed;

        while(timer <= duration)
        {
            timer += Time.deltaTime;
            Vector3 newScale = Vector3.Lerp(_fade.transform.localScale, scale, timer / duration);
            _fade.transform.localScale = newScale;
            _background.transform.localScale = newScale;
            _glow.transform.localScale = newScale;
            _icon.transform.localScale = newScale;
            _label.transform.localScale = newScale;
            yield return null;
        }

        _scaling = null;
    }
}

public enum CardState { NORMAL, GLOWED, FADED, BONUS }
