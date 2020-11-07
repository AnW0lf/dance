using UnityEngine;
using System.Collections;

public class StartLevelScript : MonoBehaviour
{
    [SerializeField] private LevelName _levelName = null;
    [SerializeField] private MovePanel[] _panels = null;

    private void Start()
    {
        LeanTween.delayedCall(0.3f, () => _levelName.Show());

        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(4f);
        foreach (var panel in _panels) panel.Visible = true;
    }
}
