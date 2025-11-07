using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine;

namespace origin.dialogue {
	public class LINE_DIALOGUE {
		public string name;
		public string spriteCode;
		public string speakEvent;

		public bool hasName => name != "";
		public bool hasSpriteCode => spriteCode != "";
		public bool hasSpeakEvent => speakEvent != "";

		public struct LINE_SEGMENT {
			public string dialogue;
			public StartSignal startSignal;
			public float waitDelay;

			public enum StartSignal { NONE, B, A, WB, WA }
			public readonly bool isAppend => startSignal == StartSignal.A || startSignal == StartSignal.WA;
		}

		public List<LINE_SEGMENT> segments;

		private const string speakerPattern = @"\[(?<name>[^:]*):(?<spriteCode>[^:]*):(?<speakEvent>[^\]]*)\]";
		private const string dialoguePattern = @"\{[ba]\}|\{w[ba]\:\d*\.?\d*\}";

		public LINE_DIALOGUE(string rawLine) {

			segments = new List<LINE_SEGMENT>();

			Match headerMatch = Regex.Match(rawLine, speakerPattern);

			if (!headerMatch.Success) Debug.LogError("Invalid format: Missing or incorrect header.");

			name = headerMatch.Groups["name"].Value;
			spriteCode = headerMatch.Groups["spriteCode"].Value;
			speakEvent = headerMatch.Groups["speakEvent"].Value;

			string body = rawLine[headerMatch.Length..].Trim();

			if (body.StartsWith("\"") && body.EndsWith("\"")) body = body[1..^1];

			MatchCollection segmentMatches = Regex.Matches(body, dialoguePattern);

			LINE_SEGMENT segment = new();
			segment.dialogue = segmentMatches.Count == 0 ? body : body.Substring(0, segmentMatches[0].Index);
			segment.startSignal = LINE_SEGMENT.StartSignal.NONE;
			segment.waitDelay = 0;
			segments.Add(segment);

			if (segmentMatches.Count == 0) return;

			int lastIndex = segmentMatches[0].Index;

			for (int i = 0; i < segmentMatches.Count; i++) {
				Match match = segmentMatches[i];
				segment = new LINE_SEGMENT();

				string signalMatch = match.Value;
				signalMatch = signalMatch.Substring(1, match.Length - 2);
				string[] signalSplit = signalMatch.Split(':');

				segment.startSignal = (LINE_SEGMENT.StartSignal)Enum.Parse(typeof(LINE_SEGMENT.StartSignal), signalSplit[0].ToUpper());

				if (signalSplit.Length > 1) float.TryParse(signalSplit[1], out segment.waitDelay);

				int nextIndex = i + 1 < segmentMatches.Count ? segmentMatches[i + 1].Index : body.Length;
				segment.dialogue = body[(lastIndex + match.Length)..nextIndex];
				lastIndex = nextIndex;

				segments.Add(segment);
			}
		}

		public override string ToString() {
			string speaker = $"name: '{name}'\nspriteCode: '{spriteCode}'\nspeakEvent: '{speakEvent}'\n";
			string dialogue = "";

			for (int i = 0; i < segments.Count; i++) {
				dialogue += $"=====segment {i}=====\n";
				dialogue += $"dialogue: '{segments[i].dialogue}'\nstartSignal: '{segments[i].startSignal}'\nwaitDelay: '{segments[i].waitDelay}'\n";
			}

			return speaker + dialogue;
		}
	}
}