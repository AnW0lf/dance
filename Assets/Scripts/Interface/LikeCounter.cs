using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class LikeCounter : MonoBehaviour
{
    [SerializeField] private int _count = 0;
    [SerializeField] private TextMeshProUGUI _counter = null;
    [SerializeField] private Image _icon = null;

    private bool _doPulse = false;
    private Coroutine _pulse = null;

    public int Count
    {
        get => _count;
        set
        {
            _doPulse = _count != value;
            _count = value;
            _counter.text = _count.ToString();
        }
    }

    private void Start()
    {
        Count = _count;
    }

    private void Update()
    {
        if (_doPulse && _pulse == null)
        {
            _doPulse = false;
            _pulse = StartCoroutine(Pulse());
        }
    }

    private IEnumerator Pulse()
    {
        Vector3 startScale = _icon.transform.localScale;
        Vector3 maxScale = _icon.transform.localScale * 1.2f;

        float timer = 0f;
        float duration = 0.3f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            _icon.transform.localScale = Vector3.Lerp(startScale, maxScale, timer / duration);
            yield return null;
        }

        timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            _icon.transform.localScale = Vector3.Lerp(maxScale, startScale, timer / duration);
            yield return null;
        }

        _pulse = null;
    }
}
