using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour 
{
    [SerializeField] private Canvas canvas;



    // Manage the drag event so the buttons can be draged elsewhere on the canvas
    public void DragHandler(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, pointerData.position, 
                                                                 canvas.worldCamera, out position);
        transform.position = canvas.transform.TransformPoint(position);
    }
}
