using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame
{
    public static class Ensure
    {
        public static void NotNull<T>( T field ) where T : class
        {
            if( field == null )
            {
                throw new InvalidOperationException( $"Ensure: '{nameof( T )} {nameof( field )} not null' failed." );
            }
        }

        public static void Is<T>( T field, Func<T, bool> predicate ) where T : class
        {
            if( !predicate( field ) )
            {
                throw new InvalidOperationException( $"Ensure: '{nameof( T )} {nameof( field )} is predicate' failed." );
            }
        }
    }
}