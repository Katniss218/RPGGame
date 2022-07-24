using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{
    public static class RectTransformEx
    {
        public static void MoveOverCentered( this RectTransform mover, RectTransform target )
        {
            // null cameras because this assumed canvas with 'screen space - overlay'.

            // I'd need to spawn the object you want to move, and then use this.
            // It'll get you the position that'll make the center of 'mover' directly over the center of 'target'.
            Vector2 fromPivotDerivedOffset = new Vector2( target.rect.width * 0.5f + target.rect.xMin, target.rect.height * 0.5f + target.rect.yMin ); // (0, -30) // this applies offset to move it to the middle.
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint( null, target.position ); // (1720.00, 837.00)
            screenP += fromPivotDerivedOffset; // (1720.00, 807.00)

            RectTransformUtility.ScreenPointToLocalPointInRectangle( mover, screenP, null, out Vector2 localPoint ); // (50.00, -90.00)
            Vector2 pivotDerivedOffset = new Vector2( mover.rect.width * 0.5f + mover.rect.xMin, mover.rect.height * 0.5f + mover.rect.yMin ); // (0, 0)

            //return mover.anchoredPosition + localPoint - pivotDerivedOffset; // (0, 0) + (50, 90) + (0, 0)
            mover.anchoredPosition = mover.anchoredPosition + localPoint - pivotDerivedOffset; // (0, 0) + (50, 90) + (0, 0)
        }

        public static void MoveOver( this RectTransform mover, RectTransform target )
        {
            // This one returns the position that'll make the pivot of 'mover' be directly over pivot of 'target'.

            Vector2 fromPivotDerivedOffset = new Vector2( target.rect.width * target.pivot.x + target.rect.xMin, target.rect.height * target.pivot.y + target.rect.yMin ); // xmin = -30 ymin = -60 h/w = 60
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint( null, target.position );
            screenP += fromPivotDerivedOffset;

            RectTransformUtility.ScreenPointToLocalPointInRectangle( mover, screenP, null, out Vector2 localPoint );
            Vector2 pivotDerivedOffset = new Vector2( mover.rect.width * mover.pivot.x + mover.rect.xMin, mover.rect.height * mover.pivot.y + mover.rect.yMin );

            //return mover.anchoredPosition + localPoint - pivotDerivedOffset;
            mover.anchoredPosition = mover.anchoredPosition + localPoint - pivotDerivedOffset;
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