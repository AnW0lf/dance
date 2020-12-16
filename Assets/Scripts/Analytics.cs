using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class Analytics : MonoBehaviour
    {

        public static Analytics Instance { get; private set; } = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this) Destroy(gameObject);

            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(() =>
                {
                    FB.ActivateApp();
                });
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            // Check the pauseStatus to see if we are in the foreground
            // or background
            if (!pauseStatus)
            {
                //app resume
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                }
                else
                {
                    //Handle FB.Init
                    FB.Init(() =>
                    {
                        FB.ActivateApp();
                    });
                }
            }
        }

        /**
 * Include the Facebook namespace via the following code:
 * using Facebook.Unity;
 *
 * For more details, please take a look at:
 * developers.facebook.com/docs/unity/reference/current/FB.LogAppEvent
 */
        public void LogLevelStartedEvent(int number)
        {
            var parameters = new Dictionary<string, object>();
            parameters["number"] = number;
            FB.LogAppEvent(
                "LevelStarted",
                parameters: parameters
            );
        }

        /**
 * Include the Facebook namespace via the following code:
 * using Facebook.Unity;
 *
 * For more details, please take a look at:
 * developers.facebook.com/docs/unity/reference/current/FB.LogAppEvent
 */
        public void LogLevelCompleteEvent(int number)
        {
            var parameters = new Dictionary<string, object>();
            parameters["number"] = number;
            FB.LogAppEvent(
                "LevelComplete",
                parameters: parameters
            );
        }
    }
}