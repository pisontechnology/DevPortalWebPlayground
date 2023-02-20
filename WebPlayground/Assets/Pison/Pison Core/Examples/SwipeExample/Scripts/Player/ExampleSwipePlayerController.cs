using PisonCore;
using System.Collections;
using UnityEngine;

namespace com.pison.examples.swipe
{
	/// <summary>
	/// Provides a player controller for swiping to move in the direction indicated
	/// 
	/// Written by: Arthur Wollocko (arthur@pison.com), 2020; Updated by Joseph Tecce (joe.tecce@pison.com), 2022
	public class ExampleSwipePlayerController : MonoBehaviour
	{
		// Public Variables
		[Tooltip("The amount of time to move the player to a new spot after a gesture")]
		public float moveTimeSec = .5f;
		//[Tooltip("the game object that indicates index activation. Default this is set to the Pison Logo")]
		//public GameObject activationIndicator;
		[Tooltip("Reference to the swipe game manager. Required due to move positions being referenced in that script")]
		public ExampleSwipeGameManager swipeGameManager;

		// Private Variables
		private PisonEvents pisonEvents;
		private Vector3 cachedStartPos;
		private bool isMoving = false;      // If the player is actively moving, reset to false when movement has stopped. Gestures will not be considered while actively moving.

		// Joseph Tecce Update
		public bool indexChosen;
		public bool thumbChosen;
		public bool handChosen;

		private PisonManager _pisonManager;

		#region Public API

		/// <summary>
		/// Function that allows coroutine-based movement of an object over the specified time
		/// </summary>
		/// <param name="destinationPoint"></param>
		/// <param name="moveTimeSeconds"></param>
		/// <returns></returns>
		public IEnumerator MoveOverSeconds(Vector3 destinationPoint, float moveTimeSeconds, bool shouldReturnAfterDestination)
		{
			float elapsedTimeSeconds = 0;
			Vector3 startingPos = transform.position;
			while (elapsedTimeSeconds < moveTimeSeconds)
			{
				transform.position = Vector3.Lerp(startingPos, destinationPoint, (elapsedTimeSeconds / moveTimeSeconds));
				elapsedTimeSeconds += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			transform.position = destinationPoint;
			if(shouldReturnAfterDestination)
            {
				// Move it back after reaching the end
				StartCoroutine(MoveOverSeconds(cachedStartPos, moveTimeSec, false));
			}
			else
            {
				isMoving = false;
			}
		}

		/// <summary>
		/// Moves player to the destination using a coroutine to lerp to the destination position.
		/// 
		/// </summary>
		/// <param name="newPos"></param>
		/// <param name="shouldReturnAfterDestination">If provided, will automatically return to the starting position</param>
		public void MovePlayer(Vector3 newPos, bool shouldReturnAfterDestination)
        {
			isMoving = true;
			StartCoroutine(MoveOverSeconds(newPos, moveTimeSec, shouldReturnAfterDestination));
        }

		#endregion

		#region Private Helpers

		private void OnExtension(string gesture)
		{
			// Only register swipes if the player is NOT currently moving
			if(isMoving)
            {
				return;
            }
			//Debug.Log(gesture);

            // Reminder, swipeGameManager.moveTransforms[x] where x == (0 = up, 1 = down, 2 = left, 3 = right)
            if (indexChosen)
            {
                switch (gesture)
                {
					case "INDEX_SWIPE_UP":
						MovePlayer(swipeGameManager.moveTransforms[0].position, true);
						//_pisonManager.SendHapticBursts(200, 1, 300);
						break;
					case "INDEX_SWIPE_DOWN":
						MovePlayer(swipeGameManager.moveTransforms[1].position, true);
						//_pisonManager.SendHapticBursts(200, 1, 300);
						break;
					case "INDEX_SWIPE_LEFT":
						MovePlayer(swipeGameManager.moveTransforms[2].position, true);
						//_pisonManager.SendHapticBursts(200, 1, 300);
						break;
					case "INDEX_SWIPE_RIGHT":
						MovePlayer(swipeGameManager.moveTransforms[3].position, true);
						//_pisonManager.SendHapticBursts(200, 1, 300);
						break;
				}
            }
			else if (thumbChosen)
            {
				switch (gesture)
				{
					case "THUMB_SWIPE_UP":
						MovePlayer(swipeGameManager.moveTransforms[0].position, true);
						//_pisonManager.SendHapticBursts(200, 1, 200);
						break;
					case "THUMB_SWIPE_DOWN":
						MovePlayer(swipeGameManager.moveTransforms[1].position, true);
						//_pisonManager.SendHapticBursts(200, 1, 200);
						break;
					case "THUMB_SWIPE_LEFT":
						MovePlayer(swipeGameManager.moveTransforms[2].position, true);
						//_pisonManager.SendHapticBursts(200, 1, 200);
						break;
					case "THUMB_SWIPE_RIGHT":
						MovePlayer(swipeGameManager.moveTransforms[3].position, true);
						//_pisonManager.SendHapticBursts(200, 1, 200);
						break;
				}
			}
			else if (handChosen)
            {
				switch (gesture)
				{
					case "HAND_SWIPE_UP":
						MovePlayer(swipeGameManager.moveTransforms[0].position, true);
						//_pisonManager.SendHapticBursts(200, 1, 200);
						break;
					case "HAND_SWIPE_DOWN":
						MovePlayer(swipeGameManager.moveTransforms[1].position, true);
						//_pisonManager.SendHapticBursts(200, 1, 200);
						break;
					case "HAND_SWIPE_LEFT":
						MovePlayer(swipeGameManager.moveTransforms[2].position, true);
						//_pisonManager.SendHapticBursts(200, 1, 200);
						break;
					case "HAND_SWIPE_RIGHT":
						MovePlayer(swipeGameManager.moveTransforms[3].position, true);
						//_pisonManager.SendHapticBursts(200, 1, 200);
						break;
				}
			}
		}

		/// <summary>
		/// Simple interaction - bind the activity of the activationIndicator to the index activation
		/// </summary>
		/// <param name="activation"></param>
		private void PisonEvents_OnActivation(ActivationStates activation)
		{
			bool isActivated = activation.Index == ActivationState.Hold;
			//activationIndicator?.SetActive(isActivated);
		}

		#endregion


		#region Unity Functions

		void Start()
		{
			cachedStartPos = transform.position;
			//activationIndicator.SetActive(false);

			_pisonManager = GameObject.FindObjectOfType<PisonManager>();

			// Handle events objects and event bindings
			pisonEvents = GameObject.FindObjectOfType<PisonEvents>();
			if(pisonEvents == null)
            {
				Debug.Log("No pison event listener, disabling player controller.");
				this.enabled = false;
				return;
            }

            pisonEvents.OnExtension += OnExtension;
            pisonEvents.OnActivation += PisonEvents_OnActivation;
		}

        private void OnDestroy()
        {
			pisonEvents.OnExtension -= OnExtension;
			pisonEvents.OnActivation -= PisonEvents_OnActivation; 
		}

		#endregion
	}
}
