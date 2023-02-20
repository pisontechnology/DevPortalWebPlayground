using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.pison.examples.swipe
{
	/// <summary>
	/// Controls the mini swipe game. 
	/// Will alternate through different swipe requests by moving the ExampleSwipeGoalArea around the environment, to one of the four positions.
	/// 
	/// 
	/// Written by: Arthur Wollocko (arthur@pison.com), 2020
	public class ExampleSwipeGameManager : MonoBehaviour
	{
		// Public Variables
		[Tooltip("The time delay (sec) between the successfully swiping in the right direction and the next swipe")]
		public float delayTimeBetweenSwipesSec = .75f;
		[Tooltip("The transform positions for moves in each of the directions. 0 = up, 1 = down, 2 = left, 3 = right")]
		public Transform[] moveTransforms;
		[Tooltip("the text area (UI) for instructions to show up")]
		public Text instructionText;
		public ExampleSwipeGoalArea swipeGoalArea;
		// Private Variables

		private string[] swipeInstructionTextArray = { "Swipe Up!", "Swipe Down!", "Swipe Left!", "Swipe Right!" }; // Textual feedback to the user for each swipe direction

		#region Public API

		/// <summary>
		/// Called by the ExampleSwipeGoalArea script when the player enters the goal area.
		/// Deactivates the goal area, and then begins the countdown to move the goal to a new position
		/// </summary>
		public void PlayerEnteredGoal()
        {
			//instructionText.text = "";
			swipeGoalArea.gameObject.SetActive(false);
			Invoke("AdjustGoalToNewPosition", delayTimeBetweenSwipesSec);
		}
		
		
		#endregion
		
		#region Private Helpers

		/// <summary>
		/// Randomly places the goal to one of the four provided positions
		/// </summary>
		private void AdjustGoalToNewPosition()
        {
			CancelInvoke(); // In case there are multiple invokes occuring to this call
			int index = Random.Range(0, moveTransforms.Length);
			Vector3 newGoalPos = moveTransforms[index].position;
			//instructionText.text = swipeInstructionTextArray[index];
			swipeGoalArea.transform.position = newGoalPos;
			swipeGoalArea.gameObject.SetActive(true);
		}
		
		#endregion
		
		#region Unity Functions
		
		void Start()
		{
			PlayerEnteredGoal();
		}

		#endregion
	}
}
