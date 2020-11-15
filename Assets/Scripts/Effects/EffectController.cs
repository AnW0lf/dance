using UnityEngine;
using System.Collections;

public class EffectController : MonoBehaviour
{
    [SerializeField] private Transform _labelSpawnPoint = null;
    [SerializeField] private MinionController _minion = null;
    [Space(20)]
    [Header("Label prefabs")]
    [SerializeField] private GameObject[] _missLabelPrefabs = null;
    [SerializeField] private GameObject[] _goodLabelPrefabs = null;
    [SerializeField] private GameObject[] _perfectLabelPrefabs = null;
    [SerializeField] private GameObject[] _tooSlowLabelPrefabs = null;

    [Space(20)]
    [Header("Effects")]
    [SerializeField] private EffectSpawner[] _missSpawners = null;
    [SerializeField] private EffectSpawner[] _goodSpawners = null;
    [SerializeField] private EffectSpawner[] _perfectSpawners = null;
    [SerializeField] private EffectSpawner[] _tooSlowSpawners = null;

    private void Start()
    {
        _minion.OnMiss += OnMiss;
        _minion.OnGood += OnGood;
        _minion.OnPerfect += OnPerfect;
        _minion.OnTooSlow += OnTooSlow;
    }

    private void OnMiss(Dance dance)
    {
        GameObject prefab = _missLabelPrefabs[Random.Range(0, _missLabelPrefabs.Length)];
        SpawnLabel(prefab);
        BeginEffect(_missSpawners);
    }

    private void OnGood(Dance dance)
    {
        GameObject prefab = _goodLabelPrefabs[Random.Range(0, _goodLabelPrefabs.Length)];
        SpawnLabel(prefab);
        BeginEffect(_goodSpawners);
    }

    private void OnPerfect(Dance dance)
    {
        GameObject prefab = _perfectLabelPrefabs[Random.Range(0, _perfectLabelPrefabs.Length)];
        SpawnLabel(prefab);
        BeginEffect(_perfectSpawners);
    }

    private void OnTooSlow(Dance dance)
    {
        GameObject prefab = _tooSlowLabelPrefabs[Random.Range(0, _tooSlowLabelPrefabs.Length)];
        SpawnLabel(prefab);
        BeginEffect(_tooSlowSpawners);
    }

    private void SpawnLabel(GameObject prefab)
    {
        Instantiate(prefab, _labelSpawnPoint);
    }

    private void BeginEffect(EffectSpawner[] spawners)
    {
        if (spawners == null || spawners.Length == 0) return;
        foreach (var spawner in spawners) spawner.SpawnEffect();
    }
}
