using UnityEngine;

using EventSystem;
using PoolSystem;
using GameSystem;
using BubbleSystem;
using WordBubbleSystem;
using SO;

namespace BoundarySystem {
	public class Boundary : MonoBehaviour {
		[SerializeField] private Gravity gravityToKill;

        private int score = 0;
		private Gravity currentGravity;

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnGravityChange>(OnGravityChanged);
        }

        private void OnGravityChanged(EVT_OnGravityChange evt) {
            currentGravity = evt.gravity;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (currentGravity != gravityToKill) return;

            if (collision.gameObject.GetComponent<Bubble>() != null) {
                Bubble bubble = collision.gameObject.GetComponent<Bubble>();
                PoolItemSO poolItemSO = collision.gameObject.GetComponent<Bubble>().GetPoolItemSO();

                PoolRuntimeSystem.Instance.ReturnToPool(poolItemSO.itemName, collision.gameObject);
                EventBus.Publish(new EVT_OnBubblePop(score));
                EventBus.Publish(new EVT_OnBubbleInBoundary(bubble.GetHealthDamage()));
            }
            else if (collision.gameObject.GetComponent<WordBubble>() != null) {
                PoolItemSO poolItemSO = collision.gameObject.GetComponent<WordBubble>().GetPoolItemSO();
                PoolRuntimeSystem.Instance.ReturnToPool(poolItemSO.itemName, collision.gameObject);
                EventBus.Publish(new EVT_OnWordDestroyed());
            }
        }
    }
}