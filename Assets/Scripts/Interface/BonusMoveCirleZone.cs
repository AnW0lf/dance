using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class BonusMoveCirleZone : MonoBehaviour
{
    [SerializeField] private GameObject _bonusCirclePrefab = null;

    private RectTransform _rect = null;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public BonusMoveCircle AddBonusCircle()
    {
        RectTransform bonusCircle = Instantiate(_bonusCirclePrefab, transform).GetComponent<RectTransform>();
        Vector2 newPosition = new Vector2(Random.Range(-_rect.anchoredPosition.x / 2, _rect.anchoredPosition.x / 2)
            , Random.Range(-_rect.anchoredPosition.y / 2, _rect.anchoredPosition.y / 2));
        bonusCircle.anchoredPosition = newPosition;
        return bonusCircle.GetComponent<BonusMoveCircle>();
    }
}
