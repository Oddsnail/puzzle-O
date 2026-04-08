using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace origin.puzzle
{
    public class Trial : MonoBehaviour
    {
        public GameObject numberPrefab;
        private bool isActive = false;
        private int numDigits = 0;
        private int digitCount = 0;
        private List<(Image, TMPro.TMP_Text)> digitDisplays = new List<(Image, TMPro.TMP_Text)>();

        public void Initialize(int numDigits)
        {
            this.numDigits = numDigits;
            for (int i = 0; i < numDigits; i++)
            {
                GameObject digitObj = Instantiate(numberPrefab, transform);
                Image digitImage = digitObj.GetComponent<Image>();
                TMPro.TMP_Text digitText = digitObj.GetComponentInChildren<TMPro.TMP_Text>();
                digitText.text = "";
                digitDisplays.Add((digitImage, digitText));
            }
            isActive = true;
        }

        public void InputDigit(int digit, Color color, Color subcolor)
        {
            if (!isActive || digitCount >= numDigits) return;

            digitDisplays[digitCount].Item1.color = color;
            digitDisplays[digitCount].Item2.color = subcolor;
            digitDisplays[digitCount].Item2.text = digit.ToString();
            digitCount++;
        }
    }
}
