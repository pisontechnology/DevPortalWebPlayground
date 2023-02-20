using Google.Protobuf;
using PisonCore;
using PisonDotNetCore;
using PisonFrameParser.V0;
using System;
using System.Collections.Generic;
using System.Text;
//// Editor support (Can use PisonUnitySdkDesktopUdp too)
#if UNITY_EDITOR
using PisonUnitySdkDesktop;
#endif
// Win/Mac/Linux Standalone
#if UNITY_STANDALONE && !UNITY_EDITOR
using PisonUnitySdkDesktopUdp;
#endif
// Hololens support
#if UNITY_WSA_10_0 && !UNITY_EDITOR
using PisonUnitySdkUwp;
#endif
// Android support
#if UNITY_ANDROID && !UNITY_EDITOR
using PisonUnitySdkAndroid;
#endif
using UnityEngine;

namespace com.pison
{
    /// <summary>
    /// Allows for specification of the connection type desired. 
    /// This will either allow for classified gestures from the PisonServer, or for RawDataStream
    /// </summary>
    [Serializable]
    public enum PisonDeviceConnectionType
    {
        GesturesAndConnection, RawDataAndConnection
    }

    /// <summary>
    /// REQUIRED for Pison Device Usage.
    /// Manages the connection to the Pison Server (typically running via HubApp on the Android Device).
    /// Binds to the PisonServer, and allows Gestures and device sensor updates to flow into the Unity Environment.
    /// 
    /// Implements IPisonListener, which defines callbacks for device sensor values and Gestures from underlying Pison DLLs
    /// Allows raw access to all values within this class. For Event-based access, see <see cref="PisonEvents"/>
    /// 
    /// ** Please ensure HubApp is running on the device pointed to by the hubappServerAddress, or if using discovery, that a device on the local network is using HubApp
    /// with device activated **
    /// 
    /// Note: This utilizes Monobehavior's DontDestroyOnLoad to persist this connection between scenes. Please ensure only one PisonManager exists in your environment through all scenes!
    /// Note2: Device discovery (use directed!) is currently non operational on Windows standalone platforms - Being fixed ASAP
    /// 
    /// Written by: Brodey Lajoie (brodey@pison.com), 2022
    public class PisonManager : MonoBehaviour, IPisonListener, IPisonConnectionsListener, IPisonSensorDataListener
	{
        // Public Variables
        [Header("Device Connection")]
        [Tooltip("If enabled, will NOT use the directed IP/port below, but will instead attempt to discover and then bind to FIRST discovered HubApp Pison Server Instance")]
        public bool useAutoDiscoveryBindFirst = false;
        [Tooltip("For directed connections, the IP address of the device running HubApp (the Pison Server). For local connections (running on same device) - use 127.0.0.1")]
		public string hubappServerAddress = "127.0.0.1";
        public PisonDeviceConnectionType pisonConnectionType = PisonDeviceConnectionType.GesturesAndConnection;
        [Header("Device Status")]
        public bool deviceConnected = false;
        public int batteryLevel = 1;

        [Header("Device Orientation")]
        public Vector3 currentDeviceEulerAngles = Vector3.zero;         // The current angles of the device, relative to its starting point. (Pitch, Roll, Yaw). *** Note: YAW acts as a compass, pointing north at 0 degrees.
        public Vector4 currentDeviceQuaternion = Vector3.zero;          // Quaternion representation of the device
        public Vector3 currentDeviceGyro = Vector3.zero;                // Gyroscope measurements from the device. 
        public Vector3 currentDeviceAccelerometer = Vector3.zero;       // Instantaneous accelerometer values, typically in the range [-9.8 - 9.8] 

        [Header("Device Activations")]
        public bool indexActivated = false;         // Corresponds to activating (raising with intent) the index finger
        public bool watchCheck = false;             // Corresponds to a watch-check activation, which is the natural motion of "looking at your watch"

        [Header("Sensor Data (If enabled)")]
        public IEnumerable<V0DenormalizedPacket> currentDeviceSensorData; // The sensor data last received from the device
        public Dictionary<string, string> gestures;  //Dictionary to parse gesture events for incoming frame tags
        public Dictionary<string, string> shakeGestures;  //Dictionary to parse shake gesture events for incoming frame tags
        
        // Private Variables
        private int port = 8002;                    // The port on which to bind to the Hubapp Service. By default, this is set to 8002. Private to avoid unintentional adjustments
        private IPisonClient pisonClient;           // Allows for binding and discovery to the HubApp Pison Server
        private bool isBoundToStream = false;       // Indicates if we are currently bound to the PisonServer stream
        public PisonEvents pisonEvents;            // (Optional) Delegate event based system for binding to from Unity application. 

        #region Haptic API

