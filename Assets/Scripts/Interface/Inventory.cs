using Assets.Scripts.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private Asset _asset = null;
        [SerializeField] private Storage _storage = null;
        [SerializeField] private float _priceMultiplier = 1.3f;
        [SerializeField] private int[] _priceList = null;

        private void Start()
        {
            if (Player.Instance.DanceBought < _priceList.Length)
                Player.Instance.Price = _priceList[Player.Instance.DanceBought];
        }

        private void Update()
        {
            if (_asset.HasEmptyCell && !_storage.IsEmpty)
            {
                Cell cell = _storage.FirstCell;
                if (_asset.FillEmpty(cell.Dance))
                    cell.Clear();
            }
        }

        public bool BuyDance(int price)
        {
            if(Player.Instance.Money >= price)
            {
                _storage.AddCell(Player.Instance.RandomDance, true);
                Player.Instance.Money -= price;
                Player.Instance.DanceBought++;

                int newPrice = Player.Instance.Price;
                if (Player.Instance.DanceBought < _priceList.Length)
                    newPrice = _priceList[Player.Instance.DanceBought];
                else
                    newPrice = (int)((float)newPrice * _priceMultiplier);
                Player.Instance.Price = newPrice;
                return true;
            }
            return false;
        }
    }
}
