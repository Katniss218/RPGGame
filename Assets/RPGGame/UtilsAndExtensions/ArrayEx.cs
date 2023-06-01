using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System.Linq
{
    public static class ArrayEx
    {
        public static bool Any<T>( this T[] arr, Func<T, bool> predicate )
        {
            for( int i = 0; i < arr.Length; i++ )
            {
                if( predicate( arr[i] ) )
                {
                    return true;
                }
            }

            return false;
        }

        public static int Count<T>( this T[] arr, Func<T, bool> predicate )
        {
            int count = 0;
            for( int i = 0; i < arr.Length; i++ )
            {
                if( predicate( arr[i] ) )
                {
                    count++;
                }
            }

            return count;
        }
    }
}