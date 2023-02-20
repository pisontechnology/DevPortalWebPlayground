using PisonCore;
using UnityEngine;

namespace com.pison.emulator
{
	/// <summary>
	/// Keyboard AND in-editor GUI (See PisonEmulatorEditor.cs) implementation of the Pison Emulator.
	/// This will allow for quick simulations of interactions, movements, and Activations
	/// 
	/// Requires a PisonManager in order to function (optionally, a PisonEvents)
	/// 
	/// Written by: Brodey Lajoie (brodey@pison.com), 2022
	[RequireComponent(typeof(PisonManager))]
	public class PisonEmulator : MonoBehaviour
	{
		// Public Variables
		[Tooltip("If the keyboard bindings (provided in this file) should be executed")]
		public bool keyboardShortcuts = true;

		// Private Variables
		private PisonManager pisonManager;
		private PisonEvents pisonEvents;

		#region Public API -- Callbacks from EditorGUI adjustments (and Keyboard inputs)

		public void OrientationChanged(Vector3 newEulers)
        {
			pisonManager.currentDeviceEulerAngles = newEulers;
			pisonEvents?.InvokeOrientationEvent(newEulers);
		}

		public void QuaternionChanged(Vector4 newQuat)
		{
			pisonManager.currentDeviceQuaternion = newQuat;
			pisonEvents?.InvokeQuaternionEvent(new UnityEngine.Quaternion(newQuat.x,newQuat.y,newQuat.z,newQuat.w ));
		}

		public void AccelerometerGyroscopeChanged(Vector3 newAccel, Vector3 newGyro)
		{
			pisonManager.currentDeviceAccelerometer = newAccel;
			pisonManager.currentDeviceGyro = newGyro;
			pisonEvents?.InvokeImuAccelerationGyroEvent(newAccel, newGyro);
		}

		public void GestureMade(ImuGesture gesture)
		{
			pisonEvents?.InvokeGestureEvent(gesture);
		}

		public void ExtensionMade(string gesture)
        {
			pisonEvents?.InvokeExtensionEvent(gesture);
        }

		public void ShakeGestureMade(string gesture)
		{
			pisonEvents?.InvokeShakeEvent(gesture);
		}

		public void ActivationMade(ActivationStates activation)
		{
			pisonManager.indexActivated = activation.Index == ActivationState.Hold;
			pisonEvents?.InvokeActivationEvent(activation);
		}

		#endregion

		#region Unity Functions

        void Start()
		{
			pisonManager = GetComponent<PisonManager>();
			pisonEvents = GetComponent<PisonEvents>();
		}

        void Update()
		{
			// Provide simulation via keyboard if enabled
			if(keyboardShortcuts)
            {
				if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					pisonEvents.InvokeGestureEvent(ImuGesture.SwipeUp);
				}
				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					pisonEvents.InvokeGestureEvent(ImuGesture.SwipeDown);
				}
				if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
					pisonEvents.InvokeGestureEvent(ImuGesture.SwipeLeft);
				}
				if (Input.GetKeyDown(KeyCode.RightArrow))
				{
					pisonEvents.InvokeGestureEvent(ImuGesture.SwipeRight);
				}
				if (Input.GetKeyDown(KeyCode.Alpha1))
				{
					pisonEvents.InvokeGestureEvent(ImuGesture.RollLeft);
				}
				if (Input.GetKeyDown(KeyCode.Alpha2))
				{
					pisonEvents.InvokeGestureEvent(ImuGesture.RollRight);
				}
				if (Input.GetKeyDown(KeyCode.LeftControl))
				{
					pisonEvents.InvokeGestureEvent(ImuGesture.IndexClick);
				}
				if (Input.GetKeyDown(KeyCode.LeftShift))
				{
					pisonEvents.InvokeGestureEvent(ImuGesture.IndexHold);
				}
			}
		}
#endregion
	}
}
