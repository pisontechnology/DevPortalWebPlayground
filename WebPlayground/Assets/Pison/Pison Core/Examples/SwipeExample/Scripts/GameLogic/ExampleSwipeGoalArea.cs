using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.pison.examples.swipe
{
	/// <summary>
	/// Controls the 'goal area' for the example exercise. 
	/// Allows for it to be moved to a new location, and detects collisions with the player
	/// 
	/// Written by: Arthur Wollocko (arthur@pison.com), 2020
	public class ExampleSwipeGoalArea : MonoBehaviour
	{
		// Public Variables
		public ExampleSwipeGameManager swipeGameManager;

        #region Unity Functions

        private void OnTriggerEnter(Collider other)
        {
			ExampleSwipePlayerController playerController = other.gameObject.GetComponent<ExampleSwipePlayerController>();
			if(playerController != null)
            {
				// Collision with a player trigger - notify the ExampleSwipeGameManager
				swipeGameManager?.PlayerEnteredGoal();
			}
			
        }

		#endregion
	}
}
