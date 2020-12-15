using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform _target = null;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private Transform[] _points = null;

    [Header("Following")]
    [SerializeField] private Transform _minion = null;
    [SerializeField] private float _distance = 2f;

    [Header("Wiggle")]
    [SerializeField] private Vector3 _wiggleOffset = Vector3.zero;
    [SerializeField] private float _wigglePeriod = 1f;
    private Coroutine _wiggle = null;

    #region PID Controller
    [Space(20)]
    [Header("PID Controller")]
    [SerializeField] private float _kProportional = 1f;
    [SerializeField] private float _kIntegrating = 1f;
    [SerializeField] private float _kDifferentiating = 1f;
    private float _f_proportional = 0f;
    private float _s_integrating = 0f;
    private float _f_integrating = 0f;
    private float _f_differentiating = 0f;
    private float _error = 0f;
    private float _old_error = 0f;
    #endregion PID Controller

    public UnityAction OnBegin { get; set; } = null;
    public UnityAction OnEnd { get; set; } = null;

    public void Begin()
    {
        OnBegin?.Invoke();
        StartCoroutine(Moving());
    }

    private IEnumerator Wiggle()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + _wiggleOffset;
        float timer = 0;

        while (true)
        {
            timer += Time.deltaTime;

            float omega = Mathf.PI * 2f / _wigglePeriod;
            transform.position = Vector3.Lerp(startPosition, endPosition, (Mathf.Sin(omega * timer) + 1f) / 2f);

            yield return null;
        }
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

    private Vector3 unit = Vector3.zero;
    private bool _followingActive = false;

    public void BeginFollow()
    {
        unit = (_minion.position - _target.position).normalized;
        unit.y = 0f;
        unit.Normalize();
        _followingActive = true;
    }

    public void EndFollow()
    {
        _followingActive = false;
    }

    private void Start()
    {
        _wiggle = StartCoroutine(Wiggle());
        OnBegin += () => StopCoroutine(_wiggle);
        OnEnd += BeginFollow;
    }

    private void FixedUpdate()
    {
        if (_followingActive)
        {
            Vector3 direction = Vector3.Project((_minion.position - _target.position), unit);
            _target.transform.position -= unit * PID(direction.magnitude, _distance) * Time.fixedDeltaTime;
        }
    }

    private float PID(float currentValue, float asignmentValue)
    {
        _error = asignmentValue - currentValue;

        // proportional
        _f_proportional = _error * _kProportional;

        // integrating
        _s_integrating += _error * Time.fixedDeltaTime;
        _f_integrating = _kIntegrating * _s_integrating;
        _f_integrating = Mathf.Sign(_f_integrating) * Mathf.Clamp(Mathf.Abs(_f_integrating), 0f, Mathf.Abs(_f_proportional));

        // differentiating
        _f_differentiating = _kDifferentiating * (_error - _old_error);

        _old_error = _error;
        return _f_proportional + _f_integrating + _f_differentiating;
    }
}
