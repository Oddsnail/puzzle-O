using System;
using System.Collections.Generic;
using UnityEngine;

namespace origin.graphic {

	[Serializable]
	public class Palette {
		public string ID;
		public Color primary;
		public Color secondary;
	}

	[CreateAssetMenu(fileName = "ColorPalette", menuName = "Scriptable Objects/ColorPalette")]
	public class ColorPalette : ScriptableObject {

		public List<Palette> colors;

		public Color Getcolor(string code, bool lookSecondary = false) {
			foreach (Palette p in colors) {
				if (p.ID == code) {
					if (!lookSecondary) return p.primary;
					else return p.secondary;
                }

			}
			Debug.LogError($"[Error] No such color {code} exists in Pallete");
			return Color.white;
		}
	}
}