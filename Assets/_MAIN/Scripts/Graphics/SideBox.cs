using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace origin.graphic {
    public class SideBox : MonoBehaviour{

        public bool shown { get; private set; }
        public RectTransform sideBoxRect;

        private Coroutine co_showing;
        private bool isShowing => shown && co_showing != null;
        private bool isHiding => !shown && co_showing != null;
        public float animationXoffset = 0.0f;
        public float animationSpeed = 2.0f;

		public void OnHoverEnter() { Show(); }

		public void OnHoverExit() { Hide(); }

		private void Show() {
			if (isShowing) return;
			if (isHiding) StopCoroutine(co_showing);

			shown = true;
			co_showing = StartCoroutine(Showing(true));
			return;
		}

		private void Hide() {
			if (isHiding) return;
			if (isShowing) StopCoroutine(co_showing);

			shown = false;
			co_showing = StartCoroutine(Showing(false));
			return;
		}

		private IEnumerator Showing(bool state) {
			float startX = sideBoxRect.anchoredPosition.x;
			float startY = sideBoxRect.anchoredPosition.y;

			float endX = state ? 0f : 0f + animationXoffset;

			float percent = 0f;
			while (percent < 1f) {
				percent += Time.deltaTime * animationSpeed;

				sideBoxRect.anchoredPosition = Vector2.Lerp(new(startX, startY), new(endX, startY), percent);

				yield return null;
			}

			sideBoxRect.anchoredPosition = new(endX, startY);

			co_showing = null;
		}
    }
}
