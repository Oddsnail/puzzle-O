using UnityEngine;
using TMPro;
using System.Collections;
using origin.audio;

namespace origin.dialogue {
    public class TextArchitect {
        public TextMeshProUGUI tmpro;

        public string CurrentText => tmpro.text;
        public string TargetText { get; private set; } = "";
        public string PreText { get; private set; } = "";
        public string FullTargetText => PreText + TargetText;

        public enum TextBuildMethod {
            instant,
            typewriter,
        }

        public TextBuildMethod buildMethod = TextBuildMethod.typewriter;
        public Color TextColor { get => tmpro.color; set { tmpro.color = value; } }

        public float Speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
        private const float baseSpeed = 1;
        private float speedMultiplier = 1;

        public int CharactersPerCycle { get { return Speed <= 2f ? characterMultiplier : Speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
        private const int characterMultiplier = 1;

        public bool speedUp = false;

		public TextArchitect(TextMeshProUGUI tmpro) {
			this.tmpro = tmpro;
		}

		public void SetSpeed(float speed) => speedMultiplier = speed;

        public Coroutine Build(string text) {
            PreText = "";
            TargetText = text;

            Stop();

            buildProcess = tmpro.StartCoroutine(Building());
            return buildProcess;
        }

        public Coroutine Append(string text) {
            PreText = tmpro.text;
            TargetText = text;

            Stop();

            buildProcess = tmpro.StartCoroutine(Building());
            return buildProcess;
        }

        private Coroutine buildProcess = null;
        public bool isBuilding => buildProcess != null;

        public void Stop() {
            if (!isBuilding) return;

            tmpro.StopCoroutine(buildProcess);
            buildProcess = null;
        }

        IEnumerator Building() {

            switch (buildMethod) {
                case TextBuildMethod.instant:
                    Prepare_Instant();
                    break;
                case TextBuildMethod.typewriter:
                    Prepare_Typewriter();
                    yield return Build_Typewriter();
                    break;
            }

            OnComplete();
        }

        private void OnComplete() { buildProcess = null; speedUp = false; }
        public void ForceComplete() {
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
            Stop();
            OnComplete();
        }

        private void Prepare_Instant() {
            tmpro.color = tmpro.color;
            tmpro.text = FullTargetText;
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }

        private void Prepare_Typewriter() {
            tmpro.color = tmpro.color;
            tmpro.maxVisibleCharacters = 0;
            tmpro.text = PreText;

            if (PreText != "") {
                tmpro.ForceMeshUpdate();
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
            }

            tmpro.text += TargetText;
            tmpro.ForceMeshUpdate();
        }

        private IEnumerator Build_Typewriter() {
            while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount) {
                tmpro.maxVisibleCharacters += speedUp ? 3 * CharactersPerCycle : CharactersPerCycle;
                AudioManager.instance.PlayPreloadedSFX("textBuild", AudioManager.instance.sfxMixer, 1.0f + Random.Range(-0.05f, 0.05f));
                yield return new WaitForSeconds(0.02f / Speed);
            }
        }
    }
}