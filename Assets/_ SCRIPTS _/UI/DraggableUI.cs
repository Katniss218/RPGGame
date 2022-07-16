using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPGGame.UI
{
    public class DraggableUI : EventTrigger
    {
        public RectTransform Window;

        bool isDragging = false;
        Vector2 cursorOffset = Vector2.zero;

        void Update()
        {
            if( isDragging )
            {
                Window.position = new Vector2( Input.mousePosition.x, Input.mousePosition.y ) + cursorOffset;
            }
        }

        public override void OnPointerDown( PointerEventData eventData )
        {
            cursorOffset = new Vector2( Window.position.x, Window.position.y ) - new Vector2( Input.mousePosition.x, Input.mousePosition.y );
            isDragging = true;
        }

        public override void OnPointerUp( PointerEventData eventData )
        {
            isDragging = false;
        }
    }
}