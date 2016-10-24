
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Immerseum {
    namespace VRSimulator {

        [CustomEditor(typeof(MovementManager))]
        public class MovementManagerEditor : Editor {
            SerializedProperty _useDefaultInputActions;
            SerializedProperty _isStrafeEnabled;
            SerializedProperty _primaryGamepadTrigger;
            SerializedProperty _keyboardRotationRatchet;
            SerializedProperty _accelerationRate;
            SerializedProperty _isRunEnabled;
            SerializedProperty _isRunActive;

            SerializedProperty _forwardDampingRate;
            SerializedProperty _backAndSideDampingRate;
            SerializedProperty _gravityAdjustment;

            bool _displayHelpBoxes = true;

            bool _showAdvancedSettings = false;

            void OnEnable() {
                _useDefaultInputActions = serializedObject.FindProperty("_useDefaultInputActions");
                _isStrafeEnabled = serializedObject.FindProperty("_isStrafeEnabled");
                _primaryGamepadTrigger = serializedObject.FindProperty("_primaryGamepadTrigger");
                _keyboardRotationRatchet = serializedObject.FindProperty("_keyboardRotationRatchet");
                _accelerationRate = serializedObject.FindProperty("_accelerationRate");
                _isRunEnabled = serializedObject.FindProperty("_isRunEnabled");
                _isRunActive = serializedObject.FindProperty("_isRunActive");
                _forwardDampingRate = serializedObject.FindProperty("_forwardDampingRate");
                _backAndSideDampingRate = serializedObject.FindProperty("_backAndSideDampingRate");
                _gravityAdjustment = serializedObject.FindProperty("_gravityAdjustment");

            }

            void displayAdvancedSettings() {
                EditorGUILayout.HelpBox("The settings in this box are advanced settings that have a significant impact on user experience. Use with caution!", MessageType.Warning);

                EditorGUILayout.Space();

                GUIContent keyboardRotationRatchetLabel = new GUIContent("Rotation Ratchet", "Determines the degrees by which the gamepad bumpers / keyboard rotation rotate the player's avatar.");
                EditorGUILayout.PropertyField(_keyboardRotationRatchet, keyboardRotationRatchetLabel);

                GUIContent accelerationRateLabel = new GUIContent("Acceleration Rate", "Determines the rate (m / s) at which the player/avatar accelerates when moving.");
                EditorGUILayout.PropertyField(_accelerationRate, accelerationRateLabel);

                GUIContent forwardDampingRateLabel = new GUIContent("Forward Damping Rate", "Determines the rate by which forward movement is dampened.");
                EditorGUILayout.PropertyField(_forwardDampingRate, forwardDampingRateLabel);

                GUIContent backAndSideDampingRateLabel = new GUIContent("Back/Side Damping Rate", "Determines the rate by which movement backwards or to the side is dampened.");
                EditorGUILayout.PropertyField(_backAndSideDampingRate, backAndSideDampingRateLabel);

                GUIContent gravityAdjustmentLabel = new GUIContent("Gravity Adjustment", "An adjustment made to the strength of gravity to produce more natural movement.");
                EditorGUILayout.PropertyField(_gravityAdjustment, gravityAdjustmentLabel);
            }


            public override void OnInspectorGUI() {
                serializedObject.Update();

                EditorGUIUtility.labelWidth = 160;

                HMDSimulatorEditor[] editors = (HMDSimulatorEditor[])Resources.FindObjectsOfTypeAll(typeof(HMDSimulatorEditor));
                if (editors.Length > 0) {
                    _displayHelpBoxes = editors[0].getDisplayHelpBoxes();
                }


                EditorGUILayout.HelpBox("Use the settings below to configure how the avatar/player moves in your VR scene.", MessageType.Info);

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                GUIContent useImmerseumDefaultInputActionsLabel = new GUIContent("Default Input Map", "If true, interprets input from your devices according to Immerseum's default input mappings. Set this to false to handle input actions using custom logic (requires scripting).");
                EditorGUILayout.PropertyField(_useDefaultInputActions, useImmerseumDefaultInputActionsLabel);
                if (_useDefaultInputActions.boolValue == false) {
                    EditorGUILayout.HelpBox("You have chosen to supply your own input mapping. This is an advanced technique, and one which does require scripting. For more information, please view the Immerseum SDK documentation.", MessageType.Warning);
                } else if (_displayHelpBoxes == true) {
                    EditorGUILayout.HelpBox("Enable this setting to use Immerseum's default Input Action mapping. Disable it to supply your own logic for handling Input Actions.", MessageType.Info);
                }

                EditorGUILayout.Space();

                GUIContent isStrafeEnabledLabel = new GUIContent("Is Strafe Enabled", "If true, the x-axis on your input devices moves the player/avatar laterally. If false, the x-axis rotates your player/avatar laterally (yaw).");
                EditorGUILayout.PropertyField(_isStrafeEnabled, isStrafeEnabledLabel);

                GUIContent isRunEnabledLabel = new GUIContent("Is Run Enabled", "If true, the player/avatar can run (move at twice their normal speed).");
                EditorGUILayout.PropertyField(_isRunEnabled, isRunEnabledLabel);

                if (_isRunEnabled.boolValue == true) {
                    GUIContent isRunActiveLabel = new GUIContent("Is Run Active", "If true, the player/avatar is currently running (moving at twice their normal speed).");
                    EditorGUILayout.PropertyField(_isRunActive, isRunActiveLabel);
                }

                EditorGUILayout.Space();

                GUIContent primaryGamepadTriggerLabel = new GUIContent("Gamepad Primary Trigger", "Determines which trigger on your gamepad is used as the primary trigger.");
                EditorGUILayout.PropertyField(_primaryGamepadTrigger, primaryGamepadTriggerLabel);

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                GUIContent advancedSettingsLabel = new GUIContent("Advanced Settings", "Expand to adjust advanced settings that may have adverse effects on your user experience.");
                _showAdvancedSettings = Foldout(_showAdvancedSettings, advancedSettingsLabel, true, EditorStyles.foldout);
                if (_showAdvancedSettings) {
                    displayAdvancedSettings();
                }

                EditorGUILayout.Space();

                serializedObject.ApplyModifiedProperties();
            }

            public static bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick, GUIStyle style) {
                Rect position = GUILayoutUtility.GetRect(40f, 40f, 16f, 16f, style);
                // EditorGUI.kNumberW == 40f but is internal
                return EditorGUI.Foldout(position, foldout, content, toggleOnLabelClick, style);
            }
            public static bool Foldout(bool foldout, string content, bool toggleOnLabelClick, GUIStyle style) {
                return Foldout(foldout, new GUIContent(content), toggleOnLabelClick, style);
            }
        }
    }
}
