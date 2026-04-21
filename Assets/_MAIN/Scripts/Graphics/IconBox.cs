using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using origin.audio;
using origin.settings;
using origin.dialogue;
using origin.tutorial;

namespace origin.graphic {
    public class IconBox : MonoBehaviour {

        public RectTransform iconBoxRect;
        public Graphic hoverTarget;
        public float highlightSize = 1.3f;
        public float animationSpeed = 6.0f;

        private bool isHovered = false;
        private Coroutine co_scaling = null;

        private PointerEventData pointerData;
        private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();


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
                ScaleTo(highlightSize);
            }
            else if (!mouseOver && isHovered) {
                isHovered = false;
                ScaleTo(1.0f);
            }
        }

        private void ScaleTo(float targetScale) {
            if (co_scaling != null) StopCoroutine(co_scaling);
            co_scaling = StartCoroutine(Scaling(targetScale));
        }

        private IEnumerator Scaling(float targetScale) {
            float startScale = iconBoxRect.localScale.x;
            float percent = 0f;
            if (targetScale != 1.0f) AudioManager.instance.PlayPreloadedSFX("iconBoxHover", AudioManager.instance.sfxMixer);

            while (percent < 1f) {
                percent += Time.deltaTime * animationSpeed;
                float s = Mathf.Lerp(startScale, targetScale, percent);
                float a = percent * (2 * percent - 1) * (percent - 1);
                iconBoxRect.localScale = new Vector3(s, s, 1f);
                iconBoxRect.rotation = Quaternion.Euler(0f, 0f, a * 20);
                yield return null;
            }
            iconBoxRect.localScale = new Vector3(targetScale, targetScale, 1f);
            iconBoxRect.rotation = Quaternion.identity;

            iconBoxRect.localScale = new Vector3(targetScale, targetScale, 1f);
            co_scaling = null;
        }
    }
}
