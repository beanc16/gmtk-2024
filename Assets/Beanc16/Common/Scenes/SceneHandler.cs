using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;



namespace Beanc16.Common.Scenes
{
    [System.Serializable]
    public class BeforeSceneLoadedEvent : UnityEvent { }

    [System.Serializable]
    public class AfterSceneLoadedEvent : UnityEvent { }

    [System.Serializable]
    public class SceneLoadFailedEvent : UnityEvent { }

    [System.Serializable]
    public class BeforeExitGameEvent : UnityEvent { }



    public class SceneHandler : MonoBehaviour
    {
        private static SceneHandler instance;

        // Load events (works for reloads / restarts too)
        public static BeforeSceneLoadedEvent OnBeforeSceneLoaded;
        
        public static AfterSceneLoadedEvent OnAfterSceneLoaded;
        
        public static SceneLoadFailedEvent OnSceneLoadFailed;

        // Exit events
        public static BeforeExitGameEvent OnBeforeExitGame;

        private static bool IsPaused
        {
            get => Time.timeScale == 0;
        }

        private void Awake()
        {
            SceneHandler sceneHandler = GetComponent<SceneHandler>();

            if (sceneHandler != null)
            {
                instance = sceneHandler;
            }
        }



        public static void LoadScene(string sceneName)
        {
            // The scene CAN be loaded
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                TryCallOnBeforeSceneLoadedEvent();  // OnBeforeSceneLoaded()
                SceneManager.LoadScene(sceneName);  // Load scene
                TryCallOnAfterSceneLoadedEvent();   // OnAfterSceneLoaded()
            }

            // The scene CAN NOT be loaded
            else
            {
                Debug.LogError("ERROR - Scene: " + sceneName + " could not be loaded.");
                TryCallOnSceneLoadFailedEvent();    // OnSceneLoadFailed()
            }
        }
        
        public static void LoadSceneAfterSeconds(string sceneName, float seconds)
        {
            IEnumerator coroutine = LoadSceneAfterSecondsPassed(sceneName, seconds);
            instance.StartCoroutine(coroutine);
        }
        
        public static void ReloadCurrentScene()
        {
            Scene curScene = SceneManager.GetActiveScene();
            LoadScene(curScene.name);
        }
        
        public static void ReloadCurrentSceneAfterSeconds(float seconds)
        {
            Scene curScene = SceneManager.GetActiveScene();
            LoadSceneAfterSeconds(curScene.name, seconds);
        }

        public static void TogglePauseScene()
        {
            PauseScene(!IsPaused);
        }

        public static void PauseScene(bool shouldPause = true)
        {
            if (shouldPause)
            {
                Time.timeScale = 0;
            }

            else
            {
                Time.timeScale = 1;
            }
        }

        public static void UnpauseScene()
        {
            PauseScene(false);
        }

        public static void ExitGame()
        {
            TryCallOnBeforeExitGameEvent();         // OnBeforeExitGame()
            Application.Quit();
        }



        public static void NextLevel()
        {
            Scene curScene = SceneManager.GetActiveScene();
            int levelNumber = curScene.name[curScene.name.Length - 1] - 0;
            string sceneNameWithoutLevel = curScene.name.Remove(curScene.name.Length - 1, 1);
            string sceneName = sceneNameWithoutLevel + (levelNumber + 1).ToString();

            // The scene CAN be loaded
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                TryCallOnBeforeSceneLoadedEvent();  // OnBeforeSceneLoaded()
                SceneManager.LoadScene(sceneName);  // Load scene
                TryCallOnAfterSceneLoadedEvent();   // OnAfterSceneLoaded()
            }

            // The scene CAN NOT be loaded
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
        
        
        
        private static IEnumerator LoadSceneAfterSecondsPassed(string sceneName, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            LoadScene(sceneName);
        }



        private static void TryCallOnAfterSceneLoadedEvent()
        {
            if (OnAfterSceneLoaded != null)
            {
                OnAfterSceneLoaded.Invoke();
            }
        }

        private static void TryCallOnBeforeSceneLoadedEvent()
        {
            if (OnBeforeSceneLoaded != null)
            {
                OnBeforeSceneLoaded.Invoke();
            }
        }

        private static void TryCallOnSceneLoadFailedEvent()
        {
            if (OnSceneLoadFailed != null)
            {
                OnSceneLoadFailed.Invoke();
            }
        }

        private static void TryCallOnBeforeExitGameEvent()
        {
            if (OnBeforeExitGame != null)
            {
                OnBeforeExitGame.Invoke();
            }
        }
    }
}
