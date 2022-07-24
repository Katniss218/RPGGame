using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System.IO
{
    public static class FileEx
    {
        public static void EnsureExists( string path )
        {
            string directoryPath = Path.GetDirectoryName( path );
            if( !Directory.Exists( directoryPath ) )
            {
                Directory.CreateDirectory( directoryPath );
            }

            if( !File.Exists( path ) )
            {
                File.Create( path );
            }
        }
    }
}