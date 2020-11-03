using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.ObjectModel;

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

    public void SetBonusMoves(ReadOnlyCollection<BonusMove> bonusMoves)
    {
        for (int i = _bonusMoveContainer.childCount - 1; i >= 0; i--)
            Destroy(_bonusMoveContainer.GetChild(i).gameObject);

        foreach (var bonusMove in bonusMoves)
        {
            RectTransform bonusMoveZone = Instantiate(_bonusMoveZonePrefab, _bonusMoveContainer).GetComponent<RectTransform>();
            bonusMoveZone.anchoredPosition = Vector2.right * bonusMove.Start * 1125f;
            bonusMoveZone.sizeDelta = Vector2.right * bonusMove.Length * 1125f;
        }
    }
}
