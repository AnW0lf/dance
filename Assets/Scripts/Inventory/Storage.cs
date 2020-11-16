using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.Events;

namespace Assets.Scripts.Inventory
{
    public class Storage : MonoBehaviour, IDropHandler
    {
        [SerializeField] private Transform _cellContainer = null;
        [SerializeField] private GameObject _cellPrefab = null;
        [SerializeField] private Cell _draggedCell = null;
        private List<Cell> _cells = new List<Cell>();

        public UnityAction<Cell> OnAddCell { get; set; } = null;
        public UnityAction<Cell> OnRemoveCell { get; set; } = null;

        private void Start()
        {
            foreach (var dance in Player.Instance.Storage)
                AddCell(dance);

            OnAddCell += (cell) => SaveStorage();
            OnRemoveCell += (cell) => SaveStorage();
        }

        public void AddCell(Dance dance)
        {
            Cell cell = Instantiate(_cellPrefab, _cellContainer).GetComponent<Cell>();
            cell.SetDance(dance);
            cell.DraggedCell = _draggedCell;
            _cells.Add(cell);
            OnAddCell?.Invoke(cell);
        }

        public void RemoveCell(Cell cell)
        {
            _cells.Remove(cell);
            OnRemoveCell?.Invoke(cell);
            Destroy(cell.gameObject);
        }

        private void Update()
        {
            for (int i = _cells.Count - 1; i >= 0; i--)
                if (_cells[i].IsEmpty) RemoveCell(_cells[i]);
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

        private void SaveStorage()
        {
            List<Dance> dances = _cells.Select((cell) => cell.Dance).ToList();
            Player.Instance.SetStorage(dances);
        }
    }
}