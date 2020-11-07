using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform _target = null;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private Transform[] _points = null;

    public UnityAction OnBegin { get; set; } = null;
    public UnityAction OnEnd { get; set; } = null;

    public void Begin()
    {
        OnBegin?.Invoke();
        StartCoroutine(Moving());
    }

    private IEnumerator Moving()
    {
        Queue<Transform> queue = new Queue<Transform>();
        foreach (var point in _points) queue.Enqueue(point);
        while (queue.Count > 0)
        {
            Transform point = queue.Dequeue();
            Quaternion startRotation = _target.rotation;
            Quaternion endRotation = point.rotation;
            Vector3 startPosition = _target.position;
            Vector3 endPosition = point.position;
            float timer = 0f;
            float duration = Vector3.Distance(startPosition, endPosition) / _speed;

            while (timer <= duration)
            {
                timer += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, endPosition, timer / duration);
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, timer / duration);
                yield return null;
            }
        }

        OnEnd?.Invoke();
    }
}
