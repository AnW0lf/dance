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
                Player.Instance.Money -= price;
                _storage.AddCell(Player.Instance.RandomDance, true);
                return true;
            }
            return false;
        }
    }
}
