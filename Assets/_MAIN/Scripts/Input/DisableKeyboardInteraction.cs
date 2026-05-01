using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisableKeyboardInteraction : Button
{
    public override void OnSubmit(BaseEventData eventData) { }
    public override void OnMove(AxisEventData eventData) { }
    public override void OnSelect(BaseEventData eventData) { }
}