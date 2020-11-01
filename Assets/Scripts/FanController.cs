using UnityEngine;
using System.Collections;

public class FanController : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint = null;
    [SerializeField] private Transform _targetInterfacePoint = null;
    [SerializeField] private LikeCounter _likeCounter = null;
    [SerializeField] private GameObject _likePrefab = null;

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

    private void Start()
    {
        StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        yield return new WaitForSeconds(2f + Random.Range(-0.1f, 0.1f));
        CreateLike();

        yield return new WaitForSeconds(3f + Random.Range(-0.1f, 0.1f));
        CreateLike();

        yield return new WaitForSeconds(4f + Random.Range(-0.1f, 0.1f));
        CreateLike();
    }
}
