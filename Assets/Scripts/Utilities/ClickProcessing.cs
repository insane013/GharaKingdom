using UnityEngine;
using UnityEngine.EventSystems;

public class ClickProcessing : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float threshold = 10f;

    Vector2 clickPos; float dist;

    public void OnPointerDown(PointerEventData eventData)
    {
        dist= 0;
        clickPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dist = Vector2.Distance(clickPos, eventData.position);
        if (dist <=threshold )
        {
            this.gameObject.SendMessage("MouseClickProcessing");
        }
    }
}
