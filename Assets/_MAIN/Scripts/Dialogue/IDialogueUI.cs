using System;
using System.Collections;

namespace origin.dialogue {
    public interface IDialogueUI {
        event Action onNextDialogueRequest;

        bool isNameTransitioning { get; }

        void Show();
        void Hide();
        void Empty();

        void ChangeNameAndTheme(string text, string ID);
        void ChangeLetterBoxTheme(string ID);
        void SetLetterboxSpeed(float speed);
        void SetPromptVisible(bool visible);

        IEnumerator AvailableChoices((string, string, string)[] choices, bool isColored);
    }
}
