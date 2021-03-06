using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UI
{
    [DisallowMultipleComponent]
    public class InventoryItemUI : MonoBehaviour
    {
        private Image __icon;
        private Image icon
        {
            get
            {
                if( __icon == null ) __icon = this.GetComponentInChildren<Image>();
                return __icon;
            }
        }

        private TMPro.TextMeshProUGUI __amountText;
        private TMPro.TextMeshProUGUI amountText
        {
            get
            {
                if( __amountText == null ) __amountText = this.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                return __amountText;
            }
        }

        public void SetIcon( Sprite sprite )
        {
            if( sprite == null )
            {
                icon.gameObject.SetActive( false );
            }
            else
            {
                icon.gameObject.SetActive( true );
            }
            icon.sprite = sprite;
        }

        public void SetIconSize( Vector2 size )
        {
            icon.rectTransform.sizeDelta = size;
        }

        public void SetAmount( int? amount )
        {
            if( amount == null )
            {
                amountText.text = null;
            }
            amountText.text = $"{amount}";
        }
    }
}