using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fans : MonoBehaviour
{
    [SerializeField] private int _visibleCount = 0;
    private List<Transform> fans = new List<Transform>();

    public int VisibleCount
    {
        get => _visibleCount;
        set
        {
            _visibleCount = Mathf.Clamp(value, 0, fans.Count);
            for (int i = 0; i < fans.Count; i++)
                fans[i].gameObject.SetActive(i < _visibleCount);
        }
    }

    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
            fans.Add(transform.GetChild(i));

        for (int i = 0; i < fans.Count; i++)
        {
            Transform fan = fans[i];
            fan.GetChild(0).GetChild(0).GetComponent<Animator>().SetFloat("IdleOffset", Random.Range(0f, 1f));
            if (i >= _visibleCount)
                fans[i].gameObject.SetActive(false);
        }
    }
}
