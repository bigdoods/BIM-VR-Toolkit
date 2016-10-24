using UnityEngine;
using UnityEditor;

namespace Immerseum {
    namespace VRSimulator {

        [CustomEditor(typeof(InputActionManager))]
        public class InputActionManagerEditor : Editor {
            SerializedProperty _createImmerseumDefaults;
            SerializedProperty _areInputAxesDefined;

            void OnEnable() {
                _createImmerseumDefaults = serializedObject.FindProperty("_createImmerseumDefaults");
                _areInputAxesDefined = serializedObject.FindProperty("_areInputAxesDefined");
            }

            public override void OnInspectorGUI() {
                serializedObject.Update();

                EditorGUIUtility.labelWidth = 160;

                GUIContent createImmerseumDefaultLabel = new GUIContent("Use Immerseum Defaults", "If true, creates Immerseum's default Input Actions using the default input mappings.");
                EditorGUILayout.PropertyField(_createImmerseumDefaults, createImmerseumDefaultLabel);
                if (_createImmerseumDefaults.boolValue == false) {
                    EditorGUILayout.HelpBox("BE CAREFUL! You have decided not to use Immerseum's default input actions. If you want to handle user input, be sure that you have either defined your own custom input actions and registered them with the InputActionManager or handling input outside of the Immerseum SDK.", MessageType.Warning);
                } else {
                    EditorGUILayout.HelpBox("You are using Immerseum's default Input Actions. These input actions will generate input events when the user does something with their input device, but how your VR scene responds to those inputs is configured elsewhere (either in the Movement Manager or in your code).", MessageType.Info);
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                if (InputManagerSetup.areImmerseumAxesDefined) {
                    _areInputAxesDefined.boolValue = true;
                    EditorGUILayout.HelpBox("Your project is currently using Immerseum's InputManager settings.", MessageType.Info);
                } else {
                    _areInputAxesDefined.boolValue = false;
                    EditorGUILayout.HelpBox("Your project's InputManager settings do not include all of the input axes used by Immerseum's Input Actions. To prevent errors, either disable default Immerseum's default Input Actions or update your InputManager using the button below.", MessageType.Error);
                    GUIContent applyInputManagerLabel = new GUIContent("Apply Immerseum InputManager Settings", "Clicking this button will over-write your project's InputManager settings, defining input axes compatible with the Immerseum SDK.");
                    if (GUILayout.Button(applyInputManagerLabel)) {
                        if (EditorUtility.DisplayDialog("Are you sure?", "You are about to over-write your InputManager settings with Immerseum's defaults. This cannot be undone. Are you sure?", "Yes, I'm sure.", "No, I'd rather not.")) {
                            InputManagerSetup.setupInputManager();
                        }
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
