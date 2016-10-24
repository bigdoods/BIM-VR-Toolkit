using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Immerseum {
    namespace VRSimulator {

        [CustomEditor(typeof(ControllerManager))]
        public class ControllerManagerEditor : Editor {
            SerializedProperty _startingPosition;
            SerializedProperty _primaryHand;
            SerializedProperty _movementSpeed;

            SerializedProperty _applyCollider;
            SerializedProperty _applyColliderHand;
            SerializedProperty _isTrigger;
            SerializedProperty _colliderPhysicMaterial;
            SerializedProperty _colliderCenter;
            SerializedProperty _colliderRadius;
            SerializedProperty _colliderSize;
            SerializedProperty _colliderHeight;
            SerializedProperty _colliderDirection;

            SerializedProperty _applyRigidbody;
            SerializedProperty _applyRigidbodyHand;
            SerializedProperty _isKinematic;
            SerializedProperty _rbMass;
            SerializedProperty _rbDrag;
            SerializedProperty _rbAngularDrag;
            SerializedProperty _rbUseGravity;

            void OnEnable() {
                _startingPosition = serializedObject.FindProperty("_startingPosition");
                _primaryHand = serializedObject.FindProperty("_primaryHand");
                _movementSpeed = serializedObject.FindProperty("_movementSpeed");

                _applyCollider = serializedObject.FindProperty("_applyCollider");
                _applyColliderHand = serializedObject.FindProperty("_applyColliderHand");
                _isTrigger = serializedObject.FindProperty("_isTrigger");
                _colliderPhysicMaterial = serializedObject.FindProperty("_colliderPhysicMaterial");
                _colliderCenter = serializedObject.FindProperty("_colliderCenter");
                _colliderRadius = serializedObject.FindProperty("_colliderRadius");
                _colliderSize = serializedObject.FindProperty("_colliderSize");
                _colliderHeight = serializedObject.FindProperty("_colliderHeight");
                _colliderDirection = serializedObject.FindProperty("_colliderDirection");

                _applyRigidbody = serializedObject.FindProperty("_applyRigidbody");
                _applyRigidbodyHand = serializedObject.FindProperty("_applyRigidbodyHand");
                _isKinematic = serializedObject.FindProperty("_isKinematic");
                _rbMass = serializedObject.FindProperty("_rbMass");
                _rbDrag = serializedObject.FindProperty("_rbDrag");
                _rbAngularDrag = serializedObject.FindProperty("_rbAngularDrag");
                _rbUseGravity = serializedObject.FindProperty("_rbUseGravity");
            }

            public override void OnInspectorGUI () {
                float _defaultLabelWidth = EditorGUIUtility.labelWidth;
                serializedObject.Update();
                bool _simulateControllers = false;
                bool _displayHelpBoxes = true;
                int _controllerPrimitive = (int)PrimitiveType.Sphere;
                HMDSimulatorEditor[] editors = (HMDSimulatorEditor[])Resources.FindObjectsOfTypeAll(typeof(HMDSimulatorEditor));
                if (editors.Length > 0) {
                    _simulateControllers = editors[0].getSimulateControllers();
                    _displayHelpBoxes = editors[0].getDisplayHelpBoxes();
                    _controllerPrimitive = editors[0].getControllerPrimitive();
                }

                if (_simulateControllers) {
                    EditorGUILayout.HelpBox("Use the settings below to configure how you want your simulated controllers to behave.", MessageType.Info);

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    GUIContent startingPositionLabel = new GUIContent("Starting Position", "Determines the position in which the simulated controllers are displayed. Select prefab to use the prefab's original positioning.");
                    EditorGUILayout.PropertyField(_startingPosition, startingPositionLabel);
                    if (_displayHelpBoxes) {
                        switch (_startingPosition.enumValueIndex) {
                            case (int)ControllerPositions.Origin:
                                EditorGUILayout.HelpBox("ORIGIN will display controllers as if hands were hanging down at the user's waist (but slightly forward, so that they are still in view.", MessageType.Info);
                                break;
                            case (int)ControllerPositions.Forward:
                                EditorGUILayout.HelpBox("FORWARD will display controllers as if hands were resting on a desk or keyboard, slightly above the user's waist and a little further forward so that they are in view.", MessageType.Info);
                                break;
                            case (int)ControllerPositions.Boxer:
                                EditorGUILayout.HelpBox("BOXER will display controllers in a stereotypical pugilist's pose (fisticuffs, anyone?). Which hand is most forward will depend on other settings below.", MessageType.Info);
                                break;
                            case (int)ControllerPositions.Reaching:
                                EditorGUILayout.HelpBox("REACHING will display controllers as if they were held out at approximate shoulder/eye-level.", MessageType.Info);
                                break;
                            case (int)ControllerPositions.Prefab:
                                EditorGUILayout.HelpBox("PREFAB will display controllers at whatever native position was recorded in their prefab.", MessageType.Info);
                                break;
                        }
                    }

                    EditorGUILayout.Space();

                    GUIContent primaryHandLabel = new GUIContent("Primary Hand", "Determines which hand should be considered the dominant / primary hand. Affects which controller is positioned most-forward when in BOXER position.");
                    EditorGUILayout.PropertyField(_primaryHand, primaryHandLabel);

                    GUIContent movementSpeedLabel = new GUIContent("Movement Speed", "Determines how fast (in m / s) controllers should move.");
                    EditorGUILayout.PropertyField(_movementSpeed, movementSpeedLabel);

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    if (_controllerPrimitive != (int)PrimitiveType.Custom) {
                        if (_displayHelpBoxes) {
                            EditorGUILayout.HelpBox("Use the settings below to configure what type of Collider to apply to your simulated controllers.", MessageType.Info);
                        }
                        GUIContent applyColliderLabel = new GUIContent("Apply Colliders", "If true, creates a Collider on your simulated controllers.");
                        EditorGUILayout.PropertyField(_applyCollider, applyColliderLabel);

                        if (_applyCollider.boolValue == true) {
                            GUIContent applyColliderHandLabel = new GUIContent("Apply to Hand", "Selects which simulated controller should receive a Collider.");
                            EditorGUILayout.PropertyField(_applyColliderHand, applyColliderHandLabel);
                            if (_displayHelpBoxes) {
                                switch (_applyColliderHand.enumValueIndex) {
                                    case (int)Handedness.Left:
                                        EditorGUILayout.HelpBox("LEFT will apply the Collider to the left-hand controller.", MessageType.Info);
                                        break;
                                    case (int)Handedness.Right:
                                        EditorGUILayout.HelpBox("RIGHT will apply the Collider to the right-hand controller.", MessageType.Info);
                                        break;
                                    case (int)Handedness.Ambidextrous:
                                        EditorGUILayout.HelpBox("AMBIDEXTROUS will apply the Collider to both controllers.", MessageType.Info);
                                        break;
                                }
                            }

                            EditorGUILayout.Space();

                            GUIContent isTriggerLabel = new GUIContent("Is Trigger", "If true, makes the applied Collider a Trigger.");
                            EditorGUILayout.PropertyField(_isTrigger, isTriggerLabel);

                            EditorGUIUtility.labelWidth = 160;
                            GUIContent colliderPhysicMaterialLabel = new GUIContent("Collider Physic Material", "Determines the Physic Material to use on the applied Collider.");
                            EditorGUILayout.PropertyField(_colliderPhysicMaterial, colliderPhysicMaterialLabel);
                            EditorGUIUtility.labelWidth = _defaultLabelWidth;

                            GUIContent colliderCenterLabel = new GUIContent("Collider Center", "Determines the center of the applied Collider.");
                            EditorGUILayout.PropertyField(_colliderCenter, colliderCenterLabel);

                            if (_controllerPrimitive != (int)PrimitiveType.Cube) {
                                GUIContent colliderRadiusLabel = new GUIContent("Collider Radius", "Determines the radius of the applied Collider.");
                                EditorGUILayout.PropertyField(_colliderRadius, colliderRadiusLabel);
                            } else {
                                GUIContent colliderSizeLabel = new GUIContent("Collider Size", "Determines the size of the applied Collider.");
                                EditorGUILayout.PropertyField(_colliderSize, colliderSizeLabel);
                            }

                            if (_controllerPrimitive != (int)PrimitiveType.Sphere && _controllerPrimitive != (int)PrimitiveType.Cube) {
                                GUIContent colliderHeightLabel = new GUIContent("Collider Height", "Determines the height of the applied Collider.");
                                EditorGUILayout.PropertyField(_colliderHeight, colliderHeightLabel);

                                GUIContent colliderDirectionLabel = new GUIContent("Collider Direction", "Determines the direction in which the Collider is facing.");
                                EditorGUILayout.PropertyField(_colliderDirection, colliderDirectionLabel);
                            }
                        }

                        EditorGUILayout.Space();
                        EditorGUILayout.Space();

                        if (_displayHelpBoxes) {
                            EditorGUILayout.HelpBox("Use the settings below to configure what type of Rigidbody to apply to your simulated controllers.", MessageType.Info);
                        }
                        GUIContent applyRigidbodyLabel = new GUIContent("Apply Rigidbody", "If true, creates a Rigidbody on your simulated controllers.");
                        EditorGUILayout.PropertyField(_applyRigidbody, applyRigidbodyLabel);

                        if (_applyRigidbody.boolValue == true) {
                            GUIContent applyRigidbodyHandLabel = new GUIContent("Apply to Hand", "Selects which simulated controller should receive a Rigidbody.");
                            EditorGUILayout.PropertyField(_applyRigidbodyHand, applyRigidbodyHandLabel);
                            if (_displayHelpBoxes) {
                                switch (_applyColliderHand.enumValueIndex) {
                                    case (int)Handedness.Left:
                                        EditorGUILayout.HelpBox("LEFT will apply the Rigidbody to the left-hand controller.", MessageType.Info);
                                        break;
                                    case (int)Handedness.Right:
                                        EditorGUILayout.HelpBox("RIGHT will apply the Rigidbody to the right-hand controller.", MessageType.Info);
                                        break;
                                    case (int)Handedness.Ambidextrous:
                                        EditorGUILayout.HelpBox("AMBIDEXTROUS will apply the Rigidbody to both controllers.", MessageType.Info);
                                        break;
                                }
                            }

                            EditorGUILayout.Space();

                            GUIContent isKinematicLabel = new GUIContent("Is Kinematic", "If true, makes the applied Rigidbody Kinematic.");
                            EditorGUILayout.PropertyField(_isKinematic, isKinematicLabel);

                            GUIContent rbMassLabel = new GUIContent("Mass", "Determines the mass to apply to the Rigidbody.");
                            EditorGUILayout.PropertyField(_rbMass, rbMassLabel);

                            GUIContent rbDragLabel = new GUIContent("Drag", "Determines the drag to apply to the Rigidbody.");
                            EditorGUILayout.PropertyField(_rbDrag, rbDragLabel);

                            GUIContent rbAngularDragLabel = new GUIContent("Angular Drag", "Determines the angular drag to apply to the Rigidbody.");
                            EditorGUILayout.PropertyField(_rbAngularDrag, rbAngularDragLabel);

                            GUIContent rbUseGravityLabel = new GUIContent("Use Gravity", "If true, makes the Rigidbody respond to gravity.");
                            EditorGUILayout.PropertyField(_rbUseGravity, rbUseGravityLabel);
                        }
                    }

                } else {
                    EditorGUILayout.HelpBox("Since simulating controllers is disabled at the moment, there is nothing to configure here.", MessageType.Warning);
                }

                EditorGUILayout.Space();

                serializedObject.ApplyModifiedProperties();
            }
        }

    }
}
