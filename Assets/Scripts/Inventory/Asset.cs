using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Inventory
{
    public class Asset : MonoBehaviour
    {
        private List<Cell> _cells = null;

        private void Awake()
        {
            _cells = transform.GetComponentsInChildren<Cell>().ToList();
        }

        private void Start()
        {
            for (int i = 0; i < Player.Instance.AssetCount; i++)
                _cells[i].SetDance(Player.Instance.Asset[i]);

            foreach (var cell in _cells)
            {
                cell.OnCellDrop += (c) => SaveAsset();
                cell.OnCellEndDrag += (c) => SaveAsset();
            }
        }

        private void SaveAsset()
        {
            List<Dance> dances = _cells.Select((cell) => cell.Dance).ToList();
            Player.Instance.SetAsset(dances);
        }
    }
}
