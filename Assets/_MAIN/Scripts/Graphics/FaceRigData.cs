using System;

namespace origin.graphic {
    [Serializable]
    public class FaceRigData {
        public string poseID; // char-pose string e.g. "leedoeun-std1"
        public AnchorTRS root;
        public AnchorTRS eye;
        public AnchorTRS eyebrow;
        public AnchorTRS mouth;
        public AnchorTRS expression1;
        public AnchorTRS expression2;
    }
}