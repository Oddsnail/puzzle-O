using System.Collections.Generic;
using System.Text;

public class LINE_COMMAND
{
	private const char COMMANDSPLIT = '@';
	private const char ARGCONTAINER = '(';
	private const string WAITCOMMAND_ID = "wait-";

	public List<COMMAND> commands;

	public struct COMMAND {
		public string name;
		public string[] arguments;
		public bool waitForCompletion;
	}

    public LINE_COMMAND(string rawLine) {
		string[] data = rawLine.Split(COMMANDSPLIT, System.StringSplitOptions.RemoveEmptyEntries);
		commands = new();

		foreach(string cmd in data) {
			COMMAND command = new();
			int index = cmd.IndexOf(ARGCONTAINER);
			command.name = cmd[..index].Trim();

			if (command.name.ToLower().StartsWith(WAITCOMMAND_ID)) {
				command.name = command.name[WAITCOMMAND_ID.Length..];
				command.waitForCompletion = true;
			} else command.waitForCompletion = false;

			command.arguments = GetArgs(cmd.Substring(index + 1, cmd.Length - index - 2));
			commands.Add(command);
		}
	}

	private string[] GetArgs(string args) {
		List<string> argList = new();
		StringBuilder currentArg = new();
		bool inQuotes = false;

		for (int i = 0; i < args.Length; i++) {
			if (args[i] == '"') {
				inQuotes = !inQuotes;
				continue;
			}

			if ((!inQuotes && args[i] == ' ') || args[i] == ')') {
				argList.Add(currentArg.ToString());
				currentArg.Clear();
				continue;
			}

			currentArg.Append(args[i]);
		}

		if (currentArg.Length > 0) argList.Add(currentArg.ToString());

		return argList.ToArray();
	}

	public override string ToString() {
		string commandString = "";

        for (int i = 0; i < commands.Count; i++) {
			commandString += $"=====command {i}=====\n";
			commandString += $"name: '{commands[i].name}'\narguments: ";
			for (int j = 0; j < commands[i].arguments.Length; j++) {
				commandString += $"{commands[i].arguments[j]},";
			}
			commandString += $"\ndoesWait: '{commands[i].waitForCompletion}'\n";
		}

		return commandString;
	}
}
