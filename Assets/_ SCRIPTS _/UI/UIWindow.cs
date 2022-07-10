using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UI
{
    public class UIWindow : MonoBehaviour
    {
        [SerializeField] private Button closeButton;

        [SerializeField] private bool startHidden = true;

        protected virtual void Awake()
        {
            closeButton.onClick.AddListener( Hide );
        }

        protected virtual void Start()
        {
            if( startHidden )
            {
                Hide();
            }
        }

        public void Show()
        {
            this.gameObject.SetActive( true );
        }

        public void Hide()
        {
            this.gameObject.SetActive( false );
        }

        public void Toggle()
        {
            this.gameObject.SetActive( !this.gameObject.activeSelf );
        }
    }
}