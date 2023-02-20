using PisonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.pison.examples
{
	/// <summary>
	/// Scene used to initialize the PisonManager as the first scene loaded, then will have no functionality. Basically, a forwarding method to another scene.
	/// 
	/// Written by: Arthur Wollocko (arthur@pison.com), 2020
	public class LoadingScene : MonoBehaviour
	{
		public PisonManager pisonManager;

		private void PisonEvents_OnDeviceConnection(ConnectedDevice device, bool connected)
		{
			if(connected)
            {
				SceneManager.LoadScene(1);
			}
		}

		#region Unity Functions

		void Start()
		{
			if (pisonManager.useAutoDiscoveryBindFirst)
			{
				pisonManager.pisonEvents.OnDeviceConnection += PisonEvents_OnDeviceConnection;
			}
			else
			{
				SceneManager.LoadScene(1);
			}
		}

        private void OnDestroy()
        {
			pisonManager.pisonEvents.OnDeviceConnection -= PisonEvents_OnDeviceConnection;
		}

        #endregion
    }
}
