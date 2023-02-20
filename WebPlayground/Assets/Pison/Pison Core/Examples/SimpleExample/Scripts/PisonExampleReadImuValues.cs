using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.pison.examples
{
	/// <summary>
	/// Controls the UI for updating values from the Pison SDK
	/// This example shows real-time access to the variables directly from the PisonManager (rather than event driven via PisonEvents)
	/// 
	/// Written by: Arthur Wollocko (arthur@pison.com), 2020
	public class PisonExampleReadImuValues : MonoBehaviour
	{
		// Public Variables
		[Tooltip("The UI representations of various device states")]
		public Text orientationText, quaternionText, gyroText, accelText;

		// Private Variables
		private PisonManager pisonManager;
		
		#region Private Helpers
		
		/// <summary>
		/// Provides UI updates top the various text fields from the PisonManager. Called from Update()
		/// </summary>
		private void UpdateDeviceValues()
        {
			orientationText.text = pisonManager.currentDeviceEulerAngles.ToString();
			quaternionText.text = pisonManager.currentDeviceQuaternion.ToString();
			gyroText.text = pisonManager.currentDeviceGyro.ToString();
			accelText.text = pisonManager.currentDeviceAccelerometer.ToString();
		}
		
		#endregion
		
		#region Unity Functions
		
		void Start()
		{
			pisonManager = GameObject.FindObjectOfType<PisonManager>();
			if(pisonManager == null)
            {
				Debug.LogError("There is no PisonManager in the scene - disabling");
				this.enabled = false;
            }
		}

		void Update()
		{
			UpdateDeviceValues();
		}
		
		#endregion
	}
}
