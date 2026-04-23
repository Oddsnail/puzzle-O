using System.Text;
using UnityEngine;

namespace origin.tutorial {
	public static class TutorialSetParser {

		public static TutorialSet Parse(TextAsset asset, string setID) => Parse(asset.text, setID);

		public static TutorialSet Parse(string text, string setID) {
			var set = new TutorialSet { setID = setID };
			var lines = text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

			bool foundSet = false;
			foreach (var raw in lines) {
				var line = raw.Trim();
				if (line == "") continue;

				if (!foundSet) {
					if (line == setID) foundSet = true;
					continue;
				}

				var parts = line.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
				if (parts[0] != "light" && parts[0] != "force") break;

				set.steps.Add(ParseStep(parts));
			}

			return set;
		}

		private static TutorialStep ParseStep(string[] parts) {
			var step = new TutorialStep();
			step.stepType = parts[0] == "light" ? StepType.HighlightArea : StepType.ForceInput;

			step.dialogueX      = F(parts[1]);
			step.dialogueY      = F(parts[2]);
			step.dialogueLength = F(parts[3]);
			step.highlightRect  = new Rect(F(parts[4]), F(parts[5]), F(parts[6]), F(parts[7]));

			switch (parts[8]) {
				case "RO": step.doEvent = true;  step.stepEvent = StepEvent.RightSideboxOpen;  break;
				case "RC": step.doEvent = true;  step.stepEvent = StepEvent.RightSideboxClose; break;
				case "LO": step.doEvent = true;  step.stepEvent = StepEvent.LeftSideboxOpen;   break;
				case "LC": step.doEvent = true;  step.stepEvent = StepEvent.LeftSideboxClose;  break;
				case "NN": step.doEvent = false; break;
			}

			if (step.stepType == StepType.ForceInput && parts.Length >= 10) {
				step.forceCondition = new ForceInputCondition();
				if (parts[9] == "point") {
					step.forceCondition.inputType = ForceInputType.PointerInArea;
				} else {
					step.forceCondition.inputType = ForceInputType.KeyboardInput;
					step.forceCondition.requiredKey = parts[9][0];
				}
			}

			return step;
		}

		public static string Unparse(TutorialSet set) {
			var sb = new StringBuilder();
			sb.AppendLine(set.setID);

			for (int i = 0; i < set.steps.Count; i++) {
				var step = set.steps[i];
				string typePart  = step.stepType == StepType.HighlightArea ? "light" : "force";
				string eventPart = step.doEvent ? EventToCode(step.stepEvent) : "NN";
				Rect r = step.highlightRect;

				sb.Append($"{typePart} {S(step.dialogueX)} {S(step.dialogueY)} {S(step.dialogueLength)} {S(r.x)} {S(r.y)} {S(r.width)} {S(r.height)} {eventPart}");

				if (step.stepType == StepType.ForceInput) {
					string forcePart = step.forceCondition.inputType == ForceInputType.PointerInArea
						? "point"
						: step.forceCondition.requiredKey.ToString();
					sb.Append($" {forcePart}");
				}

				if (i < set.steps.Count - 1) sb.AppendLine();
			}

			return sb.ToString();
		}

		private static string EventToCode(StepEvent e) => e switch {
			StepEvent.RightSideboxOpen  => "RO",
			StepEvent.RightSideboxClose => "RC",
			StepEvent.LeftSideboxOpen   => "LO",
			StepEvent.LeftSideboxClose  => "LC",
			_ => "NN"
		};

		private static float F(string s) =>
			float.Parse(s, System.Globalization.CultureInfo.InvariantCulture);

		private static string S(float v) =>
			v.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
	}
}
