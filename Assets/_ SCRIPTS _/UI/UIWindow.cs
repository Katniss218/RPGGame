using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UI
{
    public class UIWindow : MonoBehaviour
    {
        [SerializeField] private Button closeButton;

        [SerializeField] private bool startHidden = false;
        [SerializeField] private bool destroyOnClose = false;

        private static List<UIWindow> uiWindows = new List<UIWindow>();

        private RectTransform __rectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if( __rectTransform == null )
                {
                    __rectTransform = (RectTransform)this.transform;
                }
                return __rectTransform;
            }
        }

        /// <summary>
        /// Checks if a UI window of the specified type (exact match) exists.
        /// </summary>
        public static bool ExistsAny<T>() where T : UIWindow
        {
            foreach( var window in uiWindows )
            {
                // Type must match exactly, no matter the inheritance.
                if( window.GetType() == typeof( T ) )
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            closeButton.onClick.AddListener( Hide );
            if( startHidden )
            {
                Hide();
            }
        }

        protected virtual void OnDestroy()
        {
            uiWindows.Remove( this );
        }

        public void Show()
        {
            this.gameObject.SetActive( true );
        }

        public void Hide()
        {
            if( destroyOnClose )
            {
                Destroy( this.gameObject );
            }
            else
            {
                this.gameObject.SetActive( false );
            }
        }

        public void Destroy()
        {
            Destroy( this.gameObject );
        }

        public void Toggle()
        {
            this.gameObject.SetActive( !this.gameObject.activeSelf );
        }

        public static (RectTransform rt, T ui) Create<T>( string objectName, Canvas parent ) where T : UIWindow
        {
            RectTransform rt = GameObjectUtils.CreateUI( objectName, parent.transform );

            rt.gameObject.AddComponent<CanvasRenderer>();
            Image image = rt.gameObject.AddComponent<Image>();
            image.type = Image.Type.Sliced;
            image.sprite = AssetManager.GetSprite( "Sprites/ui_window" );

            GameObject closeButton = Instantiate( AssetManager.GetPrefab( "Prefabs/UI/x_button" ), rt );

            T uiWindow = rt.gameObject.AddComponent<T>();
            uiWindow.closeButton = closeButton.GetComponent<Button>();
            uiWindow.startHidden = false;
            uiWindow.destroyOnClose = true;

            uiWindows.Add( uiWindow );

            return (rt, uiWindow);
        }
    }
}