using TMPro;
using UnityEngine;

namespace origin.graphic {
	public class PromptObject : MonoBehaviour {
		private RectTransform root;

		[SerializeField] private Animator anim;
		[SerializeField] private TextMeshProUGUI tmpro;

		public bool isShowing => anim.gameObject.activeSelf;

		void Start() {
			root = GetComponent<RectTransform>();
		}
	}
}