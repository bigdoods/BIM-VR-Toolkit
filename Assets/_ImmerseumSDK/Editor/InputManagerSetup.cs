using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {

        public class InputManagerSetup {

            public static SerializedObject serializedTemplate;
            public static SerializedObject serializedInputManager;

            public enum AxisType {
                KeyOrMouseButton = 0,
                MouseMovement = 1,
                JoystickAxis = 2
            }

            public class InputManagerAxis {
                public string name;
                public string descriptiveName;
                public string descriptiveNegativeName;
                public string negativeButton;
                public string positiveButton;
                public string altNegativeButton;
                public string altPositiveButton;

                public float gravity;
                public float dead;
                public float sensitivity;

                public bool snap = false;
                public bool invert = false;

                public AxisType type;

                public int axis;
                public int joyNum;
            }

            private static void clearAxisDefinitions() {
                SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
                SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");
                axesProperty.ClearArray();
                serializedObject.ApplyModifiedProperties();
            }

            private static SerializedProperty getChildProperty(SerializedProperty parent, string name) {
                SerializedProperty child = parent.Copy();
                child.Next(true);
                do {
                    if (child.name == name) {
                        return child;
                    }
                }
                while (child.Next(false));
                return null;
            }

            private static bool isAxisDefined(string axisName) {
                serializedInputManager = loadSerializedInputManager();
                SerializedProperty axesProperty = serializedInputManager.FindProperty("m_Axes");

                axesProperty.Next(true);
                axesProperty.Next(true);
                while (axesProperty.Next(false)) {
                    SerializedProperty axis = axesProperty.Copy();
                    axis.Next(true);
                    if (axis.stringValue == axisName) {
                        return true;
                    }
                }
                return false;
            }

            private static SerializedProperty getAxis(string axisName) {
                if (isAxisDefined(axisName) == false) {
                    return null;
                }
                SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
                SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

                axesProperty.Next(true);
                axesProperty.Next(true);
                while (axesProperty.Next(false)) {
                    SerializedProperty axis = axesProperty.Copy();
                    axis.Next(true);
                    if (axis.stringValue == axisName) {
                        return axis;
                    }
                }
                return null;
            }

            private static SerializedObject loadSerializedTemplate(string filePath) {
                string[] guidArray = AssetDatabase.FindAssets(filePath);
                if (guidArray.Length == 0) {
                    serializedTemplate = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(filePath)[0]);
                }
                return serializedTemplate;
            }

            private static SerializedObject loadSerializedInputManager() {
                string[] guidArray = AssetDatabase.FindAssets("ProjectSettings/InputManager.asset");
                if (guidArray.Length == 0) {
                    serializedInputManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
                }
                return serializedInputManager;
            }

            private static void SetupFromTemplate() {
                serializedTemplate = loadSerializedTemplate("Assets/_ImmerseumSDK/Resources/InputManagerTemplate.dat");
                SerializedProperty templateAxesProperty = serializedTemplate.FindProperty("m_Axes");

                serializedInputManager = loadSerializedInputManager();
                SerializedProperty axesProperty = serializedInputManager.FindProperty("m_Axes");

                axesProperty.arraySize = templateAxesProperty.arraySize;

                serializedInputManager.CopyFromSerializedProperty(templateAxesProperty);

                serializedInputManager.ApplyModifiedProperties();
            }

            private static void AddAxis(InputManagerAxis axis) {
                if (isAxisDefined(axis.name)) {
                    return;
                }


                SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
                SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

                axesProperty.arraySize++;
                serializedObject.ApplyModifiedProperties();

                SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

                getChildProperty(axisProperty, "m_Name").stringValue = axis.name;
                getChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
                getChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
                getChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
                getChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
                getChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
                getChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
                getChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
                getChildProperty(axisProperty, "dead").floatValue = axis.dead;
                getChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
                getChildProperty(axisProperty, "snap").boolValue = axis.snap;
                getChildProperty(axisProperty, "invert").boolValue = axis.invert;
                getChildProperty(axisProperty, "type").intValue = (int)axis.type;
                getChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
                getChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

                serializedObject.ApplyModifiedProperties();
            }

            public static void setupInputManager() {
                if (areImmerseumAxesDefined == false) {
                    SetupFromTemplate();
                } else {
                    if (HMDSimulator.logDiagnostics) {
                        Debug.Log("[ImmerseumSDK] Immerseum InputManager settings are already defined.");
                    }
                }
            }

            static bool shouldSetupImmerseumInputAxes() {
                return EditorUtility.DisplayDialog("Use Immerseum Input Axes", "Should the VR Simulator apply Immerseum's default input axes? Saying yes will over-write your project's Input Manager configuration.", "Yes, use Immerseum's Input Axes.", "No, leave them as-is.");
            }

            public static bool areImmerseumAxesDefined {
                get {
                    List<string> axisNameList = new List<string>();
                    axisNameList.Add("Horizontal");
                    axisNameList.Add("Vertical");
                    axisNameList.Add("Fire1");
                    axisNameList.Add("Fire2");
                    axisNameList.Add("Fire3");
                    axisNameList.Add("Jump");
                    axisNameList.Add("Mouse X");
                    axisNameList.Add("Mouse Y");
                    axisNameList.Add("Mouse ScrollWheel");
                    axisNameList.Add("Horizontal");
                    axisNameList.Add("Vertical");
                    axisNameList.Add("Fire1");
                    axisNameList.Add("Fire2");
                    axisNameList.Add("Fire3");
                    axisNameList.Add("Jump");
                    axisNameList.Add("Submit");
                    axisNameList.Add("Submit");
                    axisNameList.Add("Cancel");
                    axisNameList.Add("Oculus_GearVR_LThumbstickX");
                    axisNameList.Add("Oculus_GearVR_LThumbstickY");
                    axisNameList.Add("Oculus_GearVR_RThumbstickX");
                    axisNameList.Add("Oculus_GearVR_RThumbstickY");
                    axisNameList.Add("Oculus_GearVR_DpadX");
                    axisNameList.Add("Oculus_GearVR_DpadY");
                    axisNameList.Add("Oculus_GearVR_LIndexTrigger");
                    axisNameList.Add("Oculus_GearVR_RIndexTrigger");

                    axisNameList.Add("Gamepad_RightTrigger_Windows");
                    axisNameList.Add("Gamepad_LeftTrigger_Windows");
                    axisNameList.Add("Gamepad_RThumbstickX_Windows");
                    axisNameList.Add("Gamepad_RThumbstickY_Windows");
                    axisNameList.Add("Gamepad_LThumbstickX_Windows");
//                     axisNameList.Add("Gamepad_LThumbstickY_Windows");
                    axisNameList.Add("Gamepad_A_Windows");
                    axisNameList.Add("Gamepad_B_Windows");
                    axisNameList.Add("Gamepad_X_Windows");
                    axisNameList.Add("Gamepad_Y_Windows");
                    axisNameList.Add("Gamepad_LBumper_Windows");
                    axisNameList.Add("Gamepad_RBumper_Windows");
                    axisNameList.Add("Gamepad_View_Windows");
                    axisNameList.Add("Gamepad_Start_Windows");
                    axisNameList.Add("Gamepad_LStickClick_Windows");
                    axisNameList.Add("Gamepad_RStickClick_Windows");
                    axisNameList.Add("Gamepad_DPadX_Windows");
                    axisNameList.Add("Gamepad_DPadY_Windows");

                    axisNameList.Add("Gamepad_RightTrigger_MacOS");
                    axisNameList.Add("Gamepad_LeftTrigger_MacOS");
                    axisNameList.Add("Gamepad_RThumbstickX_MacOS");
                    axisNameList.Add("Gamepad_RThumbstickY_MacOS");
                    axisNameList.Add("Gamepad_LThumbstickX_MacOS");
                    axisNameList.Add("Gamepad_LThumbstickY_MacOS");
                    axisNameList.Add("Gamepad_A_MacOS");
                    axisNameList.Add("Gamepad_B_MacOS");
                    axisNameList.Add("Gamepad_X_MacOS");
                    axisNameList.Add("Gamepad_Y_MacOS");
                    axisNameList.Add("Gamepad_LBumper_MacOS");
                    axisNameList.Add("Gamepad_RBumper_MacOS");
                    axisNameList.Add("Gamepad_View_MacOS");
                    axisNameList.Add("Gamepad_Start_MacOS");
                    axisNameList.Add("Gamepad_LStickClick_MacOS");
                    axisNameList.Add("Gamepad_RStickClick_MacOS");
                    axisNameList.Add("Gamepad_DPadLeft_MacOS");
                    axisNameList.Add("Gamepad_DPadRight_MacOS");
                    axisNameList.Add("Gamepad_DPadUp_MacOS");
                    axisNameList.Add("Gamepad_DPadDown_MacOS");

                    int n = axisNameList.Count;
                    for (int x = 0; x < n; x++) {
                        if (isAxisDefined(axisNameList[x]) == false) {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }
    }
}