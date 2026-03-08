using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace origin.graphic {
	public class OptionButton : MonoBehaviour{

		public bool highlighted { get; private set; } = false;
		public RectTransform buttonColor;
		public RectTransform button;

		private Coroutine co_highlighting;
		private Image buttonColorImage;
		private Outline buttonSeletedOutline;
		private bool isHighlighting => highlighted && co_highlighting != null;
		private bool isUnHighlighting => !highlighted && co_highlighting != null;
		private const float highlightOffsetX = 25f;
		private const float highlightSize = 1.07f;
		private const float highlightSpeed = 5.0f;
		public Color DEFAULT_UI_GRAY;
		public ColorPalette colorPalette;
		public Color selectedColor;
		public Color outlineColor;

		void Start() {
			buttonColorImage = buttonColor.GetComponent<Image>();
			buttonSeletedOutline = button.GetComponent<Outline>();
        }
		
		public void OnHoverEnter() { Highlight(); }

		public void OnHoverExit() { UnHighlight(); }

		public void SetSelectColor(string colorCode) {
			if (!colorCode.StartsWith('_')) {
                selectedColor = colorPalette.Getcolor(colorCode);
                outlineColor = colorPalette.Getcolor(colorCode, true);
            } else {
				selectedColor = colorPalette.Getcolor(colorCode, true);
				outlineColor = colorPalette.Getcolor(colorCode);
            }
			
		}

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

			Color startColor = buttonColorImage.color;
			Color startOutlineColor = buttonSeletedOutline.effectColor;
			Vector3 startSize = button.localScale;
			float startX = buttonColor.anchoredPosition.x;

			Color endColor = state ? selectedColor : DEFAULT_UI_GRAY;
			Color endOutlineColor = state ? outlineColor : new(outlineColor.r, outlineColor.g, outlineColor.b, 0.0f);
			Vector3 endSize = state ? new(highlightSize, highlightSize, 1.0f) : new(1.0f, 1.0f, 1.0f);
			float endX = state ? 15f + highlightOffsetX : 15f;

			float percent = 0f;
			while (percent < 1f) {
				percent += Time.deltaTime * highlightSpeed;

				button.localScale = Vector3.Lerp(startSize, endSize, percent);
				buttonColor.localScale = Vector3.Lerp(startSize, endSize, percent);
				buttonColor.anchoredPosition = Vector2.Lerp(new(startX, -15f), new(endX, -15f), percent);
				buttonColor.GetComponent<Image>().color = Color.Lerp(startColor, endColor, percent);
				buttonSeletedOutline.effectColor = Color.Lerp(startOutlineColor, endOutlineColor, percent);

				yield return null;
			}

			button.localScale = endSize;
			buttonColor.localScale = endSize;
			buttonColor.anchoredPosition = new(endX, -15f);
			buttonColor.GetComponent<Image>().color = endColor;
			buttonSeletedOutline.effectColor = endOutlineColor;

			co_highlighting = null;
        }
	}
}