using PisonCore;
using UnityEditor;
using UnityEngine;

namespace com.pison.emulator
{
	/// <summary>
	/// Controls the in-editor GUI window and script representation for the PisonEmulator.cs monobehavior
    /// Allows for control of a currently running application by manipulating the PisonManager with values that could be achieved from the Pison device
	/// 
	/// Written by: Brodey Lajoie (brodey@pison.com), 2022
	[CustomEditor(typeof(PisonEmulator))]
	public class PisonEmulatorEditor : Editor
	{
        // Private class level variables
        private float buttonWidth = 100f;
        private float buttonHeight = 40f;
        private GUIStyle horizontalLine;
        private float rightPadding = 12f;
        private bool showGyro = false;
        private bool showAccel = false;
        private bool showQuaternion = false;
        private bool isIndexHeld = false;
        private int toolbarInt = 0;

        // Device data
        private Vector3 deviceOrientation = Vector3.zero;
        private Vector3 deviceGyro = Vector3.zero;
        private Vector3 deviceAccel = Vector3.zero;
        private Vector4 deviceQuaternion = Vector4.zero;

        // Target data
        private PisonEmulator pisonEmulator;

        #region Editor Functions
        private void OnEnable()
        {
            horizontalLine = new GUIStyle();
            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            horizontalLine.margin = new RectOffset(0, 0, 4, 4);
            horizontalLine.fixedHeight = 1;
        }

        public override void OnInspectorGUI()
		{
            // Calculate button width
            buttonWidth = Screen.width / 3 - rightPadding;

            pisonEmulator = (PisonEmulator)target;

            string[] labels = { "Gambit", "Vulcan" };
            GUILayout.BeginHorizontal();
            toolbarInt = GUILayout.Toolbar(toolbarInt, labels);

            if (toolbarInt >= 0)
            {
                switch (labels[toolbarInt])
                {
                    case "Gambit":
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.BeginVertical();

                        DrawDefaultInspector();
                        BuildGestureRegionGambit();
                        HorizontalLine(Color.grey);
                        BuildImuRegion();
                        HorizontalLine(Color.grey);
                        BuildAdvancedRegion();

                        GUILayout.EndVertical();
                        break;
                    case "Vulcan":
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.BeginVertical();

                        DrawDefaultInspector();
                        BuildGestureRegionVulcan();
                        HorizontalLine(Color.grey);
                        BuildImuRegion();
                        HorizontalLine(Color.grey);
                        BuildAdvancedRegion();

                        GUILayout.EndVertical();
                        break;
                    default:
                        break;
                }
            }
        }


        /// <summary>
        /// utility method to draw a horizontal line across the entire width of the editor window
        /// </summary>
        /// <param name="color">the color of the line, visually</param>
        private void HorizontalLine(Color color)
        {
            var c = GUI.color;
            GUI.color = color;
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = c;
        }

        #endregion

        #region Private GUI Building Methods

