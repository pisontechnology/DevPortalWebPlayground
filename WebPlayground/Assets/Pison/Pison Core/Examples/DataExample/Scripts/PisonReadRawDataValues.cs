using PisonFrameParser.V0;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.pison.examples
{
	/// <summary>
	/// Provides an implementation of reading the raw data values coming from the GAMBIT device.
	/// In this case, adjusts the position of two game objects based on the channel data
	/// 
	/// Written by: Arthur Wollocko (arthur@pison.com), 2020
	public class PisonReadRawDataValues : MonoBehaviour
	{
		// Public Variables
		public Image connectionImage;
		public Text connectedText, dataText;
		public PisonManager pisonManagerDataCollection;

		// Private Variables


		#region Public API


		#endregion

		#region Private Helpers

		/// <summary>
		/// Adjusts the color and text of the connection UI elements
		/// </summary>
		/// <param name="connected"></param>
		private void AdjustDeviceConnected(bool connected)
		{
			string connectedStr = connected ? "Connected" : "Not Connected";
			connectedText.text = connectedStr;
			Color connectedColor = connected ? Color.green : Color.red;

			connectionImage.color = connectedColor;
		}


		private void PisonEvents_OnDeviceConnection(PisonCore.ConnectedDevice device, bool connected)
		{
			AdjustDeviceConnected(connected);
		}

		private void PisonEvents_OnSensorData(IEnumerable<PisonFrameParser.V0.V0DenormalizedPacket> packets)
		{
			// Iterate through the provided packets, getting the timestamp and ADC data
			foreach(V0DenormalizedPacket packet in packets)
            {
				double[] adcRaw = packet.Contents.Adc.AdcRaw;
				double timestamp = packet.TimeStampMs;
				string combined = "Time: " + timestamp + "   ADCRaw Data: " + adcRaw[0] + "," + adcRaw[1];
				dataText.text = combined;

			}
		}


		#endregion


		#region Unity Functions

		void Start()
		{
			// bind to connection and device data events
			pisonManagerDataCollection.pisonEvents.OnDeviceConnection += PisonEvents_OnDeviceConnection;
			pisonManagerDataCollection.pisonEvents.OnSensorData += PisonEvents_OnSensorData;
		}

        private void OnDestroy()
        {
			pisonManagerDataCollection.pisonEvents.OnSensorData -= PisonEvents_OnSensorData;
			pisonManagerDataCollection.pisonEvents.OnDeviceConnection -= PisonEvents_OnDeviceConnection;

			// This is unique -- Destroying the Extra PisonManager that we are created specifically for data binding.
			// Each pison manager is directed at either gesture or raw data -- this removes the raw data one from the environment (re-loaded next scene load)
			Destroy(pisonManagerDataCollection.gameObject);
		}

		#endregion
	}
}
