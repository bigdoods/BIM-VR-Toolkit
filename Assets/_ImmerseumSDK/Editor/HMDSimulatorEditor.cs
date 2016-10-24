using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Immerseum {
    namespace VRSimulator {

        [CustomEditor(typeof(HMDSimulator))]
        public class HMDSimulatorEditor : Editor {
            SerializedProperty _displayHelpBoxes;
            SerializedProperty _disableInStandaloneBuild;
            SerializedProperty _logDiagnostics;
            SerializedProperty _logInputActions;
            SerializedProperty _simulateControllers;
            SerializedProperty _controllerPrimitive;
            SerializedProperty _primitiveScaling;
            SerializedProperty _leftControllerPrefab;
            SerializedProperty _rightControllerPrefab;
            SerializedProperty _cameraRig;
            SerializedProperty _cameraInitializationTimeout;
            SerializedProperty _heightTarget;
            SerializedProperty _customHeadHeight;

            void OnEnable() {
                _displayHelpBoxes = serializedObject.FindProperty("_displayHelpBoxes");
                _disableInStandaloneBuild = serializedObject.FindProperty("_disableInStandaloneBuild");
                _logDiagnostics = serializedObject.FindProperty("_logDiagnostics");
                _logInputActions = serializedObject.FindProperty("_logInputActions");
                _simulateControllers = serializedObject.FindProperty("_simulateControllers");
                _controllerPrimitive = serializedObject.FindProperty("_controllerPrimitive");
                _primitiveScaling = serializedObject.FindProperty("_primitiveScaling");
                _leftControllerPrefab = serializedObject.FindProperty("_leftControllerPrefab");
                _rightControllerPrefab = serializedObject.FindProperty("_rightControllerPrefab");
                _cameraRig = serializedObject.FindProperty("_CameraRig");
                _cameraInitializationTimeout = serializedObject.FindProperty("_cameraInitializationTimeout");
                _heightTarget = serializedObject.FindProperty("_heightTarget");
                _customHeadHeight = serializedObject.FindProperty("_customHeadHeight");
            }

            public bool getSimulateControllers() {
                return _simulateControllers.boolValue;
            }

            public bool getDisplayHelpBoxes() {
                return _displayHelpBoxes.boolValue;
            }

            public int getControllerPrimitive() {
                return _controllerPrimitive.enumValueIndex;
            }

            public override void OnInspectorGUI () {
                serializedObject.Update();

                EditorGUILayout.HelpBox("Use the settings below to configure how you want to simumate a VR experience.", MessageType.Info);

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                GUIContent disableInStandaloneBuildLabel = new GUIContent("Disable in Standalone", "If true, disables the VR Simulator when running outside of the Unity Editor.");
                EditorGUILayout.PropertyField(_disableInStandaloneBuild, disableInStandaloneBuildLabel);

                EditorGUILayout.Space();

                GUIContent displayHelpBoxesLabel = new GUIContent("Display Help Boxes", "If true, displays help boxes in the Unity Inspector that explain how to configure the VR Simulator.");
                EditorGUILayout.PropertyField(_displayHelpBoxes, displayHelpBoxesLabel);

                EditorGUILayout.Space();

                GUIContent logDiagnosticsLabel = new GUIContent("Log Diagnostics", "If true, logs diagnostic information to the console.");
                EditorGUILayout.PropertyField(_logDiagnostics, logDiagnosticsLabel);

                GUIContent logInputActionsLabel = new GUIContent("Log Input Actions", "If true, logs information to the console whenever an input is detected from any of the configured Input Actions.");
                EditorGUILayout.PropertyField(_logInputActions, logInputActionsLabel);

                EditorGUILayout.Space();

                GUIContent cameraRigLabel = new GUIContent("Camera Rig", "The GameObject that you intend to use as your camera rig within your VR scene. If using the SteamVR Plugin, must be [CameraRig]. If using the Oculus Rift OVR plugin, must be OVRCameraRig.");
                EditorGUILayout.PropertyField(_cameraRig, cameraRigLabel);
                if (_cameraRig.objectReferenceValue != null) {
                    if (_cameraRig.objectReferenceValue.name != "[CameraRig]" && _cameraRig.objectReferenceValue.name != "OVRCameraRig" && _cameraRig.objectReferenceValue.name != "OVRPlayerController") {
                        EditorGUILayout.HelpBox("ERROR. Must either be a SteamVR:[CameraRig] object or an Oculus:OVRCameraRig object.", MessageType.Error);
                    } else if (_cameraRig.objectReferenceValue.name == "OVRPlayerController") {
                        EditorGUILayout.HelpBox("WARNING. The Oculus:OVRPlayerController is not the same thing as the Oculus:OVRCameraRig. It does, however, contain the Oculus:OVRCameraRig. The VRSimulator will automatically switch to the OVRPlayerController's OVRCameraRig when entering play mode.", MessageType.Warning);
                    }
                } else if (_displayHelpBoxes.boolValue == true) {
                    EditorGUILayout.HelpBox("OPTIONAL. If not specified, will default to the SteamVR [CameraRig] if present in the scene. Otherwise, will default to Oculus Rift OVRCameraRig if present in the scene. If neither is present, will produce an error.", MessageType.Warning);
                }

                EditorGUILayout.Space();

                GUIContent cameraInitializationTimeoutLabel = new GUIContent("HMD Discovery Time", "Determines the number of seconds to try finding a connected HMD.");
                EditorGUILayout.PropertyField(_cameraInitializationTimeout, cameraInitializationTimeoutLabel);

                EditorGUILayout.Space();

                GUIContent heightTargetLabel = new GUIContent("Height", "Select the height / position that you wish the camera to simulate.");
                EditorGUILayout.PropertyField(_heightTarget, heightTargetLabel);
                if (_heightTarget.enumValueIndex == (int)HeightTargets.Custom) {
                    GUIContent customHeadHeightLabel = new GUIContent("Head Height", "Indicate the head height that you wish the camera to simulate, expressed in meters from the ground. Average human eye-level is 1.65.");
                    EditorGUILayout.PropertyField(_customHeadHeight, customHeadHeightLabel);
                }
                if (_displayHelpBoxes.boolValue == true) {
                    EditorGUILayout.HelpBox("In a VR scene, this height is typically determined by your position-tracked HMD, your user profile settings, and/or your character controller. When simulating VR, the height needs to be explicitly determined.", MessageType.Info);
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                GUIContent simulateControllersLabel = new GUIContent("Simulate Controllers", "If true, displays objects to simulate position-tracked controllers.");
                EditorGUILayout.PropertyField(_simulateControllers, simulateControllersLabel);
                if (_simulateControllers.boolValue == true) {
                    GUIContent controllerPrimitiveLabel = new GUIContent("Controller Primitive", "Select which primitive to use for your simulated controllers.");
                    EditorGUILayout.PropertyField(_controllerPrimitive, controllerPrimitiveLabel);

                    if (_controllerPrimitive.enumValueIndex != (int)PrimitiveType.Custom) {
                        GUIContent primitiveScalingLabel = new GUIContent("Controller Scaling", "Set the scale at which the simulated controllers will be displayed, relative to the standard 1m x 1m x 1m scale.");
                        EditorGUILayout.PropertyField(_primitiveScaling, primitiveScalingLabel);
                    } else {
                        GUIContent leftControllerPrefabLabel = new GUIContent("Left Controller Prefab", "Select the prefab you wish to use to simulate your left-hand controller.");
                        EditorGUILayout.PropertyField(_leftControllerPrefab, leftControllerPrefabLabel);
                        GUIContent rightControllerPrefabLabel = new GUIContent("Right Controller Prefab", "Select the prefab you wish to use to simulate your right-hand controller.");
                        EditorGUILayout.PropertyField(_rightControllerPrefab, rightControllerPrefabLabel);
                    }
                }

                EditorGUILayout.Space();

                serializedObject.ApplyModifiedProperties();
            }            
        }
    }
}
