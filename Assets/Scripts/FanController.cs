using UnityEngine;
using System.Collections;

public class FanController : MonoBehaviour
{
    [SerializeField] private bool _isFan = false;

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
        float duration = 0.6f;

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
}
