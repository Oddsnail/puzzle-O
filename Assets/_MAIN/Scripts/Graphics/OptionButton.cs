using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace origin.graphic {
	public class OptionButton : MonoBehaviour{

		public bool highlighted { get; private set; } = false;
		public RectTransform buttonColor;
		public RectTransform button;

		private Coroutine co_highlighting;
		private bool isHighlighting => highlighted && co_highlighting != null;
		private bool isUnHighlighting => !highlighted && co_highlighting != null;
		private const float highlightOffsetX = 25f;
		private const float highlightSize = 1.04f;
		private const float highlightSpeed = 3.0f;
		public Color defaultColor;
		public Color selectColor;
		
		public void OnHoverEnter() { Highlight(); }

		public void OnHoverExit() { UnHighlight(); }

		public void SetDefaultColor(Color color) { defaultColor = color; }
		public void SetSelectColor(Color color) { selectColor = color; }

		private void Highlight() {
			if (isHighlighting) return;
			if (isUnHighlighting) StopCoroutine(co_highlighting);

			highlighted = true;
			co_highlighting = StartCoroutine(Highlighting(true));
			return;
		}

		private void UnHighlight() {
			if (isUnHighlighting) return;
			if (isHighlighting) StopCoroutine(co_highlighting);
			
			highlighted = false;
			co_highlighting = StartCoroutine(Highlighting(false));
			return;
		}
		
		private IEnumerator Highlighting(bool state) {

			Color startColor = buttonColor.GetComponent<Image>().color;
			Vector3 startSize = button.localScale;
			float startX = buttonColor.anchoredPosition.x;

			Color endColor = state ? selectColor : defaultColor;
			Vector3 endSize = state ? new(highlightSize, highlightSize, 1.0f) : new(1.0f, 1.0f, 1.0f);
			float endX = state ? 15f + highlightOffsetX : 15f;

			float percent = 0f;
			while (percent < 1f) {
				percent += Time.deltaTime * highlightSpeed;
				float k = Mathf.SmoothStep(0f, 1f, percent);

				button.localScale = Vector3.Lerp(startSize, endSize, k);
				buttonColor.localScale = Vector3.Lerp(startSize, endSize, k);
				buttonColor.anchoredPosition = Vector2.Lerp(new(startX, -15f), new(endX, -15f), k);
				buttonColor.GetComponent<Image>().color = Color.Lerp(startColor, endColor, k);

				yield return null;
			}

			button.localScale = endSize;
			buttonColor.localScale = endSize;
			buttonColor.anchoredPosition = new(endX, -15f);
			buttonColor.GetComponent<Image>().color = endColor;

			co_highlighting = null;
        }
	}
}