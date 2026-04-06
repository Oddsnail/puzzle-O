using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using origin.graphic;

namespace origin.character {
	public class CHARACTER {

		protected CharacterManager characterManager => CharacterManager.instance;

		private const float UNHIGHLIGHTED_STRENGTH = 0.65f;
		private const float UNHIGHLIGHTED_SPEED = 5.5f;

		public string ID;
		public CharacterConfigData config;
		public RectTransform rectTransform;
		private CanvasGroup canvasGroup;
		private Coroutine co_appearing;
		private Coroutine co_disappearing;
		private Coroutine co_moving_x;
		private Coroutine co_moving_y;
		private Coroutine co_highlighting;
		public SpriteManager spriteManager;

		public Color color { get; protected set; } = Color.white;
		public Color highlightedColor => color;
		public Color unhighlightedColor => new(color.r*UNHIGHLIGHTED_STRENGTH, color.b*UNHIGHLIGHTED_STRENGTH, color.g*UNHIGHLIGHTED_STRENGTH, color.a);

		public float visibility => canvasGroup.alpha;
		public bool highlighted { get; protected set; } = true;
		public bool isAppearing => co_appearing != null;
		public bool isDisappearing => co_disappearing != null;
		public bool isMovingX => co_moving_x != null;
		public bool isMovingY => co_moving_y != null;
		public bool isHighlighting => highlighted && co_highlighting != null;
		public bool isUnHighlighting => !highlighted && co_highlighting != null;

		public CHARACTER(string name, CharacterConfigData config, bool isClient = false) {
			this.ID = name.ToLower();
			this.config = config;

			if (!isClient) NormalInitiate();
			else ClientInitiate();
		}

		public void Remove() => UnityEngine.Object.Destroy(rectTransform.gameObject);

		//========================================================================
		//    <!!!> normal / client game object change invoked by command <!!!>
		//========================================================================
		private const float defaultNorMalYPos = 0.0f;
		private const float defaultClientXPos = 0.0f;

		private void NormalInitiate() {
			if (config.prefabNormal == null) Debug.LogError($"[ERROR] No prefabNormal found for {ID}");
			GameObject ob = UnityEngine.Object.Instantiate(config.prefabNormal, characterManager.characterLayer);
			ob.name = $"Character - [{ID}]";
			rectTransform = ob.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = new(0.0f, defaultNorMalYPos);
			canvasGroup = rectTransform.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0;
			ob.SetActive(true);
			spriteManager = rectTransform.GetComponent<SpriteManager>();
		}

		private void ClientInitiate() {
			if (config.prefabClient == null) Debug.LogError($"[ERROR] No prefabClient found for {ID}");
			GameObject ob = UnityEngine.Object.Instantiate(config.prefabClient, characterManager.characterLayer);
			ob.name = $"Client - [{ID}]";
			rectTransform = ob.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = new(defaultClientXPos, defaultNorMalYPos);
			canvasGroup = rectTransform.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0;
			ob.SetActive(true);
		}

		// SET SPRITE & SPRITE X INVERT
		// spriteCode ex) std1-111 or _-221
		public void SetSprite(string spriteCode) {
			string[] spriteNames = spriteCode.Split("-");
			spriteManager.SetSprite(ID, spriteNames[0], spriteNames[1], spriteNames.Length > 2 ? spriteNames[2..] : Array.Empty<string>());
		}

		public void InvertX() {
			var scale = rectTransform.localScale;
			scale.x = -scale.x;
			rectTransform.localScale = scale;
		}

		public Coroutine Highlight(float speed = UNHIGHLIGHTED_SPEED) {
			if (isHighlighting) {
				characterManager.StopCoroutine(co_highlighting);
				co_highlighting = null;
			}
			if (isUnHighlighting) {
				characterManager.StopCoroutine(co_highlighting);
				co_highlighting = null;
			}

			highlighted = true;
			co_highlighting = characterManager.StartCoroutine(Highlighting(highlighted, speed));
			return co_highlighting;
		}

		public Coroutine UnHighlight(float speed = UNHIGHLIGHTED_SPEED) {
			if (isUnHighlighting) {
				characterManager.StopCoroutine(co_highlighting);
				co_highlighting = null;
			}
			if (isHighlighting) {
				characterManager.StopCoroutine(co_highlighting);
				co_highlighting = null;
			}

			highlighted = false;
			co_highlighting = characterManager.StartCoroutine(Highlighting(highlighted, speed));
			return co_highlighting;
		}
		
		public virtual IEnumerator Highlighting(bool highlight, float speed) {
			Color target = highlight ? highlightedColor : unhighlightedColor;
			yield return spriteManager.HighlightTransition(highlight, target, speed);
			co_highlighting = null;
		}

		//========================================================================
		// <!!!> appear / dissappear coroutine invoked by command or header <!!!>
		//========================================================================
		public Coroutine Appear() {
			if (isAppearing) characterManager.StopCoroutine(co_appearing);
			if (isDisappearing) characterManager.StopCoroutine(co_disappearing);

			co_appearing = characterManager.StartCoroutine(Appearing());

			return co_appearing;
		}

