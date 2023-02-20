using PisonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.pison.examples
{
	/// <summary>
	/// Simple example script to model event-based device behaviors
	/// Subscribes to orientation, swipe, device state (battery), and connection events
	/// 
	/// Will also send a short haptic burst to the device on each GESTURE
	/// 
	/// Written by: Arthur Wollocko (arthur@pison.com), 2020
	public class ExamplePisonReadEvents : MonoBehaviour
	{
		// Public Variables
		[Tooltip("The different colors corresponding to each of the swipe directions")]
		public Color swipeRightColor, swipeLeftColor, swipeUpColor, swipeDownColor;
		[Tooltip("The cube that will be adjusted via orientation and gesture events")]
		public GameObject cubeToAdjust;
		[Tooltip("UI fields corresponding to each type of data")]
		public Text lastGestureText, connectionText, indexActivatedText, watchCheckText, batteryLevelText;
		public Image connectionImage;										// Note: Currently connection status update is non functional and will be updated next version

		// Private Variables
		private float timerForGestureSec = 2f;								// The current timer for the gesture text
		private float timerResetAmountSecondsForGestureExpire = 2f;			// The amount of time the gesture text should appear on the screen
		private Renderer cubeRend;											// The renderer to adjust color on (from the cubeToAdjust)

		// Pison objects
		private PisonManager pisonManager;
		private PisonEvents pisonEvents;

		#region Pison Events and Private Functions

		private void PisonEvents_OnActivation(ActivationStates activation)
		{
			string indexActivated = (activation.Index == ActivationState.Hold) ? "Activated" : "Not Activated";
			string watchCheck = (activation.WatchCheck == ActivationState.Hold) ? "Activated" : "Not Activated";

			indexActivatedText.text = indexActivated;
			watchCheckText.text = watchCheck;
		}

		/// <summary>
		/// On each orientation update (euler), update the provided cube
		/// </summary>
		/// <param name="newEulers"></param>
		private void PisonEvents_OnOrientation(Vector3 newEulers)
		{
			if(cubeToAdjust != null)
            {
				cubeToAdjust.transform.eulerAngles = newEulers;
            }
		}

		/// <summary>
		/// Adjust the color of the provided cube based on the swipe event (and direction)
		/// </summary>
		/// <param name="gesture"></param>
		private void PisonEvents_OnGesture(ImuGesture gesture)
		{
			// Send a short haptic burst to the device on each gesture
			pisonManager.SendHapticBursts(100);

			UpdateLastGestureText(gesture.ToString());
			// Change color based on swipe
			switch(gesture)
            {
				case ImuGesture.SwipeUp:
					cubeRend.material.color = swipeUpColor;
					break;
				case ImuGesture.SwipeDown:
					cubeRend.material.color = swipeDownColor;
					break;
				case ImuGesture.SwipeLeft:
					cubeRend.material.color = swipeLeftColor;
					break;
				case ImuGesture.SwipeRight:
					cubeRend.material.color = swipeRightColor;
					break;
			}
		}

		/// <summary>
		/// Meant to provide connection updates via the UI. Adjusts the image color between Green/Red and adjusts the provided text field
		/// Note: Connection status update is not currently functional in the current SDK
		/// </summary>
		/// <param name="connected"></param>
		private void PisonEvents_OnDeviceConnection(ConnectedDevice device, bool connected)
		{
			string deviceName = device == null ? " DISCONNECTION -- no device" : device.DeviceName;
			AdjustDeviceConnected(connected);
		}

		private void PisonEvents_OnDeviceState(DeviceState deviceState)
		{
			batteryLevelText.text = deviceState.Battery.ToString() + "%";
		}

		/// <summary>
		/// Adjusts the visualizations for device connection based on the passed in boolean
		/// </summary>
		/// <param name="connected"></param>
		private void AdjustDeviceConnected(bool connected)
        {
			string connectedStr = connected ? "Connected" : "Not Connected";
			Color connectedColor = connected ? Color.green : Color.red;

			connectionText.text = connectedStr;
			connectionImage.color = connectedColor;
		}

		/// <summary>
		/// Updates the last gesture text, resets the timer. 
		/// </summary>
		/// <param name="text"></param>
		private void UpdateLastGestureText(string text)
        {
			timerForGestureSec = timerResetAmountSecondsForGestureExpire;
			lastGestureText.text = text;
		}

		#endregion

		#region Unity Functions

		void Start()
		{
			pisonEvents = GameObject.FindObjectOfType<PisonEvents>();

			if(pisonEvents == null)
            {
				Debug.Log("No pison events, disabling cube example");
				this.enabled = false;
				return;
            }
			else
            {
                pisonEvents.OnGesture += PisonEvents_OnGesture;
                pisonEvents.OnOrientation += PisonEvents_OnOrientation;
                pisonEvents.OnActivation += PisonEvents_OnActivation;
                pisonEvents.OnDeviceConnection += PisonEvents_OnDeviceConnection;
                pisonEvents.OnDeviceState += PisonEvents_OnDeviceState;
            }
			pisonManager = GameObject.FindObjectOfType<PisonManager>();
			AdjustDeviceConnected(pisonManager.deviceConnected);
			batteryLevelText.text = pisonManager?.batteryLevel + "%";
			cubeRend = cubeToAdjust.GetComponentInChildren<Renderer>();
		}
		
        private void OnDestroy()
        {
			if (pisonEvents != null)
            {
				pisonEvents.OnGesture -= PisonEvents_OnGesture;
				pisonEvents.OnOrientation -= PisonEvents_OnOrientation;
				pisonEvents.OnActivation -= PisonEvents_OnActivation;
				pisonEvents.OnDeviceConnection -= PisonEvents_OnDeviceConnection;
				pisonEvents.OnDeviceState -= PisonEvents_OnDeviceState;
			}
        }

        void Update()
		{
			// Reset the gesture text after the provided amount of time
			if(lastGestureText.text != "")
            {
				timerForGestureSec -= Time.deltaTime;
				if(timerForGestureSec <= 0)
                {
					lastGestureText.text = "";
				}
			}
		}
		
		#endregion
	}
}
