using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusableUI : EventTrigger
{
    [SerializeField] public RectTransform Window;

    public override void OnPointerDown( PointerEventData eventData )
    {
        Window.SetAsLastSibling();
    }
}
