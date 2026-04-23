using UnityEngine;


namespace origin.graphic {
	public class PerspectiveMover : MonoBehaviour {
		
		public float maxX;
		public float maxY;

		void Update() {
			float nx = Input.mousePosition.x / Screen.width;
			float ny = Input.mousePosition.y / Screen.height;

			Vector2 target = new Vector2(
				Mathf.Lerp(maxX, -maxX, nx),
				Mathf.Lerp(maxY, -maxY, ny)
			);

			RectTransform rt = (RectTransform)transform;
			rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, target, Time.deltaTime * 5f);
		}
	}
}
