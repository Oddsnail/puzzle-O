using UnityEngine;
using UnityEngine.EventSystems;

namespace origin.graphic {
    public class SideBoxHoverRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        private SideBox fileImage;

        private void Awake() {
            fileImage = GetComponentInParent<SideBox>();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            fileImage.OnHoverEnter();
        }

        public void OnPointerExit(PointerEventData eventData) {
            fileImage.OnHoverExit();
        }
        
    }
}