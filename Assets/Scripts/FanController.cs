using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using Random = UnityEngine.Random;

public class FanController : MonoBehaviour
{
    [Space(20)]
    [Header("Look At settings")]
    [SerializeField] private Transform _lookAtTransform = null;
    [SerializeField] private Transform _bodyTransform = null;
    [SerializeField] private float _smoothness = 0.05f;

    [Space(20)]
    [Header("Wish settings")]
    [SerializeField] private Wish _wish = null;
    [SerializeField] private float _holdDuration = 6f;
    [Range(0f, 1f)]
    [SerializeField] public float _chancePerSecond = 0.2f;
    [SerializeField] private StyleSprite[] _styleSprites = null;
    private Coroutine _wishSpawner = null;
    

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
    public bool LikePowerUpped { get; private set; } = false;

    public void CreateLike()
    {
        Like like = Instantiate(_likePrefab).GetComponent<Like>();
        like.transform.position = _spawnPoint.position;
        like.Count = LikePowerUpped ? 2 : 1;
        LikePowerUpped = false;

        Vector3 endPosition = _targetInterfacePoint.position;
        StartCoroutine(MoveToCounter(like, endPosition));
    }

    private IEnumerator MoveToCounter(Like like, Vector3 endPosition)
    {
        Vector3 startPosition = like.transform.position;
        float timer = 0;
        float duration = Random.Range(0.5f, 1.2f);

        while (timer <= duration)
        {
            timer += Time.deltaTime;
            like.transform.LookAt(Camera.main.transform);
            like.transform.position = Vector3.Lerp(startPosition, endPosition, timer / duration);
            yield return null;
        }

        _likeCounter.Count += like.Count;
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
            LikeWithClapping();
        }
        else
        {
            float rnd = Random.Range(0f, 1f);
            if (rnd >= 0.5f)
            {
                LikeWithClapping();
            }
        }
    }

    private void OnPerfect()
    {
        LikeWithClapping();
    }

    private void OnTooSlow()
    {
        float rnd = Random.Range(0f, 1f);
        if (rnd >= 0.8f)
            Like();
        else if (rnd < 0.2f)
            Fail();
    }

    private void OnSetDance(Dance dance)
    {
        if (!IsWishSpawned) return;
        if (Wish == dance.Style)
        {
            LikePowerUpped = true;
            BreakWish();
        }
    }

    private bool LoopClapping
    {
        get => _animator.GetBool("LoopClapping");
        set => _animator.SetBool("LoopClapping", value);
    }

    private void DoClap()
    {
        Randomizer = Random.Range(0f, 1f);
        if (Randomizer == 0.5f) Randomizer += 0.1f;
        _animator.SetTrigger("Succes");
    }

    private void DoFail()
    {
        _animator.SetTrigger("Fail");
    }

    public bool Dancing
    {
        get => _animator.GetBool("Dancing");
        set => _animator.SetBool("Dancing", value);
    }

    private bool IsTimerActive => _timer.Active && !_timer.TimeOver;

    private float RandomDelay => Random.Range(0f, 0.3f);

    private float Randomizer
    {
        get => _animator.GetFloat("Randomizer");
        set => _animator.SetFloat("Randomizer", value);
    }

    public void LikeWithClapping()
    {
        StartCoroutine(DelayedAction(RandomDelay, () => {
            CreateLike();
            DoClap();
        }));
    }

    public void Like()
    {
        StartCoroutine(DelayedAction(RandomDelay, () => { CreateLike(); }));
    }

    public void Fail()
    {
        StartCoroutine(DelayedAction(RandomDelay * 3f, () => { DoFail(); }));
    }

    public void DoLoopClapping()
    {
        StartCoroutine(DelayedAction(RandomDelay * 2f, () => LoopClapping = true));
    }

    #endregion Actions

    #region Wish
    private IEnumerator SpawnWishProcessor(float delay)
    {
        yield return new WaitForSeconds(delay);

        WaitForSeconds second = new WaitForSeconds(1f);
        while (Random.Range(0f, 1f) > _chancePerSecond)
            yield return second;

        StyleSprite styleSprite = _styleSprites[Random.Range(0, _styleSprites.Length)];
        Wish = styleSprite.Style;
        _wish.SetWish(styleSprite.Sprite);

        yield return new WaitForSeconds(_holdDuration);

        _wish.Visible = false;
        Wish = DanceStyle.UNSTYLED;
        _wishSpawner = null;
    }

    private void SpawnWish(float delay)
    {
        if (_wishSpawner != null) StopCoroutine(_wishSpawner);
        _wishSpawner = StartCoroutine(SpawnWishProcessor(3f));
    }

    private void BreakWish()
    {
        _wish.Visible = false;
        StopCoroutine(_wishSpawner);
        _wishSpawner = null;
    }

    public bool IsWishSpawning => _wishSpawner != null;
    public bool IsWishSpawned => Wish != DanceStyle.UNSTYLED;

    public DanceStyle Wish { get; private set; } = DanceStyle.UNSTYLED;

    [Serializable]
    private class StyleSprite
    {
        [SerializeField] private DanceStyle _style = DanceStyle.UNSTYLED;
        [SerializeField] private Sprite _sprite = null;

        public DanceStyle Style => _style;
        public Sprite Sprite => _sprite;
    }
    #endregion Wish

    private void Awake()
    {
        _animator.SetFloat("IdleOffset", Random.Range(0f, 1f));
    }

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
        _minion.OnSetDance += OnSetDance;
    }

    private void Update()
    {
        if (IsTimerActive)
        {
            if (!Dancing) Dancing = true;
            if (!IsWishSpawning) SpawnWish(2f);
        }
        else
        {
            if (IsWishSpawning) BreakWish();
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
