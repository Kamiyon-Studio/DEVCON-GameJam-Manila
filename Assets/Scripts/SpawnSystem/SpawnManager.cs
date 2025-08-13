using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BubbleSystem;
using WordBubbleSystem;
using EventSystem;
using GameSystem;
using SO;
using PoolSystem;

namespace SpawnSystem {
    public class SpawnManager : MonoBehaviour {
        public static SpawnManager Instance { get; private set; }

        [Header("SpawnPoints")]
        [SerializeField] private SpawnPointsSO leftSpawnPoints;
        [SerializeField] private SpawnPointsSO rightSpawnPoints;
        [SerializeField] private SpawnPointsSO topSpawnPoints;
        [SerializeField] private SpawnPointsSO bottomSpawnPoints;

        [Header("Spawn Object")]
        [SerializeField] private PoolItemSO letterPoolItem;
        [SerializeField] private PoolItemSO wordPoolItem;

        [Header("Spawn Settings")]
        [SerializeField] private float letterSpawnInterval;
        [SerializeField] private float wordSpawnInterval;
        [SerializeField] private int maxLetterSpawnCount = 20;
        [SerializeField] private int maxWordSpawnCount = 5;

        private int letterSpawnCount = 0;
        private int wordSpawnCount = 0;

        private bool isFreezed;

        private GameState gameState;
        private Gravity gravity;

        private SpawnPointsSO selectedSpawnPoints = null;
        private Coroutine _spawnLetterCoroutine;
        private Coroutine _spawnWordCoroutine;
        private List<Transform> availableSpawnPoints;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("SpawnManager: Instance Already Exists!");
                Destroy(gameObject);
                return;
            }
            Instance = this;

