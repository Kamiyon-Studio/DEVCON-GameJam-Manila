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

namespace BubbleSystem {
    public class Bubble : MonoBehaviour {

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed;

        [Header("Letter Settings")]
        [SerializeField] private TextMeshProUGUI letterText;
        [SerializeField] private PoolItemSO letterPoolItem;
        [SerializeField, Range(0f, 1f)] private float uniqueLetterChances = 0.5f;

        [Header("Score Points and damage")]
        [SerializeField] private int scorePoints;
        [SerializeField] private float healthDamage = 0.1f;

        [Header("SFX source SO")]
        [SerializeField] private PoolItemSO sfxSourceSO;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip popClip;

        [Header("Particle System")]
        [SerializeField] private GameObject particlesPrefab;

        private Camera mainCamera;
        private CircleCollider2D circleCollider;
        private Rigidbody2D rb;
        private AudioSource audioSource;

        private Vector2 mousePos;
        private Gravity gravity;
        private GameState gameState;

        private string randomLetterString;
        private bool isDragging;

        private Coroutine DebugPowerUpRoutine;
        private Coroutine RefactorPowerUpRoutine;
        private Coroutine ChatGPTPowerUpRoutine;

        private string uniqueLetters = "ABCDEFGHILMOPRSTU";
        private string letters = "JKNQVWXYZ";

        private GameObject currentAudioSource;
        private AudioClip lastPlayedClip;

        // PowerUp Variables
        private bool canMove;
        private bool isFreezed;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            mainCamera = Camera.main;
            circleCollider = GetComponent<CircleCollider2D>();
            rb = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnLeftClickDown>(OnLeftClickDown);
            EventBus.Subscribe<EVT_OnRightClickDown>(OnDragDown);
            EventBus.Subscribe<EVT_OnRightClickUp>(OnDragUp);

            EventBus.Subscribe<EVT_OnGameStateChange>(OnGameStateChange);
            EventBus.Subscribe<EVT_OnGravityChange>(OnGravityChanged);
            EventBus.Subscribe<EVT_OnLetterSpeedChange>(OnLetterSpeedChange);

            EventBus.Subscribe<EVT_OnLetterClick>(OnLetterClick);


            // PowerUp Events
            EventBus.Subscribe<EVT_OnDEBUGPowerUpEffect>(OnDEBUGPowerUpEffect);
            EventBus.Subscribe<EVT_OnREFACTORPowerUpEffect>(OnREFACTORPowerUpEffect);
            EventBus.Subscribe<EVT_OnPUSHPowerUpEffect>(OnPUSHPowerUpEffect);
            EventBus.Subscribe<EVT_OnCHATGPTPowerUpEffect>(OnCHATGPTPowerUpEffect);

            canMove = true;
            isFreezed = false;

            if (GameManager.Instance != null) {
                gravity = GameManager.Instance.GetGravity();
                moveSpeed = GameManager.Instance.GetLetterSpeed();
                gameState = GameManager.Instance.GetGameState();
            }
            else {
                Debug.LogWarning("WordBubble: GameManager is null!");
            }

            BubbleLetterGen();
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnLeftClickDown>(OnLeftClickDown);
            EventBus.Unsubscribe<EVT_OnRightClickDown>(OnDragDown);
            EventBus.Unsubscribe<EVT_OnRightClickUp>(OnDragUp);

            EventBus.Unsubscribe<EVT_OnGameStateChange>(OnGameStateChange);
            EventBus.Unsubscribe<EVT_OnLetterSpeedChange>(OnLetterSpeedChange);

            EventBus.Unsubscribe<EVT_OnLetterClick>(OnLetterClick);


            // PowerUp Events
            EventBus.Unsubscribe<EVT_OnDEBUGPowerUpEffect>(OnDEBUGPowerUpEffect);
            EventBus.Unsubscribe<EVT_OnREFACTORPowerUpEffect>(OnREFACTORPowerUpEffect);
            EventBus.Unsubscribe<EVT_OnPUSHPowerUpEffect>(OnPUSHPowerUpEffect);
            EventBus.Unsubscribe<EVT_OnCHATGPTPowerUpEffect>(OnCHATGPTPowerUpEffect);


