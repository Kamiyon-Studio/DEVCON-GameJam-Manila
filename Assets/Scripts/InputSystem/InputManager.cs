using EventSystem;
using PoolSystem;
using SO;
using SoundSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystem {
    public class InputManager : MonoBehaviour {
        public static InputManager Instance { get; private set; }

        [Header("SFX source SO")]
        [SerializeField] PoolItemSO sfxSourceSO;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip clickClip;

        private GameObject currentAudioSource;
        private AudioClip lastPlayedClip;

        private InputSystem_Actions inputActions;
        private Dictionary<Key, InputAction> keyActions = new Dictionary<Key, InputAction>();

        private char letterPressed;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================

        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("InputManager: Instance Already Exists!");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            inputActions = new InputSystem_Actions();
        }

        private void OnEnable() {
            if (inputActions == null) {
                Debug.LogWarning("InputManager: inputActions is null!");
                return;
            }

            inputActions.Enable();
            inputActions.Player.Attack.performed += OnAttackPerformed;

            inputActions.Player.Drag.performed += OnDragPerformed;
            inputActions.Player.Drag.canceled += OnDragCanceled;

            inputActions.Player.Start.performed += OnStartPerformed;

            inputActions.Player.Inventory1.performed += OnInventory1Performed;
            inputActions.Player.Inventory2.performed += OnInventory2Performed;
            inputActions.Player.Inventory3.performed += OnInventory3Performed;

            inputActions.Player.skip.performed += OnSkipPerformed;
            CreateAlphabetActions();
        }

        private void OnDisable() {
            if (inputActions == null) {
                Debug.LogWarning("InputManager: inputActions is null!");
                return;
            }

            inputActions.Disable();
            inputActions.Player.Attack.performed -= OnAttackPerformed;

            inputActions.Player.Drag.performed -= OnDragPerformed;
            inputActions.Player.Drag.canceled -= OnDragCanceled;

            inputActions.Player.Start.performed -= OnStartPerformed;

            inputActions.Player.Inventory1.performed -= OnInventory1Performed;
            inputActions.Player.Inventory2.performed -= OnInventory2Performed;
            inputActions.Player.Inventory3.performed -= OnInventory3Performed;

            inputActions.Player.skip.performed -= OnSkipPerformed;
        }

        private void Update() {
            CheckKeyInputHeldDown();
        }


        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================

        private void OnAttackPerformed(InputAction.CallbackContext context) {
            PlaySFX(clickClip);
            EventBus.Publish(new EVT_OnLeftClickDown());
        }

        private void OnDragPerformed(InputAction.CallbackContext context) { EventBus.Publish(new EVT_OnRightClickDown()); }
        private void OnDragCanceled(InputAction.CallbackContext context) { EventBus.Publish(new EVT_OnRightClickUp()); }

        private void OnStartPerformed(InputAction.CallbackContext context) { EventBus.Publish(new EVT_OnStartClick()); }

        private void OnInventory1Performed(InputAction.CallbackContext context) { EventBus.Publish(new EVT_OnInventory1Click()); }
        private void OnInventory2Performed(InputAction.CallbackContext context) { EventBus.Publish(new EVT_OnInventory2Click()); }
        private void OnInventory3Performed(InputAction.CallbackContext context) { EventBus.Publish(new EVT_OnInventory3Click()); }

        private void OnSkipPerformed(InputAction.CallbackContext context) { EventBus.Publish(new EVT_EscapeClick()); }

        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================

        /// <summary>
        /// Creates a new InputAction for each letter in the alphabet
        /// </summary>
        private void CreateAlphabetActions() {
            for (char letter = 'A'; letter <= 'Z'; letter++) {
                var key = (Key)System.Enum.Parse(typeof(Key), letter.ToString());
                var action = new InputAction($"{letter}Hold", binding: $"<Keyboard>/{letter.ToString().ToLower()}");
                action.Enable();
                keyActions[key] = action;
            }
        }


        /// <summary>
        /// Checks if any letter is being held down
        /// </summary>
        private void CheckKeyInputHeldDown() {
            bool isLetterPressed = false;

            foreach (var kvp in keyActions) {
                if (kvp.Value.ReadValue<float>() > 0) {
                    if (!isLetterPressed) {
                        letterPressed = kvp.Key.ToString()[0];
                        EventBus.Publish(new EVT_OnLetterClick(letterPressed.ToString()));
                        isLetterPressed = true;
                    }
                }
            }

            if (!isLetterPressed) {
                letterPressed = '\0';
                EventBus.Publish(new EVT_OnLetterClick(letterPressed.ToString()));
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

        /// <summary>
        /// Get the current mouse position
        /// </summary>
        /// <returns>Vector2 of mouse position</returns>
        public Vector2 GetMousePosition() {
            return inputActions.Player.MouseAxis.ReadValue<Vector2>();
        }


        /// <summary>
        /// Get the currently pressed letter
        /// </summary>
        /// <returns></returns>
        public char GetLetterPressed() {
            return letterPressed;
        }
    }
}