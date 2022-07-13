using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame
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
    }
}