using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] private bool _visible = true;
    [SerializeField] private GameObject _cardPrefab = null;
    [SerializeField] private RectTransform _rect = null;
    [SerializeField] private MinionController _minion = null;
    [SerializeField] private Dance[] _dances = null;
    [SerializeField] private DanceStyleColor[] _colors = null;

    public bool Visible
    {
        get => _visible;
        set
        {
            _visible = value;
            if (_moveTo != null) StopCoroutine(_moveTo);
            _moveTo = StartCoroutine(MoveTo());
        }
    }

    private Coroutine _moveTo = null;

    /// <summary>
    /// Destroy all children
    /// </summary>
    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }

    /// <summary>
    /// Clear and Spawn 'count' new random cards
    /// </summary>
    /// <param name="count"> count of new cards </param>
    public void Spawn(int count)
    {
        Clear();
        List<Dance> dances = new List<Dance>(_dances);
        for (int i = 0; i < count; i++)
        {
            Dance dance = dances[Random.Range(0, _dances.Length)];
            dances.Remove(dance);
            Card card = Instantiate(_cardPrefab, transform).GetComponent<Card>();
            card.SetCard(dance, GetColor(dance.Style));
            card.SetAction(() =>
            {
                _minion.SetDance(dance);
                Spawn(3);
            });
        }
    }

    /// <summary>
    /// Spawn one new random card
    /// </summary>
    private void Spawn()
    {
        
    }

    private IEnumerator MoveTo()
    {
        float timer = 0f;
        float speed = 1500f;
        Vector2 startPosition = _rect.anchoredPosition;
        Vector2 endPosition = Visible ? Vector2.zero : Vector2.down * 500f;
        float duration = Vector2.Distance(startPosition, endPosition) / speed;

        while (timer <= duration)
        {
            timer += Time.deltaTime;
            _rect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, timer / duration);
            yield return null;
        }

        _moveTo = null;
    }

    private void Start()
    {
        if (_visible) _rect.anchoredPosition = Vector2.zero;
        else _rect.anchoredPosition = Vector2.down * 500f;

        Spawn(3);
    }

    private Color GetColor(DanceStyle style)
    {
        foreach(var pair in _colors)
            if (pair.Style == style) return pair.Color;
        return Color.white;
    }

    [Serializable]
    class DanceStyleColor
    {
        [SerializeField] private DanceStyle _style = DanceStyle.CLASSIC;
        [SerializeField] private Color _color = Color.white;

        public DanceStyle Style => _style;
        public Color Color => _color;
    }
}
