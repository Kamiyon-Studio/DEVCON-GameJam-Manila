using UnityEngine;
using UnityEngine.UI;

using SceneLoaderSystem;
using System.Collections;

namespace UI.MainMenu {
    public class MainMenuUI : MonoBehaviour {
        [SerializeField] private Button playButton;

        [SerializeField] private Button creditButton;
        [SerializeField] private Button creditCloseButton;
        [SerializeField] private GameObject creditPanel;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip startSFX;

        private AudioSource audioSource;

        private void Awake() {
            audioSource = GetComponent<AudioSource>();

            Application.targetFrameRate = 60;
        }

        private void Start() {
            creditPanel.SetActive(false);

            playButton.onClick.AddListener(() => {
                playButton.enabled = false;
                StartCoroutine(PlayClickAndLoad());
            });

            creditButton.onClick.AddListener(() => {
                creditPanel.SetActive(true);
            });

            creditCloseButton.onClick.AddListener(() => {
                creditPanel.SetActive(false);
            });
        }

        private IEnumerator PlayClickAndLoad() {
            if (startSFX != null && audioSource != null) {
                audioSource.PlayOneShot(startSFX);
                yield return new WaitForSeconds(startSFX.length);
            }

            if (SceneLoader.Instance != null) {
                SceneLoader.Instance.LoadScene(1);
            }
            else {
                Debug.LogWarning("MainMenuUI: SceneLoader is null!");
            }
        }
    }
}
