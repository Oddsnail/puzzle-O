using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace origin.graphic {
	public class SpriteManager : MonoBehaviour {

		public RectTransform entire;
		public RectTransform body;
		public RectTransform eye;
		public RectTransform eyebrow;
		public RectTransform mouth;
		public List<RectTransform> expressions;

		public FaceRigSO faceRigSO;
		
		private static HashSet<string> poseIndependentExpressions = new() { "sweat" };

		private string prev_poseID = "";
		private FaceRigData currentRig;
		private const float unhighlightYOffset = -40.0f;
		private const string concatenator = "-";
		private const string empty_poseID = "_";

		// poseID 			ex) std2, std1, std1-eq
		// componentID 		ex) 221, 1__, 1_3
		// expressionsID 	ex) 1, 12, 21, 11
		public void SetSprite(string charID, string poseID, string componentID, string[] expressionsIDs) {
			
			// pose
			bool poseChanged = false;
			if (poseID != empty_poseID) {
				prev_poseID = poseID.Split("_")[0];

				FaceRigData rig = faceRigSO.GetFaceRigData(charID + concatenator + prev_poseID);
				if (rig == null) return;

				currentRig = rig;

				body.GetComponent<Image>().sprite = body.GetComponent<SpriteSheetHolder>().GetSprite(poseID);
				eye.anchoredPosition = rig.eye.localPosition;
				eyebrow.anchoredPosition = rig.eyebrow.localPosition;
				mouth.anchoredPosition = rig.mouth.localPosition;

				poseChanged = true;
			}
			
			// component
			if (componentID[0] != '_') { Image img = eye.GetComponent<Image>(); img.sprite = eye.GetComponent<SpriteSheetHolder>().GetSprite($"{prev_poseID}_eye{componentID[0]}"); if (poseChanged) img.SetNativeSize(); }
			if (componentID[1] != '_') { Image img = eyebrow.GetComponent<Image>(); img.sprite = eyebrow.GetComponent<SpriteSheetHolder>().GetSprite($"{prev_poseID}_eyebrow{componentID[1]}"); if (poseChanged) img.SetNativeSize(); }
			if (componentID[2] != '_') { Image img = mouth.GetComponent<Image>(); img.sprite = mouth.GetComponent<SpriteSheetHolder>().GetSprite($"{prev_poseID}_mouth{componentID[2]}"); if (poseChanged) img.SetNativeSize(); }

			// expressions
			foreach (RectTransform rt in expressions) rt.gameObject.SetActive(false);
			if (currentRig != null) {
				foreach (string exprID in expressionsIDs) {
					if (exprID == empty_poseID) continue;

					ExpressionRigData exprData = currentRig.expressions.Find(e => e.name == exprID);
					if (exprData == null || exprData.layer >= expressions.Count) continue;
					
					RectTransform rt = expressions[exprData.layer];

					rt.anchoredPosition = exprData.data.localPosition;
					Image img = rt.GetComponent<Image>();
					img.sprite = rt.GetComponent<SpriteSheetHolder>().GetSprite(poseIndependentExpressions.Contains(exprID) ? exprID : $"{prev_poseID}_{exprID}");
					img.SetNativeSize();
					rt.gameObject.SetActive(true);
				}
			}
		}

		public IEnumerator HighlightTransition(bool highlight, Color targetColor, float speed) {
			Color oldColor = body.GetComponent<Image>().color;
			
			Vector2 oldPosition = entire.anchoredPosition;
			Vector2 targetPosition = new(oldPosition.x, oldPosition.y + (highlight ? -1 * unhighlightYOffset : unhighlightYOffset));
			
			List<Image> images = new List<Image> {
				body.GetComponent<Image>(),
				eye.GetComponent<Image>(),
				eyebrow.GetComponent<Image>(),
				mouth.GetComponent<Image>()
			};
			foreach (RectTransform rt in expressions) { images.Add(rt.GetComponent<Image>()); }

			float percent = 0;
			while (percent < 1) {
				percent += speed * Time.deltaTime;

				entire.anchoredPosition = Vector2.Lerp(oldPosition, targetPosition, percent);
				foreach (Image img in images) {
					img.color = Color.Lerp(oldColor, targetColor, percent);
				}
				yield return null;
			}
		}
	}
}