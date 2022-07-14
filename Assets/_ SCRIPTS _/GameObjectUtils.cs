using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame
{
    public static class GameObjectUtils
    {
        public static RectTransform CreateUI( string name, Transform parent )
        {
            GameObject mainUI = new GameObject( name );
            mainUI.transform.SetParent( parent );

            RectTransform rt = mainUI.AddComponent<RectTransform>();

            return rt;
        }

        public static void ApplyTransformUI( this RectTransform rt, Vector2 pivot, Vector2 anchor, Vector2 anchoredPosition, Vector2 sizeDelta )
        {
            rt.pivot = pivot;
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = sizeDelta;
        }

        public static void ApplyTransformUI( this RectTransform rt, Vector2 pivot, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta )
        {
            rt.pivot = pivot;
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = sizeDelta;
        }

        public static void ApplyTransformUI( this RectTransform rt, Vector2 pivot, float left, float right, float top, float bottom )
        {
            // Stretches it to the edges, sets the pivot, left, right, top, bottom are padding.
            rt.pivot = pivot;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            // left = 10, right = 20
            // pivot.x 0 => 10, 0.5 => -5, 1 => -20 -> lerp
            // so Mathf.Lerp( left, -right, pivot.x )

            // top = 10, bottom = 20
            // pivot.y 0 => 20, 0.5 => 5, 1 => -10
            // fo Mathf.Lerp( bottom, -top, pivot.y )
            rt.anchoredPosition = new Vector2( Mathf.Lerp( left, -right, pivot.x ), Mathf.Lerp( bottom, -top, pivot.y ) );
            rt.sizeDelta = new Vector2( -(left + right), -(top + bottom) );
        }
    }
}