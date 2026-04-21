using System.Collections;

using origin.puzzle;
using origin.dialogue;

namespace origin.command {

	public class CMD_DatabaseExtension_Puzzle : CMD_DatabaseExtension {

		new public static void Extend(CommandDatabase database) {
			// puzzle(charID digitCount diff ruleSetCode successJumpCode failJumpCode)
			database.AddCommand("puzzle", new System.Func<string[], IEnumerator>(Puzzle));

			// puzzleT(charID tutoID successJumpCode failJumpCode)
			database.AddCommand("puzzleT", new System.Func<string[], IEnumerator>(PuzzleWithTutorial));
		}

		private static IEnumerator Puzzle(string[] data) {
			IDialogueUI ui = DialogueManager.instance;
			IConversationControl conv = DialogueManager.instance;

			int.TryParse(data[1], out int digitCount);
			int.TryParse(data[2], out int difficulty);
			ui.Hide();
			bool successed = false;
			yield return PuzzleManager.instance.StartPuzzle(data[0], digitCount, difficulty, success => successed = success, data[3]);
			if (successed) conv.Jump(data[4]);
			else conv.Jump(data[5]);
			ui.Empty();
			ui.Show();
		}

		private static IEnumerator PuzzleWithTutorial(string[] data) {
			IDialogueUI ui = DialogueManager.instance;
			IConversationControl conv = DialogueManager.instance;

			ui.Hide();
			bool successed = false;
			yield return PuzzleManager.instance.StartPuzzleWithTutorial(data[0], data[1], success => successed = success);
			if (successed) conv.Jump(data[2]);
			else conv.Jump(data[3]);
			ui.Empty();
			ui.Show();
		}
	}
}
