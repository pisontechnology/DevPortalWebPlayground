using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace com.pison.wristgestures
{
	/// <summary>
	/// Three new added wrist associated gestures
	/// </summary>
	public enum WristGesture
    {
		WristFlickRight, WristFlickLeft, WristShake
	}

	/// <summary>
	/// Helper enum for flicking
	/// </summary>
	public enum FlickState
	{
		NONE,
		RIGHT,
		LEFT
	}


	/// <summary>
	/// Event dispatcher for discrete wrist-based events. See the above WristGesture class for gesture types
	/// Place on the same object as the PisonManager
	/// Unity based implementation -- operates at game frame rate based on variables ingested from PisonManager and underlying SDK
	/// 
	/// Written by: Arthur Wollocko (arthur@pison.com), 2020
	[RequireComponent(typeof(PisonManager))]
	public class PisonWristEvents : MonoBehaviour
	{
		// Public Variables
		[Header("Wrist Flick Thresholds")]
		[Tooltip("The gyro measurement threshold for a flick of the wrist")]
		public float rightFlickThreshold = 500f;
		[Tooltip("The gyro measurement threshold for a flick of the wrist")]
		public float leftFlickThreshold = 250f;
		[Tooltip("The amount of time for a wrist flick to cool down before another flick is activated")]
		private float wristFlickCooldownTimeSec = .2f;
		

		[Header("Wrist Shake Thresholds")]
		[Tooltip("The threshold at which a shake of the gyro.z needs to break. 350 by default for a moderate shake (left/right)")]
		public float shakeThreshold = 350f;
		[Tooltip("The amount of time a shake needs to be conducted within")]
		public float shakeExpireTimerSec = .5f;

		// Pison Wrist Events
		public delegate void OnWristGestureDelegate(WristGesture wristGesture);
		public event OnWristGestureDelegate OnWristGesture;

		// Private Variables
		private PisonManager pisonManager;
		private bool rightShakeActive, leftShakeActive;
		private Stopwatch flickTimer;
		private FlickState flickState;


		#region Public API

		public void InvokeWristGesture(WristGesture wristGesture)
        {
			OnWristGesture?.Invoke(wristGesture);
        }


		#endregion

		#region Wrist Flick Checks

		/// <summary>
		/// Checks the IMU for wrist flick, based on the thresholds and current flick state.
		/// </summary>
		private void CheckWristFlick()
		{
			float roll = pisonManager.currentDeviceGyro.y;
			switch (flickState)
			{
				case FlickState.NONE:
					// Starting RIGHT flick
					if (roll > Math.Abs(rightFlickThreshold))
					{
						flickState = FlickState.RIGHT;
						StartFlickTimer();
					}
					// Starting LEFT flick
					if (roll < -leftFlickThreshold)
					{
						flickState = FlickState.LEFT;
						StartFlickTimer();
					}
					break;
				case FlickState.LEFT:
					if (roll > rightFlickThreshold)
					{
						// Debug.Log("Left Flick Detected");
						StartCoroutine(OnWristFlick());
					}
					break;
				case FlickState.RIGHT:
					if (roll < -leftFlickThreshold)
					{
						// Debug.Log("Left Flick Detected");
						StartCoroutine(OnWristFlick());
					}
					break;
			}

		}

		private void StartFlickTimer()
		{
			flickTimer = new Stopwatch();
			flickTimer.Start();
		}


		private IEnumerator OnWristFlick()
		{
			flickTimer.Stop();
			flickTimer.Reset();
			// Fire flick event
			WristGesture wristFlickDir = (flickState == FlickState.RIGHT) ? WristGesture.WristFlickRight : WristGesture.WristFlickLeft;
			InvokeWristGesture(wristFlickDir);
			// reset flick state
			flickState = FlickState.NONE;
			yield return new WaitForSeconds(wristFlickCooldownTimeSec);
			flickState = FlickState.NONE;
		}

        #endregion

        #region Wrist Shake Checks

        /// <summary>
        /// Checks the IMU from PisonManager for a wrist shake, defined by the threshold variables 
        /// </summary>
        private void CheckWristShake()
		{
			Vector3 pisonGyro = pisonManager.currentDeviceGyro;
			// if gyro on z axis goes beyond declutter threshold, start right roll timer
			if (pisonGyro.z > shakeThreshold && rightShakeActive == false && leftShakeActive == false)
			{
				StartCoroutine(RightShakeThreshold());
			}

			// if left roll is performed right after right roll, toggle hud object
			if (pisonGyro.z < -shakeThreshold && rightShakeActive && leftShakeActive == false)
			{
				StartCoroutine(LeftShakeThreshold());
			}

			if (rightShakeActive && leftShakeActive)
			{
				InvokeWristGesture(WristGesture.WristShake);
				rightShakeActive = false;
				leftShakeActive = false;
			}
		}

		/// <summary>
		/// Helper functions for the shake gesture functionality. Instead of using timers, uses a brief coroutine
		/// </summary>
		/// <returns></returns>
		IEnumerator RightShakeThreshold()
		{
			rightShakeActive = true;
			yield return new WaitForSeconds(shakeExpireTimerSec);
			rightShakeActive = false;
		}

		IEnumerator LeftShakeThreshold()
		{
			leftShakeActive = true;
			yield return new WaitForSeconds(shakeExpireTimerSec);
			leftShakeActive = false;
		}

		#endregion


		#region Unity Functions

		void Start()
		{
			pisonManager = GetComponent<PisonManager>();
		}

		void Update()
		{
			CheckWristFlick();
			CheckWristShake();
		}
		
		#endregion
	}
}
