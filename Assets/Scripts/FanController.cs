using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class FanController : MonoBehaviour
{
    [Space(20)]
    [Header("Look At settings")]
    [SerializeField] private Transform _lookAtTransform = null;
    [SerializeField] private Transform _bodyTransform = null;
    [SerializeField] private float _smoothness = 0.05f;

    [Space(20)]
    [Header("Fan settings")]
    [SerializeField] private bool _isFan = false;
    [SerializeField] private Material _fanMaterial = null;
    [SerializeField] private Material _viewerMaterial = null;
    [SerializeField] private Renderer _renderer = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private Timer _timer = null;

    public bool IsFan
    {
        get => _isFan;
        set => _isFan = value;
    }

    private MinionController _minion = null;

    [Space(20)]
    [Header("Like")]
    [SerializeField] private Transform _spawnPoint = null;
    [SerializeField] private Transform _targetInterfacePoint = null;
    [SerializeField] private LikeCounter _likeCounter = null;
    [SerializeField] private GameObject _likePrefab = null;

    #region Like
    public void CreateLike()
    {
        GameObject like = Instantiate(_likePrefab, _targetInterfacePoint);
        like.transform.position = Camera.main.WorldToScreenPoint(_spawnPoint.position);

        Vector3 endPosition = _targetInterfacePoint.position;
        StartCoroutine(MoveToCounter(like.transform, endPosition));
    }

    private IEnumerator MoveToCounter(Transform like, Vector3 endPosition)
    {
        Vector3 startPosition = like.position;
        float timer = 0;
        float speed = Random.Range(1200f, 1800f);
        float duration = Vector2.Distance(startPosition, endPosition) / speed;

        while (timer <= duration)
        {
            timer += Time.deltaTime;
            like.position = Vector3.Lerp(startPosition, endPosition, timer / duration);
            yield return null;
        }

        _likeCounter.Count++;
        Destroy(like.gameObject);
    }
    #endregion Like

    #region Actions
    private void OnMiss()
    {
        float rnd = Random.Range(0f, 1f);
        if (rnd >= 0.8f)
            Like();
        else if (rnd < 0.2f)
            Fail();
    }

    private void OnGood()
    {
        if (IsFan)
        {
            LikeWithClamping();
        }
        else
        {
            float rnd = Random.Range(0f, 1f);
            if (rnd >= 0.5f)
            {
                LikeWithClamping();
            }
        }
    }

    private void OnPerfect()
    {
        LikeWithClamping();
    }

    private void OnTooSlow()
    {
        float rnd = Random.Range(0f, 1f);
        if (rnd >= 0.8f)
            Like();
        else if (rnd < 0.2f)
            Fail();
    }

    private void DoClamp()
    {
        _animator.SetTrigger("Succes");
    }

    private void DoFail()
    {
        _animator.SetTrigger("Fail");
    }

    private bool Dancing
    {
        get => _animator.GetBool("Dancing");
        set => _animator.SetBool("Dancing", value);
    }

    private bool IsTimerActive => _timer.Active && !_timer.TimeOver;

    private float RandomDelay => Random.Range(0f, 0.1f);

    private void LikeWithClamping()
    {
        StartCoroutine(DelayedAction(RandomDelay, () => {
            CreateLike();
            DoClamp();
        }));
    }

    private void Like()
    {
        StartCoroutine(DelayedAction(RandomDelay, () => { CreateLike(); }));
    }

    private void Fail()
    {
        StartCoroutine(DelayedAction(RandomDelay, () => { DoFail(); }));
    }

    #endregion Actions

    private void Start()
    {
        IsFan = Random.Range(0, 2) == 1;

        if (IsFan) _renderer.material = _fanMaterial;
        else _renderer.material = _viewerMaterial;

        _minion = FindObjectOfType<MinionController>();
        _minion.OnMiss += OnMiss;
        _minion.OnGood += OnGood;
        _minion.OnPerfect += OnPerfect;
        _minion.OnTooSlow += OnTooSlow;
    }

    private void Update()
    {
        if (IsTimerActive)
        {
            if (!Dancing) Dancing = true;
        }
        else
        {
            if (Dancing) Dancing = false;
        }

        Vector3 direction = _lookAtTransform.position - _bodyTransform.position;
        direction.y = 0f;
        direction.Normalize();
        Quaternion lookAtRotation = Quaternion.LookRotation(direction, Vector3.up);
        _bodyTransform.rotation = Quaternion.Lerp(_bodyTransform.rotation, lookAtRotation, _smoothness);
    }

    private IEnumerator DelayedAction(float delay, UnityAction action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
