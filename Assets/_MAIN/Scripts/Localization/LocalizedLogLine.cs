using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace origin.language
{
    public class LocalizedLogLine : MonoBehaviour
    {

        [SerializeField] private TMP_Text Speaker;
        [SerializeField] private TMP_Text Content;
        [SerializeField] private GameObject Arrow;

        private string _speakerKey;
        private List<string> _contentList = new();
        private bool isNarration => string.IsNullOrEmpty(_speakerKey);

        void OnEnable()
        {
            LocalizationManager.OnLanguageChanged += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            LocalizationManager.OnLanguageChanged -= Refresh;
        }

        public void SetInfo(string key, List<string> contents)
        {
            _speakerKey = key;
            _contentList = new List<string>(contents);
            Arrow.SetActive(!isNarration);
            Refresh();
        }

        public void Refresh()
        {
            if (LocalizationManager.instance == null) return;

            Speaker.text = string.IsNullOrEmpty(_speakerKey)
                ? ""
                : LocalizationManager.instance.Get(_speakerKey);

            string content = "";
            foreach (var raw in _contentList)
            {
                content += LocalizationManager.instance.Resolve(raw);
            }
            if(isNarration) Content.text = $"<i>{content}</i>";
            else Content.text = content;
        }
    }
}
