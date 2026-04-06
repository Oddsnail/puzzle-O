using UnityEngine;

namespace origin.graphic {
	public class SpriteSheetHolder : MonoBehaviour {
		public Sprite[] sprites;

		public Sprite GetSprite(string spriteName) {
			foreach (Sprite sprite in sprites) {
				if (sprite.name == spriteName) return sprite;
			}
			Debug.LogWarning($"Sprite '{spriteName}' not found in {gameObject.name}. Returning first sprite.");
			return sprites[0];
		}
	}
}
