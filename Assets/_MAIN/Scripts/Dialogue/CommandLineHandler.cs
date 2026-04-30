using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using origin.command;

namespace origin.dialogue {
	public class CommandLineHandler : ILineHandler {

		private readonly ICommandExecutor commandExecutor;
		private readonly HashSet<string> autoWaitCommands = new() { "wait", "add", "choice", "choiceC", "puzzle", "puzzleT", "hl", "uhl" };

		public CommandLineHandler(ICommandExecutor commandExecutor) {
			this.commandExecutor = commandExecutor;
		}

		public bool CanHandle(LINE_DATA data) => data.isCommand;

		public IEnumerator Handle(LINE_DATA data) => ReadingCommand(data.command);

		public void Dispose() {
			return;
		}

		private IEnumerator ReadingCommand(LINE_COMMAND data) {
			foreach (LINE_COMMAND.COMMAND command in data.commands) {
				if (command.waitForCompletion || autoWaitCommands.Contains(command.name))
					yield return commandExecutor.Execute(command.name, command.arguments);
				else
					commandExecutor.Execute(command.name, command.arguments);
			}
			yield return null;
		}
	}
}
