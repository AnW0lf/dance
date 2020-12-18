using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Assets.Scripts.Interface.ChestMiniGame;

public class VictoryScreen : MonoBehaviour
{
    [Header("Title")]
    [SerializeField] private MovePanel _smile = null;
    [SerializeField] private MovePanel _label = null;

    [Header("RewardMiniGame")]
    [SerializeField] private RewardMiniGame _miniGame = null;

    [Header("End buttons")]
    [SerializeField] private MovePanel _button = null;

    private void OnEnable()
    {
        ShowTitle();
    }

    public void ShowTitle()
    {
        _smile.Visible = true;
        _label.Visible = true;
        LeanTween.delayedCall(0.9f, ShowMiniGame);
    }

    public void ShowMiniGame()
    {
        _miniGame.transform.localScale = Vector3.zero;
        _miniGame.gameObject.SetActive(true);
        _miniGame.gameObject.LeanScale(Vector3.one * 1f, 0.25f);
    }

    private void Update()
    {
        if (_miniGame.KeyCount == 0 && !_button.Visible)
            ShowContinueButton();
    }

    public void ShowContinueButton()
    {
        _button.Visible = true;
    }
}
