using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Inventory
{
    public class Cell : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
    {
        [SerializeField] private Image _background = null;
        [SerializeField] private Image _icon = null;
        [SerializeField] private TextMeshProUGUI _label = null;
        [SerializeField] private Cell _draggedCell = null;
        [SerializeField] private bool _isAsseted = false;

        private MinionDancePreview _dancePreview = null;

        private void Start()
        {
            _dancePreview = FindObjectOfType<MinionDancePreview>();
        }

        public Dance Dance { get; private set; } = null;

        public bool IsEmpty => Dance == null;
        public bool IsAsseted => _isAsseted;
        public UnityAction<Cell> OnCellDrop { get; set; } = null;
        public UnityAction<Cell> OnCellEndDrag { get; set; } = null;

        public Cell DraggedCell
        {
            get => _draggedCell;
            set => _draggedCell = value;
        }

        public static Cell TargetCell { get; set; } = null;

        public void SetDance(Dance dance)
        {
            if (dance == null) Clear();
            else
            {
                Dance = dance;

                _background.sprite = Dance.BackgroundSprite;
                _icon.sprite = Dance.IconSprite;
                _label.text = Dance.LabelText;

                _background.gameObject.SetActive(true);
                _icon.gameObject.SetActive(true);
                _label.gameObject.SetActive(true);
            }
        }

        public void Clear()
        {
            Dance = null;

            _background.sprite = null;
            _icon.sprite = null;
            _label.text = string.Empty;

            _background.gameObject.SetActive(false);
            _icon.gameObject.SetActive(false);
            _label.gameObject.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            TargetCell = this;

            DraggedCell.SetDance(Dance);
        }

        public void OnDrag(PointerEventData eventData)
        {
            DraggedCell.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (TargetCell != null)
            {
                TargetCell = null;
                DraggedCell.Clear();
            }
            OnCellEndDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (TargetCell != this)
            {
                Dance dance = TargetCell.Dance;
                if (dance == Dance && Dance.HasNextLevel)
                {
                    if (TargetCell.IsAsseted && !IsAsseted)
                    {
                        TargetCell.SetDance(dance.NextLevel);
                        this.Clear();
                    }
                    else
                    {
                        TargetCell.Clear();
                        this.SetDance(dance.NextLevel);
                    }
                }
                else
                {
                    TargetCell.SetDance(Dance);
                    this.SetDance(dance);
                }
                TargetCell = null;
                DraggedCell.Clear();
                OnCellDrop?.Invoke(this);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsEmpty) return;

            _dancePreview.ShowDancePreview(Dance);
        }
    }
}
