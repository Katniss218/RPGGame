using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items
{
    public class ItemStack
    {
        /// <summary>
        /// Returns an empty item stack.
        /// </summary>
        public static ItemStack Empty => new ItemStack( null, 0 );

        /// <summary>
        /// The type of item held in this item stack.
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// the amount of items held in this item stack.
        /// </summary>
        public int Amount { get; set; }

        public int MaxAmount => Item.MaxStack;
        public int SpaceLeft => Item.MaxStack - Amount;
        public bool IsEmpty => Item == null;
        public bool IsFull => Item != null && Amount >= Item.MaxStack;

        public ItemStack( Item item, int amount )
        {
            this.Item = item;
            this.Amount = amount;
        }

        public virtual void MakeEmpty()
        {
            this.Item = null;
            this.Amount = 0;
        }

        public virtual bool CanStackWith( ItemStack other )
        {
            if( this.IsEmpty || other.IsEmpty )
            {
                return true;
            }
            return this.Item.CanStackWith( other.Item );
        }

        public virtual int AmountToAdd( ItemStack itemStack, bool unstacked )
        {
            if( this.IsEmpty )
            {
                return itemStack.MaxAmount;
            }
            return unstacked ? itemStack.Amount : Mathf.Min( this.SpaceLeft, itemStack.Amount );
        }

        public virtual int AmountToRemove( int desiredAmount )
        {
            return Mathf.Min( this.Amount, desiredAmount );
        }

        public ItemStack Copy()
        {
            return new ItemStack( this.Item, this.Amount );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unstacked">If true, the max amount of this item stack will be ignored, allowing it to overflow.</param>
        /// <returns>The amount of items that were added to the item stack.</returns>
        public int Add( ItemStack desiredItemStack, bool unstacked )
        {
            if( desiredItemStack.IsEmpty )
            {
                return 0;
            }

            if( this.CanStackWith( desiredItemStack ) )
            {
                if( this.IsEmpty )
                {
                    this.Item = desiredItemStack.Item;
                }

                int amountToAdd = AmountToAdd( desiredItemStack, unstacked );

                this.Amount += amountToAdd;

                return amountToAdd;
            }

            return 0;
        }

        public int Sub( int desiredAmount )
        {
            if( this.IsEmpty )
            {
                return 0;
            }

            int amountToRemove = AmountToRemove( desiredAmount );

            this.Amount -= amountToRemove;

            if( this.Amount < 0 )
            {
                throw new Exception( $"this.Amount was negative ({this.Amount}) after subtracting {desiredAmount}, wtf!" );
            }

            if( this.Amount == 0 )
            {
                this.MakeEmpty();
            }
            return amountToRemove;
        }

        public override string ToString()
        {
            return $"({Amount}x '{Item}')";
        }
    }
}