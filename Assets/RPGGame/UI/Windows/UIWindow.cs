using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPlus.AssetManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPGGame.UI.Windows
{
    public class UIWindow : MonoBehaviour
    {
        [SerializeField] private Button closeButton;

        public bool StartHidden = false;
        public bool DestroyOnClose = false;
        public Transform Owner;

        public bool IsHidden { get => this.gameObject.activeSelf; }

        /// <summary>
        /// Hidden windows still exist in this list. Only destroyed (hard-closed) windows don't.
        /// </summary>
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
        /// Fired when the window is shown (including on creation).
        /// </summary>
        public UnityEvent onShow = new UnityEvent();
        /// <summary>
        /// Fired when the window is hidden (including on creation).
        /// </summary>
        public UnityEvent onHide = new UnityEvent();
        /// <summary>
        /// Fired when the PLAYER presses the 'x' button.
        /// </summary>
        public UnityEvent onClosed = new UnityEvent();

        /// <summary>
        /// Checks if a UI window of the specified type (exact match) exists.
        /// </summary>
        public static bool ExistsAny<T>() where T : UIWindow
        {
            foreach( var window in uiWindows )
            {
                if( window == null )
                {
                    throw new Exception( "Window was null." );
                }

                // Type must match exactly, no matter the inheritance.
                if( window.GetType() == typeof( T ) )
                {
                    return true;
                }
            }

            return false;
        }

        public static List<UIWindow> GetFor( Transform owner, bool includeHidden )
        {
            List<UIWindow> windowsWithOwner = new List<UIWindow>();
            foreach( var window in uiWindows )
            {
                if( window == null )
                {
                    throw new Exception( "Window was null." );
                }

                if( window.Owner == owner )
                {
                    if( !includeHidden && window.IsHidden )
                    {
                        continue;
                    }

                    windowsWithOwner.Add( window );
                }
            }

            return windowsWithOwner;
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

        protected virtual void OnDestroy()
        {
            uiWindows.Remove( this );
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
        /// DO NOT call 'Destroy( obj )' on the window to close it. Set <see cref="DestroyOnClose"/> = true and use <see cref="Hide"/> to close it.
        /// If you don't, it'll lead to memory leaks and the onHide event not being fired.
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
            onHide?.Invoke();
        }

        public void Toggle()
        {
            this.gameObject.SetActive( !this.gameObject.activeSelf );
        }

        /// <summary>
        /// Creates a new UI Window of the specified type.
        /// </summary>
        /// <param name="objectName">The gameObject name for the new UI window.</param>
        /// <param name="owner">The object this window 'belongs' to. Can be null.</param>
        /// <param name="parent">The canvas onto which the UI window will be placed.</param>
        public static (RectTransform rt, T window) Create<T>( string objectName, Transform owner, Canvas parent ) where T : UIWindow
        {
            RectTransform rt = GameObjectEx.CreateUI( objectName, parent.transform );

            rt.gameObject.AddComponent<CanvasRenderer>();
            Image image = rt.gameObject.AddComponent<Image>();
            image.type = Image.Type.Sliced;
            image.sprite = AssetRegistry.Get<Sprite>( "builtin::Resources/Sprites/ui_window" );

            GameObject dragStrip = Instantiate( AssetRegistry.Get<GameObject>( "builtin::Resources/Prefabs/UI/drag_strip" ), rt );
            dragStrip.GetComponent<DraggableUI>().Window = rt;
            dragStrip.GetComponent<FocusableUI>().Window = rt;

            GameObject closeButtonGO = Instantiate( AssetRegistry.Get<GameObject>( "builtin::Resources/Prefabs/UI/x_button" ), rt );

            Button closeButton = closeButtonGO.GetComponent<Button>();

            T uiWindow = rt.gameObject.AddComponent<T>();
            uiWindow.closeButton = closeButton;
            uiWindow.StartHidden = false;
            uiWindow.DestroyOnClose = true;
            uiWindow.Owner = owner;

            closeButton.onClick.AddListener( () =>
            {
                uiWindow.onClosed?.Invoke();
            } );

            uiWindows.Add( uiWindow );

            return (rt, uiWindow);
        }
    }
}