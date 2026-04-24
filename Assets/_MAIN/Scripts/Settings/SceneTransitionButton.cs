using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using origin.audio;

namespace origin.IO
{
    public class SceneTransitionButton : MonoBehaviour
    {
        [SerializeField] private string targetScene;
        [SerializeField] private Image sceneTransitionFog;
        [SerializeField] private float fadeDuration = 1f;
		[SerializeField] private InputManager inputManager;

		private bool _transitioning = false;
		
		void Start()
		{
			if (!sceneTransitionFog.enabled) {
				_transitioning = true;
				StartCoroutine(WentFromScene());
			}
		}

        public void OnButtonPressed()
        {
			if (_transitioning) return;
			
			AudioManager.instance.PlayPreloadedSFX("optionChange");
            _transitioning = true;

            inputManager.enabled = false;
            StartCoroutine(GoToScene());
        }

		private IEnumerator GoToScene() {
			if (AudioManager.instance != null)
				AudioManager.instance.StopAllBGMGradient();

			sceneTransitionFog.enabled = true;

			float elapsed = 0f;
			while (elapsed < fadeDuration) {
				elapsed += Time.deltaTime;
				sceneTransitionFog.color = new Color(
					sceneTransitionFog.color.r,
					sceneTransitionFog.color.g,
					sceneTransitionFog.color.b,
					Mathf.Clamp01(elapsed / fadeDuration)
				);
				yield return null;
			}

			yield return new WaitForSeconds(1.1f);

			SceneManager.LoadScene(targetScene);
		}
		
		private IEnumerator WentFromScene() {
			sceneTransitionFog.enabled = true;
			sceneTransitionFog.color = new Color(
				sceneTransitionFog.color.r,
				sceneTransitionFog.color.g,
				sceneTransitionFog.color.b,
				1f
			);

			float elapsed = 0f;
			while (elapsed < fadeDuration) {
				elapsed += Time.deltaTime;
				sceneTransitionFog.color = new Color(
					sceneTransitionFog.color.r,
					sceneTransitionFog.color.g,
					sceneTransitionFog.color.b,
					Mathf.Clamp01(1 - elapsed / fadeDuration)
				);
				yield return null;
			}

			_transitioning = false;
		}
    }
}