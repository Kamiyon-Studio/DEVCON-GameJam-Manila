using EventSystem;
using InputSystem;
using System.Collections;
using UnityEngine;
using SceneLoaderSystem;

namespace UI.Intro {
    public class IntroUI : MonoBehaviour {
        private Animator animator;

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        private void OnEnable() {
            EventBus.Subscribe<EVT_EscapeClick>(OnEscapeClicked);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_EscapeClick>(OnEscapeClicked);
        }


        private void OnEscapeClicked(EVT_EscapeClick evt) {
            AnimEnd();
        }

        public void AnimEnd() {
            SceneLoader.Instance.LoadScene(2);

        }
    }
}