using UnityEngine;
using System.Collections;

public class FanController : MonoBehaviour
{
    [SerializeField] private bool _isFan = false;
    [SerializeField] private Material _fanMaterial = null;
    [SerializeField] private Material _viewerMaterial = null;
    [SerializeField] private Renderer _renderer = null;

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

        while(timer <= duration)
        {
            timer += Time.deltaTime;
            like.position = Vector3.Lerp(startPosition, endPosition, timer / duration);
            yield return null;
        }

        _likeCounter.Count++;
        Destroy(like.gameObject);
    }
    #endregion Like

    public bool IsFan
    {
        get => _isFan;
        set => _isFan = value;
    }

    private void Start()
    {
        IsFan = Random.Range(0, 2) == 1;

        if (IsFan) _renderer.material = _fanMaterial;
        else _renderer.material = _viewerMaterial;

        _minion = FindObjectOfType<MinionController>();
        _minion.OnPerfect += CreateLike;
        _minion.OnGood += () => { if (IsFan || Random.Range(0f, 1f) >= 0.5f) CreateLike(); };
        _minion.OnTooSlow += () => { if (Random.Range(0f, 1f) >= 0.8f) CreateLike(); };
        _minion.OnMiss += () => { if (Random.Range(0f, 1f) >= 0.8f) CreateLike(); };
    }
}
