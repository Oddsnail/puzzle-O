using System;
using UnityEngine;

namespace origin.graphic {
    [Serializable]
    public class AnchorTransform {
        public Vector2 localPosition;

		public AnchorTransform(Vector2 localPosition) {
			this.localPosition = localPosition;
		}
    }
}