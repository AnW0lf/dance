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
        [SerializeField] private bool _addTargetDance = false;
        [SerializeField] private Dance _targetDance = null;
        [SerializeField] private Transform _cellContainer = null;
        [SerializeField] private GameObject _cellPrefab = null;
        [SerializeField] private Cell _draggedCell = null;
        private List<Cell> _cells = new List<Cell>();

        public UnityAction<Cell> OnAddCell { get; set; } = null;
        public UnityAction<Cell> OnRemoveCell { get; set; } = null;

        private void Start()
        {
            foreach (var dance in Player.Instance.Storage)
                AddCell(dance, false);

            OnAddCell += (cell) => SaveStorage();
            OnRemoveCell += (cell) => SaveStorage();
        }

        public void AddCell(Dance dance, bool pulse)
        {
            Cell cell = Instantiate(_cellPrefab, _cellContainer).GetComponent<Cell>();
            cell.SetDance(dance);
            cell.DraggedCell = _draggedCell;
            _cells.Add(cell);
            OnAddCell?.Invoke(cell);

            if (pulse) StartCoroutine(CellPulse(cell.transform));
        }

        private IEnumerator CellPulse(Transform cell)
        {
            cell.localScale = Vector3.zero;

            Vector3 startScale = cell.localScale;
            Vector3 endScale = Vector3.one * 1.2f;
            float speed = 15f;
            float timer = 0f;
            float duration = Vector3.Distance(startScale, endScale) / speed;

            while (timer <= duration)
            {
                timer += Time.deltaTime;
                cell.localScale = Vector3.Lerp(startScale, endScale, timer / duration);
                yield return null;
            }

            startScale = cell.localScale;
            endScale = Vector3.one * 1f;
            timer = 0f;
            duration = Vector3.Distance(startScale, endScale) / speed;

            while (timer <= duration)
            {
                timer += Time.deltaTime;
                cell.localScale = Vector3.Lerp(startScale, endScale, timer / duration);
                yield return null;
            }
        }

        public bool IsEmpty => _cells == null || _cells.Count == 0;

        public Cell FirstCell
        {
            get
            {
                if (IsEmpty) return null;
                return _cells[0];
            }
        }

        public Cell LastCell
        {
            get
            {
                if (IsEmpty) return null;
                return _cells[_cells.Count - 1];
            }
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

            if (_addTargetDance)
            {
                _addTargetDance = false;

                AddCell(_targetDance, false);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (Cell.TargetCell == null) return;
            if (_cells.Contains(Cell.TargetCell)) return;
            if (Cell.TargetCell.IsAsseted) return;
            Dance dance = Cell.TargetCell.Dance;
            Cell.TargetCell.DraggedCell.Clear();
            Cell.TargetCell.Clear();
            Cell.TargetCell = null;
            AddCell(dance, false);
        }

        private void SaveStorage()
        {
            List<Dance> dances = _cells.Select((cell) => cell.Dance).ToList();
            Player.Instance.SetStorage(dances);
        }
    }
}