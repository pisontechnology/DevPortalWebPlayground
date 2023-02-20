using PisonCore;
using PisonFrameParser.V0;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace com.pison
{
	/// <summary>
	/// Provides C# Delegate event-based functionality for Pison Device state changes and gestures.
	/// This Monobehaviour should exist on the same object as the PisonManager, and be bound to for receiving event messaging.
	/// 
	/// Events will be fired by the PisonManager when appropriate. Events will only fire if they have an active event listener
	/// 
	/// Written by: Brodey Lajoie (brodey@pison.com), 2022
	public class PisonEvents : MonoBehaviour
	{
		// Delegates/Events for Gesture, Orientation, and Activation
		public delegate void OnGestureDelegate(ImuGesture gesture);
		public event OnGestureDelegate OnGesture;
		public delegate void OnExtensionDeleagte(string gesture);
		public event OnExtensionDeleagte OnExtension;
		public delegate void OnShakeDelegate(string shakeGesture);
		public event OnShakeDelegate OnShake;
		public delegate void OnOrientationChangedDelegate(Vector3 newEulers);
		public event OnOrientationChangedDelegate OnOrientation;
		public delegate void OnQuaternionChangedDelegate(Quaternion newQuat);
		public event OnQuaternionChangedDelegate OnQuaternion;
		public delegate void OnImuChangedDelegate(Vector3 newAcceleration, Vector3 newGyroscope);
		public event OnImuChangedDelegate OnImuAccelerationGyro;
		public delegate void OnActivationDelegate(ActivationStates activation); 
		public event OnActivationDelegate OnActivation;

		// Device Connection/State Delegates/Events
		public delegate void OnDeviceStateDelegate(DeviceState deviceState);
		public event OnDeviceStateDelegate OnDeviceState;
		public delegate void OnDeviceConnectionDelegate(ConnectedDevice device, bool connected);
		public event OnDeviceConnectionDelegate OnDeviceConnection;
		public delegate void OnDeviceErrorDelegate(string error);
		public event OnDeviceErrorDelegate OnDeviceError;

		// Data Delegates/events
		public delegate void OnSensorDataDelegate(IEnumerable<V0DenormalizedPacket> packets);
		public event OnSensorDataDelegate OnSensorData;

		#region Event API
		// Public to allow for multitude of connections/alterations to this (e.g., emulator, network based, etc)
		// Note that events will ONLY fire if they have an active bind to them, meaning there is some class listening for the specific events.

		public void InvokeGestureEvent(ImuGesture gesture)
        {
			OnGesture?.Invoke(gesture); 
        }

		public void InvokeExtensionEvent(string gesture)
		{
			OnExtension?.Invoke(gesture);
		}
		
		public void InvokeShakeEvent(string shakeGesture)
		{
			OnShake?.Invoke(shakeGesture);
		}

		public void InvokeSensorDataEvent(IEnumerable<V0DenormalizedPacket> packets)
		{
			OnSensorData?.Invoke(packets);
		}

		public void InvokeOrientationEvent(Vector3 newEulers)
		{
			OnOrientation?.Invoke(newEulers); 
		}

		public void InvokeImuAccelerationGyroEvent(Vector3 newAcceleration, Vector3 newGyroscope)
		{
			OnImuAccelerationGyro?.Invoke(newAcceleration, newGyroscope); 
		}

		public void InvokeQuaternionEvent(Quaternion newQuat)
		{
			OnQuaternion?.Invoke(newQuat); 
		}

		public void InvokeActivationEvent(ActivationStates activationStates)
		{
			OnActivation?.Invoke(activationStates); 
		}

		public void InvokeDeviceStateEvent(DeviceState newState)
		{
			OnDeviceState?.Invoke(newState); 
		}

		public void InvokeDeviceConnectionEvent(ConnectedDevice device, bool isConnected)
		{
			OnDeviceConnection?.Invoke(device, isConnected); 
		}

		public void InvokeDeviceErrorEvent(string errorMessage)
		{
			OnDeviceError?.Invoke(errorMessage); 
		}
		#endregion
	}
}
