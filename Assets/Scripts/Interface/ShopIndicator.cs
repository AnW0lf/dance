using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.Interface
{
    [RequireComponent(typeof(Image))]
    public class ShopIndicator : MonoBehaviour
    {
        private Image _image = null;

        [SerializeField]
        private Text _text = null;

        private void Start()
        {
            _image = GetComponent<Image>();
            CanSpend = Player.Instance.Price <= Player.Instance.Money;
            Player.Instance.OnMoneyChanged += OnMoneyChanged;
            Player.Instance.OnPriceChanged += OnPriceChanged;
        }

        private void OnDestroy()
        {
            Player.Instance.OnMoneyChanged -= OnMoneyChanged;
            Player.Instance.OnPriceChanged -= OnPriceChanged;
        }

        private void OnMoneyChanged(int money)
        {
            CanSpend = money >= Player.Instance.Price;
        }

        private void OnPriceChanged(int price)
        {
            CanSpend = Player.Instance.Money >= price;
        }

        private bool _canSpend = false;
        public bool CanSpend
        {
            get => _canSpend;
            set
            {
                _canSpend = value;
                _image.enabled = _canSpend;
                _text.enabled = _canSpend;
            }
        }
    }
}