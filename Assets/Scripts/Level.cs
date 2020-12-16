using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Assets.Scripts;

public class Level : MonoBehaviour
{
    [SerializeField] private Fade _fade = null;
    public static string LastSceneName { get; private set; } = string.Empty;
    public static string LastLevelSceneName { get; private set; } = string.Empty;
    public const string INVENTORY_SCENE_NAME = "Inventory";
    public const string FANS_SCENE_NAME = "Fans";

    private void Start()
    {
        _fade.Visible = false;
    }

    public void LoadScene(string name)
    {
        LastSceneName = SceneManager.GetActiveScene().name;
        print($"Last scene {LastSceneName}");
        if (!LastSceneName.Equals(INVENTORY_SCENE_NAME)
            && !LastSceneName.Equals(FANS_SCENE_NAME))
            LastLevelSceneName = LastSceneName;
        print($"Last level scene {LastLevelSceneName}");

        _fade.Visible = true;
        _fade.OnComplete += (visible) => SceneManager.LoadSceneAsync(name);
    }

    public void LoadInventoryScene()
    {
        LoadScene(INVENTORY_SCENE_NAME);
    }

    public void LoadFansScene()
    {
        LoadScene(FANS_SCENE_NAME);
    }

    public void LoadLastScene()
    {
        LoadScene(LastSceneName);
    }

    public void LoadLastLevel()
    {
        LoadScene(LastLevelSceneName);
    }
}
