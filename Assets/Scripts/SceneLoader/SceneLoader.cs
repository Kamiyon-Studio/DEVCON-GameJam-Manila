using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneLoaderSystem {
    public class SceneLoader : MonoBehaviour {
        public static SceneLoader Instance { get; set; }

        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("SceneLoader: Instance Already Exists!");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        /// <summary>
        /// Loads a scene by its name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        public void LoadScene(string sceneName) {
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Loads a scene by its build index.
        /// </summary>
        /// <param name="buildIndex">The build index of the scene to load.</param>
        public void LoadScene(int buildIndex) {
            SceneManager.LoadScene(buildIndex);
        }

        /// <summary>
        /// Loads a scene asynchronously by its name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        public void LoadSceneAsync(string sceneName) {
            SceneManager.LoadSceneAsync(sceneName);
        }

        /// <summary>
        /// Loads a scene asynchronously by its build index.
        /// </summary>
        /// <param name="buildIndex">The build index of the scene to load.</param>
        public void LoadSceneAsync(int buildIndex) {
            SceneManager.LoadSceneAsync(buildIndex);
        }
    }
}