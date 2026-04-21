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
    public class SideBox : MonoBehaviour {

        public RectTransform sideBoxRect;
        public Graphic hoverTarget;
        public float hideXPos;
        public float peekXPos;
        public float showXPos;
        public float animationSpeed = 2.0f;

        private bool hoverEnabled = false;
        private bool isHovered = false;
        private Coroutine co_sliding = null;
        public bool isSliding => co_sliding != null;

        private PointerEventData pointerData;
        private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

        public void EnableHover() {
            hoverEnabled = true;
            isHovered = false;
        }

        public void DisableHover() {
            hoverEnabled = false;
            isHovered = false;
        }

        private void Update() {
            if (!hoverEnabled) return;
			if (GameSettingManager.instance.isMenuOn) return;
			if (DialogueManager.instance.IsLogOpen) return;
			if (TutorialManager.instance.IsRunning) return;

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
                SlideTo(showXPos);
            }
            else if (!mouseOver && isHovered) {
                isHovered = false;
                SlideTo(peekXPos);
            }
        }

        public void SlideTo(float targetX) {
            if (co_sliding != null) StopCoroutine(co_sliding);
            co_sliding = StartCoroutine(Sliding(targetX));
        }

        private IEnumerator Sliding(float targetX) {
            Vector2 start = sideBoxRect.anchoredPosition;
            float startX = start.x;
            float percent = 0f;
            AudioManager.instance.PlayPreloadedSFX("sideBoxHover", AudioManager.instance.sfxMixer);

            while (percent < 1f) {
                percent += Time.deltaTime * animationSpeed;
                float eased = 1f - Mathf.Pow(1f - percent, 3f);
                float x = Mathf.Lerp(startX, targetX, eased);
                sideBoxRect.anchoredPosition = new Vector2(x, start.y);
                yield return null;
            }

            sideBoxRect.anchoredPosition = new Vector2(targetX, start.y);
            co_sliding = null;
        }
    }
}
