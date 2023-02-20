using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace com.pison.handzones
{
	/// <summary>
	/// New added hand zone events -- Inverted == hand flipped upside down
	/// Note: NormalInverted checks both for NORMAL and INVERTED
	/// </summary>
	[Serializable]
	public enum HandZone
	{
		Normal, HandUp, HandDown
	}


	/// <summary>
	/// Event dispatcher for discrete hand zone-based events. See the above HandZone class for hand zone types
	/// Place on the same object as the PisonManager
	/// Will issue a direct update on the HANDZONE when a new one is entered -- Direct Unity based implementation
	/// 
	/// Written by: Arthur Wollocko (arthur@pison.com), 2021
	[RequireComponent(typeof(PisonManager))]
	public class PisonHandZoneEvents : MonoBehaviour
	{
		// Public Variables
		[Header("Hand Zone Thresholds")]
		[Tooltip("The thresholds for the normal zone. Above will be UP, below will be DOWN")]
		public Vector2 normalUpDownZoneThresholds = new Vector2(-4f, 4f);
		[Tooltip("The Accelerometer.z needs to be less than this, and it needs to be in the NORMAL threshold to consider this")]
		public float handInvertedThreshold = -6f;
		[Tooltip("Will adjust based on flipping of the hand")]
		public bool isHandInverted = false;
		public HandZone currentHandZone = HandZone.Normal; 

		// Pison Hand Zone Events
		public delegate void OnHandZoneDelegate(HandZone handZone);
		public event OnHandZoneDelegate OnHandZoneChanged;
		public delegate void OnHandInvertedDelegate(bool isInverted);
		public event OnHandInvertedDelegate OnHandInversionChanged;

		// Private Variables
		private PisonManager pisonManager;

		#region Public API

		public void InvokeHandZone(HandZone handZone)
		{
			currentHandZone = handZone;
			OnHandZoneChanged?.Invoke(handZone);
		}
		public void InvokeHandInverted(bool newInverted)
		{
			isHandInverted = newInverted;
			OnHandInversionChanged?.Invoke(newInverted);
		}


		/// <summary>
		/// Returns the current hand zone
		/// </summary>
		/// <returns></returns>
		public HandZone GetHandZone()
        {
			return currentHandZone;
        }

		#endregion

		#region Wrist Flick Checks

		/// <summary>
		/// Checks the IMU for hand zone based on the accelerometer.
		/// Invokes a change in the hand zone (and events) if needed
		/// </summary>
		private void CheckHandZone()
		{
			float accelPitch = pisonManager.currentDeviceAccelerometer.y;
			float accelRoll = pisonManager.currentDeviceAccelerometer.z;
			if(isHandInverted)
            {
				if(accelRoll >= handInvertedThreshold)
                {
					InvokeHandInverted(false);
                }
            }
			else
            {
				if (accelRoll < handInvertedThreshold)
				{
					InvokeHandInverted(true);
				}
			}
			isHandInverted = (accelRoll < handInvertedThreshold);
			if (accelPitch <= normalUpDownZoneThresholds.x)
            {
				// Less than the min -- HandDown state
				if(currentHandZone != HandZone.HandDown)
                {
					InvokeHandZone(HandZone.HandDown);
                }
            }
			else if (accelPitch >= normalUpDownZoneThresholds.y)
            {
				// More than the max
				// HandUp state
				if (currentHandZone != HandZone.HandUp)
				{
					InvokeHandZone(HandZone.HandUp);
				}
			}
			else
            {
				// Normal normal
				if (currentHandZone != HandZone.Normal)
				{
					InvokeHandZone(HandZone.Normal);
				}
			}
		}

		#endregion

		#region Unity Functions

		void Start()
		{
			pisonManager = GetComponent<PisonManager>();
		}

		void Update()
		{
			CheckHandZone();
		}

		#endregion
	}
}
