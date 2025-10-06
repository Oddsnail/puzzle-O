using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.PackageManager;

public class CHARACTER {

	protected CharacterManager characterManager => CharacterManager.instance;

	private const string SPRITE_PARENT_ID = "Renderers";

    public string ID;
	public CharacterConfigData config;
	public RectTransform rectTransform;
	private CanvasGroup canvasGroup;
	private Coroutine co_appearing;
	private Coroutine co_disappearing;
	private Coroutine co_moving_x;
	private Coroutine co_moving_y;
	public List<SpriteSheetHolder> layers = new();

	public float visibility => canvasGroup.alpha;
	public bool isAppearing => co_appearing != null;
	public bool isDisappearing => co_disappearing != null;
	public bool isMovingX => co_moving_x != null;
	public bool isMovingY => co_moving_y != null;

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
	private const float defaultClientXPos = -736.0f;

	private void NormalInitiate() {
		if (config.prefabNormal == null) Debug.LogError($"[ERROR] No prefabNormal found for {ID}");
		GameObject ob = UnityEngine.Object.Instantiate(config.prefabNormal, characterManager.characterLayer);
		ob.name = $"Character - [{ID}]";
		rectTransform = ob.GetComponent<RectTransform>();
		rectTransform.anchoredPosition = new(0.0f, 0.0f);
		canvasGroup = rectTransform.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0;
		ob.SetActive(true);
		GetLayers();
	}

	private void ClientInitiate() {
		if (config.prefabClient == null) Debug.LogError($"[ERROR] No prefabClient found for {ID}");
		GameObject ob = UnityEngine.Object.Instantiate(config.prefabClient, characterManager.characterLayer);
		ob.name = $"Client - [{ID}]";
		rectTransform = ob.GetComponent<RectTransform>();
		rectTransform.anchoredPosition = new(defaultClientXPos, 0.0f);
		canvasGroup = rectTransform.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0;
		ob.SetActive(true);
		GetLayers();
	}

	private void GetLayers() {
		Transform renderer = rectTransform.Find(SPRITE_PARENT_ID);
		for (int i = 0; i < renderer.transform.childCount; i++) {
			Transform child = renderer.transform.GetChild(i);
			SpriteSheetHolder rendererImage = child.GetComponentInChildren<SpriteSheetHolder>();

			if (rendererImage != null) {
				layers.Add(rendererImage);
				child.name = $"Layer : {i}";
			}
		}
	}

	public Sprite GetSprite((int, string) layerChange) => Array.Find(layers[layerChange.Item1].sprites, sprite => sprite.name == layerChange.Item2);

	public void SetSprite(string spriteCode) {
		string[] spriteNames = spriteCode.Split("-");
		for (int i = 0; i < spriteNames.Length; i++) {
			if (spriteNames[i] == "_") continue;
			Sprite sprite = GetSprite((i, spriteNames[i]));
			layers[i].gameObject.GetComponent<Image>().sprite = sprite;
		}
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

	public Coroutine Move(float position, float duration = 0.5f, bool immediate = false) {
		if (isMovingX) characterManager.StopCoroutine(co_moving_x);

		co_moving_x = characterManager.StartCoroutine(Moving(position, duration, immediate));

		return co_moving_x;
	}

	public Coroutine Shiver() {
		if (isMovingX) characterManager.StopCoroutine(co_moving_x);

		co_moving_x = characterManager.StartCoroutine(Shivering());

		return co_moving_x;
	}

	private const float defaultHopTime = 0.2f;
	private const float defaultHopLength = 60f;
	private const float defaultShiverTime = 0.07f;
	private const float defaultShiverLength = 40f;

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

	public IEnumerator Moving(float position, float duration = 0.5f, bool immediate = false) {
        if (!immediate) yield return rectTransform.DOAnchorPosX(position, duration);
		else {
			float y = rectTransform.anchoredPosition.y;
			rectTransform.anchoredPosition = new(position, y);
		}
        co_moving_x = null;
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
	//========================================================================
}
