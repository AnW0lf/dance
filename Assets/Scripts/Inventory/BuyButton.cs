using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

namespace Assets.Scripts.Inventory
{
    public class BuyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
    {
        [SerializeField] private Transform _button = null;
        [SerializeField] private TextMeshProUGUI _price = null;
        [SerializeField] private GameObject _fade = null;
        [SerializeField] private Inventory _inventory = null;

        public bool Interactable { get; private set; } = false;

        private int Money => Player.Instance.Money;
        private int Price => Player.Instance.Price;

        private bool _pressed = false;
        private bool _onButton = false;

        private Coroutine _scaling = null;

        private void Start()
        {
            SetPrice(Price);
            UpdateButtonState(0);
            Player.Instance.OnPriceChanged += SetPrice;
            Player.Instance.OnPriceChanged += UpdateButtonState;
            Player.Instance.OnMoneyChanged += UpdateButtonState;
        }

        private void OnDisable()
        {
            Player.Instance.OnPriceChanged -= SetPrice;
            Player.Instance.OnPriceChanged -= UpdateButtonState;
            Player.Instance.OnMoneyChanged -= UpdateButtonState;
        }

        private void OnDestroy()
        {
            Player.Instance.OnPriceChanged -= SetPrice;
            Player.Instance.OnPriceChanged -= UpdateButtonState;
            Player.Instance.OnMoneyChanged -= UpdateButtonState;
        }

        private void OnDestroy()
        {
            Player.Instance.OnPriceChanged -= (price) => SetPrice(price);
            Player.Instance.OnPriceChanged -= (price) => SetButtonState(Money, price);
            Player.Instance.OnMoneyChanged -= (money) => SetButtonState(money, Price);
        }

        public void SetPrice(int price)
        {
            _price.text = price.ToString();
        }

        public void UpdateButtonState(int useless)
        {
            Interactable = Money >= Price;
            _fade.SetActive(!Interactable);
        }

        private void BuyDance()
        {
            _inventory.BuyDance(Player.Instance.Price);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!Interactable) return;

            _pressed = true;
            _onButton = true;

            if (_scaling != null) StopCoroutine(_scaling);
            _scaling = StartCoroutine(Press());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!Interactable) return;

            if (_pressed && _onButton) BuyDance();

            if (_scaling != null) StopCoroutine(_scaling);
            _scaling = StartCoroutine(Unpress());

            _pressed = false;
            _onButton = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!Interactable) return;

            _onButton = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Interactable) return;

            _onButton = true;
        }

        private IEnumerator Press()
        {
            Vector3 startScale = _button.localScale;
            Vector3 endScale = Vector3.one * 0.95f;
            float speed = 5f;
            float timer = 0f;
            float duration = Vector3.Distance(startScale, endScale) / speed;

            while (timer <= duration)
            {
                timer += Time.deltaTime;
                _button.localScale = Vector3.Lerp(startScale, endScale, timer / duration);
                yield return null;
            }

            _scaling = null;
        }

        private IEnumerator Unpress()
        {
            Vector3 startScale = _button.localScale;
            Vector3 endScale = Vector3.one * 1.05f;
            float speed = 5f;
            float timer = 0f;
            float duration = Vector3.Distance(startScale, endScale) / speed;

            while (timer <= duration)
            {
                timer += Time.deltaTime;
                _button.localScale = Vector3.Lerp(startScale, endScale, timer / duration);
                yield return null;
            }

            startScale = _button.localScale;
            endScale = Vector3.one * 1f;
            timer = 0f;
            duration = Vector3.Distance(startScale, endScale) / speed;

            while (timer <= duration)
            {
                timer += Time.deltaTime;
                _button.localScale = Vector3.Lerp(startScale, endScale, timer / duration);
                yield return null;
            }

            _scaling = null;
        }
    }
}