        private void BuildGestureRegionGambit()
        {
            EditorGUILayout.LabelField("Gestures", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("The buttons and sliders below will update the PisonManager in real time, and also invoke the associated events. \n" +
                "This emulator is meant to function at runtime, for testing purposes only.", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Roll Left", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.GestureMade(ImuGesture.RollLeft);
            }
            if (GUILayout.Button("Swipe Up", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.GestureMade(ImuGesture.SwipeUp);
            }
            if (GUILayout.Button("Roll Right", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.GestureMade(ImuGesture.RollRight);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Swipe Left", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.GestureMade(ImuGesture.SwipeLeft);
            }
            if (GUILayout.Button("Index Click", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.GestureMade(ImuGesture.IndexClick);
            }
            if (GUILayout.Button("Swipe Right", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.GestureMade(ImuGesture.SwipeRight);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Button("", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight));                     // Placeholder button

            if (GUILayout.Button("Swipe Down", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.GestureMade(ImuGesture.SwipeDown);
            }

            GUILayout.Button("", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight));                     // Placeholder button

            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();

            // Index Activation Toggle
            GUILayout.BeginHorizontal();
            // Move it over to the CENTER
            GUILayout.Space(Screen.width / 2 - 75f);
            EditorGUI.BeginChangeCheck();
            isIndexHeld = GUILayout.Toggle(isIndexHeld, "Index Activation", GUILayout.Width(3 * buttonWidth + 6f), GUILayout.Height(buttonHeight / 2));
            if (EditorGUI.EndChangeCheck())
            {
                ActivationState isHeld = isIndexHeld ? ActivationState.Hold : ActivationState.None;
                ActivationStates state = new ActivationStates();
                state.Index = isHeld;
                pisonEmulator.ActivationMade(state);
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Hold", GUILayout.Width(3 * buttonWidth + 6f), GUILayout.Height(buttonHeight / 2)))
            {
                pisonEmulator.GestureMade(ImuGesture.IndexHold);
            }

            GUILayout.EndVertical();
        }
        private void BuildGestureRegionVulcan()
        {
            EditorGUILayout.LabelField("Gestures", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("The buttons and sliders below will update the PisonManager in real time, and also invoke the associated events. \n" +
                "This emulator is meant to function at runtime, for testing purposes only.", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Roll Left", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.GestureMade(ImuGesture.RollLeft);
            }
            if (GUILayout.Button("Swipe Up", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.GestureMade(ImuGesture.SwipeUp);
            }
            if (GUILayout.Button("Roll Right", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.GestureMade(ImuGesture.RollRight);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Swipe Left", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.GestureMade(ImuGesture.SwipeLeft);
            }
            if (GUILayout.Button("Index \nExtension", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.ExtensionMade("INDEX");
            }
            if (GUILayout.Button("Swipe Right", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.GestureMade(ImuGesture.SwipeRight);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            //GUILayout.Button("", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight));                     // Placeholder button

            if (GUILayout.Button("Thumb \nExtension", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.ExtensionMade("THUMB");
            }

            if (GUILayout.Button("Swipe Down", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.GestureMade(ImuGesture.SwipeDown);
            }
            //GUILayout.Button("", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight));                     // Placeholder button

            if (GUILayout.Button("Hand \nExtension", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.ExtensionMade("HAND");
            }

            GUILayout.EndHorizontal();
            
            EditorGUILayout.LabelField("Shake Gestures", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Shake", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.ShakeGestureMade("SHAKE");
            }
            if (GUILayout.Button("Shake Hand", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.ShakeGestureMade("SHAKE_FHEH");
            }
            if (GUILayout.Button("Shake Thumb", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                pisonEmulator.ShakeGestureMade("SHAKE_TEH");
            }
            
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();

            // Index Activation Toggle
            GUILayout.BeginHorizontal();
            // Move it over to the CENTER
            GUILayout.Space(Screen.width / 2 - 75f);
            EditorGUI.BeginChangeCheck();
            isIndexHeld = GUILayout.Toggle(isIndexHeld, "Index Activation", GUILayout.Width(3 * buttonWidth + 6f), GUILayout.Height(buttonHeight / 2));
            if (EditorGUI.EndChangeCheck())
            {
                ActivationState isHeld = isIndexHeld ? ActivationState.Hold : ActivationState.None;
                ActivationStates state = new ActivationStates();
                state.Index = isHeld;
                pisonEmulator.ActivationMade(state);
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Hold", GUILayout.Width(3 * buttonWidth + 6f), GUILayout.Height(buttonHeight / 2)))
            {
                pisonEmulator.GestureMade(ImuGesture.IndexHold);
            }

            GUILayout.EndVertical();
        }

        private void HapticRegion()
        {
            
        }

        private void BuildImuRegion()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Device Orientation", EditorStyles.boldLabel);
            if (GUILayout.Button("Reset Orientation"))
            {
                deviceOrientation = Vector3.zero;
                deviceAccel = Vector3.zero;
                deviceGyro = Vector3.zero;
            }
            GUILayout.EndHorizontal();


            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Device X");
            deviceOrientation.x = EditorGUILayout.Slider(deviceOrientation.x, -180, 180);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Device Y");
            deviceOrientation.y = EditorGUILayout.Slider(deviceOrientation.y, -180, 180);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Device Z");
            deviceOrientation.z = EditorGUILayout.Slider(deviceOrientation.z, -180, 180);
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                pisonEmulator.OrientationChanged(deviceOrientation);
            }

            GUILayout.Space(30f);
        }

        private void BuildAdvancedRegion()
        {
            EditorGUILayout.LabelField("Advanced Device Options", EditorStyles.boldLabel);

            // Gyro
            showGyro = EditorGUILayout.Foldout(showGyro, "Device Gyro");
            if (showGyro)
            {
                EditorGUI.BeginChangeCheck();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Gyro X");
                deviceGyro.x = EditorGUILayout.Slider(deviceGyro.x, -1000, 1000);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Gyro Y");
                deviceGyro.y = EditorGUILayout.Slider(deviceGyro.y, -1000, 1000);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Gyro Z");
                deviceGyro.z = EditorGUILayout.Slider(deviceGyro.z, -1000, 1000);
                GUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    pisonEmulator.AccelerometerGyroscopeChanged(deviceAccel, deviceGyro);
                }
            }

            // Accelerometer

            showAccel = EditorGUILayout.Foldout(showAccel, "Device Acceleration");
            if (showAccel)
            {
                EditorGUI.BeginChangeCheck();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Acceleration X");
                deviceAccel.x = EditorGUILayout.Slider(deviceAccel.x, -10, 10);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Acceleration Y");
                deviceAccel.y = EditorGUILayout.Slider(deviceAccel.y, -10, 10);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Acceleration Z");
                deviceAccel.z = EditorGUILayout.Slider(deviceAccel.z, -10, 10);
                GUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    pisonEmulator.AccelerometerGyroscopeChanged(deviceAccel, deviceGyro);
                }
            }

            // Quaternion
            showQuaternion = EditorGUILayout.Foldout(showQuaternion, "Device Quaternion");
            if (showQuaternion)
            {
                EditorGUI.BeginChangeCheck();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Quaternion X");
                deviceQuaternion.x = EditorGUILayout.Slider(deviceQuaternion.x, -180, 180);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Quaternion Y");
                deviceQuaternion.y = EditorGUILayout.Slider(deviceQuaternion.y, -180, 180);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Quaternion Z");
                deviceQuaternion.z = EditorGUILayout.Slider(deviceQuaternion.z, -180, 180);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Quaternion W");
                deviceQuaternion.w = EditorGUILayout.Slider(deviceQuaternion.w, -180, 180);
                GUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    pisonEmulator.QuaternionChanged(deviceQuaternion);
                }
            }
        }

        #endregion
    }
}
