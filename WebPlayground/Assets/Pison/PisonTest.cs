using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.pison
{
	/// <summary>
	/// 
	/// 
	/// Written by: Brodey Lajoie (blajoie95@gmail.com), 2020
	public class PisonTest : MonoBehaviour
	{
		// Public Variables
		public Transform capsule;
		
		// Private Variables
		private PisonEvents pisonEvents;
		private PisonManager pisonManager;
		
		#region Public API
		
		
		
		#endregion
		
		#region Private Helpers
		
		
		#endregion
		
		
		#region Unity Functions
		
		void Start()
		{
			pisonEvents = FindObjectOfType<PisonEvents>();
			pisonManager = FindObjectOfType<PisonManager>();
			
			pisonEvents.OnExtension += OnExtension;
			pisonEvents.OnOrientation += OnOrientation;
			pisonEvents.OnQuaternion += OnQuaternion;
		}
		
		void Update()
		{
			
		}

		private void OnQuaternion(Quaternion newquat)
		{
			capsule.rotation = newquat;
		}

		private void OnOrientation(Vector3 neweulers)
		{
			
		}

		private bool debounce;
		private void OnExtension(string gesture)
		{
			if (gesture == "REST")
				debounce = false;
			   
			Debug.Log(gesture);
			if (!debounce)
			{
				switch (gesture)
				{
					case "INDEX":
						pisonManager.SendHapticBursts(500,1, 700);
						debounce = true;
						break;
					case "THUMB":
						pisonManager.SendHapticOn();
						debounce = true;
						break;
					case "HAND":
						pisonManager.SendHapticOff();
						debounce = true;
						break;
				}	
			}
		}

		#endregion
	}
}
