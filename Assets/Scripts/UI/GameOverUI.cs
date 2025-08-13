using UnityEngine;
using UnityEngine.UI;

using SceneLoaderSystem;
using EventSystem;
using GameSystem;
using TMPro;
using ScoreSystem;

namespace UI {
    public class GameOverUI : MonoBehaviour {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private AudioSource bgMusicSource;

        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI comboText;

        [SerializeField] private AudioClip bgMusic;

        private GameState gameState;

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnGameStateChange>(OnGameStateChange);

            if (GameManager.Instance != null) {
                gameState = GameManager.Instance.GetGameState();
                comboText.text = $"COMBO: {GameManager.Instance.GetMaxCombos()}x";
            }

            if (ScoreManager.Instance != null) {
                scoreText.text = $"SCORE: {ScoreManager.Instance.GetScore()}";
            }
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnGameStateChange>(OnGameStateChange);
        }


        private void OnGameStateChange(EVT_OnGameStateChange evt) {
            gameState = evt.state;

            HandleSFX();
        }

        private void Start() {
            restartButton.onClick.AddListener(() => {
                if (SceneLoader.Instance != null) {
                    SceneLoader.Instance.LoadScene(2);
                }
            });

            mainMenuButton.onClick.AddListener(() => {
                if (SceneLoader.Instance != null) {
                    SceneLoader.Instance.LoadScene(0);
                }
            });

            HandleSFX();
        }

        private void HandleSFX() {
            switch (gameState) {
                case GameState.GAMEOVER:
                    bgMusicSource.Stop();
                    bgMusicSource.clip = bgMusic;
                    bgMusicSource.loop = false;
                    bgMusicSource.Play();
                    break;
                default:
                    break;
            }
        }
    }
}