namespace origin.dialogue {
	public class LINE_DATA {
		public LINE_DIALOGUE dialogue;
		public LINE_COMMAND command;
		public bool isDialogue => dialogue != null;
		public bool isCommand => !isDialogue;

		public LINE_DATA(string rawLine) {
			if (rawLine.StartsWith('[')) {
				dialogue = new LINE_DIALOGUE(rawLine);
				command = null;
			}
			else {
				dialogue = null;
				command = new LINE_COMMAND(rawLine);
			}
		}

		public override string ToString() {
			if (isDialogue) return dialogue.ToString();
			else return command.ToString();
		}
	}
}