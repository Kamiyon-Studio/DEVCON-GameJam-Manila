using BubbleSystem;
using EventSystem;
using GameSystem;
using InputSystem;
using PoolSystem;
using PowerUpSystem;
using SO;
using SoundSystem;
using System.Collections;
using TMPro;
using UnityEngine;


namespace WordBubbleSystem {
    public class WordBubble : MonoBehaviour {
        [Header("Word Settings")]
        [SerializeField] private PowerUpSOList powerUpSOList;
        [SerializeField] private PoolItemSO wordPoolItem;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI wordText;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 0.5f;

        [Header("Score Points")]
        [SerializeField] private int scorePoints;

        [Header("SFX source SO")]
        [SerializeField] private PoolItemSO sfxSourceSO;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip getPowerUpSFX;
        [SerializeField] private AudioClip popSFX;

        [Header("Particle System")]
        [SerializeField] private GameObject particlesPrefab;

        private Camera mainCamera;
        private CapsuleCollider2D capsuleCollider2D;
        private Rigidbody2D rb;
        private PowerUpSO powerUpSO;

        private Gravity gravity;
        private GameState gameState;

        private Coroutine DebugPowerUpRoutine;
        private Coroutine RefactorPowerUpRoutine;
        private Coroutine ChatGPTPowerUpRoutine;

        private string originalWord;
        private string displayWord;
        private string missingLetter;
        private bool canPop;

        private GameObject currentAudioSource;
        private AudioClip lastPlayedClip;

        // PowerUp Variables
        private bool canMove;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            mainCamera = Camera.main;
            capsuleCollider2D = GetComponent<CapsuleCollider2D>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnLeftClickDown>(OnLeftClickDown);
            EventBus.Subscribe<EVT_OnGravityChange>(OnGravityChanged);

            EventBus.Subscribe<EVT_OnGameStateChange>(OnGameStateChange);
            EventBus.Subscribe<EVT_OnWordSpeedChange>(OnWordSpeedChange);

            // PowerUp Events
            EventBus.Subscribe<EVT_OnDEBUGPowerUpEffect>(OnDEBUGPowerUpEffect);
            EventBus.Subscribe<EVT_OnREFACTORPowerUpEffect>(OnREFACTORPowerUpEffect);
            EventBus.Subscribe<EVT_OnCHATGPTPowerUpEffect>(OnCHATGPTPowerUpEffect);

            canMove = true;
            canPop = false;

            if (GameManager.Instance != null) {
                gravity = GameManager.Instance.GetGravity();
                moveSpeed = GameManager.Instance.GetWordSpeed();
                gameState = GameManager.Instance.GetGameState();
            }
            else {
                Debug.LogWarning("WordBubble: GameManager is null!");
            }

            SelectRandomWord();
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnLeftClickDown>(OnLeftClickDown);

            EventBus.Unsubscribe<EVT_OnGameStateChange>(OnGameStateChange);
            EventBus.Unsubscribe<EVT_OnWordSpeedChange>(OnWordSpeedChange);

            // PowerUp Events
            EventBus.Unsubscribe<EVT_OnDEBUGPowerUpEffect>(OnDEBUGPowerUpEffect);
            EventBus.Unsubscribe<EVT_OnREFACTORPowerUpEffect>(OnREFACTORPowerUpEffect);
            EventBus.Unsubscribe<EVT_OnCHATGPTPowerUpEffect>(OnCHATGPTPowerUpEffect);

            canPop = false;

            if (DebugPowerUpRoutine != null) StopCoroutine(DebugPowerUpRoutine);
            if (RefactorPowerUpRoutine != null) StopCoroutine(RefactorPowerUpRoutine);
        }


        private void FixedUpdate() {
            if (canMove && gameState == GameState.PLAYING) {
                Movement();
            } else {
                rb.linearVelocity = Vector2.zero;
            }
        }

        private void OnDestroy() {
            EventBus.Unsubscribe<EVT_OnGravityChange>(OnGravityChanged);
        }

        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================
        private void OnGameStateChange(EVT_OnGameStateChange evt) => gameState = evt.state;
        private void OnLeftClickDown(EVT_OnLeftClickDown evt) {
            if (IsMouseOver() && canPop) {
                PlaySFX(popSFX);
                PoolRuntimeSystem.Instance.ReturnToPool(wordPoolItem.itemName, gameObject);
                EventBus.Publish(new EVT_OnWordDestroyed());
                EventBus.Publish(new EVT_OnPowerUpCollected(powerUpSO));
            }
        }

        private void OnGravityChanged(EVT_OnGravityChange evt) {
            gravity = evt.gravity;
        }



        // PowerUp Events
        private void OnDEBUGPowerUpEffect(EVT_OnDEBUGPowerUpEffect evt) {
            if (DebugPowerUpRoutine != null) StopCoroutine(DebugPowerUpRoutine);
            DebugPowerUpRoutine = StartCoroutine(DEBUGPowerUpRoutine(evt.PowerUp));
        }

