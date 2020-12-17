using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Assets.Scripts;

public class LevelScript : MonoBehaviour
{
    [Header("Start level")]
    [SerializeField] private string _currentLevelName = string.Empty;
    [SerializeField] private MoveCamera _moveCamera = null;
    [SerializeField] private LevelName _levelName = null;
    [SerializeField] private MovePanel[] _panels = null;

    [Space(20)]
    [Header("End level")]
    [SerializeField] private string _nextLevelName = string.Empty;
    [SerializeField] private MinionController _minion = null;
    [SerializeField] private Timer _timer = null;
    [SerializeField] private MovePanel _bottomPanel = null;
    [SerializeField] private LevelProgress _levelProgress = null;
    [SerializeField] private GameObject _victoryScreen = null;
    [SerializeField] private GameObject _defeatScreen = null;

    private bool _levelEnded = false;

    private void Start()
    {
        Analytics.Instance.LogLevelStartedEvent(Player.Instance.LevelNumber);
        Player.Instance.LastLevel = _currentLevelName;
    }

    private void LateUpdate()
    {
        if (_timer.TimeOver)
        {
            if (_bottomPanel.Visible) _bottomPanel.Visible = false;

            if (!_levelEnded && _minion.CurrentAnimationTag == MinionController.MinionAnimationTag.END)
            {
                _levelEnded = true;
                EndLevel();
            }
        }
    }

    public void StartLevel()
    {
        _moveCamera.OnBegin += () => LeanTween.delayedCall(0.3f, () => _levelName.Show());
        _moveCamera.OnEnd += () =>
        {
            foreach (var panel in _panels) panel.Visible = true;
        };

        _moveCamera.Begin();
    }

    private void EndLevel()
    {
        FanController[] fans = FindObjectsOfType<FanController>();
        foreach (var fan in fans)
        {
            fan.Dancing = false;
            fan.LikeWithClapping();
        }

        StartCoroutine(OpenScreen(2.2f));
    }

    private IEnumerator OpenScreen(float delay)
    {
        yield return new WaitForSeconds(delay);

        bool win = _levelProgress.Progress >= 1f;

        _minion.EndGame(win);

        if (win)
        {
            Analytics.Instance.LogLevelCompleteEvent(Player.Instance.LevelNumber);
            Player.Instance.LastLevel = _nextLevelName;

            _victoryScreen.SetActive(true);

            FanController[] fans = FindObjectsOfType<FanController>();
            foreach (var fan in fans) fan.DoLoopClapping();
        }
        else _defeatScreen.SetActive(true);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
