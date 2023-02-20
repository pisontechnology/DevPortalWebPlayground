using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.pison.examples
{
	/// <summary>
	/// Allows for loading other scenes in the environment
	/// 
	/// Written by: Arthur Wollocko (arthur@pison.com), 2020
	public class ExampleSceneNavigator : MonoBehaviour
	{
		
		#region Public API
		
		public void LoadSceneByIndex(int index)
        {
			SceneManager.LoadScene(index);
        }
		
		#endregion
		
	}
}
