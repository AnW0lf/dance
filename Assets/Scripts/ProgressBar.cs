using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.ObjectModel;
using System.Collections.Generic;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private bool _visible = false;
    [SerializeField] private Slider _slider = null;

    [Space(20)]
    [Header("Bonus moves")]
    [SerializeField] private Transform _bonusMoveContainer = null;
    [SerializeField] private GameObject _bonusMoveZonePrefab = null;

    public bool Visible
    {
        get => _visible;
        set
        {
            if (_visible != value)
            {
                _visible = value;
                _slider.gameObject.SetActive(_visible);
                _bonusMoveContainer.gameObject.SetActive(_visible);
            }
        }
    }
    public float Progress
    {
        get => _slider.value;
        set => _slider.value = value;
    }

    private void Start()
    {
        _slider.gameObject.SetActive(_visible);
    }

    public void Clear()
    {
        for (int i = _bonusMoveContainer.childCount - 1; i >= 0; i--)
            Destroy(_bonusMoveContainer.GetChild(i).gameObject);
    }

    public void SetBonusMoves(List<BonusMove> bonusMoves)
    {
        Clear();

        foreach (var bonusMove in bonusMoves)
        {
            RectTransform bonusMoveZone = Instantiate(_bonusMoveZonePrefab, _bonusMoveContainer).GetComponent<RectTransform>();
            bonusMoveZone.anchorMin = new Vector2(bonusMove.Start, bonusMoveZone.anchorMin.y);
            bonusMoveZone.anchorMax = new Vector2(bonusMove.End, bonusMoveZone.anchorMax.y);
        }
    }
}
