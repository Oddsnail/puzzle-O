using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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
		
		private string prev_poseID = "std1";
		private readonly float unhighlightOffset = -40.0f;
		private readonly string concatenator = "-";
		private readonly string empty_poseID = "_";

		// poseID 			ex) std2, std1, _
		// componentID 		ex) 221, 1__, 1_3
		// expressionsID 	ex) 1, 12, 21, 11
		public void SetSprite(string charID, string poseID, string componentID, string expressionsID) {
			if (poseID != prev_poseID && poseID != empty_poseID) {
				FaceRigData rig = faceRigSO.GetFaceRigData(charID + concatenator + poseID);
				if (rig == null) return;

				body.GetComponent<Image>().sprite = body.GetComponent<SpriteSheetHolder>().sprites[componentID[3] - '1'];
				eye.anchoredPosition = rig.eye.localPosition;
				eyebrow.anchoredPosition = rig.eyebrow.localPosition;
				mouth.anchoredPosition = rig.mouth.localPosition;
				foreach (RectTransform rt in expressions) {
					rt.anchoredPosition = rig.expression.localPosition;
					rt.gameObject.SetActive(false);
				}

				prev_poseID = poseID;
			}
			if (componentID[0] != '_') eye.GetComponent<Image>().sprite = eye.GetComponent<SpriteSheetHolder>().sprites[componentID[0] - '1'];
			if (componentID[1] != '_') eyebrow.GetComponent<Image>().sprite = eyebrow.GetComponent<SpriteSheetHolder>().sprites[componentID[1] - '1'];
			if (componentID[2] != '_') mouth.GetComponent<Image>().sprite = mouth.GetComponent<SpriteSheetHolder>().sprites[componentID[2] - '1'];
			for (int i = 0; i < expressions.Count; i++) {
				if (i < expressionsID.Length && expressionsID[i] != '_') {
					expressions[i].GetComponent<Image>().sprite = expressions[i].GetComponent<SpriteSheetHolder>().sprites[expressionsID[i] - '1'];
					expressions[i].gameObject.SetActive(true);
				}
				else if(i < expressionsID.Length && expressionsID[i] == '_') {
					continue;
                }
				else {
					expressions[i].gameObject.SetActive(false);
				}
			}
		}

		public IEnumerator HighlightTransition(bool highlight, Color targetColor, float speed) {
			Color oldColor = body.GetComponent<Image>().color;
			
			Vector2 oldPosition = entire.anchoredPosition;
			Vector2 targetPosition = new(oldPosition.x, oldPosition.y + (highlight ? -1 * unhighlightOffset : unhighlightOffset));
			
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