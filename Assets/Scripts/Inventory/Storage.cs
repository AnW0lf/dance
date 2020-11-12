using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Inventory
{
    public class Storage : MonoBehaviour, IDropHandler
    {
        [SerializeField] private bool _addCell = false;
        [SerializeField] private Transform _cellContainer = null;
        [SerializeField] private GameObject _cellPrefab = null;
        [SerializeField] private Cell _draggedCell = null;
        [SerializeField] private List<Dance> _dances = null;
        private List<Cell> _cells = new List<Cell>();

        public void AddCell(Dance dance)
        {
            Cell cell = Instantiate(_cellPrefab, _cellContainer).GetComponent<Cell>();
            cell.SetDance(dance);
            cell.DraggedCell = _draggedCell;
            _cells.Add(cell);
        }

        public void RemoveCell(Cell cell)
        {
            _cells.Remove(cell);
            Destroy(cell.gameObject);
        }

        private void Update()
        {
            for (int i = _cells.Count - 1; i >= 0; i--)
                if (_cells[i].IsEmpty) RemoveCell(_cells[i]);

            if (_addCell)
            {
                _addCell = false;
                AddCell(_dances[Random.Range(0, _dances.Count)]);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (_cells.Contains(Cell.TargetCell)) return;
            Dance dance = Cell.TargetCell.Dance;
            Cell.TargetCell.DraggedCell.Clear();
            Cell.TargetCell.Clear();
            Cell.TargetCell = null;
            AddCell(dance);
        }
    }
}