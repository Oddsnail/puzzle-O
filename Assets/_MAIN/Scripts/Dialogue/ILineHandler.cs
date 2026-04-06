using System.Collections;

namespace origin.dialogue {
    public interface ILineHandler {
        bool CanHandle(LINE_DATA data);
        IEnumerator Handle(LINE_DATA data);
        void OnConversationEnd() { }
    }
}
