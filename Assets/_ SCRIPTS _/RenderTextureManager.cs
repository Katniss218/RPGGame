using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame
{
    public class RenderTextureManager : MonoBehaviour
    {
        [SerializeField] private Item[] items;

        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        const int ICON_RENDER_LAYER = 31;

        
        void Start()
        {
            foreach( var item in items )
            {
                // Doing this in awake seems to produce weird highlight artifacts and things (ambient light-related??)
                // So we do it in Start.
                ScreenMesh( item.ID, 128, item.mesh, item.materials, Quaternion.Euler( -11.25f, 180, 22.5f ), Quaternion.Euler( 10, -30, 0 ) );
            }
        }

        /// <summary>
        /// Looks up a result in the database.
        /// </summary>
        /// <param name="name">Unique name of the result to look up.</param>
        public static Texture2D GetTexture( string name )
        {
            return textures[name];
        }

        /// <summary>
        /// Screenstors a model, adds the result to the database, and returns the resulting texture.
        /// </summary>
        /// <param name="name">Unique name to save the result under.</param>
        public static Texture2D ScreenMesh( string name, int resolutionXY, Mesh mesh, Material[] materials, Quaternion modelRot, Quaternion lightRot )
        {
            RenderTexture renderTex = new RenderTexture( resolutionXY, resolutionXY, 8, RenderTextureFormat.ARGB32 );
            renderTex.Create();

            (GameObject camGo, Camera cam) = CreateCamera( Vector3.zero, Quaternion.identity, renderTex );
            GameObject lightGo = CreateLight( Vector3.zero, lightRot );
            GameObject modelGo = CreateModel( Vector2.zero, modelRot, mesh, materials );

            cam.Render();

            Texture2D tex = RenderTextureToTexture2D( renderTex );

            Destroy( camGo );
            DestroyImmediate( lightGo );
            DestroyImmediate( modelGo );

            textures.Add( name, tex );
            return tex;
        }

        private static (GameObject g, Camera c) CreateCamera( Vector3 position, Quaternion rotation, RenderTexture renderTexture )
        {
            GameObject gameObj = new GameObject( "cam" );
            gameObj.layer = ICON_RENDER_LAYER;

            Transform transform = gameObj.transform;
            transform.position = position;
            transform.rotation = rotation;

            Camera camera = gameObj.AddComponent<Camera>();
            camera.nearClipPlane = -0.5f;
            camera.farClipPlane = 0.5f;
            camera.cullingMask = 1 << ICON_RENDER_LAYER;
            camera.orthographic = true;
            camera.orthographicSize = 0.5f;
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

        private static GameObject CreateModel( Vector3 position, Quaternion rotation, Mesh mesh, Material[] materials )
        {
            GameObject gameObj = new GameObject( "model" );
            gameObj.layer = ICON_RENDER_LAYER;

            Transform transform = gameObj.transform;
            transform.position = position;
            transform.rotation = rotation;

            MeshFilter meshFilter = gameObj.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = gameObj.AddComponent<MeshRenderer>();
            //meshRenderer.material = materials[0];
            meshRenderer.materials = materials;

            return gameObj;
        }

        private static Texture2D RenderTextureToTexture2D( RenderTexture renderTexture )
        {
            RenderTexture previousActive = RenderTexture.active;

            Texture2D texture2D = new Texture2D( 128, 128 );
            RenderTexture.active = renderTexture;

            texture2D.ReadPixels( new Rect( 0, 0, renderTexture.width, renderTexture.height ), 0, 0 );
            texture2D.Apply();

            RenderTexture.active = previousActive;

            return texture2D;
        }
    }
}