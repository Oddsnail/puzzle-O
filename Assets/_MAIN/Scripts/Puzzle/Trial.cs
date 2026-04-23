using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using origin.graphics;
using Unity.VisualScripting;
using origin.graphic;

namespace origin.puzzle {

	public class Digit {
		public RectTransform rect;
		public Image image;
		public TMPro.TMP_Text text;
		public ColorblindIcon icon;

		public Digit(RectTransform rect, Image image, TMPro.TMP_Text text, ColorblindIcon icon) {
			this.rect = rect;
			this.image = image;
			this.text = text;
			this.icon = icon;
		}
	} 

	public class Trial : MonoBehaviour {
		private static readonly float unHighlightedTransparency = 0.6f;
		private static readonly float unHighlightedScale = 0.8f;
		private static readonly float highlightSpeed = 6.0f;

		public GameObject numberPrefab;
		private bool isActive = false;
		private int numDigits = 0;
		private int digitCount = 0;
		private List<Digit> digitDisplays = new();
		private Coroutine co_highlighting;
		private bool is_highlighting => co_highlighting != null;

		public void Initialize(int numDigits) {
			this.numDigits = numDigits;
			for (int i = 0; i < numDigits; i++) {
				GameObject digitObj = Instantiate(numberPrefab, transform);

				RectTransform digitRect = digitObj.GetComponent<RectTransform>();
				Image digitImage = digitObj.GetComponent<Image>();
				TMPro.TMP_Text digitText = digitObj.GetComponentInChildren<TMPro.TMP_Text>();
				ColorblindIcon digitIcon = digitObj.GetComponentInChildren<ColorblindIcon>();

				digitText.text = "";

				digitDisplays.Add(new(digitRect, digitImage, digitText, digitIcon));
			}
			HighlightTrial(false);
			isActive = true;
		}

		public void HighlightTrial(bool highlight) {
			if (is_highlighting) StopCoroutine(co_highlighting);

			co_highlighting = StartCoroutine(Highlighting(highlight));
		}

		private IEnumerator Highlighting(bool highlight) {
			float percent = 0;
			Vector3 startScale = digitDisplays[0].rect.localScale;
			Vector3 targetScale = highlight ? new Vector3(1f, 1f, 1) : new Vector3(unHighlightedScale, unHighlightedScale, 1);
			float startTransparency = digitDisplays[0].image.color.a;
			float targetTransparency = highlight ? 1f : unHighlightedTransparency;
			while (percent < 1f) {
				percent += Time.deltaTime * highlightSpeed;
				Vector3 currentScale = Vector3.Lerp(startScale, targetScale, percent);
				foreach (var display in digitDisplays) {
					display.rect.localScale = currentScale;
					display.image.color = new Color(display.image.color.r, display.image.color.g, display.image.color.b, Mathf.Lerp(startTransparency, targetTransparency, percent));
					display.text.color = new Color(display.text.color.r, display.text.color.g, display.text.color.b, Mathf.Lerp(startTransparency, targetTransparency, percent));
				}
				yield return null;
			}
		}

		void OnDestroy() {
			if (co_highlighting != null) StopCoroutine(co_highlighting);
		}

		public void InputDigit(int digit, Color color, Color subcolor, int order, bool doHitEffect = false) {
			if (!isActive || digitCount >= numDigits) return;

			digitDisplays[digitCount].image.color = color;
			digitDisplays[digitCount].text.color = subcolor;
			digitDisplays[digitCount].text.text = digit.ToString();
			digitDisplays[digitCount].icon.SetOrder(order);
			digitDisplays[digitCount].icon.SetColor(color);
			digitDisplays[digitCount].icon.Refresh();

			if (doHitEffect) {
				GameObject hitEffector = Instantiate(digitDisplays[digitCount].rect.gameObject, digitDisplays[digitCount].rect);
				hitEffector.AddComponent<HitEffector>();
			}
			digitCount++;
		}
	}
}
