using UnityEngine;
using UnityEngine.UI;

namespace origin.graphic {
    public class MainScreenSelector : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            RefreshMainMenuBackground();
        }

        public void RefreshMainMenuBackground() {
            SpriteSheetHolder holder = GetComponent<SpriteSheetHolder>();
            Image image = GetComponent<Image>();
            if(PlayerPrefs.GetInt("Saw.Ending", 0) == 1) {
                image.sprite = holder.GetSprite("main_2");    
            }else {
                image.sprite = holder.GetSprite("main_1");   
            }
        }
    }
}
