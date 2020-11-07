using UnityEngine;
using System.Collections;

public class StartLevelScript : MonoBehaviour
{
    [SerializeField] private MoveCamera _moveCamera = null;
    [SerializeField] private LevelName _levelName = null;
    [SerializeField] private MovePanel[] _panels = null;

    private void Start()
    {
        _moveCamera.OnBegin += () => LeanTween.delayedCall(0.3f, () => _levelName.Show());
        _moveCamera.OnEnd += () =>
        {
            foreach (var panel in _panels) panel.Visible = true;
        };

        _moveCamera.Begin();
    }
}