            isFreezed = false;
        }

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnGameStateChange>(OnGameStateChange);
            EventBus.Subscribe<EVT_OnGravityChange>(OnGravityChange);
            EventBus.Subscribe<EVT_OnBubblePop>(OnBubblePop);
            EventBus.Subscribe<EVT_OnWordDestroyed>(OnWordDestroyed);
            EventBus.Subscribe<EVT_OnBubbleFreeze>(OnBubbleFreeze);

            EventBus.Subscribe<EVT_OnLetterIntervalChange>(OnLetterIntervalChange);
            EventBus.Subscribe<EVT_OnWordIntervalChange>(OnWordIntervalChange);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnGameStateChange>(OnGameStateChange);
            EventBus.Unsubscribe<EVT_OnGravityChange>(OnGravityChange);
            EventBus.Unsubscribe<EVT_OnBubblePop>(OnBubblePop);
            EventBus.Unsubscribe<EVT_OnWordDestroyed>(OnWordDestroyed);
            EventBus.Unsubscribe<EVT_OnBubbleFreeze>(OnBubbleFreeze);

            EventBus.Unsubscribe<EVT_OnLetterIntervalChange>(OnLetterIntervalChange);
            EventBus.Unsubscribe<EVT_OnWordIntervalChange>(OnWordIntervalChange);

        }

        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================

        private void OnBubblePop(EVT_OnBubblePop evt) {
            if (letterSpawnCount > 0) {
                letterSpawnCount--;
            }
        }

        private void OnWordDestroyed(EVT_OnWordDestroyed evt) {
            if (wordSpawnCount > 0) {
                wordSpawnCount--;
            }
        }

        private void OnGameStateChange(EVT_OnGameStateChange evt) {
            gameState = evt.state;

            switch (gameState) {
                case GameState.WAITING:
                    Debug.Log("Waiting");
                    break;

                case GameState.PLAYING:
                    Debug.Log("Playing");
                    if (_spawnLetterCoroutine == null) {
                        _spawnLetterCoroutine = StartCoroutine(LetterSpawnRoutine());
                    }
                    if (_spawnWordCoroutine == null) {
                        _spawnWordCoroutine = StartCoroutine(WordSpawnRoutine());
                    }
                    break;

                case GameState.PAUSED:
                    break;

                case GameState.GAMEOVER:
                    Debug.Log("Game Over");
                    if (_spawnLetterCoroutine != null) {
                        StopCoroutine(_spawnLetterCoroutine);
                        _spawnLetterCoroutine = null;
                    }
                    if (_spawnWordCoroutine != null) {
                        StopCoroutine(_spawnWordCoroutine);
                        _spawnWordCoroutine = null;
                    }
                    break;

                default:
                    break;
            }
        }

        private void OnGravityChange(EVT_OnGravityChange evt) {
            gravity = evt.gravity;
            switch (gravity) {
                case Gravity.LEFT:
                    selectedSpawnPoints = rightSpawnPoints;
                    break;
                case Gravity.RIGHT:
                    selectedSpawnPoints = leftSpawnPoints;
                    break;
                case Gravity.UP:
                    selectedSpawnPoints = bottomSpawnPoints;
                    break;
                case Gravity.DOWN:
                    selectedSpawnPoints = topSpawnPoints;
                    break;
                default:
                    Debug.LogWarning("No gravity direction set for spawning.");
                    return;
            }

            if (selectedSpawnPoints != null) {
                availableSpawnPoints = selectedSpawnPoints.spawnPoints.Select(go => go.transform).ToList();
            }
            else if (availableSpawnPoints != null) {
                availableSpawnPoints.Clear();
            }
        }

        private void OnBubbleFreeze(EVT_OnBubbleFreeze evt) {
            isFreezed = evt.isfreezed;
        }

        private void OnLetterIntervalChange(EVT_OnLetterIntervalChange evt) {
            letterSpawnInterval = evt.interval;
        }

        private void OnWordIntervalChange(EVT_OnWordIntervalChange evt) {
            wordSpawnInterval = evt.interval;
        }

        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================

        private Transform GetRandomSpawnPoint() {
            if (availableSpawnPoints == null || availableSpawnPoints.Count == 0) {
                if (selectedSpawnPoints == null || selectedSpawnPoints.spawnPoints.Count == 0) {
                    return null;
                }
                availableSpawnPoints = selectedSpawnPoints.spawnPoints.Select(go => go.transform).ToList();
            }
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform spawnPoint = availableSpawnPoints[randomIndex];
            availableSpawnPoints.RemoveAt(randomIndex);
            return spawnPoint;
        }

        private IEnumerator LetterSpawnRoutine() {
            while (true) {
                yield return new WaitForSeconds(letterSpawnInterval);

                if (gameState == GameState.PLAYING && letterSpawnCount < maxLetterSpawnCount && !isFreezed) {
                    SpawnLetter();
                    letterSpawnCount++;
                }
            }
        }

        private IEnumerator WordSpawnRoutine() {
            while (true) {
                yield return new WaitForSeconds(wordSpawnInterval);

                if (gameState == GameState.PLAYING && wordSpawnCount < maxWordSpawnCount && !isFreezed) {
                    SpawnWord();
                    wordSpawnCount++;
                }
            }
        }

        private void SpawnLetter() {
            Transform spawnPoint = GetRandomSpawnPoint();

            if (spawnPoint != null) {
                PoolRuntimeSystem.Instance.SpawnFromPool(letterPoolItem.itemName, spawnPoint.position, spawnPoint.rotation);
            }
            else {
                Debug.LogWarning($"No spawn points assigned for the current gravity direction: {gravity}");
            }
        }

        private void SpawnWord() {
            Transform spawnPoint = GetRandomSpawnPoint();

            if (spawnPoint != null) {
                PoolRuntimeSystem.Instance.SpawnFromPool(wordPoolItem.itemName, spawnPoint.position, spawnPoint.rotation);
            }
            else {
                Debug.LogWarning($"No spawn points assigned for the current gravity direction: {gravity}");
            }
        }
    }
}
