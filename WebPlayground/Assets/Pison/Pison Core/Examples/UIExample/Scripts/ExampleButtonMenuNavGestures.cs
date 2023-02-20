using PisonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.pison.examples.ui
{
	/// <summary>
	/// 
	/// 
	/// Written by: Arthur Wollocko (arthur@pison.com), 2020
	public class ExampleButtonMenuNavGestures : MonoBehaviour
	{
		// Public Variables
		public bool shouldPlayMenuAudio = true;
		public AudioClip menuNavClip, menuSelectClip;
		// Private Variables
		private ExampleButtonMenuController buttonMenuController;
		private Transform mainCamTrans;

		#region Private Helpers
		private PisonEvents pisonEvents;

		/// <summary>
		/// Plays the provided clip at the camera position, if one is provided
		/// </summary>
		/// <param name="clip"></param>
		private void PlaySound(AudioClip clip)
		{
			if (shouldPlayMenuAudio && clip != null)
			{
				AudioSource.PlayClipAtPoint(clip, mainCamTrans.position);
			}
		}


		#region Pison Impl

		private void PisonEvents_OnGesture(ImuGesture gesture)
		{
			// Only care about clicks, for focused selection
			if (gesture == ImuGesture.IndexClick)
			{
				PlaySound(menuSelectClip);
				buttonMenuController.SelectFocusedButton();
			}

			if (gesture == ImuGesture.RollRight)
			{
				PlaySound(menuNavClip);
				buttonMenuController.AdjustFocusedButtonInDirection(1);
			}
			if (gesture == ImuGesture.RollLeft)
			{
				PlaySound(menuNavClip);
				buttonMenuController.AdjustFocusedButtonInDirection(-1);
			}

		}

		#endregion


		#endregion


		#region Unity Functions

		void Start()
		{
			if (mainCamTrans == null)
			{
				mainCamTrans = Camera.main.transform;
			}

			buttonMenuController = GetComponent<ExampleButtonMenuController>();
			pisonEvents = GameObject.FindObjectOfType<PisonEvents>();

			pisonEvents.OnGesture += PisonEvents_OnGesture;
		}

       

        private void OnDestroy()
		{
			pisonEvents.OnGesture -= PisonEvents_OnGesture;
		}

		#endregion
	}
}
