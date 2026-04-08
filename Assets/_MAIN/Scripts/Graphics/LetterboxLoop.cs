using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace origin.graphic {
	public class LetterboxLoop : MonoBehaviour {
		[Range(-1f, 1f)]
		public float speed;
		public GameObject inner;

		public float hideYPos;
		public float showYPos;

		private Material material;
		private float offset = 0;
		private Coroutine co_colorTransitioning;
		private Image letterboxColorImage;
		private bool initialized = false;
		private bool is_colorTransitioning => co_colorTransitioning != null;

		void Start() {
			Image image = gameObject.GetComponent<Image>();
			Image innerImage = inner.GetComponent<Image>();
			letterboxColorImage = image;
			if (image != null) {
				material = new Material(gameObject.GetComponent<Image>().material);
				image.material = material;
				innerImage.material = material;
				initialized = true;
			}
			else {
				Debug.LogError("[ERROR] : no image found for letterbox.");
			}
		}

		public void ColorTransition(Color color) {
			if (is_colorTransitioning) StopCoroutine(co_colorTransitioning);

			co_colorTransitioning = StartCoroutine(ColorTransitioning(color));
		}
		
		private IEnumerator ColorTransitioning(Color endColor) {
			Color startColor = letterboxColorImage.color;

			float percent = 0;
			while (percent < 1f) {
				percent += Time.deltaTime * 2.0f;
				letterboxColorImage.color = Color.Lerp(startColor, endColor, percent);

				yield return null;
			}

			co_colorTransitioning = null;
        }

		void Update() {
			if (initialized) {
				offset += Time.deltaTime * speed / 10f;
				material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
			}
		}

		void OnDestroy() {
			if (initialized) {
				Destroy(material);
			}
		}
	}
}