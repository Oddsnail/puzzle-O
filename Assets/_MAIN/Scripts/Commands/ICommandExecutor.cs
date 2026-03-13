using UnityEngine;

namespace origin.command {
    public interface ICommandExecutor {
        Coroutine Execute(string commandName, params string[] args);
    }
}
