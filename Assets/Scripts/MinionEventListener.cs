using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class MinionEventListener : MonoBehaviour
{
    public UnityAction<int> OnBegin { get; set; } = null;
    public UnityAction<int> OnEnd { get; set; } = null;

    public void Begin(int id)
    {
        OnBegin?.Invoke(id);
    }

    public void End(int id)
    {
        OnEnd?.Invoke(id);
    }
}
