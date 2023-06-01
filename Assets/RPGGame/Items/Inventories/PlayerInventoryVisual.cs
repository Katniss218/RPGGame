using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items.Inventories
{
    [RequireComponent( typeof( PlayerInventory ) )]
    public class PlayerInventoryVisual : MonoBehaviour
    {
        [SerializeField] private Transform hand;
        private GameObject handModel = null;

        [SerializeField] private Transform offhand;
        private GameObject offhandModel = null;

        [SerializeField] private Transform head;
        private GameObject headModel = null;

        [SerializeField] private Transform chest;
        private GameObject chestModel = null;

        [SerializeField] private Transform legs;
        private GameObject legsModel = null;

        [SerializeField] private Transform feet;
        private GameObject feetModel = null;

        private PlayerInventory inventory;

        void Awake()
        {
            inventory = this.GetComponent<PlayerInventory>();
            inventory.onPickup?.AddListener( OnPickup );
            inventory.onDrop?.AddListener( OnDrop );
        }

        private void OnPickup( IInventory.PickupEventInfo e )
        {
            if( e.SlotOrigin == PlayerInventory.SLOT_HAND )
            {
                handModel = Instantiate( e.Item.model, hand );
            }
            if( e.SlotOrigin == PlayerInventory.SLOT_OFFHAND )
            {
                offhandModel = Instantiate( e.Item.model, offhand );
            }
            if( e.SlotOrigin == PlayerInventory.SLOT_HEAD )
            {
                headModel = Instantiate( e.Item.model, head );
            }
            if( e.SlotOrigin == PlayerInventory.SLOT_CHEST )
            {
                chestModel = Instantiate( e.Item.model, chest );
            }
            if( e.SlotOrigin == PlayerInventory.SLOT_LEGS )
            {
                legsModel = Instantiate( e.Item.model, legs );
            }
            if( e.SlotOrigin == PlayerInventory.SLOT_FEET )
            {
                feetModel = Instantiate( e.Item.model, feet );
            }
        }

        private void OnDrop( IInventory.DropEventInfo e )
        {
            if( e.SlotOrigin == PlayerInventory.SLOT_HAND )
            {
                Destroy( handModel );
            }
            if( e.SlotOrigin == PlayerInventory.SLOT_OFFHAND )
            {
                Destroy( offhandModel );
            }
            if( e.SlotOrigin == PlayerInventory.SLOT_HEAD )
            {
                Destroy( headModel );
            }
            if( e.SlotOrigin == PlayerInventory.SLOT_CHEST )
            {
                Destroy( chestModel );
            }
            if( e.SlotOrigin == PlayerInventory.SLOT_LEGS )
            {
                Destroy( legsModel );
            }
            if( e.SlotOrigin == PlayerInventory.SLOT_FEET )
            {
                Destroy( feetModel );
            }
        }
    }
}