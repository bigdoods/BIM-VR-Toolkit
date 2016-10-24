using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Immerseum {
    namespace VRSimulator {

        [CustomEditor(typeof(PlayAreaManager))]
        public class PlayAreaManagerEditor : Editor {
            SerializedProperty _inputMoveTarget;
            SerializedProperty _playAreaSize;
            SerializedProperty _customPlayAreaDimensions;
            SerializedProperty _playAreaDisplayTrigger;
            SerializedProperty _playAreaDisplayOnInputAction;
            SerializedProperty _playAreaDisplayOnApproachDistance;
            SerializedProperty _playAreaBorderThickness;
            SerializedProperty _playAreaColor;
            SerializedProperty _playAreaMaterial;

            bool _displayHelpBoxes = true;

            bool _isPlayAreaCalculated {
                get {
                    if (_playAreaDisplayTrigger.enumValueIndex != (int)PlayAreaDisplayTrigger.Never) {
                        return true;
                    }
                    return false;
                }
            }

            bool _isViveSupported {
                get {
                    SteamVR_PlayArea[] steamVR_PlayAreaArray = GameObject.FindObjectsOfType<SteamVR_PlayArea>();
                    if (steamVR_PlayAreaArray.Length > 0) {
                        return true;
                    }
                    return false;
                }
            }

            void OnEnable() {
                _inputMoveTarget = serializedObject.FindProperty("_inputMoveTarget");
                _playAreaSize = serializedObject.FindProperty("_playAreaSize");
                _customPlayAreaDimensions = serializedObject.FindProperty("_customPlayAreaDimensions");
                _playAreaDisplayTrigger = serializedObject.FindProperty("_playAreaDisplayTrigger");
                _playAreaDisplayOnInputAction = serializedObject.FindProperty("_playAreaDisplayOnInputAction");
                _playAreaDisplayOnApproachDistance = serializedObject.FindProperty("_playAreaDisplayOnApproachDistance");
                _playAreaBorderThickness = serializedObject.FindProperty("_playAreaBorderThickness");
                _playAreaColor = serializedObject.FindProperty("_playAreaColor");
                _playAreaMaterial = serializedObject.FindProperty("_playAreaMaterial");
            }

            void displayPlayAreaSizeWarning() {
                switch (_playAreaSize.enumValueIndex) {
                    case (int)PlayAreaSize._200x150:
                    case (int)PlayAreaSize._300x225:
                    case (int)PlayAreaSize._400x300:
                        break;
                    case (int)PlayAreaSize.Custom:
                        if (_isViveSupported) {
                            EditorGUILayout.HelpBox("BE CAREFUL! The HTC Vive does not support custom play area sizes. The behavior of your scene may be strange as a result.", MessageType.Error);
                        }
                        break;
                    case (int)PlayAreaSize.Calibrated:
                        EditorGUILayout.HelpBox("BE CAREFUL! Your Play Area will correspond to your calibration in the HTC Vive and the Oculus Rift. If no calibration is available, the Immerseum Play Area will default back to a 3m x 2.25m space.", MessageType.Warning);
                        break;
                    case (int)PlayAreaSize.NotApplicable:
                        EditorGUILayout.HelpBox("BE AWARE! When playing in an HMD, your Play Area will automatically default to the HMD's Calibrated Play Area Size if available.", MessageType.Warning);
                        break;
                }
            }

            void displayPlayAreaBehaviorSettings() {
                GUIContent playAreaDisplayTriggerLabel = new GUIContent("Display", "Determines when the Play Area should be displayed when playing.");
                EditorGUILayout.PropertyField(_playAreaDisplayTrigger, playAreaDisplayTriggerLabel);

                if (_playAreaDisplayTrigger.enumValueIndex == (int)PlayAreaDisplayTrigger.OnApproach) {
                    GUIContent playAreaDisplayOnApproachDistanceLabel = new GUIContent("Within Distance", "When the camera is within this value of the Play Area's bounds, the Play Area will be displayed.");
                    EditorGUILayout.PropertyField(_playAreaDisplayOnApproachDistance, playAreaDisplayOnApproachDistanceLabel);
                    if (_inputMoveTarget.enumValueIndex == (int)InputMoveTarget.CameraAndPlayArea) {
                        EditorGUILayout.HelpBox("BE CAREFUL! You have set the Play Area to display when approached, but movement input also moves the Play Area, which means the user's distance to the Play Area will be constant and your Play Area might never display. Please check your settings.", MessageType.Warning);
                    }
                } else if (_playAreaDisplayTrigger.enumValueIndex == (int)PlayAreaDisplayTrigger.OnInputAction) {
                    GUIContent playAreaDisplayOnInputActionLabel = new GUIContent("Input Action", "The name of the Input Action when the Play Area should be displayed.");
                    EditorGUILayout.PropertyField(_playAreaDisplayOnInputAction, playAreaDisplayOnInputActionLabel);
                }

                if (_isPlayAreaCalculated) {
                    EditorGUILayout.Space();

                    GUIContent inputMoveTargetLabel = new GUIContent("Input Moves", "Sets what gets moved when an input action is received.");
                    EditorGUILayout.PropertyField(_inputMoveTarget, inputMoveTargetLabel);
                    if (_inputMoveTarget.enumValueIndex == (int)InputMoveTarget.CameraOnly) {
                        EditorGUILayout.HelpBox("BE CAREFUL! The user will not be able to move beyond the Play Area's bounds unless you move the Play Area (camera rig).", MessageType.Warning);
                    } else if (_displayHelpBoxes) {
                        EditorGUILayout.HelpBox("This controls what actually moves in response to user input. If set to Camera Only, then the camera itself moves but the Play Area does not. If set to Camera and Play Area, then the entire Play Area moves with the camera centered within it.", MessageType.Info);
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }
            }

            void displayPlayAreaDesignSettings() {
                GUIContent playAreaSizeLabel = new GUIContent("Width/Depth", "Determines the width (X) and depth (Z) of the Play Area.");
                EditorGUILayout.PropertyField(_playAreaSize, playAreaSizeLabel);

                displayPlayAreaSizeWarning();

                if (_playAreaSize.enumValueIndex == (int)PlayAreaSize.Custom) {
                    GUIContent customPlayAreaDimensionsLabel = new GUIContent("Custom Dimensions", "Determines the width and depth of a custom play area.");
                    EditorGUILayout.PropertyField(_customPlayAreaDimensions, customPlayAreaDimensionsLabel);
                }

                EditorGUILayout.Space();

                GUIContent playAreaBorderThicknessLabel = new GUIContent("Border Thickness", "Determines the thickness of the border that is applied to the Play Area.");
                EditorGUILayout.PropertyField(_playAreaBorderThickness, playAreaBorderThicknessLabel);

                GUIContent playAreaColorLabel = new GUIContent("Color", "Determines the color to apply to the Play Area bounding box.");
                EditorGUILayout.PropertyField(_playAreaColor, playAreaColorLabel);

                GUIContent playAreaMaterialLabel = new GUIContent("Material", "Determines the material to apply to the Play Area bounding box.");
                EditorGUILayout.PropertyField(_playAreaMaterial, playAreaMaterialLabel);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }

            void displayPlayAreaSettings() {
                displayPlayAreaBehaviorSettings();

                if (_isPlayAreaCalculated) {
                    displayPlayAreaDesignSettings();
                }
            }

            public override void OnInspectorGUI() {
                serializedObject.Update();

                EditorGUIUtility.labelWidth = 160;

                HMDSimulatorEditor[] editors = (HMDSimulatorEditor[])Resources.FindObjectsOfTypeAll(typeof(HMDSimulatorEditor));
                if (editors.Length > 0) {
                    _displayHelpBoxes = editors[0].getDisplayHelpBoxes();
                }


                EditorGUILayout.HelpBox("Use the settings below to configure how the Immerseum Play Area appears and behaves in your VR scene.", MessageType.Info);

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                displayPlayAreaSettings();

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}