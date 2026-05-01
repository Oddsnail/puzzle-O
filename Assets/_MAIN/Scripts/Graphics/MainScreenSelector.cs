using UnityEngine;
using UnityEngine.UI;
using origin.language;

namespace origin.graphic {
    public class MainScreenSelector : MonoBehaviour
    {
		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start() {
			LocalizationManager.OnLanguageChanged += RefreshMainMenuBackground;
			RefreshMainMenuBackground();
		}

		void OnDestroy() {
			LocalizationManager.OnLanguageChanged -= RefreshMainMenuBackground;
		}

        public void RefreshMainMenuBackground() {
            SpriteSheetHolder holder = GetComponent<SpriteSheetHolder>();
			Image image = GetComponent<Image>();

			string title = LocalizationManager.instance.currentLanguageCode == "kor" ? "kor" : "eng";

            if(PlayerPrefs.GetInt("Saw.Ending", 0) == 1) {
				title += "_cl";  
            }
            image.sprite = holder.GetSprite(title);  
        }
    }
}
