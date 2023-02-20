using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace com.pison.examples.ui
{
	/// <summary>
	/// Provides the high-level control for a single UI menu, based on button interactions
	/// Uses the onClick events of the buttons to enable cross-platform and non-pison device requirements.
	/// Highlights an individual button (and adjusts its scale), allowing selection between buttons. Index click gesture will then invoke th OnClick event via Unity's UI system
	/// 
	/// Note: Each button passed in has an action (customizable) bound to its onClick Unity event.
	/// 
	/// Written by: Arthur Wollocko (arthur@pison.com), 2020
	public class ExampleButtonMenuController : MonoBehaviour
	{
		// Public Variables
		[Tooltip("The buttons, in order from first to last, toggled between via ROLL")]
		public Button[] buttonsInMenu;
		[Tooltip("The amount to adjust the scale by for a FOCUSED button")]
		public float focusedScaleMultiplier = 1.2f;
		[Tooltip("The color to adjust the IMAGE on the button to be when focused")]
		public Color focusedColor = Color.green;

		[Tooltip("Adjust this to dictate what the smalled item in the list is (e.g., setting to 1 to allow a header button)")]
		public int smallestSelectableIndex = 0;

		// Private Variables
		public int currentlyFocusedButtonIndex = 0;
		private Button currentlyFocusedButton;
		private Color nonFocusedColor = Color.white;
		private Vector3 nonFocusedScale = Vector3.one;

		#region Public API

		/// <summary>
		/// Toggles the provided game object active/inactive
		/// </summary>
		/// <param name="active"></param>
		public void ToggleObjectActive(GameObject go)
        {
			go.SetActive(!go.activeSelf);
		}

		/// <summary>
		/// Calls focusbuttonatindex for the matching button (if a match exists)
		/// </summary>
		/// <param name="but"></param>
		public void FocusBasedOnButton(Button but)
		{
			if (buttonsInMenu.Contains(but))
			{
				FocusButtonAtIndex(Array.IndexOf(buttonsInMenu, but));
			}
		}

		/// <summary>
		/// Provides the selection focus visualizations on a button at the index
		/// </summary>
		/// <param name="index"></param>
		public void FocusButtonAtIndex(int index)
		{
			if (index < smallestSelectableIndex || index >= buttonsInMenu.Length)
			{
				Debug.Log("Index outside of button array bounds -- Returning without focus");
				return;
			}

			if (currentlyFocusedButton != null)
			{
				// Reset the currently focused to its normal color/scale
				currentlyFocusedButton.GetComponent<Image>().color = nonFocusedColor;
				currentlyFocusedButton.transform.localScale = nonFocusedScale;
			}

			// Set currently focused to new button, and adjust visuals
			currentlyFocusedButton = buttonsInMenu[index];
			currentlyFocusedButtonIndex = index;
			currentlyFocusedButton.GetComponent<Image>().color = focusedColor;
			currentlyFocusedButton.transform.localScale *= focusedScaleMultiplier;
		}

		/// <summary>
		/// Adjusts the visualizations (by invoking FocusButtonAtIndex) of focused button by offsetting the currently focused button index by parameter 'direction'
		/// This is useful if tying control of this interface to gestures/rolls of the device
		/// </summary>
		/// <param name="direction">Usually +/- 1, provided </param>
		public void AdjustFocusedButtonInDirection(int direction)
		{
			int newIndex = currentlyFocusedButtonIndex + direction;
			// Detect wrap
			if (newIndex < smallestSelectableIndex) { newIndex = buttonsInMenu.Length - 1; }
			if (newIndex >= buttonsInMenu.Length) { newIndex = smallestSelectableIndex; }

			FocusButtonAtIndex(newIndex);
		}

		/// <summary>
		/// Called (usually on gesture index select) when the focused button should be executed.
		/// Note: Executed via invoking its onclick behavior (to keep this abstract to Unity's UI implementations)
		/// </summary>
		public void SelectFocusedButton()
		{
			if (currentlyFocusedButton != null)
			{
				if (currentlyFocusedButton.onClick != null)
				{
					currentlyFocusedButton.onClick.Invoke();
				}
			}
		}

		#endregion

		#region Unity Functions

		void Start()
		{
			// Set initial values before accepting any
			if (buttonsInMenu.Length > 0)
			{
				nonFocusedScale = buttonsInMenu[0].transform.localScale;
				nonFocusedColor = buttonsInMenu[0].gameObject.GetComponent<Image>().color;
			}

			FocusButtonAtIndex(smallestSelectableIndex);
		}

		#endregion
	}
}
