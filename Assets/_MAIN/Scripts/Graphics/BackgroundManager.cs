using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;

namespace origin.graphic
{
	public class BackgroundManager : MonoBehaviour
	{
		public static BackgroundManager instance;
		public GameObject background;
		public GameObject cutscene;
		public Image loadingFog;
		public Image thinkFog;

		public bool isFogOn => loadingFog.color.a != 0f;
		public bool isThinkFogOn => thinkFog.color.a != 0f;

        private const string BGfilePath = "Graphics/Backgrounds";
        private const string CSfilePath = "Graphics/Cutscenes";
		private const float defaultFogTransitionTime = 0.5f;
		private const float defaultCompleteFogTime = 0.2f;
		private const float defaultThinkFogTransitionTime = 0.4f;
		private const float defaultThinkFogTransparency = 0.8f;
		
		private Coroutine co_background = null;
		private Coroutine co_cutscene = null;
		private Coroutine co_thinkFog = null;

		public bool isBackgroundTransitioning => co_background != null;
		public bool isCutsceneTransitioning => co_cutscene != null;
		public bool isThinkFogTransitioning => co_thinkFog != null;

		public void Awake() {
			if (instance != null && instance != this) {
				DestroyImmediate(gameObject);
			}

			instance = this;
		}

        public void ChangeBackground(string backgroundName) {
            if (isBackgroundTransitioning) return;
            co_background = StartCoroutine(BackgroundTransition(backgroundName));
        }

        public void ChangeCutscene(string cutsceneName) {
            if (isCutsceneTransitioning) return;
            co_cutscene = StartCoroutine(CutsceneTransition(cutsceneName));
        }

		public void QuitCutscene() {
			if (isCutsceneTransitioning) return;
			co_cutscene = StartCoroutine(QuitCutsceneTransition());
		}

		public void thinkFogOn(bool on) {
			if (isThinkFogTransitioning) return;
			co_thinkFog = StartCoroutine(ThinkFogTransition(on));
		}

		public IEnumerator ManualFogOn(bool on) {
			yield return StartCoroutine(on ? EnterFog() : ExitFog());
		}

		public void ManualTransition(GameObject obj, string imgName) {
			Image image = obj.GetComponent<Image>();
			PerspectiveMover persp = obj.GetComponent<PerspectiveMover>();
			Sprite newSprite = Resources.Load<Sprite>($"{imgName}");
			image.sprite = newSprite;
			image.SetNativeSize();
			InitializePerspective(newSprite, persp);
		}

		private IEnumerator ThinkFogTransition(bool on) {
			yield return thinkFog.DOFade(on ? defaultThinkFogTransparency : 0f, defaultThinkFogTransitionTime).WaitForCompletion();

			co_thinkFog = null;
		}

		private IEnumerator EnterFog() {
			yield return loadingFog.DOFade(1f, defaultFogTransitionTime).WaitForCompletion();
		}

		private IEnumerator ExitFog() {
			yield return loadingFog.DOFade(0f, defaultFogTransitionTime).WaitForCompletion();
		}
		
		private IEnumerator StayFog() {
			yield return new WaitForSeconds(defaultCompleteFogTime);
		}

        private IEnumerator BackgroundTransition(string backgroundName) {
			Image bgImage = background.GetComponent<Image>();
			PerspectiveMover bgPersp = background.GetComponent<PerspectiveMover>();
            Sprite newSprite = Resources.Load<Sprite>($"{BGfilePath}/{backgroundName}");

			yield return EnterFog();

			if (newSprite != null) {
				bgImage.sprite = newSprite;
				bgImage.SetNativeSize();
				InitializePerspective(newSprite, bgPersp);
			}
			yield return StayFog();

			yield return ExitFog();

            co_background = null;
        }

        private IEnumerator CutsceneTransition(string cutsceneName) {
			Image csImage = background.GetComponent<Image>();
			PerspectiveMover csPersp = background.GetComponent<PerspectiveMover>();
            Sprite newSprite = Resources.Load<Sprite>($"{CSfilePath}/{cutsceneName}");

			yield return EnterFog();

			csImage.color = Color.white;
			if (newSprite != null) {
				csImage.sprite = newSprite;
				csImage.SetNativeSize();
				InitializePerspective(newSprite, csPersp);
			}
			yield return StayFog();

			yield return ExitFog();

            co_cutscene = null;
        }

		private IEnumerator QuitCutsceneTransition() {
			Image csImage = background.GetComponent<Image>();

			yield return EnterFog();

			csImage.color = Color.clear;
			yield return StayFog();

			yield return ExitFog();

			co_cutscene = null;
		}

		private void InitializePerspective(Sprite sprite, PerspectiveMover persp) {
			persp.maxX = math.max((sprite.rect.width - 3840f) / 2f, 0);
			persp.maxY = math.max((sprite.rect.height - 2160f) / 2f, 0);
		}


	}
}

