﻿using UnityEngine;
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
            Dance dance = dances[Random.Range(0, dances.Count)];
            dances.Remove(dance);
            Card card = Instantiate(_cardPrefab, transform).GetComponent<Card>();
            card.SetCard(dance);
            card.SetAction(() =>
            {
                _minion.SetDance(dance);
                FadingCard();
                card.State = CardState.GLOWED;
                StartCoroutine(SpawnWithHideAndShow(2f, 3));
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

    private IEnumerator SpawnWithHideAndShow(float delay, int count)
    {
        yield return new WaitForSeconds(delay);

        Visible = false;

        yield return new WaitWhile(() => _moveTo != null);

        Spawn(count);

        Visible = true;
    }
}