		public Coroutine Disappear() {
			if (isDisappearing) characterManager.StopCoroutine(co_disappearing);
			if (isAppearing) characterManager.StopCoroutine(co_appearing);

			co_disappearing = characterManager.StartCoroutine(Disappearing());

			return co_disappearing;
		}

		private const float defaultTransitionTime = 0.25f;

		public IEnumerator Appearing() {
			yield return canvasGroup.DOFade(1f, defaultTransitionTime);
			co_appearing = null;
		}

		public IEnumerator Disappearing() {
			yield return canvasGroup.DOFade(0f, defaultTransitionTime);
			co_disappearing = null;
		}
		//========================================================================
		//           <!!!> hop / crouch / move / shiver coroutine <!!!>
		//========================================================================
		public Coroutine Hop() {
			if (isMovingY) characterManager.StopCoroutine(co_moving_y);

			co_moving_y = characterManager.StartCoroutine(Hopping());

			return co_moving_y;
		}

		public Coroutine Crouch() {
			if (isMovingY) characterManager.StopCoroutine(co_moving_y);

			co_moving_y = characterManager.StartCoroutine(Crouching());

			return co_moving_y;
		}

		public Coroutine MoveX(float position, float duration = 0.5f, bool immediate = false) {
			if (isMovingX) characterManager.StopCoroutine(co_moving_x);

			co_moving_x = characterManager.StartCoroutine(MovingX(position, duration, immediate));

			return co_moving_x;
		}

		public Coroutine MoveY(float position, float duration = 0.5f, bool immediate = false) {
			if (isMovingY) characterManager.StopCoroutine(co_moving_y);

			co_moving_y = characterManager.StartCoroutine(MovingY(position, duration, immediate));

			return co_moving_y;
		}

		public Coroutine Shiver() {
			if (isMovingX) characterManager.StopCoroutine(co_moving_x);

			co_moving_x = characterManager.StartCoroutine(Shivering());

			return co_moving_x;
		}

		private const float defaultHopTime = 0.2f;
		private const float defaultHopLength = 60f;
		private const float defaultShiverTime = 0.07f;
		private const float defaultShiverLength = 20f;

		public IEnumerator Hopping() {
			float origin = rectTransform.anchoredPosition.y;

			yield return rectTransform.DOAnchorPosY(origin + defaultHopLength, defaultHopTime).SetEase(Ease.OutQuad).WaitForCompletion();
			yield return rectTransform.DOAnchorPosY(origin, defaultHopTime).SetEase(Ease.InQuad).WaitForCompletion();

			co_moving_y = null;
		}

		public IEnumerator Crouching() {
			float origin = rectTransform.anchoredPosition.y;

			yield return rectTransform.DOAnchorPosY(origin - defaultHopLength, defaultHopTime).SetEase(Ease.OutQuad).WaitForCompletion();
			yield return rectTransform.DOAnchorPosY(origin, defaultHopTime).SetEase(Ease.InQuad).WaitForCompletion();

			co_moving_y = null;
		}

		public IEnumerator MovingX(float position, float duration = 0.5f, bool immediate = false) {
			if (!immediate) yield return rectTransform.DOAnchorPosX(position, duration);
			else {
				float y = rectTransform.anchoredPosition.y;
				rectTransform.anchoredPosition = new(position, y);
			}
			co_moving_x = null;
		}

		public IEnumerator MovingY(float position, float duration = 0.5f, bool immediate = false) {
			if (!immediate) yield return rectTransform.DOAnchorPosY(defaultNorMalYPos + position, duration);
			else {
				float x = rectTransform.anchoredPosition.x;
				rectTransform.anchoredPosition = new(x, defaultNorMalYPos + position);
			}
			co_moving_y = null;
		}

		public IEnumerator Shivering() {
			float origin = rectTransform.anchoredPosition.x;

			for (int i = 0; i < 2; i++) {
				yield return rectTransform.DOAnchorPosX(origin - defaultShiverLength, defaultShiverTime).SetEase(Ease.InOutQuad).WaitForCompletion();
				yield return rectTransform.DOAnchorPosX(origin + defaultShiverLength, defaultShiverTime).SetEase(Ease.InOutQuad).WaitForCompletion();
			}
			yield return rectTransform.DOAnchorPosX(origin, defaultShiverTime).SetEase(Ease.InOutQuad).WaitForCompletion();

			co_moving_x = null;
		}

		//========================================================================
		// Cleanup method to stop all running coroutines
		//========================================================================

		public void StopAllCoroutines() {
			if (co_appearing != null) characterManager.StopCoroutine(co_appearing);
			if (co_disappearing != null) characterManager.StopCoroutine(co_disappearing);
			if (co_moving_x != null) characterManager.StopCoroutine(co_moving_x);
			if (co_moving_y != null) characterManager.StopCoroutine(co_moving_y);
			if (co_highlighting != null) characterManager.StopCoroutine(co_highlighting);

			co_appearing = null;
			co_disappearing = null;
			co_moving_x = null;
			co_moving_y = null;
			co_highlighting = null;
		}

		//========================================================================
		//========================================================================
	}

}

