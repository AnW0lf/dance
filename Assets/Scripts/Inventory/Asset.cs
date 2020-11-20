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
            List<Dance> dances = _cells.Where((cell) => !cell.IsEmpty).Select((cell) => cell.Dance).ToList();
            Player.Instance.SetAsset(dances);
        }

        public bool FillEmpty(Dance dance)
        {
            if (!HasEmptyCell) return false;
            foreach(var cell in _cells)
            {
                if (cell.IsEmpty)
                {
                    cell.SetDance(dance);
                    SaveAsset();
                    return true;
                }
            }
            return false;
        }

        public bool HasEmptyCell
        {
            get
            {
                foreach (var cell in _cells)
                    if (cell.IsEmpty) return true;
                return false;
            }
        }
    }
}
