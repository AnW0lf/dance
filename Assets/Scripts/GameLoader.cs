using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class GameLoader : MonoBehaviour
    {

        private void Start()
        {
            string sceneName = Player.Instance.LastLevel;
            SceneManager.LoadScene(sceneName);
        }
    }
}