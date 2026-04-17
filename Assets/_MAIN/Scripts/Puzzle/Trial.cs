using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace origin.puzzle
{
    public class Trial : MonoBehaviour
    {
        private static readonly float unHighlightedTransparency = 0.6f;
        private static readonly float unHighlightedScale = 0.8f;
        private static readonly float highlightSpeed = 6.0f;

        public GameObject numberPrefab;
        private bool isActive = false;
        private int numDigits = 0;
        private int digitCount = 0;
        private List<(RectTransform, Image, TMPro.TMP_Text)> digitDisplays = new List<(RectTransform, Image, TMPro.TMP_Text)>();
        private Coroutine co_highlighting;
        private bool is_highlighting => co_highlighting != null;

        public void Initialize(int numDigits)
        {
            this.numDigits = numDigits;
            for (int i = 0; i < numDigits; i++)
            {
                GameObject digitObj = Instantiate(numberPrefab, transform);

                RectTransform digitRect = digitObj.GetComponent<RectTransform>();
                Image digitImage = digitObj.GetComponent<Image>();
                TMPro.TMP_Text digitText = digitObj.GetComponentInChildren<TMPro.TMP_Text>();

                digitText.text = "";

                digitDisplays.Add((digitRect, digitImage, digitText));
            }
            HighlightTrial(false);
            isActive = true;
        }

        public void HighlightTrial(bool highlight)
        {
            if (is_highlighting) StopCoroutine(co_highlighting);

            co_highlighting = StartCoroutine(Highlighting(highlight));
        }

        private IEnumerator Highlighting(bool highlight)
        {
            float percent = 0;
            Vector3 startScale = digitDisplays[0].Item1.localScale;
            Vector3 targetScale = highlight ? new Vector3(1f, 1f, 1) : new Vector3(unHighlightedScale, unHighlightedScale, 1);
            float startTransparency = digitDisplays[0].Item2.color.a;
            float targetTransparency = highlight ? 1f : unHighlightedTransparency;
            while (percent < 1f)
            {
                percent += Time.deltaTime * highlightSpeed;
                Vector3 currentScale = Vector3.Lerp(startScale, targetScale, percent);
                foreach (var display in digitDisplays)
                {
                    display.Item1.localScale = currentScale;
                    display.Item2.color = new Color(display.Item2.color.r, display.Item2.color.g, display.Item2.color.b, Mathf.Lerp(startTransparency, targetTransparency, percent));
                    display.Item3.color = new Color(display.Item3.color.r, display.Item3.color.g, display.Item3.color.b, Mathf.Lerp(startTransparency, targetTransparency, percent));
                }
                yield return null;
            }
        }

        void OnDestroy()
        {
            if (co_highlighting != null) StopCoroutine(co_highlighting);
        }

        public void InputDigit(int digit, Color color, Color subcolor)
        {
            if (!isActive || digitCount >= numDigits) return;

            digitDisplays[digitCount].Item2.color = color;
            digitDisplays[digitCount].Item3.color = subcolor;
            digitDisplays[digitCount].Item3.text = digit.ToString();
            digitCount++;
        }
    }
}
