using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace origin.graphic {
	public class HitEffector : MonoBehaviour {

		public static readonly float maxSize = 1.3f;
		public static readonly float vanishSpeed = 2f;

		private Coroutine co_live = null;

		void Awake() {
			co_live = StartCoroutine(Vanish());
		}

		public IEnumerator Vanish() {
			float percent = 0f;
			RectTransform selfRT = gameObject.GetComponent<RectTransform>();
			selfRT.anchorMax = new Vector2(0.5f, 0.5f);
			selfRT.anchorMin = new Vector2(0.5f, 0.5f);
			selfRT.anchoredPosition = new Vector2(0f, 0f);

			Transform rootCanvas = gameObject.GetComponentInParent<SideBox>().transform;
			transform.SetParent(rootCanvas, worldPositionStays: true);

			CanvasGroup selfCG = gameObject.GetComponent<CanvasGroup>();

			while (percent < 1f) {
				percent += Time.deltaTime * vanishSpeed;
				float eased = 1f - Mathf.Pow(1f - percent, 3f);
				float x = Mathf.Lerp(1f, maxSize, eased);
				selfRT.localScale = new Vector3(x, x, 1f);
				selfCG.alpha = Mathf.Lerp(1f, 0f, eased);
				yield return null;
			}

			co_live = null;
			Destroy(gameObject);
		}
	}
}