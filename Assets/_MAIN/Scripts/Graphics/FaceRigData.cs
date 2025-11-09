using System;

namespace origin.graphic {
    [Serializable]
    public class FaceRigData {
        public string poseID; // char-pose string e.g. "leedoeun-std1"
        public AnchorTransform root;
        public AnchorTransform eye;
        public AnchorTransform eyebrow;
        public AnchorTransform mouth;
		public AnchorTransform expression;
    }
}