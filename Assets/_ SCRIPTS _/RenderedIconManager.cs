using RPGGame.Assets;
using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame
{
    public class RenderedIconManager : MonoBehaviour
    {
        public const int RESOLUTION_SIZE_MULTIPLIER = 32;

        private static Dictionary<string, (Texture2D tex, float worldSize)> textures = new Dictionary<string, (Texture2D tex, float worldSize)>();

        const int ICON_RENDER_LAYER = 31;


        void Start()
        {
            Item[] items = AssetManager.GetItems();

            foreach( var item in items )
            {
                // Doing this in awake seems to produce weird highlight artifacts and things (ambient light-related??)
                // So we do it in Start.
                Quaternion modelRot = Quaternion.Euler( -11.25f, 180, 22.5f );
                if( item.UseCustomCamera )
                {
                    modelRot = Quaternion.Euler( item.CustomCameraRot );
                }
                
                int texResolution = RESOLUTION_SIZE_MULTIPLIER * Mathf.Max( item.Size.x, item.Size.y );

                if( texResolution < 32 )
                {
                    texResolution = 32;
                }
                
                ScreenMesh( item.ID, texResolution, item.model, modelRot, Quaternion.Euler( 10, -30, 0 ) );
            }
        }

        /// <summary>
        /// Looks up a icon texture in the database.
        /// </summary>
        /// <param name="name">Unique name of the result to look up.</param>
        public static Texture2D GetTexture( string name )
        {
            return textures[name].tex;
        }

        /// <summary>
        /// Looks up the world size of an item (the max dimention along the object's principal axes).
        /// </summary>
        /// <param name="name">Unique name of the result to look up.</param>
        public static float GetTextureWorldSize( string name )
        {
            return textures[name].worldSize;
        }

        /// <summary>
        /// Screenstors a model, adds the result to the database, and returns the resulting texture.
        /// </summary>
        /// <param name="name">Unique name to save the result under.</param>
        public static Texture2D ScreenMesh( string name, int resolutionXY, GameObject prefab, Quaternion modelRot, Quaternion lightRot )
        {
            RenderTexture renderTex = new RenderTexture( resolutionXY, resolutionXY, 8, RenderTextureFormat.ARGB32 );
            renderTex.Create();

            Bounds bounds = prefab.GetBounds();
            float max = Mathf.Max( bounds.extents.x, bounds.extents.y, bounds.extents.z );

            (GameObject camGo, Camera cam) = CreateCamera( Vector3.zero, Quaternion.identity, max, renderTex );
            GameObject lightGo = CreateLight( Vector3.zero, lightRot );
            GameObject modelGo = Instantiate( prefab, Vector3.zero, modelRot );
            modelGo.layer = ICON_RENDER_LAYER;

            cam.Render();

            Texture2D tex = RenderTextureToTexture2D( renderTex );

            Destroy( camGo );
            DestroyImmediate( lightGo );
            DestroyImmediate( modelGo );

            textures.Add( name, (tex, max * 2.0f) );
            return tex;
        }

        private static (GameObject g, Camera c) CreateCamera( Vector3 position, Quaternion rotation, float meshHalfSizeMax, RenderTexture renderTexture )
        {
            GameObject gameObj = new GameObject( "cam" );
            gameObj.layer = ICON_RENDER_LAYER;

            Transform transform = gameObj.transform;
            transform.position = position;
            transform.rotation = rotation;

            Camera camera = gameObj.AddComponent<Camera>();
            camera.nearClipPlane = -meshHalfSizeMax;
            camera.farClipPlane = meshHalfSizeMax;
            camera.cullingMask = 1 << ICON_RENDER_LAYER;
            camera.orthographic = true;
            camera.orthographicSize = meshHalfSizeMax;
            camera.targetTexture = renderTexture;
            camera.forceIntoRenderTexture = true;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color( 0, 0, 0, 0 );

            return (gameObj, camera);
        }

        private static GameObject CreateLight( Vector3 position, Quaternion rotation )
        {
            GameObject gameObj = new GameObject( "light" );
            gameObj.layer = ICON_RENDER_LAYER;

            Transform transform = gameObj.transform;
            transform.position = position;
            transform.rotation = rotation;

            Light light = gameObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.cullingMask = 1 << ICON_RENDER_LAYER;

            return gameObj;
        }

        private static Texture2D RenderTextureToTexture2D( RenderTexture renderTexture )
        {
            RenderTexture previousActive = RenderTexture.active;

            Texture2D texture2D = new Texture2D( renderTexture.width, renderTexture.height );
            RenderTexture.active = renderTexture;

            texture2D.ReadPixels( new Rect( 0, 0, renderTexture.width, renderTexture.height ), 0, 0 );
            texture2D.Apply();

            RenderTexture.active = previousActive;

            return texture2D;
        }
    }
}