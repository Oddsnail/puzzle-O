using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using origin.audio;
using origin.settings;
using origin.dialogue;
using origin.tutorial;
using TMPro;

namespace origin.graphic {
    public class IconBox : MonoBehaviour {

        public RectTransform iconBoxRect;
        public Graphic hoverTarget;
        public float highlightSize = 1.3f;
		public float animationSpeed = 6.0f;
		public RectTransform descriptionText;
		public Outline outline;
		public TextMeshProUGUI invisibleText;

        private bool isHovered = false;
        private Coroutine co_scaling = null;

        private PointerEventData pointerData;
		private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

		private Coroutine co_soundBuffer = null;

        private void Update() {
			if (GameSettingManager.instance.isMenuOn) return;
			if (DialogueManager.instance.IsLogOpen) return;
			if (TutorialManager.instance != null && TutorialManager.instance.IsRunning) return;
            if (pointerData == null)
                pointerData = new PointerEventData(EventSystem.current);

            pointerData.position = Input.mousePosition;
            raycastResults.Clear();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            bool mouseOver = false;
            for (int i = 0; i < raycastResults.Count; i++) {
                if (raycastResults[i].gameObject == hoverTarget.gameObject) {
                    mouseOver = true;
                    break;
                }
            }

            if (mouseOver && !isHovered) {
                isHovered = true;
                ScaleTo(true);
            }
            else if (!mouseOver && isHovered) {
                isHovered = false;
                ScaleTo(false);
            }
        }

		private void ScaleTo(bool larger) {
			if (co_scaling != null) StopCoroutine(co_scaling);
			co_scaling = StartCoroutine(Scaling(larger));
		}
		
		private IEnumerator SoundBuffer() {
			yield return new WaitForSeconds(0.3f);
			co_soundBuffer = null;
		}

		private IEnumerator Scaling(bool larger) {

			float fixedX = descriptionText.anchoredPosition.x;

			float startScale = iconBoxRect.localScale.x;
			float startLocationY = descriptionText.anchoredPosition.y;
			float startTransparency = invisibleText.alpha;

			float targetScale = larger ? highlightSize : 1f;
			float targetLocationY = larger ? 0f : -70f;
			float targetTransparency = larger ? 1f : 0f;
			Color targetOutlineColor = larger ? Color.white : Color.black;
			float percent = 0f;

			if (targetScale != 1.0f && co_soundBuffer == null) {
				AudioManager.instance.PlayPreloadedSFX("iconBoxHover");
				co_soundBuffer = StartCoroutine(SoundBuffer());
			}

			while (percent < 1f) {
				percent += Time.deltaTime * animationSpeed;
				float s = Mathf.Lerp(startScale, targetScale, percent);
				float a = percent * (2 * percent - 1) * (percent - 1);

				iconBoxRect.localScale = new Vector3(s, s, 1f);
				iconBoxRect.rotation = Quaternion.Euler(0f, 0f, a * 20);
				descriptionText.anchoredPosition = new Vector2(fixedX, targetLocationY);
				invisibleText.alpha = Mathf.Lerp(startTransparency, targetTransparency, percent);

				yield return null;
			}
			iconBoxRect.localScale = new Vector3(targetScale, targetScale, 1f);
			iconBoxRect.rotation = Quaternion.identity;
			invisibleText.alpha = targetTransparency;
			outline.effectColor = targetOutlineColor;

			iconBoxRect.localScale = new Vector3(targetScale, targetScale, 1f);
			co_scaling = null;
		}
    }
}
