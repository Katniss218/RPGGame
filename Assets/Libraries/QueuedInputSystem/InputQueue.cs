using System;
using System.Collections.Generic;
using UnityEngine;

namespace QueuedInputSystem
{
    /// <summary>
    /// A queue of methods for a given input key/button.
    /// </summary>
    public class InputQueue
    {
        private class InputMethod
        {
            /// <summary>
            /// The action performed by the method.
            /// </summary>
            public Action<InputQueue> Method { get; set; }

            public bool IsEnabled { get; set; }

            /// <summary>
            /// If true, the method will be removed after playing.
            /// </summary>
            public bool IsOneShot { get; set; }

            /// <summary>
            /// Lower priority runs first.
            /// </summary>
            public float Priority { get; set; }
        }

        public Vector3 LastControllerPosition { get; set; }

        public int PressCount { get; set; }

        public float PressTimestamp { get; set; } = float.MinValue;

        /// <summary>
        /// True if the queue was stopped in this execution and will not execute later methods.
        /// </summary>
        public bool IsStopped { get; private set; }


        List<InputMethod> methods;

        public InputQueue()
        {
            this.methods = new List<InputMethod>();
        }

        public void Add( Action<InputQueue> method, float priority, bool isEnabled, bool isOneShot )
        {
            // Get the last index with priority still lower than the new method's priority.
            int lastBelowIndex = -1;
            for( int i = 0; i < methods.Count; i++ )
            {
                if( this.methods[i].Priority <= priority )
                {
                    lastBelowIndex = i;
                }
            }

            InputMethod inputMethod = new InputMethod()
            {
                Method = method,
                Priority = priority,
                IsEnabled = isEnabled,
                IsOneShot = isOneShot
            };

            if( this.methods.Count == 0 )
            {
                this.methods.Add( inputMethod );
                return;
            }

            if( lastBelowIndex == this.methods.Count - 1 )
            {
                this.methods.Add( inputMethod );
            }
            else
            {
                this.methods.Insert( lastBelowIndex + 1, inputMethod );
            }
        }

        public bool Remove( Action<InputQueue> method )
        {
            for( int i = 0; i < methods.Count; i++ )
            {
                if( this.methods[i].Method == method )
                {
                    this.methods.RemoveAt( i );
                    return true;
                }
            }
            return false;
        }

        public void StopExecution()
        {
            this.IsStopped = true;
        }

        public void Execute()
        {
            this.IsStopped = false;
            List<InputMethod> oneShots = new List<InputMethod>();

            foreach( var inputMethod in methods )
            {
                if( !inputMethod.IsEnabled )
                {
                    continue;
                }

                // Important, do not take it in front of the loop, because it's gonna be set from within the invoked method.
                if( this.IsStopped )
                {
                    return;
                }

                if( inputMethod.IsOneShot )
                {
                    oneShots.Add( inputMethod );
                }

                inputMethod.Method.Invoke( this );
            }

            foreach( var oneshotMethod in oneShots )
            {
                this.methods.Remove( oneshotMethod );
            }
        }

        public void Enable( Action<InputQueue> method )
        {
            foreach( var inputMethod in methods )
            {
                if( inputMethod.Method != method )
                {
                    continue;
                }

                if( inputMethod.IsEnabled )
                {
                    throw new Exception( "The method is already enabled." );
                }

                inputMethod.IsEnabled = true;
                return;
            }
        }

        public void Disable( Action<InputQueue> method )
        {
            foreach( var inputMethod in methods )
            {
                if( inputMethod.Method != method )
                {
                    continue;
                }

                if( !inputMethod.IsEnabled )
                {
                    throw new Exception( "The method is already disabled." );
                }

                inputMethod.IsEnabled = false;
                return;
            }
        }
    }
}