        /// <summary>
        /// Allows the developer to send one or many haptic 'bursts' to the user, which last durationMS.
        /// This should be called by discrete actions/events, in a non-continuous format
        /// </summary>
        /// <param name="durationMs">milliseconds describing how long the burst will last</param>
        /// <param name="numBursts">The number of bursts in sequence</param>
        /// <param name="intensity">How strong the haptic burst will be, from [0-1000]</param>
        public void SendHapticBursts(int durationMs, int numBursts = 1, int intensity = 100)
        {
            pisonClient.SendHapticBursts(durationMs, numBursts, intensity);
        }

        
        /// <summary>
        /// Allows the developer to send the ON command, creating continuous haptic feedback for the user
        /// until the OFF command is set or an amount of time determined by the Server Application
        /// </summary>
        public void SendHapticOn()
        {
            pisonClient.SendHapticOn();
        }

        /// <summary>
        /// Allows the user to send the OFF command, disabling the continuous haptic feedback
        /// </summary>
        public void SendHapticOff()
        {
            pisonClient.SendHapticOff();
        }

        #endregion

        #region IPisonListener Implementation

        
        // TODO: Implement features of class
        public void FullDeviceUpdated(FullDeviceUpdate deviceUpdate)
        {
            
        }

        /// <summary>
        /// Callback for quaternion updates
        /// </summary>
        /// <param name="quaternion">the new Quat from the device</param>
        public void QuaternionUpdated(Vector4 quat)
        {
            currentDeviceQuaternion = quat;
            pisonEvents?.InvokeQuaternionEvent(new UnityEngine.Quaternion(quat.x,quat.y,quat.z,quat.w ));
        }

        /// <summary>
        /// Callback whenever the euler angles update is issued from the Pison SDK
        /// </summary>
        /// <param name="eulerAngles">the new EulerAngles from the device. Note: eulerAngles.y doubles as a compass, with 0 pointing to true north</param>
        public void EulerAnglesUpdated(Vector3 eulerAngles)
        { 
            currentDeviceEulerAngles = eulerAngles;
            pisonEvents?.InvokeOrientationEvent(eulerAngles);
        }

        /// <summary>
        /// Callback whenever the IMU adjustments are made, sending new accel and gyro data from the Pison SDK
        /// </summary>
        /// <param name="accelerometer">Vector3 new frame accelerometer data</param>
        /// <param name="gyroscope">Vector3 bew frane gyro data</param>
        public void ImuChanged(Vector3 accelerometer, Vector3 gyroscope)
        {
            currentDeviceAccelerometer = accelerometer;
            currentDeviceGyro = gyroscope;
            pisonEvents?.InvokeImuAccelerationGyroEvent(accelerometer, gyroscope);
        }


        /// <summary>
        /// Basic included Gestures within the Pison SDK for the GAMBIT device. See <see cref="ImuGesture"/> for list of Gestures
        /// </summary>
        /// <param name="gesture"></param>
        public void GesturePerformed(ImuGesture gesture)
        {
            pisonEvents?.InvokeGestureEvent(gesture);
        }

        /// <summary>
        /// Basic included Gestures in the Pison SDK for the Vulcan device. Tags are filtered through the
        /// gestures dictionary for developer QoL. See <see cref="gestures"/> for available Vulcan gestures
        /// </summary>
        /// <param name="tags"> The complete tagged frame</param>
        public void FrameTagsUpdated(string tags)
        {
            if (tags.Contains("SHAKE"))
                pisonEvents?.InvokeShakeEvent(shakeGestures[tags]);
            else
                pisonEvents?.InvokeExtensionEvent(gestures[tags]);
        }

        public void HandPositionChanged(HandPosition handPos)
        {
            Debug.Log(handPos);
        }

        /// <summary>
        /// Contains information about the ActivationState of the device. 
        /// Currently functioning states include Index, and Watch (** Thumb is not currently supported).
        /// </summary>
        /// <param name="activationStates">The new activationStates, including information on Index and Wrist activation.</param>
        public void ActivationStateChanged(ActivationStates activationStates)
        {
            indexActivated = activationStates.Index == ActivationState.Hold;
            watchCheck = activationStates.WatchCheck == ActivationState.Hold;
            pisonEvents?.InvokeActivationEvent(activationStates);
        }

        /// <summary>
        /// Adjustments to the state of the device will be passed via this callback. See <see cref="DeviceState"/> for list of Device States
        /// </summary>
        /// <param name="deviceState">The new device state, containing information like Battery level</param>
        public void DeviceStateChanged(DeviceState deviceState)
        {
            batteryLevel = deviceState.Battery;
            pisonEvents?.InvokeDeviceStateEvent(deviceState);
        }

        public void LockStateChanged(DeviceLockState deviceLockState)
        {
            Debug.Log(deviceLockState);
        }

        /// <summary>
        /// Callback providing the error message of the device whenever an error is encountered
        /// </summary>
        /// <param name="error"></param>
        public void OnError(string error)
        {
            Debug.Log("Pison Device Error: " + error);
            pisonEvents?.InvokeDeviceErrorEvent(error);
        }

        #endregion

