using BubbleSystem;
using EventSystem;
using InputSystem;
using PowerUpSystem;
using System.Collections;
using UnityEngine;

namespace GameSystem {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }

        [Header("Bubble Letter Speed")]
        [SerializeField] private float letterSpeed;
        [SerializeField] private float letterSpeedIncrease;
        [SerializeField] private float letterMaxSpeed;
        [SerializeField] private float letterSpawnInterval;
        [SerializeField] private float letterMinSpawnInterval;

        [Header("Bubble Word Speed")]
        [SerializeField] private float wordSpeed;
        [SerializeField] private float wordSpeedIncrease;
        [SerializeField] private float wordMaxSpeed;
        [SerializeField] private float wordSpawnInterval;
        [SerializeField] private float wordMinSpawnInterval;

        [Header("Difficulty Increase Duration")]
        [SerializeField] private float difficultyIncreaseDuration;




        private GameState gameState;
        private Gravity gravity;

        private float health = 1f;
        private int combo = 0;
        private int maxCombo = 0;

        private Coroutine difficultyCoroutine;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("GameManager: Instance Already Exists!");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnBubblePop>(OnBubblePop);
            EventBus.Subscribe<EVT_OnStartClick>(OnStartClick);
            EventBus.Subscribe<EVT_OnBubbleInBoundary>(OnBubbleInBoundary);

            // Power Ups Events
            EventBus.Subscribe<EVT_OnCOFFEEPowerUpEffect>(OnCOFFEEPowerUpEffect);
            EventBus.Subscribe<EVT_OnPULLPowerUpEffect>(OnPULLPowerUpEffect);
            EventBus.Subscribe<EVT_OnCOMMITPowerUpEffect>(OnCommitPowerUp);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnBubblePop>(OnBubblePop);
            EventBus.Unsubscribe<EVT_OnStartClick>(OnStartClick);
            EventBus.Unsubscribe<EVT_OnBubbleInBoundary>(OnBubbleInBoundary);

            // Power Ups Events
            EventBus.Unsubscribe<EVT_OnCOFFEEPowerUpEffect>(OnCOFFEEPowerUpEffect);
            EventBus.Unsubscribe<EVT_OnPULLPowerUpEffect>(OnPULLPowerUpEffect);
            EventBus.Unsubscribe<EVT_OnCOMMITPowerUpEffect>(OnCommitPowerUp);
        }


        private void Start() {
            gameState = GameState.WAITING;

            gravity = RandomizeGravity();

            EventBus.Publish(new EVT_OnLetterSpeedChange(letterSpeed));
            EventBus.Publish(new EVT_OnLetterIntervalChange(letterSpawnInterval));
            EventBus.Publish(new EVT_OnWordSpeedChange(wordSpeed));
            EventBus.Publish(new EVT_OnWordIntervalChange(wordSpawnInterval));

            EventBus.Publish(new EVT_OnGameStateChange(gameState));
            EventBus.Publish(new EVT_OnGravityChange(gravity));
            EventBus.Publish(new EVT_OnHealthChange(health));
        }


        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================

        private void OnBubblePop(EVT_OnBubblePop evt) {
            combo++;
            if (combo > maxCombo) maxCombo = combo;
            EventBus.Publish(new EVT_OnComboChange(combo));
        }


        private void OnStartClick(EVT_OnStartClick evt) {
            if (gameState == GameState.WAITING) {
                gameState = GameState.PLAYING;
                StartDifficultyIncrease();
                EventBus.Publish(new EVT_OnGameStateChange(gameState));
            }
        }

        private void OnBubbleInBoundary(EVT_OnBubbleInBoundary evt) {
            health -= evt.healthDamage;
            combo = 0;

            EventBus.Publish(new EVT_OnHealthChange(health));
            EventBus.Publish(new EVT_OnComboChange(combo));

            if (health <= 0) {
                gameState = GameState.GAMEOVER;
                EventBus.Publish(new EVT_OnGameStateChange(gameState));
            }
        }

        // Power Ups Events
        private void OnCOFFEEPowerUpEffect(EVT_OnCOFFEEPowerUpEffect evt) {
            if (health < 100) {
                health += evt.healAmount;
                if (health > 100) health = 100;
                EventBus.Publish(new EVT_OnHealthChange(health));
            }
        }

        private void OnPULLPowerUpEffect(EVT_OnPULLPowerUpEffect evt) {
            Gravity randomGravity = RandomizeGravity();

            while (randomGravity == gravity) {
                randomGravity = RandomizeGravity();
            }

            gravity = randomGravity;
            EventBus.Publish(new EVT_OnGravityChange(gravity));
        }

        private void OnCommitPowerUp(EVT_OnCOMMITPowerUpEffect evt) {
            health = evt.fullHealth;
            EventBus.Publish(new EVT_OnHealthChange(health));

        }


        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================

        private Gravity RandomizeGravity() {
            var gravityValues = (Gravity[])System.Enum.GetValues(typeof(Gravity));
            return gravityValues[Random.Range(0, gravityValues.Length)];
        }

        private void StartDifficultyIncrease() {
            if (difficultyCoroutine != null) {
                StopCoroutine(difficultyCoroutine);
            }

            // If duration is 0 or less, just set to max values immediately
            if (difficultyIncreaseDuration <= 0f) {
                Debug.LogWarning("DifficultyIncreaseDuration is 0 or less. Setting difficulty to max values immediately.");
                letterSpeed = letterMaxSpeed;
                wordSpeed = wordMaxSpeed;
                letterSpawnInterval = letterMinSpawnInterval;
                wordSpawnInterval = wordMinSpawnInterval;

                EventBus.Publish(new EVT_OnLetterSpeedChange(letterSpeed));
                EventBus.Publish(new EVT_OnLetterIntervalChange(letterSpawnInterval));
                EventBus.Publish(new EVT_OnWordSpeedChange(wordSpeed));
                EventBus.Publish(new EVT_OnWordIntervalChange(wordSpawnInterval));
                return;
            }

            difficultyCoroutine = StartCoroutine(DifficultyIncreaseRoutine());
        }

        private IEnumerator DifficultyIncreaseRoutine() {
            Debug.Log($"Starting difficulty increase over {difficultyIncreaseDuration}s. Target speeds: letter={letterMaxSpeed}, word={wordMaxSpeed}");

            float elapsed = 0f;
            float initialLetterSpeed = letterSpeed;
            float initialWordSpeed = wordSpeed;
            float initialLetterSpawnInterval = letterSpawnInterval;
            float initialWordSpawnInterval = wordSpawnInterval;

            while (elapsed < difficultyIncreaseDuration) {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / difficultyIncreaseDuration);

                letterSpeed = Mathf.Lerp(initialLetterSpeed, letterMaxSpeed, progress);
                wordSpeed = Mathf.Lerp(initialWordSpeed, wordMaxSpeed, progress);
                letterSpawnInterval = Mathf.Lerp(initialLetterSpawnInterval, letterMinSpawnInterval, progress);
                wordSpawnInterval = Mathf.Lerp(initialWordSpawnInterval, wordMinSpawnInterval, progress);

                EventBus.Publish(new EVT_OnLetterSpeedChange(letterSpeed));
                EventBus.Publish(new EVT_OnLetterIntervalChange(letterSpawnInterval));
                EventBus.Publish(new EVT_OnWordSpeedChange(wordSpeed));
                EventBus.Publish(new EVT_OnWordIntervalChange(wordSpawnInterval));

                yield return null;
            }

            Debug.Log($"Difficulty increase finished. Setting final speeds to letterMaxSpeed: {letterMaxSpeed}, wordMaxSpeed: {wordMaxSpeed}");

            // Ensure final values are set precisely at the end
            letterSpeed = letterMaxSpeed;
            wordSpeed = wordMaxSpeed;
            letterSpawnInterval = letterMinSpawnInterval;
            wordSpawnInterval = wordMinSpawnInterval;

            EventBus.Publish(new EVT_OnLetterSpeedChange(letterSpeed));
            EventBus.Publish(new EVT_OnLetterIntervalChange(letterSpawnInterval));
            EventBus.Publish(new EVT_OnWordSpeedChange(wordSpeed));
            EventBus.Publish(new EVT_OnWordIntervalChange(wordSpawnInterval));
        }



        // =====================================================================
        //
        //                          Getters and Setters
        //
        // =====================================================================
        public Gravity GetGravity() { return gravity; }
        public GameState GetGameState() { return gameState; }
        public float GetLetterSpeed() { return letterSpeed; }
        public float GetWordSpeed() { return wordSpeed; }
        public int GetMaxCombos() { return maxCombo; }
    }
}