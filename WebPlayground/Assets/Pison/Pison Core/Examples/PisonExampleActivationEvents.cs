using PisonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.pison.examples
{
	/// <summary>
	/// A simple example for how to bind Unity-events for in-editor binding to Pison events
	/// 
	/// Written by: Arthur Wollocko (arthur@pison.com), 2020
	public class PisonExampleActivationEvents : MonoBehaviour
	{
		// Public Variables
		public UnityEvent onIndexActivation, onWatchCheck;

		// Private Variables
		private PisonEvents pisonEvents;

		#region Pison Events

		private void PisonEvents_OnActivation(PisonCore.ActivationStates activation)
		{
			bool indexActivated = (activation.Index == ActivationState.Hold) ;
			bool watchCheck = (activation.WatchCheck == ActivationState.Hold);

			if(indexActivated)
            {
				onIndexActivation?.Invoke();
			}

			if(watchCheck)
            {
				onWatchCheck?.Invoke();
            }
		}

		#endregion

		#region Unity Functions

		void Start()
		{
			pisonEvents = GameObject.FindObjectOfType<PisonEvents>();
			if(pisonEvents == null)
            {
				Debug.Log("No pison events, disabling");
				this.enabled = false;
				return;
            }

            pisonEvents.OnActivation += PisonEvents_OnActivation;

		}

        private void OnDestroy()
        {
			pisonEvents.OnActivation -= PisonEvents_OnActivation;
		}

		#endregion
	}
}
