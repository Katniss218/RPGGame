using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame
{
    public class RenderTextureManager : MonoBehaviour
    {
        [SerializeField] private Mesh rmesh;
        [SerializeField] private Material rmat;

        static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        const int LAYER = 31;

        // Start is called before the first frame update
        void Awake()
        {
            ScreenMesh( "item.axe", 128, rmesh, rmat, Quaternion.identity, Quaternion.Euler( 15, 45, 0 ) );
            ScreenMesh( "item.sword", 128, rmesh, rmat, Quaternion.Euler( 0, 50, 23 ), Quaternion.Euler( 15, 45, 0 ) );
            ScreenMesh( "item.goblin_mace", 128, rmesh, rmat, Quaternion.Euler( 60, 50, 0 ), Quaternion.Euler( 15, 45, 0 ) );

            //Sprite sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );
            //img.sprite = sprite;



            //Sprite sprite2 = Sprite.Create( tex2, new Rect( 0, 0, tex2.width, tex2.height ), Vector2.zero );
            //img2.sprite = sprite2;
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
        /// <param name="resolutionXY"></param>
        /// <param name="mesh"></param>
        /// <param name="material"></param>
        /// <param name="modelRot"></param>
        /// <param name="lightRot"></param>
        public static Texture2D ScreenMesh( string name, int resolutionXY, Mesh mesh, Material material, Quaternion modelRot, Quaternion lightRot )
        {
            RenderTexture renderTex = new RenderTexture( resolutionXY, resolutionXY, 8, RenderTextureFormat.ARGB32 );
            renderTex.Create();

            (GameObject camGo, Camera cam) = CreateCamera( Vector3.zero, Quaternion.identity, renderTex );
            GameObject lightGo = CreateLight( Vector3.zero, lightRot );
            GameObject modelGo = CreateModel( Vector2.zero, modelRot, mesh, material );

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
            gameObj.layer = LAYER;

            Transform transform = gameObj.transform;
            transform.position = position;
            transform.rotation = rotation;

            Camera camera = gameObj.AddComponent<Camera>();
            camera.nearClipPlane = -0.5f;
            camera.farClipPlane = 0.5f;
            camera.cullingMask = 1 << LAYER;
            camera.orthographic = true;
            camera.orthographicSize = 0.5f;
            camera.targetTexture = renderTexture;
            camera.forceIntoRenderTexture = true;

            return (gameObj, camera);
        }

        private static GameObject CreateLight( Vector3 position, Quaternion rotation )
        {
            GameObject gameObj = new GameObject( "light" );
            gameObj.layer = LAYER;

            Transform transform = gameObj.transform;
            transform.position = position;
            transform.rotation = rotation;

            Light light = gameObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.cullingMask = 1 << LAYER;

            return gameObj;
        }

        private static GameObject CreateModel( Vector3 position, Quaternion rotation, Mesh mesh, Material material )
        {
            GameObject gameObj = new GameObject( "model" );
            gameObj.layer = LAYER;

            Transform transform = gameObj.transform;
            transform.position = position;
            transform.rotation = rotation;

            MeshFilter meshFilter = gameObj.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = gameObj.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

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