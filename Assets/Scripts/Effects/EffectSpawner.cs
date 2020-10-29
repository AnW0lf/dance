using UnityEngine;
using System.Collections;

public class EffectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _effectPrefab = null;
    [SerializeField] private float _destroyDelay = 2f;
    [SerializeField] private Vector3 _offsetPosition = Vector3.zero;
    [SerializeField] private Vector3 _offsetRotation = Vector3.zero;
    [SerializeField] private Vector3 _offsetScale = Vector3.zero;
    public void SpawnEffect()
    {
        Transform effect = Instantiate(_effectPrefab, transform).transform;
        effect.localPosition = _offsetPosition;
        effect.localEulerAngles = _offsetRotation;
        effect.localScale = _offsetScale;
        Destroy(effect.gameObject, _destroyDelay);
    }

}
