using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.Interface
{
    [RequireComponent(typeof(Image))]
    public class ShopIndicator : MonoBehaviour
    {
        private Image _image = null;

        private void Start()
        {
            _image = GetComponent<Image>();
            CanSpend = Player.Instance.Price <= Player.Instance.Money;
            Player.Instance.OnMoneyChanged += (money) => CanSpend = Player.Instance.Price <= money;
            Player.Instance.OnPriceChanged += (price) => CanSpend = price <= Player.Instance.Money;
        }

        private bool _canSpend = false;
        public bool CanSpend
        {
            get => _canSpend;
            set
            {
                _canSpend = value;
                _image.enabled = _canSpend;
            }
        }
    }
}