        private void OnREFACTORPowerUpEffect(EVT_OnREFACTORPowerUpEffect evt) {
            if (RefactorPowerUpRoutine != null) StopCoroutine(RefactorPowerUpRoutine);
            RefactorPowerUpRoutine = StartCoroutine(REFACTORPowerUpRoutine(evt.PowerUp));
        }

        private void OnCHATGPTPowerUpEffect(EVT_OnCHATGPTPowerUpEffect evt) {
            if (ChatGPTPowerUpRoutine != null) StopCoroutine(ChatGPTPowerUpRoutine);
            ChatGPTPowerUpRoutine = StartCoroutine(CHATGPTPowerUpRoutine(evt.PowerUp));
        }

        private void OnWordSpeedChange(EVT_OnWordSpeedChange evt) {
            moveSpeed = evt.speed;
        }


        // =====================================================================
        //
        //                          PowerUp Methods
        //
        // =====================================================================
        private IEnumerator DEBUGPowerUpRoutine(PowerUpSO powerUpSO) {
            rb.linearVelocity = Vector2.zero;
            canMove = false;
            yield return new WaitForSeconds(powerUpSO.powerUpDuration);
            canMove = true;
        }

        private IEnumerator REFACTORPowerUpRoutine(PowerUpSO powerUpSO) {
            float elapsedTime = 0f;
            float duration = powerUpSO.powerUpDuration;

            rb.linearVelocity = Vector2.zero;
            canMove = false;
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            canMove = true;
        }

        private IEnumerator CHATGPTPowerUpRoutine(PowerUpSO powerUpSO) {
            float elapsedTime = 0f;
            float duration = powerUpSO.powerUpDuration;

            rb.linearVelocity = Vector2.zero;
            canMove = false;
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            canMove = true;
        }

        // =====================================================================
        //
        //                          Physics Methods
        //
        // =====================================================================
        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.GetComponent<Bubble>() != null) {
                Bubble letterBubble = collision.gameObject.GetComponent<Bubble>();
                if (!letterBubble.IsDragging()) return;

                if (letterBubble.GetLetterString() == missingLetter) {
                    PlaySFX(getPowerUpSFX);
                    letterBubble.MergeBubble();
                    canPop = true;
                    particlesPrefab.SetActive(true);
                    wordText.text = originalWord;
                }
            }
        }



        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================
        private void Movement() {
            switch (gravity) {
                case Gravity.UP:
                    rb.linearVelocity = Vector2.up * moveSpeed;
                    break;
                case Gravity.DOWN:
                    rb.linearVelocity = Vector2.down * moveSpeed;
                    break;
                case Gravity.LEFT:
                    rb.linearVelocity = Vector2.left * moveSpeed;
                    break;
                case Gravity.RIGHT:
                    rb.linearVelocity = Vector2.right * moveSpeed;
                    break;
            }
        }

        /// <summary>
        /// Selects a random word
        /// </summary>
        private void SelectRandomWord() {
            powerUpSO = powerUpSOList.powerUpSOs[Random.Range(0, powerUpSOList.powerUpSOs.Count)];
            originalWord = powerUpSO.powerUpName;

            int missingIndex = Random.Range(0, originalWord.Length);

            missingLetter = originalWord[missingIndex].ToString();

            char[] wordChars = originalWord.ToCharArray();
            wordChars[missingIndex] = '_';
            displayWord = new string(wordChars);

            wordText.text = displayWord;
        }


        /// <summary>
        /// Checks if the mouse is over the word
        /// </summary>
        /// <returns></returns>
        private bool IsMouseOver() {
            if (capsuleCollider2D == null) return false;
            if (InputManager.Instance == null) return false;

            Vector2 screenPoint = InputManager.Instance.GetMousePosition();
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(screenPoint);
            return capsuleCollider2D.OverlapPoint(mousePos);
        }

        private void PlaySFX(AudioClip clip) {
            if (lastPlayedClip == clip && currentAudioSource != null) {
                AudioSource existingSource = currentAudioSource.GetComponent<AudioSource>();
                if (existingSource != null && existingSource.isPlaying) {
                    return;
                }
            }

            currentAudioSource = PoolRuntimeSystem.Instance.SpawnFromPool(sfxSourceSO.name, transform.position);
            SFXSource sfxSource = currentAudioSource.GetComponent<SFXSource>();

            if (sfxSource != null) {
                sfxSource.PlayClip(clip);
                PoolRuntimeSystem.Instance.StartCoroutine(ResetAudioSource(sfxSource));
                lastPlayedClip = clip;
            }
            else {
                Debug.LogWarning("No SFX Source found");
            }
        }

        private IEnumerator ResetAudioSource(SFXSource sfxSource) {
            yield return new WaitUntil(sfxSource.IsDone);
            currentAudioSource = null;
        }


        // =====================================================================
        //
        //                          Getters and Setters
        //
        // =====================================================================
        public PoolItemSO GetPoolItemSO() => wordPoolItem;


    }
}