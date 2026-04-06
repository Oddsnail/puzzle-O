using System;
using System.Collections.Generic;

namespace origin.graphic {
    [Serializable]
    public class ExpressionRigData {
        public string name;
        public int layer;
        public AnchorTransform data;
    }

    [Serializable]
    public class FaceRigData {
        public string poseID; // char-pose string e.g. "leedoeun-std1"
        public AnchorTransform root;
        public AnchorTransform eye;
        public AnchorTransform eyebrow;
        public AnchorTransform mouth;
        public List<ExpressionRigData> expressions;
    }
}