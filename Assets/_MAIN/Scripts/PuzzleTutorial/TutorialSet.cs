using System.Collections.Generic;

namespace origin.tutorial {
	[System.Serializable]
	public class TutorialSet {
		public string setID;
		public List<TutorialStep> steps = new();
	}
}
