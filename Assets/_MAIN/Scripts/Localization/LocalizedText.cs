using TMPro;
using UnityEngine;

namespace origin.language
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string key;

        private TMP_Text _text;
        private System.Func<string> _formatter;

        void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        void OnEnable()
        {
            LocalizationManager.OnLanguageChanged += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            LocalizationManager.OnLanguageChanged -= Refresh;
        }

        public void SetKey(string newKey)
        {
            key = newKey;
            _formatter = null;
            Refresh();
        }

        public void SetFormatter(System.Func<string> formatter)
        {
            _formatter = formatter;
            Refresh();
        }

        public void Refresh()
        {
            if (LocalizationManager.instance == null || _text == null) return;

            if (_formatter != null)
                _text.text = _formatter();
            else if (!string.IsNullOrEmpty(key))
                _text.text = LocalizationManager.instance.Get(key);
        }
    }
}
