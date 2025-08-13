using UnityEngine;

using BubbleSystem;
using EventSystem;
using GameSystem;
using System.Linq;

namespace BoundarySystem {
    public class UIBoundary : MonoBehaviour {

        [SerializeField] private Gravity[] gravityToEnable;

        private BoxCollider2D boxCollider2D;
        private Gravity currentGravity;

        private void Awake() {
            boxCollider2D = GetComponent<BoxCollider2D>();

        }

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnGravityChange>(OnGravityChange);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnGravityChange>(OnGravityChange);
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.GetComponent<Bubble>() != null) {
                if (collision.gameObject.GetComponent<Bubble>().IsDragging()) {
                    collision.gameObject.GetComponent<Bubble>().StopDragging();
                }
            }
        }


        private void OnGravityChange(EVT_OnGravityChange evt) {
            currentGravity = evt.gravity;

            if (currentGravity == gravityToEnable[0] || currentGravity == gravityToEnable[1]) {
                boxCollider2D.enabled = true;
            }
            else {
                boxCollider2D.enabled = false;
            }
        }
    }
}