        #region IPisonConnection Interface

        public void DeviceConnected(ConnectedDevice device)
        {
            Debug.Log("Device: " + device.DeviceName + "_" + device.DeviceIdentifier + "   Connection status: CONNECTED");
            deviceConnected = true;
            pisonEvents.InvokeDeviceConnectionEvent(device, true);

            if(!isBoundToStream)
            {
                isBoundToStream = true;

                if(pisonConnectionType == PisonDeviceConnectionType.RawDataAndConnection)
                {
                    pisonClient.BindToSensorStream(device.DeviceIdentifier, this);
                }
                else
                {
                     pisonClient.BindToPisonServer(this);
                }
                
            }
        }

        public void ScanningStopped()
        {
            pisonEvents.InvokeDeviceErrorEvent("Scanning has stopped");
        }
        public void DeviceDisconnected()
        {
            deviceConnected = false;
            pisonEvents.InvokeDeviceConnectionEvent(null, false);
        }

        #endregion

        #region IPisonSensorDataListener interface

        /// <summary>
        /// Packets of data flow from the GAMBIT device.
        /// Note: ADC raw data is a value proportional tothe voltage detected
        /// </summary>
        /// <param name="packets"></param>
        public void SamplesAvailable(IEnumerable<V0DenormalizedPacket> packets)
        {
            pisonEvents.InvokeSensorDataEvent(packets);
        }

        #endregion

        #region Unity Functions

        void Start()
		{
            //Initialize gesture dict
            gestures = new Dictionary<string, string>(){
                {"[ \"null\" ]", "NULL"},
                {"[ \"DEBOUNCE_LDA_INACTIVE\" ]", "INACTIVE"},
                {"[ \"DEBOUNCE_LDA_NAC\" ]", "NAC"},
                {"[ \"DEBOUNCE_LDA_REST\" ]", "REST"},
                {"[ \"DEBOUNCE_LDA_INEH\" ]", "INDEX"},
                {"[ \"DEBOUNCE_LDA_TEH\" ]", "THUMB"},
                {"[ \"DEBOUNCE_LDA_FHEH\" ]", "HAND"},
                {"[ \"INEH_SWIPE_RIGHT\" ]", "INDEX_SWIPE_RIGHT"},
                {"[ \"INEH_SWIPE_LEFT\" ]", "INDEX_SWIPE_LEFT"},
                {"[ \"INEH_SWIPE_UP\" ]", "INDEX_SWIPE_UP"},
                {"[ \"INEH_SWIPE_DOWN\" ]", "INDEX_SWIPE_DOWN"},
                {"[ \"TEH_SWIPE_RIGHT\" ]", "THUMB_SWIPE_RIGHT"},
                {"[ \"TEH_SWIPE_LEFT\" ]", "THUMB_SWIPE_LEFT"},
                {"[ \"TEH_SWIPE_UP\" ]", "THUMB_SWIPE_UP"},
                {"[ \"TEH_SWIPE_DOWN\" ]", "THUMB_SWIPE_DOWN"},
                {"[ \"FHEH_SWIPE_RIGHT\" ]", "HAND_SWIPE_RIGHT"},
                {"[ \"FHEH_SWIPE_LEFT\" ]", "HAND_SWIPE_LEFT"},
                {"[ \"FHEH_SWIPE_UP\" ]", "HAND_SWIPE_UP"},
                {"[ \"FHEH_SWIPE_DOWN\" ]", "HAND_SWIPE_DOWN"}
            };
            
            shakeGestures = new Dictionary<string, string>()
            {
                {"[ \"SHAKE\" ]", "SHAKE"},
                {"[ \"SHAKE_N_INEH\" ]", "SHAKE_N_INDEX"},
                {"[ \"SHAKE_N_TEH\" ]", "SHAKE_N_THUMB"},
                {"[ \"SHAKE_N_FHEH\" ]", "SHAKE_N_HAND"}
            };
            
            // Ensure the PisonManager persists through scenes, allowing the connection to the HubApp Server instance to remain open.
            DontDestroyOnLoad(this.gameObject);
            // Discover PisonEvents if they exist within this environment
            pisonEvents = GetComponent<PisonEvents>();

            // Establish connection to Pison device
            PisonSdk sdk = new PisonSdk();
            if (useAutoDiscoveryBindFirst)
            {
                sdk.BindToFirstDiscoveredServer(client =>
                {
                    Debug.Log($"Discovered client! {client.GetIdentifier()}");
                    pisonClient = client;
                    pisonClient.BindToConnectionStream(this);
                });
            }
            else
            {
                pisonClient = sdk.newPisonClient(hubappServerAddress, "Hubapp Pison Server", port);
                pisonClient.BindToConnectionStream(this);
            }
        }

        void Update()
        {
            if(pisonClient != null)
            {
                pisonClient?.Update();
            }
           

        }

        void OnDestroy()
        {
            pisonClient?.Dispose();
        }

        #endregion
    }
}
