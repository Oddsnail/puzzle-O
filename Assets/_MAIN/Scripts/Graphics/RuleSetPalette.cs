using System;
using System.Collections.Generic;
using UnityEngine;

namespace origin.graphic {

	[Serializable]
	public class RulePalette {
		public string ruleSetCode;
		public List<Palette> palettes;
	}

	[CreateAssetMenu(fileName = "RuleSetPalette", menuName = "Scriptable Objects/RuleSetPalette")]
	public class RuleSetPalette : ScriptableObject {
		public List<RulePalette> rulePalettes;
	}
}
