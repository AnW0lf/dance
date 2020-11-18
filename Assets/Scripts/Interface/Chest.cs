using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    [SerializeField] private int _moneyReward = 0;
    [SerializeField] private int _classicFanReward = 0;
    [SerializeField] private int _jazzFanReward = 0;
    [SerializeField] private int _streetFanReward = 0;
    [SerializeField] private Image _fade = null;
    [SerializeField] private float _fadingSpeed = 2f;
    [SerializeField] private RewardMiniGame _miniGame = null;

    public void Reward()
    {
        if (_miniGame.KeyCount <= 0) return;
        _miniGame.KeyCount--;
        _miniGame.UpdateLabel();
        Player.Instance.Money += _moneyReward;
        Player.Instance.ClassicFansCount += _classicFanReward;
        Player.Instance.JazzFansCount += _jazzFanReward;
        Player.Instance.StreetFansCount += _streetFanReward;
        _fade.raycastTarget = false;
        StartCoroutine(Fading(0f));
    }

    private IEnumerator Fading(float alpha)
    {
        float startAlpha = _fade.color.a;
        float timer = 0f;
        float duration = Mathf.Abs(startAlpha - alpha) / _fadingSpeed;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            Color color = _fade.color;
            color.a = Mathf.Lerp(startAlpha, alpha, timer / duration);
            _fade.color = color;
            yield return null;
        }
    }
}
