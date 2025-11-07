using UnityEngine;

namespace origin.graphic {
    public class SpriteManager : MonoBehaviour {

        public Transform eyeL;
        public Transform eyeR;
        public Transform brow;
        public Transform mouth;

        public UnityEngine.U2D.Animation.SpriteResolver bodyResolver;
        public UnityEngine.U2D.Animation.SpriteResolver eyeResolverL;
        public UnityEngine.U2D.Animation.SpriteResolver eyeResolverR;
        public UnityEngine.U2D.Animation.SpriteResolver browResolver;
        public UnityEngine.U2D.Animation.SpriteResolver mouthResolver;

        public FaceRigSO faceRigSO; // poseLabel -> anchor TRS map

        public void SetBodyPose(string poseID) {
            // 1) swap body sprite
            bodyResolver.SetCategoryAndLabel("Body", poseID);

            // 2) apply per-pose anchor TRS
            var pose = faceRigSO.GetFaceRigData(poseID);
            ApplyTRS(eyeL, pose.eye);
            ApplyTRS(eyeR, pose.eyebrow);
            ApplyTRS(mouth, pose.mouth);
        }

        public void SetExpression(string eye, string browLabel, string mouth) {
            if (!string.IsNullOrEmpty(eye)) {
                eyeResolverL.SetCategoryAndLabel("EyeLeft", eye);
                eyeResolverR.SetCategoryAndLabel("EyeRight", eye);
            }
            if (!string.IsNullOrEmpty(browLabel))
                browResolver.SetCategoryAndLabel("Brow", browLabel);
            if (!string.IsNullOrEmpty(mouth))
                mouthResolver.SetCategoryAndLabel("Mouth", mouth);
        }

        static void ApplyTRS(Transform t, AnchorTRS d) {
            if (!t) return;
            // t.localPosition = d.localPos;
            // t.localEulerAngles = d.localEuler;
            // t.localScale = d.localScale;
        }
    }
}