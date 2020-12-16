using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

namespace Assets.Scripts.Interface.ChestMiniGame
{
    public class Chest : MonoBehaviour
    {
        [Header("Behaviour components")]
        [SerializeField] private Image _fade = null;
        [SerializeField] private Image _chestIcon = null;
        [SerializeField] private Image _rewardIcon = null;
        [SerializeField] private TextMeshProUGUI _counter = null;
        [SerializeField] private float _fadingSpeed = 2f;
        [SerializeField] private RewardMiniGame _miniGame = null;
        [SerializeField] private List<ChestSkin> _skins = null;

        [Header("Jump effect")]
        [SerializeField] private Animator _animator = null;
        [SerializeField] private float _jumpOffset = 0f;

        public bool IsOpened { get; private set; } = false;
        public bool IsDeactive { get; private set; } = false;

        public void Reward()
        {
            if (_miniGame.KeyCount <= 0) return;

            Reward reward = _miniGame.GetReward();
            ChestSkin skin = _skins.Find((s) => s.Type == reward.Type);
            _rewardIcon.sprite = skin.Skin;
            _rewardIcon.preserveAspect = true;
            _counter.text = $"+{reward.Count}";

            switch (reward.Type)
            {
                case RewardType.MONEY:
                    Player.Instance.Money += reward.Count;
                    break;
                case RewardType.CLASSIC:
                    Player.Instance.ClassicFansCount += reward.Count;
                    break;
                case RewardType.JAZZ:
                    Player.Instance.JazzFansCount += reward.Count;
                    break;
                case RewardType.STREET:
                    Player.Instance.StreetFansCount += reward.Count;
                    break;
                default:
                    break;
            }

            _fade.raycastTarget = false;
            StartCoroutine(Fading(0f));

            IsOpened = true;
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

                color = _chestIcon.color;
                color.a = Mathf.Lerp(startAlpha, alpha, timer / duration);
                _chestIcon.color = color;

                yield return null;
            }
        }

        public void Deactive()
        {
            IsDeactive = true;
            StartCoroutine(SwapColor(Color.grey));
        }

        private IEnumerator SwapColor(Color color)
        {
            Color startColor = _fade.color;
            float timer = 0f;
            float duration = 1f;
            while (timer <= duration)
            {
                timer += Time.deltaTime;
                _fade.color = Color.Lerp(startColor, color, timer / duration);
                _chestIcon.color = Color.Lerp(startColor, color, timer / duration);
                yield return null;
            }
        }

        private float _timer = 0f;
        private float _delay = 4f;

        private void Start()
        {
            _timer = _delay + _jumpOffset;
        }

        private void Update()
        {
            if (!IsOpened && !IsDeactive)
            {
                _timer -= Time.deltaTime;

                if (_timer <= 0f)
                {
                    _animator.SetTrigger("Jump");
                    _timer = _delay;
                }
            }
        }
    }

    [Serializable]
    class ChestSkin
    {
        [SerializeField] private RewardType _type = RewardType.NONE;
        [SerializeField] private Sprite _skin = null;

        public RewardType Type => _type;
        public Sprite Skin => _skin;
    }
}
