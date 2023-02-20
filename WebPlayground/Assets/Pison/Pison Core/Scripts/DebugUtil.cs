using PisonCore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.pison.util
{
	/// <summary>
	/// Allows for debugging pison device and ACTIVATION
	/// 
	/// Written by: Arthur Wollocko (arthur@pison.com) 2021
	public class DebugUtil : MonoBehaviour
	{
		// Public Variables
		public Image activationImage;
		public GameObject activationObject;
		public GameObject connectionObject;
		public TextMeshProUGUI debugText;

		// Private Variables
		private PisonManager pisonManager;
		
		#region Public API
		
		public void SetDebugText(string newText)
        {
			if(debugText != null)
            {
				debugText.text = newText;
			}
        }

		#endregion

		#region Private Helpers
		private void PisonEvents_OnImuAccelerationGyro(Vector3 newAcceleration, Vector3 newGyroscope)
		{
            //SetDebugText("Accel: " + newAcceleration);
        }

		private void PisonEvents_OnActivation(PisonCore.ActivationStates activation)
		{
			if(activationObject != null)
            {
				activationObject.SetActive(activation.Index == PisonCore.ActivationState.Hold);
			}
			if(activationImage != null)
            {
				activationImage.gameObject.SetActive(activation.Index == PisonCore.ActivationState.Hold);
			}
			
		}

		private void PisonEvents_OnDeviceConnection(ConnectedDevice device, bool connected)
		{
			if(connectionObject != null)
            {
				connectionObject.SetActive(connected);
            }
		}

		#endregion


		#region Unity Functions

		void Start()
		{
			pisonManager = GameObject.FindObjectOfType<PisonManager>();
            pisonManager.pisonEvents.OnActivation += PisonEvents_OnActivation;
            pisonManager.pisonEvents.OnImuAccelerationGyro += PisonEvents_OnImuAccelerationGyro;
            pisonManager.pisonEvents.OnDeviceConnection += PisonEvents_OnDeviceConnection;

			// Set initial connection
			PisonEvents_OnDeviceConnection(null, pisonManager.deviceConnected);

			activationImage.gameObject.SetActive(false);
		}

        private void OnDestroy()
        {
			pisonManager.pisonEvents.OnActivation -= PisonEvents_OnActivation;
			pisonManager.pisonEvents.OnImuAccelerationGyro -= PisonEvents_OnImuAccelerationGyro;
			pisonManager.pisonEvents.OnDeviceConnection -= PisonEvents_OnDeviceConnection;
		}

		
		#endregion
	}
}
