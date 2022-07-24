using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System.IO
{
    public static class DirectoryEx
    {
        public static void EnsureExists( string path )
        {
            if( !Directory.Exists( path ) )
            {
                Directory.CreateDirectory( path );
            }
        }
    }
}