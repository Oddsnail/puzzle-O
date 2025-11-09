using UnityEngine;
using UnityEngine.EventSystems;

namespace origin.graphic {
	public class OptionButtonHoverRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
		private OptionButton optionButton;

		private void Awake() {
			optionButton = GetComponentInParent<OptionButton>();
		}

		public void OnPointerEnter(PointerEventData eventData) {
			optionButton.OnHoverEnter();
		}

		public void OnPointerExit(PointerEventData eventData) {
			optionButton.OnHoverExit();
		}
	}
}