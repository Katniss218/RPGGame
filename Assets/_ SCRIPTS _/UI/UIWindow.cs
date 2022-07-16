using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPGGame.UI
{
    public class UIWindow : MonoBehaviour
    {
        [SerializeField] private Button closeButton;

        public bool StartHidden = false;
        public bool DestroyOnClose = false;

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

        public UnityEvent onShow = new UnityEvent();
        public UnityEvent onClose = new UnityEvent();

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

            // Call the methods regardless of the initial state, to always fire the events consistently.
            if( StartHidden )
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        /// <summary>
        /// Displays the UI window.
        /// </summary>
        public void Show()
        {
            onShow?.Invoke();
            this.gameObject.SetActive( true );
        }
        
        /// <summary>
        /// Hides the UI window.
        /// </summary>
        /// <remarks>
        /// DO NOT call 'Destroy( obj )' on the window. Set <see cref="DestroyOnClose"/> = true and <see cref="Hide"/> it instead.
        /// If you don't, it'll lead to memory leaks and events not being fired.
        /// </remarks>
        public void Hide()
        {
            if( DestroyOnClose )
            {
                Destroy( this.gameObject );
            }
            else
            {
                this.gameObject.SetActive( false );
            }
            onClose?.Invoke();
            uiWindows.Remove( this );
        }

        public void Toggle()
        {
            this.gameObject.SetActive( !this.gameObject.activeSelf );
        }

        /// <summary>
        /// Creates a new UI Window of the specified type.
        /// </summary>
        /// <param name="objectName">The gameObject name for the new UI window.</param>
        /// <param name="parent">The canvas onto which the UI window will be placed.</param>
        public static (RectTransform rt, T window) Create<T>( string objectName, Canvas parent ) where T : UIWindow
        {
            RectTransform rt = GameObjectUtils.CreateUI( objectName, parent.transform );

            rt.gameObject.AddComponent<CanvasRenderer>();
            Image image = rt.gameObject.AddComponent<Image>();
            image.type = Image.Type.Sliced;
            image.sprite = AssetManager.GetSprite( "Sprites/ui_window" );

            GameObject dragStrip = Instantiate( AssetManager.GetPrefab( "Prefabs/UI/drag_strip" ), rt );
            dragStrip.GetComponent<DraggableUI>().Window = rt;
            dragStrip.GetComponent<FocusableUI>().Window = rt;

            GameObject closeButton = Instantiate( AssetManager.GetPrefab( "Prefabs/UI/x_button" ), rt );

            T uiWindow = rt.gameObject.AddComponent<T>();
            uiWindow.closeButton = closeButton.GetComponent<Button>();
            uiWindow.StartHidden = false;
            uiWindow.DestroyOnClose = true;

            uiWindows.Add( uiWindow );

            return (rt, uiWindow);
        }
    }
}