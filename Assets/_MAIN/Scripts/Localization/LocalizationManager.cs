using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

namespace origin.language
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager instance;
        
        private Dictionary<string, string> _currentTable = new();
		public static event System.Action OnLanguageChanged;
		public string currentLanguageCode { get; private set; }

		void Awake() {
			if (instance == null) instance = this;
			else { DestroyImmediate(gameObject); return; }

			if (PlayerPrefs.HasKey("Language")) {
				string lang = PlayerPrefs.GetString("Language");
				SetLanguage(lang);
				currentLanguageCode = lang;
			}
			else {
				string lang = DetectLanguageCode();
				SetLanguage(lang);
				currentLanguageCode = lang;
			}
		}

		void OnDestroy() {
			instance = null;
		}

        private string DetectLanguageCode()
        {
            return Application.systemLanguage switch
            {
                SystemLanguage.Korean   => "kor",
                _                       => "eng"
            };
        }

        public void SetLanguage(string code)
        {
			PlayerPrefs.SetString("Language", code);
			PlayerPrefs.Save();
            string fileName = code switch
            {
                "kor" => "strings_ko",
                _     => "strings_en"
            };

            var asset = Resources.Load<TextAsset>($"Localization/{fileName}");
            if (asset == null)
            {
                Debug.LogError($"[Localization] File not found: Localization/{fileName}");
                return;
            }

            _currentTable = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(asset.text);
			currentLanguageCode = code;
            OnLanguageChanged?.Invoke();
        }

        public string Get(string key) {
            if(key == null || key == "" || key[0] == KeyIgnorePrefix) return key;
            return _currentTable.TryGetValue(key, out var val) ? val : $"[{key}]";
        }

        private const char KeyIgnorePrefix = '_';
        private const char KeyPrefix = '@';

        public string Resolve(string text) {
            if (string.IsNullOrEmpty(text)) return text;
            string trimmed = text.Trim();
            if (trimmed.Length > 0 && trimmed[0] == KeyPrefix) {
                string key = trimmed[1..];
                return _currentTable.TryGetValue(key, out var val) ? val : key;
            }
            return text;
        }
    }
}
