using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D cursorIdle;
    public Texture2D cursorClick;


    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter.layer == LayerMask.GetMask("Button"))
        {
            Cursor.SetCursor(cursorClick, Vector2.zero, CursorMode.Auto);
        }
    }
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerEnter.layer == LayerMask.GetMask("Button"))
        {
            Cursor.SetCursor(cursorIdle, Vector2.zero, CursorMode.Auto);
        }
    }
}
