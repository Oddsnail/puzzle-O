using System;
using UnityEngine;

namespace origin.graphic {
    [Serializable]
    public class AnchorTRS {
        public Vector2 localPosition;
        public float localRotation;
        public Vector2 localScale;

        public AnchorTRS(Vector2 localPosition, float localRotation, Vector2 localScale) {
            this.localPosition = localPosition;
            this.localRotation = localRotation;
            this.localScale = localScale;
        }
    }
}