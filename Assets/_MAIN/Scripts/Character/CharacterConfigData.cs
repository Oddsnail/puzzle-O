using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace origin.character {
	[System.Serializable]
	public class CharacterConfigData {

		// ID is initial letter in lower case : lde, yjh etc.
		public string ID;
		public string nameKey => $"char.{ID}.name";
		public string subnameKey => $"char.{ID}.subname";
		public GameObject prefabNormal;
		public GameObject prefabClient;
		public Vector2 reactionLocation;
		public List<ReactionPair> reactionField;

		public static CharacterConfigData Default {
			get {
				return new() {
					ID = "<default>",
					prefabNormal = null,
					prefabClient = null,
				};
			}
		}

		public string FindReaction(ClientReactionType T) {
			foreach (ReactionPair pair in reactionField) {
				if (T == pair.type) return pair.setSpriteID;
			}
			Debug.LogWarning($"Cannot find reaction {T} for character ID {ID}");
			return null;
		}
	}

	[System.Serializable]
	public enum ClientReactionType {
		Best,
		Better,
		Good,
		Normal,
		Bad,
		Idle,
	}

	[System.Serializable]
	public class ReactionPair {
		public ClientReactionType type;
		public string setSpriteID;
	}
}