            if (DebugPowerUpRoutine != null) StopCoroutine(DebugPowerUpRoutine);
            if (RefactorPowerUpRoutine != null) StopCoroutine(RefactorPowerUpRoutine);
        }

        private void Update() {
            if (isDragging) {
                DragBubble();

                if (InputManager.Instance.GetLetterPressed() != randomLetterString[0]) {
                    isDragging = false;
                }
            }
        }

        private void FixedUpdate() {
            if (canMove && gameState == GameState.PLAYING) {
                Movement();
            }
            else {
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
        private void OnLetterClick(EVT_OnLetterClick evt) {
            if (evt.letter == randomLetterString) {
                particlesPrefab.SetActive(true);
            } else {
                particlesPrefab.SetActive(false);
            }
        }

        private void OnGameStateChange(EVT_OnGameStateChange evt) {
            gameState = evt.state;
        }

        private void OnLeftClickDown(EVT_OnLeftClickDown evt) {
            if (IsMouseOver()) {
                if (randomLetterString == null) return;
                if (InputManager.Instance.GetLetterPressed() == randomLetterString[0]) {
                    PlaySFX(popClip);
                    PopBubble();
                }
            }
        }

        private void OnDragDown(EVT_OnRightClickDown evt) {
            if (IsMouseOver()) {
                if (randomLetterString == null) {
                    isDragging = false;
                    return;
                }
                else if (InputManager.Instance.GetLetterPressed() == randomLetterString[0]) {
                    isDragging = true;
                }
            }
        }

        private void OnDragUp(EVT_OnRightClickUp evt) {
            isDragging = false;
        }

        private void OnGravityChanged(EVT_OnGravityChange evt) {
            gravity = evt.gravity;
        }

        // PowerUp Events
        private void OnDEBUGPowerUpEffect(EVT_OnDEBUGPowerUpEffect evt) {
            if (DebugPowerUpRoutine != null) StopCoroutine(DebugPowerUpRoutine);

            isFreezed = true;
            EventBus.Publish(new EVT_OnBubbleFreeze(isFreezed));

            DebugPowerUpRoutine = StartCoroutine(DEBUGPowerUpRoutine(evt.PowerUp));
        }

        private void OnREFACTORPowerUpEffect(EVT_OnREFACTORPowerUpEffect evt) {
            if (RefactorPowerUpRoutine != null) StopCoroutine(RefactorPowerUpRoutine);

            isFreezed = true;
            EventBus.Publish(new EVT_OnBubbleFreeze(isFreezed));

            RefactorPowerUpRoutine = StartCoroutine(REFACTORPowerUpRoutine(evt.PowerUp));
        }

        private void OnPUSHPowerUpEffect(EVT_OnPUSHPowerUpEffect evt) {
            PopBubble();
        }

        private void OnCHATGPTPowerUpEffect(EVT_OnCHATGPTPowerUpEffect evt) {
            if (ChatGPTPowerUpRoutine != null) StopCoroutine(ChatGPTPowerUpRoutine);

            isFreezed = true;
            EventBus.Publish(new EVT_OnBubbleFreeze(isFreezed));

            ChatGPTPowerUpRoutine = StartCoroutine(CHATGPTPowerUpRoutine(evt.PowerUp, evt.letter));
        }

        private void OnLetterSpeedChange(EVT_OnLetterSpeedChange evt) {
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
            isFreezed = false;
            EventBus.Publish(new EVT_OnBubbleFreeze(isFreezed));
        }

        private IEnumerator REFACTORPowerUpRoutine(PowerUpSO powerUpSO) {
            float elapsedTime = 0f;
            float duration = powerUpSO.powerUpDuration;
            char randomLetter;

            rb.linearVelocity = Vector2.zero;
            canMove = false;
            randomLetterString = string.Empty;
            while (elapsedTime < duration) {
                if (Random.value < uniqueLetterChances) {
                    randomLetter = uniqueLetters[Random.Range(0, uniqueLetters.Length)];
                }
                else {
                    randomLetter = letters[Random.Range(0, letters.Length)];
                }
                letterText.text = randomLetter.ToString();
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            canMove = true;
            isFreezed = false;
            EventBus.Publish(new EVT_OnBubbleFreeze(isFreezed));

            if (Random.value < uniqueLetterChances) {
                randomLetter = uniqueLetters[Random.Range(0, uniqueLetters.Length)];
            }
            else {
                randomLetter = letters[Random.Range(0, letters.Length)];
            }

            randomLetterString = randomLetter.ToString();
            letterText.text = randomLetterString;
        }

        private IEnumerator CHATGPTPowerUpRoutine(PowerUpSO powerUpSO, string letter) {
            float elapsedTime = 0f;
            float duration = powerUpSO.powerUpDuration;
            char randomLetter;

            rb.linearVelocity = Vector2.zero;
            canMove = false;
            randomLetterString = string.Empty;
            while (elapsedTime < duration) {
                if (Random.value < uniqueLetterChances) {
                    randomLetter = uniqueLetters[Random.Range(0, uniqueLetters.Length)];
                }
                else {
                    randomLetter = letters[Random.Range(0, letters.Length)];
                }
                letterText.text = randomLetter.ToString();
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            canMove = true;
            isFreezed = false;
            EventBus.Publish(new EVT_OnBubbleFreeze(isFreezed));

            randomLetterString = letter;
            letterText.text = randomLetterString;
        }


        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================

        /// <summary>
        /// Moves the bubble up, down, left, or right
        /// </summary>
        private void Movement() {
            if (isDragging) return;

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
        /// Checks if the letter pressed is the same as the letter on the bubble
        /// </summary>
        private void PopBubble() {
            EventBus.Publish(new EVT_OnBubblePop(scorePoints));
            PoolRuntimeSystem.Instance.ReturnToPool(letterPoolItem.itemName, gameObject);
        }


        /// <summary>
        /// Moves the bubble to the mouse position
        /// </summary>
        private void DragBubble() {
            Vector2 screenPoint = InputManager.Instance.GetMousePosition();
            mousePos = mainCamera.ScreenToWorldPoint(screenPoint);
            Vector2 postion = new Vector2(mousePos.x, mousePos.y);
            rb.MovePosition(postion);
        }


        /// <summary>
        /// Checks if the mouse is over the bubble
        /// </summary>
        /// <returns></returns>
        private bool IsMouseOver() {
            if (circleCollider == null) return false;
            if (InputManager.Instance == null) return false;

            Vector2 screenPoint = InputManager.Instance.GetMousePosition();
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(screenPoint);
            return circleCollider.OverlapPoint(mousePos);
        }


        /// <summary>
        /// Generates a random letter, with a higher chance for vowels.
        /// </summary>
        private void BubbleLetterGen() {
            char randomLetter;

            if (Random.value < uniqueLetterChances) {
                randomLetter = uniqueLetters[Random.Range(0, uniqueLetters.Length)];
            }
            else {
                randomLetter = letters[Random.Range(0, letters.Length)];
            }

            if (letterText != null) {
                randomLetterString = randomLetter.ToString();
                letterText.text = randomLetterString;
            }
            else {
                Debug.LogWarning("Bubble: Letter Text is null");
            }
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
        public PoolItemSO GetPoolItemSO() => letterPoolItem;
        public bool IsDragging() => isDragging;
        public void StopDragging() => isDragging = false;
        public string GetLetterString() => randomLetterString;
        public void MergeBubble() => PopBubble();
        public float GetHealthDamage() => healthDamage;
    }
}