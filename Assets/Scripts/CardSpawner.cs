using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] private bool _visible = true;
    [SerializeField] private float _moveDuration = 0.7f;
    [SerializeField] private GameObject _cardPrefab = null;
    [SerializeField] private RectTransform _rect = null;
    [SerializeField] private MinionController _minion = null;
    [SerializeField] private Timer _timer = null;
    [SerializeField] private Dance[] _dances = null;
    [SerializeField] private GameObject _startLabel = null;

    private Dance _notUse = null;

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
        if (_notUse != null)
            dances.Remove(_notUse);
        for (int i = 0; i < count; i++)
        {
            Dance dance = dances[Random.Range(0, dances.Count)];
            dances.Remove(dance);
            Card card = Instantiate(_cardPrefab, transform).GetComponent<Card>();
            card.SetCard(dance);
            card.SetAction(() =>
            {
                _notUse = dance;
                _minion.SetDance(dance);
                FadingCard();
                card.State = CardState.GLOWED;
                Visible = false;

                if (_startLabel.activeSelf)
                    _startLabel.SetActive(false);
            });
        }
    }

    private void FadingCard()
    {
        foreach (var card in GetComponentsInChildren<Card>())
            card.State = CardState.FADED;
    }

    private IEnumerator MoveTo()
    {
        float timer = 0f;
        Vector2 startPosition = _rect.anchoredPosition;
        Vector2 endPosition = Visible ? Vector2.zero : Vector2.down * 500f;

        while (timer <= _moveDuration)
        {
            timer += Time.deltaTime;
            _rect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, timer / _moveDuration);
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

    private void Update()
    {
        if (_timer.TimeOver && Visible) Visible = false;
    }
}
