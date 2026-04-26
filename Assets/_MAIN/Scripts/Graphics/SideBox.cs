using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using origin.audio;
using origin.settings;
using origin.dialogue;
using origin.tutorial;
using origin.puzzle;

namespace origin.graphic {
    public class SideBox : MonoBehaviour {

		public RectTransform sideBoxRect;
		public Outline clipOutline;
        public Graphic hoverTarget;
        public float hideXPos;
        public float peekXPos;
        public float showXPos;
        public float animationSpeed = 2.0f;

        private bool hoverEnabled = false;
        private bool isHovered = false;
        private Coroutine co_sliding = null;
		public bool isSliding => co_sliding != null;

		private bool clipped = false;
		private Coroutine co_soundBuffer = null;

		private IEnumerator SoundBuffer() {
			yield return new WaitForSeconds(0.1f);
			co_soundBuffer = null;
		}

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
		
		public void OnClipButtonPressed() {
            if (!hoverEnabled) return;
			if (TutorialManager.instance.IsRunning) return;

			clipped = !clipped;
			AudioManager.instance.PlayPreloadedSFX("optionChange");
			if (co_soundBuffer != null) StopCoroutine(co_soundBuffer);
			co_soundBuffer = StartCoroutine(SoundBuffer());
			
			if (clipped) {
				clipOutline.effectColor = Color.white;
				SlideTo(showXPos);
			} else {
				clipOutline.effectColor = Color.black;
			}
		}

        private void Update() {
            if (!hoverEnabled) return;
			if (GameSettingManager.instance.isMenuOn) return;
			if (DialogueManager.instance.IsLogOpen) return;
			if (TutorialManager.instance.IsRunning) return;

			if (clipped) return;

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
			if (targetX != showXPos && clipped) {
				clipped = false;
				clipOutline.effectColor = Color.black;
			}else if (co_soundBuffer == null) {
				AudioManager.instance.PlayPreloadedSFX("sideBoxHover");
				co_soundBuffer = StartCoroutine(SoundBuffer());
			}
            co_sliding = StartCoroutine(Sliding(targetX));
        }

        private IEnumerator Sliding(float targetX) {
            Vector2 start = sideBoxRect.anchoredPosition;
            float startX = start.x;
            float percent = 0f;

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
