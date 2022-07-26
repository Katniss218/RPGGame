using System;
using System.Collections.Generic;
using UnityEngine;

namespace QueuedInputSystem
{
	public sealed class QueuedMouseInput : MonoBehaviour
	{
		Dictionary<MouseCode, InputQueue> press = new Dictionary<MouseCode, InputQueue>();
		Dictionary<MouseCode, InputQueue> hold = new Dictionary<MouseCode, InputQueue>();
		Dictionary<MouseCode, InputQueue> release = new Dictionary<MouseCode, InputQueue>();

		public void EnableHold( MouseCode button, Action<InputQueue> method )
		{
			if( this.hold.TryGetValue( button, out InputQueue inputQueue ) )
			{
				inputQueue.Enable( method );
			}
		}

		public void DisableHold( MouseCode button, Action<InputQueue> method )
		{
			if( this.hold.TryGetValue( button, out InputQueue inputQueue ) )
			{
				inputQueue.Disable( method );
			}
		}


		public void EnableRelease( MouseCode button, Action<InputQueue> method )
		{
			if( this.hold.TryGetValue( button, out InputQueue inputQueue ) )
			{
				inputQueue.Enable( method );
			}
		}

		public void DisableRelease( MouseCode button, Action<InputQueue> method )
		{
			if( this.hold.TryGetValue( button, out InputQueue inputQueue ) )
			{
				inputQueue.Disable( method );
			}
		}


		public void EnablePress( MouseCode button, Action<InputQueue> method )
		{
			if( this.hold.TryGetValue( button, out InputQueue inputQueue ) )
			{
				inputQueue.Enable( method );
			}
		}

		public void DisablePress( MouseCode button, Action<InputQueue> method )
		{
			if( this.hold.TryGetValue( button, out InputQueue inputQueue ) )
			{
				inputQueue.Disable( method );
			}
		}

		/// <summary>
		/// Registers a function to run when a button is pressed.
		/// </summary>
		public void RegisterOnPress( MouseCode button, float priorityId, Action<InputQueue> method, bool isEnabled = true, bool isOneShot = false )
		{
			InputQueue inputQueue;
			if( this.press.TryGetValue( button, out inputQueue ) )
			{
				inputQueue.Add( method, priorityId, isEnabled, isOneShot );
				return;
			}
			inputQueue = new InputQueue();
			inputQueue.Add( method, priorityId, isEnabled, isOneShot );
			this.press.Add( button, inputQueue );
		}

		/// <summary>
		/// Registers a function to run when a button is being held down.
		/// </summary>
		public void RegisterOnHold( MouseCode button, float priorityId, Action<InputQueue> method, bool isEnabled = true, bool isOneShot = false )
		{
			InputQueue inputQueue;
			if( this.hold.TryGetValue( button, out inputQueue ) )
			{
				inputQueue.Add( method, priorityId, isEnabled, isOneShot );
				return;
			}
			inputQueue = new InputQueue();
			inputQueue.Add( method, priorityId, isEnabled, isOneShot );
			this.hold.Add( button, inputQueue );
		}

		/// <summary>
		/// Registers a function to run when a button is released.
		/// </summary>
		public void RegisterOnRelease( MouseCode button, float priorityId, Action<InputQueue> method, bool isEnabled = true, bool isOneShot = false )
		{
			InputQueue inputQueue;
			if( this.release.TryGetValue( button, out inputQueue ) )
			{
				inputQueue.Add( method, priorityId, isEnabled, isOneShot );
				return;
			}
			inputQueue = new InputQueue();
			inputQueue.Add( method, priorityId, isEnabled, isOneShot );
			this.release.Add( button, inputQueue );
		}

		/// <summary>
		/// Unregisters a function from running.
		/// </summary>
		public void ClearOnPress( MouseCode button, Action<InputQueue> method )
		{
			if( this.press.TryGetValue( button, out InputQueue inputQueue ) )
			{
				inputQueue.Remove( method );
			}
		}

		/// <summary>
		/// Unregisters a function from running.
		/// </summary>
		public void ClearOnHold( MouseCode button, Action<InputQueue> method )
		{
			if( this.hold.TryGetValue( button, out InputQueue inputQueue ) )
			{
				inputQueue.Remove( method );
			}
		}

		/// <summary>
		/// Unregisters a function from running.
		/// </summary>
		public void ClearOnRelease( MouseCode button, Action<InputQueue> method )
		{
			if( this.release.TryGetValue( button, out InputQueue inputQueue ) )
			{
				inputQueue.Remove( method );
			}
		}

		/// <summary>
		/// Unregisters every input.
		/// </summary>
		public void ClearRegistries()
		{
			this.press.Clear();
			this.hold.Clear();
			this.release.Clear();
		}

		const float MAX_DOUBLE_CLICK_DELAY = 0.2f;
		const int MAX_CLICK_COUNT = 2;

		void Update()
		{
			// Copy the queue so if the function running in the keypress wants to register another keypress it won't throw that the dictionary was modified.
			Dictionary<MouseCode, InputQueue> pressQueue = new Dictionary<MouseCode, InputQueue>( this.press );

			foreach( var kvp in pressQueue )
			{
				if( Input.GetMouseButtonDown( (int)kvp.Key ) )
				{
					// count number of presses.
					if( 
						   (Time.time <= kvp.Value.PressTimestamp + MAX_DOUBLE_CLICK_DELAY)
						&& (kvp.Value.PressCount < MAX_CLICK_COUNT)
						&& (kvp.Value.LastControllerPosition - Input.mousePosition).sqrMagnitude <= (3.0f * 3.0f)
						)
					{
						kvp.Value.PressCount++;
					}
					else
					{
						kvp.Value.PressCount = 1;
					}
					kvp.Value.Execute();
					kvp.Value.LastControllerPosition = Input.mousePosition;
				}
				if( Input.GetMouseButtonUp( (int)kvp.Key ) )
				{
					kvp.Value.PressTimestamp = Time.time;
				}
			}

			// Copy the queue so if the function running in the keypress wants to register another keypress it won't throw that the dictionary was modified.
			Dictionary<MouseCode, InputQueue> holdQueue = new Dictionary<MouseCode, InputQueue>( this.hold );

			foreach( var kvp in holdQueue )
			{
				if( Input.GetMouseButton( (int)kvp.Key ) )
				{
					kvp.Value.Execute();
				}
			}

			// Copy the queue so if the function running in the keypress wants to register another keypress it won't throw that the dictionary was modified.
			Dictionary<MouseCode, InputQueue> releaseQueue = new Dictionary<MouseCode, InputQueue>( this.release );

			foreach( var kvp in releaseQueue )
			{
				if( Input.GetMouseButtonUp( (int)kvp.Key ) )
				{
					kvp.Value.Execute();
				}
			}
		}
	}
}