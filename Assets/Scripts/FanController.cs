using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using Random = UnityEngine.Random;
using System.Linq;

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
    [SerializeField] private DanceStyle _likedStyle = DanceStyle.UNSTYLED;
    [SerializeField] private Material _classicSkin = null;
    [SerializeField] private Material _jazzSkin = null;
    [SerializeField] private Material _streetSkin = null;
    [SerializeField] private Renderer _renderer = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private Timer _timer = null;

    public DanceStyle LikedStyle
    {
        get => _likedStyle;
        set => _likedStyle = value;
    }

    private MinionController _minion = null;

    [Space(20)]
    [Header("Resource")]
    [SerializeField] private Transform _spawnPoint = null;

    [Space(20)]
    [Header("Like")]
    [SerializeField] private Transform _likeTargetInterfacePoint = null;
    [SerializeField] private InterfaceCounter _likeCounter = null;
    [SerializeField] private GameObject _likePrefab = null;

    [Space(20)]
    [Header("Money")]
    [SerializeField] private Transform _moneyTargetInterfacePoint = null;
    [SerializeField] private InterfaceCounter _moneyCounter = null;
    [SerializeField] private GameObject _moneyPrefab = null;
    [SerializeField] private int _moneyCount = 1;

    #region Like
    public bool LikePowerUpped { get; private set; } = false;

    public void CreateLike()
    {
        int count = LikePowerUpped ? 2 : 1;
        LikePowerUpped = false;

        CreateFlyResource(_likePrefab, _spawnPoint.position,
            count, _likeTargetInterfacePoint,
            _likeCounter, Random.Range(0.5f, 1.2f));
    }
    #endregion Like

    #region Money
    public void CreateMoney(int count)
    {
        if (count <= 0) return;

        StartCoroutine(CoinSpawner(count, 0.035f));
    }

    private IEnumerator CoinSpawner(int count, float delay)
    {
        WaitForSeconds wait = new WaitForSeconds(delay);
        for (int i = 0; i < 14 && i < count; i++)
        {
            CreateCoin(1);
            yield return wait;
        }

        if (count > 14)
            CreateCoin(count - 14);
    }

    private void CreateCoin(int count)
    {
        CreateFlyResource(_moneyPrefab, _spawnPoint.position,
            count, _moneyTargetInterfacePoint,
            _moneyCounter, 0.75f);
    }
    #endregion Money

    #region Resource
    public void CreateFlyResource(GameObject prefab, Vector3 spawnPoint, int count, Transform target, InterfaceCounter counter, float flyDuration)
    {
        FlyResource resource = Instantiate(prefab).GetComponent<FlyResource>();
        resource.transform.position = spawnPoint;
        resource.Count = count;

        StartCoroutine(MoveToCounter(resource, target, counter, flyDuration));
    }

    private IEnumerator MoveToCounter(FlyResource resource, Transform target, InterfaceCounter counter, float duration)
    {
        Vector3 startPosition = resource.transform.position;
        float timer = 0;

        while (timer <= duration)
        {
            timer += Time.deltaTime;
            resource.transform.LookAt(Camera.main.transform);
            resource.transform.position = Vector3.Lerp(startPosition, target.position, timer / duration);
            yield return null;
        }

        counter.Count += resource.Count;
        Destroy(resource.gameObject);
    }
    #endregion Resource

    #region Actions
    private void OnMiss(Dance dance)
    {
        float rnd = Random.Range(0f, 1f);
        if (rnd >= 0.8f)
            Like();
        else if (rnd < 0.2f)
            Fail();
    }

    private void OnGood(Dance dance)
    {
        if (LikedStyle == dance.Style)
        {
            if (!LikePowerUpped)
            {
                bool styleIsPoweredUp = FindObjectsOfType<FanController>().Where((fan) => fan.LikePowerUpped).Count() > 0;
                if (styleIsPoweredUp) LikePowerUpped = Random.Range(0f, 1f) > 0.8f;
            }

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

    private void OnPerfect(Dance dance)
    {
        if (LikedStyle == dance.Style)
        {
            if (!LikePowerUpped)
            {
                bool styleIsPoweredUp = FindObjectsOfType<FanController>().Where((fan) => fan.LikePowerUpped).Count() > 0;
                if (styleIsPoweredUp) LikePowerUpped = Random.Range(0f, 1f) > 0.5f;
            }

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

    private void OnTooSlow(Dance dance)
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
            CreateMoney(_moneyCount);
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
        LikedStyle = (DanceStyle)Random.Range(1, 4);

        switch (LikedStyle)
        {
            case DanceStyle.CLASSIC:
                _renderer.material = _classicSkin;
                break;
            case DanceStyle.JAZZ:
                _renderer.material = _jazzSkin;
                break;
            case DanceStyle.STREET:
                _renderer.material = _streetSkin;
                break;
            default:
                break;
        }

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
