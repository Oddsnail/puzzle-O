using UnityEngine;

namespace origin.graphic {

	[CreateAssetMenu(fileName = "Face Rigging Configuration", menuName = "Scriptable Objects/Face Rigging Configurations")]
	public class FaceRigSO : ScriptableObject {
		public FaceRigData[] FaceRigs;

		public FaceRigData GetFaceRigData(string ID) {
			for (int i = 0; i < FaceRigs.Length; i++) {
				if (FaceRigs[i].poseID == ID) return FaceRigs[i];
			}
			Debug.LogError($"Pose {ID} do not exist in the SO database");
			return null;
		}

	}

}
