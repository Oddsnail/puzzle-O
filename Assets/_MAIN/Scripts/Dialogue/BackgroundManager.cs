using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace origin.dialogue
{
    public class BackgroundManager
    {
		private MonoBehaviour runner;
        private GameObject background;
        private GameObject backgroundFog;
        private GameObject cutscene;

        private bool bgFogOn = false;

        private const string BGfilePath = "Graphics/Backgrounds";
        private const string CSfilePath = "Graphics/Cutscenes";
        private const float defaultChangeImageDuration = 0.5f;
        private const float defaultFogTransparency = 0.9f;

        private Coroutine co_background = null;
        private Coroutine co_backgroundFog = null;
        private Coroutine co_cutscene = null;

        public bool isBackgroundTransitioning => co_background != null;
        public bool isBackgroundFogTransitioning => co_backgroundFog != null;
        public bool isCutsceneTransitioning => co_cutscene != null;

        public void Initialize(GameObject backgroundImage, GameObject cutsceneImage, GameObject backgroundFogImage, MonoBehaviour runner) {
			this.background = backgroundImage;
            this.cutscene = cutsceneImage;
            this.backgroundFog = backgroundFogImage;
            this.runner = runner;
		}

        public void BackgroundFog(bool show) {
            if (isBackgroundFogTransitioning) return;
            if (bgFogOn == show) return;
            co_backgroundFog = runner.StartCoroutine(BackgroundFogTransition(show));
        }

        public void ChangeBackground(string backgroundName) {
            if (isBackgroundTransitioning) return;
            co_background = runner.StartCoroutine(BackgroundTransition(backgroundName));
        }

        public void ChangeCutscene(string cutsceneName) {
            if (isCutsceneTransitioning) return;
            co_cutscene = runner.StartCoroutine(CutsceneTransition(cutsceneName));
        }

        public void QuitCutscene() {
            if (isCutsceneTransitioning) return;
            co_cutscene = runner.StartCoroutine(QuitCutsceneTransition());
        }

        private IEnumerator BackgroundTransition(string backgroundName) {
            Image bgImage = background.GetComponent<Image>();
            Sprite newSprite = Resources.Load<Sprite>($"{BGfilePath}/{backgroundName}");

            yield return bgImage.DOFade(0f, defaultChangeImageDuration).WaitForCompletion();

            if (newSprite != null)
                bgImage.sprite = newSprite;

            yield return bgImage.DOFade(1f, defaultChangeImageDuration).WaitForCompletion();

            co_background = null;
        }

        private IEnumerator BackgroundFogTransition(bool show) {
            Image bgFog = backgroundFog.GetComponent<Image>();
            float targetTransparency = show ? defaultFogTransparency : 0f;

            yield return bgFog.DOFade(targetTransparency, defaultChangeImageDuration).WaitForCompletion();

            bgFogOn = show;
            co_backgroundFog = null;
        }

        private IEnumerator CutsceneTransition(string cutsceneName) {
            Image csImage = cutscene.GetComponent<Image>();
            Sprite newSprite = Resources.Load<Sprite>($"{CSfilePath}/{cutsceneName}");

            if (newSprite != null)
                csImage.sprite = newSprite;

            yield return csImage.DOFade(1f, defaultChangeImageDuration).WaitForCompletion();

            co_cutscene = null;
        }

        private IEnumerator QuitCutsceneTransition() {
            Image csImage = cutscene.GetComponent<Image>();

            yield return csImage.DOFade(0f, defaultChangeImageDuration).WaitForCompletion();

            co_cutscene = null;
        }
    }

}
