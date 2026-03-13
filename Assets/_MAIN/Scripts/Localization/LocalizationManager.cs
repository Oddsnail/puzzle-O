using System.Collections.Generic;
using UnityEngine;

namespace origin.language
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance;
        
        private Dictionary<string, string> _currentTable = new();
        public static event System.Action OnLanguageChanged;

        void Awake()
        {
            Instance = this;
            SetLanguage(DetectLanguageCode());
        }

        private string DetectLanguageCode()
        {
            return Application.systemLanguage switch
            {
                SystemLanguage.Korean   => "kor",
                SystemLanguage.Japanese => "jpn",
                _                       => "eng"
            };
        }

        public void SetLanguage(string code)
        {
            string fileName = code switch
            {
                "kor" => "strings_ko",
                "jpn" => "strings_ja",
                _     => "strings_en"
            };

            var asset = Resources.Load<TextAsset>($"Localization/{fileName}");
            if (asset == null)
            {
                Debug.LogError($"[Localization] File not found: Localization/{fileName}");
                return;
            }

            _currentTable = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(asset.text);
            OnLanguageChanged?.Invoke();
        }

        public string Get(string key) =>
            _currentTable.TryGetValue(key, out var val) ? val : $"[{key}]";

        private const char KeyPrefix = '@';

        /// Resolves a dialogue segment to its localized string.
        /// Segments prefixed with '@' are treated as localization keys (prefix stripped before lookup).
        /// All other text is returned as-is — no lookup is performed.
        ///   "@loc_test_001"  →  looks up "loc_test_001" in the current table
        ///   "."              →  returned unchanged
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
