using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;

namespace Immerseum {
    namespace VRSimulator {

        /// <summary>Enum which indicates either right or left.</summary>
        public enum Hands { Left, Right }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Singleton responsible for moving the user's avatar in the virtual scene. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public class MovementManager : MonoBehaviour {
            /// <summary>The singleton Instance of this class.</summary>
            public static MovementManager Instance { get; private set; }

            /// <summary>   true if the primary trigger is being used. </summary>
            protected bool _isPrimaryTriggerInUse = false;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Gets a value indicating whether the <see cref="primaryGamepadTrigger" /> is currently being used (is $$held$$).</summary>
            /// <value>
            ///   <strong>true</strong> if the <see cref="primaryGamepadTrigger">Primary Trigger</see> is being $$held$$, <strong>false</strong> if not.</value>
            /// <seealso cref="primaryGamepadTrigger"></seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool isPrimaryTriggerInUse {
                get {
                    return Instance._isPrimaryTriggerInUse;
                }
            }

            /// <summary>   true if the secondary trigger is in use. </summary>
            protected bool _isSecondaryTriggerInUse = false;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Indicates whether the $$Secondary Trigger$$ is being used (i.e. is being $$held$$).</summary>
            /// <value>
            ///   <strong>true</strong> if the Secondary Trigger (trigger other than the <see cref="primaryGamepadTrigger">Primary Trigger</see>) is being $$held$$, <strong>false</strong>
            /// if not.</value>
            /// <seealso cref="primaryGamepadTrigger"></seealso>
            /// <seealso cref="isPrimaryTriggerInUse"></seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool isSecondaryTriggerInUse {
                get {
                    return Instance._isSecondaryTriggerInUse;
                }
            }

            /// <summary>   true to use default input action to movement mappings. </summary>
            [SerializeField]
            protected bool _useDefaultInputActions = true;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            ///   <para>Indicates whether defined <see cref="InputAction">InputActions</see> are mapped to movement using Immerseum's <see cref="!:Immerseum Movement Mapping">Default Movement Mappings</see>.</para>
            ///   <innovasys:widget type="Tip Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">This property is configured in the <strong>Unity Editor</strong>. Please see <see cref="Configuring Input &amp;amp; Movement">Configuring Input
            ///     &amp; Movement</see>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <para>
            ///         <strong>BE CAREFUL!</strong> If you have set this property to <strong>false</strong> but want to support user input, then you will need to
            ///         either:</para>
            ///       <list type="bullet">
            ///         <item>code your own <see cref="InputAction" /> mapping (see <strong><see cref="!:Custom InputAction Mapping">Custom InputAction Mapping</see></strong>), or;</item>
            ///         <item>write your own input-handling code (see <strong><see cref="!:Custom Input Handling">Custom Input Handling</see></strong>).</item>
            ///       </list>
            ///     </innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <remarks>   Immerseum's default mappings are as follows:<br /></remarks>
            /// <value>
            ///   <strong>true</strong> to map <see cref="InputAction">InputActions</see> to default movement controls, <strong>false</strong> to handle <see cref="InputAction" /> events and their mapping
            /// to movement logic using your own code.</value>
            /// <seealso cref="!:Immerseum Movement Mapping"></seealso>
            /// <seealso cref="!:Custom InputAction Mapping"></seealso>
            /// <seealso cref="!:Custom Input Handling"></seealso>
            /// <seealso cref="!:Configuring Input &amp; Movement"></seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool useDefaultInputActions {
                get {
                    return Instance._useDefaultInputActions;
                }
            }

            [SerializeField]
            protected bool _isStrafeEnabled = true;
            /// <summary>
            ///   <para>Indicates whether $$strafing$$ is supported in this VR Scene. </para>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <strong>Be careful</strong>. Strafing is known to contribute to $$simulation sickness$$. We
            ///     recommend it be disabled.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            ///   <innovasys:widget type="Tip Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">This property is configured in the <strong>Unity Editor</strong>. Please see <see cref="Configuring Input &amp;amp; Movement">Configuring Input
            ///     &amp; Movement</see>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If strafing is disabled, lateral <see cref="InputAction">InputActions</see> (e.g. left/right arrows, A/D keys,
            ///     etc.) will rotate the camera instead of moving the user's avatar.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>
            ///   <strong>true</strong> if $$strafing$$ is enabled, <strong>false</strong> if not.</value>
            public static bool isStrafeEnabled {
                get {
                    return Instance._isStrafeEnabled;
                }
            }

            [SerializeField]
            protected Hands _primaryGamepadTrigger = Hands.Right;
            /// <summary>
            ///   <para>Indicates which gamepad trigger should be considered the "primary" trigger. </para>
            ///   <innovasys:widget type="Tip Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">This property is configured in the <strong>Unity Editor</strong>. Please see <see cref="Configuring Input &amp;amp; Movement">Configuring Input
            ///     &amp; Movement</see>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>
            ///   <para>Accepts one of the following values:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>Hands.Left</strong> for the left-hand trigger, or;</item>
            ///     <item>
            ///       <strong>Hands.Right</strong> for the right-hand trigger.</item>
            ///   </list>
            /// </value>
            public static Hands primaryGamepadTrigger {
                get {
                    return Instance._primaryGamepadTrigger;
                }
            }

            [SerializeField]
            [Range(0, 360)]
            protected float _keyboardRotationRatchet = 45.0f;
            /// <summary>
            ///   <para>Determines the number of degrees that the camera shoud rotate when $$ratcheted$$ by an <see cref="InputAction" />. </para>
            ///   <innovasys:widget type="Tip Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">This property is configured in the <strong>Unity Editor</strong>. Please see <see cref="Configuring Input &amp;amp; Movement">Configuring Input
            ///     &amp; Movement</see>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A floating point value representing the number of degrees to rotate the camera when $$ratcheted$$. Must be in the range of 0.0 to 360.0.</value>
            public float keyboardRotationRatchet {
                get {
                    return Instance._keyboardRotationRatchet;
                }
            }

            [SerializeField]
            [Range(0, 10)]
            protected float _accelerationRate = 0.1f;
            /// <summary>
            ///   <para>Indicates the rate at which the user's avatar accelerates when moving in your VR Scene.</para>
            ///   <innovasys:widget type="Tip Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">This property is configured in the <strong>Unity Editor</strong>. Please see <see cref="Configuring Input &amp;amp; Movement">Configuring Input
            ///     &amp; Movement</see>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A floating point value in a range from 0.0 - 10.0.</value>
            public static float accelerationRate {
                get {
                    return Instance._accelerationRate;
                }
            }

            [SerializeField]
            protected bool _isRunEnabled = true;
            /// <summary>
            ///   <para>Indicates whether the VR Scene supports $$Run Mode$$. </para>
            ///   <innovasys:widget type="Tip Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">This property is configured in the <strong>Unity Editor</strong>. Please see <see cref="Configuring Input &amp;amp; Movement">Configuring Input
            ///     &amp; Movement</see>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>
            ///   <strong>true</strong> if $$Run Mode$$ is supported, <strong>false</strong> if not.</value>
            public static bool isRunEnabled {
                get {
                    return Instance._isRunEnabled;
                }
            }

            [SerializeField]
            protected bool _isRunActive = false;
            /// <summary>Indicates whether the user's movement is currently in "Run" mode (i.e. movement set to twice the normal speed).</summary>
            /// <value>
            ///   <strong>true</strong> if in "Run" mode, <strong>false</strong> if not.</value>
            public static bool isRunActive {
                get {
                    return Instance._isRunActive;
                }
            }
            /// <summary>Controls whether the avatar's movement mode is set to Run (i.e. moving at twice the normal speed).</summary>
            /// <param name="value">
            ///   <strong>true</strong> to set the avatar's movement mode to "Run" (making the avatar move at twice their normal speed), <strong>false</strong> to set the
            /// avatar's mode to "Walk" (moving at the standard rate).</param>
            /// <example>
            ///   <innovasys:widget type="Authoring Note" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">TODO: Make one example for "setRunActive(true)" and one example for
            ///     "setRunActive(false)"</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </example>
            public static void setRunActive (bool value) {
                Instance._isRunActive = value;
            }

            [SerializeField]
            [Range(0, 1)]
            protected float _forwardDampingRate = 0.3f;
            /// <summary>
            ///   <para>The rate by which the user's forward motion should be dampened.</para>
            ///   <innovasys:widget type="Tip Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">This property is configured in the <strong>Unity Editor</strong>. Please see <see cref="Configuring Input &amp;amp;amp;amp; Movement">Configuring Input
            ///     &amp; Movement</see>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">This has been configured based on Oculus Rift best practices to minimize $$simulation sickness$$.
            ///     Please be careful when modifying this value.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A floating point value in the range from 0.0 to 1.0.</value>
            public static float forwardDampingRate {
                get {
                    return Instance._forwardDampingRate;
                }
            }

            [SerializeField]
            [Range(0, 1)]
            protected float _backAndSideDampingRate = 0.5f;
            /// <summary>
            ///   <para>The rate by which backwards and sideways movement should be dampened.</para>
            ///   <innovasys:widget type="Tip Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">This property is configured in the <strong>Unity Editor</strong>. Please see <see cref="Configuring Input &amp;amp;amp; Movement">Configuring Input
            ///     &amp; Movement</see>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">This has been configured based on Oculus Rift best practices to minimize $$simulation sickness$$.
            ///     Please be careful when modifying this value.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A floating point value between 0.0 and 1.0.</value>
            public static float backAndSideDampingRate {
                get {
                    return Instance._backAndSideDampingRate;
                }
            }

            [SerializeField]
            [Range(0, 1)]
            protected float _gravityAdjustment = 0.379f;
            /// <summary>
            ///   <para> The value by which gravity should be adjusted.</para>
            ///   <innovasys:widget type="Tip Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">This property is configured in the <strong>Unity Editor</strong>. Please see <see cref="Configuring Input &amp;amp;amp; Movement">Configuring Input
            ///     &amp; Movement</see>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">This has been configured based on Oculus Rift best practices to minimize $$simulation sickness$$.
            ///     Please be careful when modifying this value.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A floating point value in a range from 0.0 to 1.0.</value>
            public static float gravityAdjustment {
                get {
                    return Instance._gravityAdjustment;
                }
            }

            protected bool _hasMoved = false;
            /// <summary>Indicates whether the user's Avatar has changed positions since the last frame.</summary>
            /// <value>
            ///   <strong>true</strong> if the Avatar has moved, <strong>false</strong> if not.</value>
            public bool hasMoved {
                get {
                    return Instance._hasMoved;
                }
            }
            /// <summary>Sets the value of <see cref="hasMoved">MovementManager.hasMoved</see>.</summary>
            /// <param name="value">
            ///   <strong>True</strong> if the user's avatar has moved since the last frame, <strong>False</strong> if not.</param>
            public void setHasMoved(bool value) {
                Instance._hasMoved = value;
            }

            protected static float rotationRate = 90f;
            protected static float movementRate = 60f;
            protected static Vector3 moveThrottle = Vector3.zero;
            protected static float fallSpeed = 0.0f;





            void Awake () {
                if (Instance != null && Instance != this) {
                    Destroy(gameObject);
                }

                Instance = this;
            }

            void OnEnable () {
                EventManager.OnMoveAvatarStart += OnMoveAvatarStart;
                if (useDefaultInputActions) {
                    EventManager.OnInputActionStart += OnInputActionStart;
                    EventManager.OnPitchRotationStart += OnPitchRotationStart;
                    EventManager.OnYawRotationStart += OnYawRotationStart;
                    EventManager.OnXAxisMovementStart += OnXAxisMovementStart;
                    EventManager.OnZAxisMovementStart += OnZAxisMovementStart;

                    EventManager.OnRightBumperActivation += OnRightBumperActivation;
                    EventManager.OnLeftBumperActivation += OnLeftBumperActivation;

                    EventManager.OnLeftThumbstickClickActivation += OnLeftThumbstickClickActivation;
                    EventManager.OnLeftThumbstickClickDeactivation += OnLeftThumbstickClickDeactivation;
                }
            }

            void OnDisable () {
                EventManager.OnMoveAvatarStart -= OnMoveAvatarStart;
                if (useDefaultInputActions) {
                    EventManager.OnInputActionStart -= OnInputActionStart;
                    EventManager.OnPitchRotationStart -= OnPitchRotationStart;
                    EventManager.OnYawRotationStart -= OnYawRotationStart;
                    EventManager.OnXAxisMovementStart -= OnXAxisMovementStart;
                    EventManager.OnZAxisMovementStart -= OnZAxisMovementStart;

                    EventManager.OnRightBumperActivation -= OnRightBumperActivation;
                    EventManager.OnLeftBumperActivation -= OnLeftBumperActivation;

                    EventManager.OnLeftThumbstickClickActivation -= OnLeftThumbstickClickActivation;
                    EventManager.OnLeftThumbstickClickDeactivation -= OnLeftThumbstickClickDeactivation;

                    EventManager.OnRightThumbstickClickActivation -= OnRightThumbstickClickActivation;
                    EventManager.OnRightThumbstickClickDeactivation -= OnRightThumbstickClickDeactivation;
                }
            }


            void OnInputActionStart (InputAction inputAction) {
                if (useDefaultInputActions) {
                    switch (inputAction.name) {
                        case "togglePauseMenu":
                            EventManager.activatePauseButton();
                            break;
                        case "toggleView":
                            EventManager.activateViewButton();
                            break;
                        case "togglePrimaryTrigger":
                            if (isPrimaryTriggerInUse) {
                                Instance._isPrimaryTriggerInUse = false;
                                EventManager.deactivatePrimaryTrigger(inputAction);
                            } else {
                                Instance._isPrimaryTriggerInUse = true;
                                EventManager.activatePrimaryTrigger(inputAction);
                            }
                            break;
                        case "toggleSecondaryTrigger":
                            if (isSecondaryTriggerInUse) {
                                Instance._isSecondaryTriggerInUse = false;
                                EventManager.deactivateSecondaryTrigger(inputAction);
                            } else {
                                Instance._isSecondaryTriggerInUse = true;
                                EventManager.activateSecondaryTrigger(inputAction);
                            }
                            break;
                        case "toggleRightThumbstickClick":
                            if (InputActionManager.isRightThumbstickClickActivated) {
                                EventManager.deactivateRightThumbstickClick();
                            } else {
                                EventManager.activateRightThumbstickClick();
                            }
                            break;
                        case "toggleLeftThumbstickClick":
                            if (InputActionManager.isLeftThumbstickClickActivated) {
                                EventManager.deactivateLeftThumbstickClick();
                            } else {
                                EventManager.activateLeftThumbstickClick();
                            }
                            break;
                        case "toggleRightBumper":
                            EventManager.activateRightBumper();
                            break;
                        case "toggleLeftBumper":
                            EventManager.activateLeftBumper();
                            break;
                        case "toggleSelectionButton":
                            EventManager.activateSelectionButton();
                            break;
                        case "toggleCancelButton":
                            EventManager.activateCancelButton();
                            break;
                        case "toggleSecondaryButton":
                            EventManager.activateSecondaryButton();
                            break;
                        case "toggleTertiaryButton":
                            EventManager.activateTertiaryButton();
                            break;
                        case "xAxisMovement":
                            EventManager.startXAxisMovement(inputAction);
                            break;
                        case "zAxisMovement":
                            EventManager.startZAxisMovement(inputAction);
                            break;
                        case "pitchRotation":
                            EventManager.startPitchRotation(inputAction);
                            break;
                        case "yawRotation":
                            EventManager.startYawRotation(inputAction);
                            break;
                    }
                }
            }

            void OnPitchRotationStart (InputAction inputAction) {
                Vector3 originalEuler = HMDSimulator.getCamera.rotation.eulerAngles;
                Vector3 modifiedEuler = originalEuler;

                float rotateInfluence = rotationRate * Time.deltaTime;

                InputType axisValueSource = inputAction.getAxisValueSource(inputAction.pitchAxisList.ToArray());
                if (axisValueSource == InputType.Gamepad) {
                    modifiedEuler.x += inputAction.pitchAxisValue * rotateInfluence;
                } else {
                    modifiedEuler.x -= inputAction.pitchAxisValue * rotateInfluence;
                }

                HMDSimulator.getCamera.rotation = Quaternion.Euler(modifiedEuler);
            }

            void OnYawRotationStart (InputAction inputAction) {
                Vector3 originalEuler = HMDSimulator.getCamera.rotation.eulerAngles;
                Vector3 modifiedEuler = originalEuler;

                float axisValue;
                if (inputAction.yawAxisValue == 0) {
                    axisValue = inputAction.buttonValue;
                } else {
                    axisValue = inputAction.yawAxisValue;
                }

                float rotateInfluence = rotationRate * Time.deltaTime;

                modifiedEuler.y += axisValue * rotateInfluence;

                HMDSimulator.getCamera.rotation = Quaternion.Euler(modifiedEuler);
            }

            void OnXAxisMovementStart (InputAction inputAction) {
                float moveScale = 1.0f;
                float moveInfluence;
                InputAction zAxisMovement = InputActionManager.getInputAction("zAxisMovement");

                Quaternion originalRotation = HMDSimulator.getCamera.transform.rotation;
                Vector3 originalEuler = originalRotation.eulerAngles;
                originalEuler.z = originalEuler.x = 0f;
                originalRotation = Quaternion.Euler(originalEuler);

                if (zAxisMovement.isZAxisAtRest == false) {
                    moveScale *= 0.7072f;
                }

                moveScale *= movementRate * Time.deltaTime;
                moveInfluence = accelerationRate * moveScale;

                if (isRunActive) {
                    moveInfluence *= 2.0f;
                }


                if (inputAction.xAxisValue > 0 || inputAction.buttonValue > 0) {
                    Debug.LogWarning("xAxisValue: " + inputAction.xAxisValue);
                    Debug.LogWarning("buttonValue: " + inputAction.buttonValue);
                    moveThrottle += originalRotation * (HMDSimulator.getCamera.root.lossyScale.x * moveInfluence * backAndSideDampingRate * Vector3.right);
                    moveThrottle += originalRotation * (Mathf.Max(inputAction.buttonValue, inputAction.xAxisValue) * HMDSimulator.getCamera.lossyScale.x * moveInfluence * backAndSideDampingRate * Vector3.right);
                    Debug.LogWarning("moveThrottle: " + moveThrottle.ToString());
                }
                if (inputAction.xAxisValue < 0 || inputAction.buttonValue < 0) {
                    moveThrottle += originalRotation * (HMDSimulator.getCamera.root.lossyScale.x * moveInfluence * backAndSideDampingRate * Vector3.left);
                    moveThrottle += originalRotation * (Mathf.Max(Mathf.Abs(inputAction.buttonValue), Mathf.Abs(inputAction.xAxisValue)) * HMDSimulator.getCamera.lossyScale.x * moveInfluence * backAndSideDampingRate * Vector3.left);
                }

                moveAvatar(inputAction);
                EventManager.endXAxisMovement(inputAction);
            }

            void OnZAxisMovementStart (InputAction inputAction) {
                float moveScale = 1.0f;
                float moveInfluence;
                InputAction xAxisMovement = InputActionManager.getInputAction("xAxisMovement");

                Quaternion originalRotation = HMDSimulator.getCamera.transform.rotation;
                Vector3 originalEuler = originalRotation.eulerAngles;
                originalEuler.z = originalEuler.x = 0f;
                originalRotation = Quaternion.Euler(originalEuler);

                if (xAxisMovement.isXAxisAtRest == false) {
                    moveScale *= 0.7072f;
                }

                moveScale *= movementRate * Time.deltaTime;
                moveInfluence = accelerationRate * moveScale;

                if (isRunActive) {
                    moveInfluence *= 2.0f;
                }

                if (inputAction.zAxisValue > 0 || inputAction.buttonValue > 0) {
                    moveThrottle += originalRotation * (HMDSimulator.getCamera.lossyScale.z * moveInfluence * forwardDampingRate * Vector3.forward);
                    moveThrottle += originalRotation * (Mathf.Max(inputAction.buttonValue, inputAction.zAxisValue) * HMDSimulator.getCamera.lossyScale.z * moveInfluence * forwardDampingRate * Vector3.forward);
                }
                if (inputAction.zAxisValue < 0 || inputAction.buttonValue < 0) {
                    moveThrottle += originalRotation * (HMDSimulator.getCamera.lossyScale.z * moveInfluence * backAndSideDampingRate * Vector3.back);
                    moveThrottle += originalRotation * (Mathf.Max(Mathf.Abs(inputAction.buttonValue), Mathf.Abs(inputAction.zAxisValue)) * HMDSimulator.getCamera.lossyScale.z * moveInfluence * backAndSideDampingRate * Vector3.back);
                }

                moveAvatar(inputAction);
                EventManager.endZAxisMovement(inputAction);
            }

            void OnRightBumperActivation () {
                Vector3 originalEuler = HMDSimulator.getCamera.rotation.eulerAngles;
                Vector3 modifiedEuler = originalEuler;

                modifiedEuler.y += keyboardRotationRatchet;

                HMDSimulator.getCamera.rotation = Quaternion.Euler(modifiedEuler);
            }

            void OnLeftBumperActivation () {
                Vector3 originalEuler = HMDSimulator.getCamera.rotation.eulerAngles;
                Vector3 modifiedEuler = originalEuler;

                modifiedEuler.y -= keyboardRotationRatchet;

                HMDSimulator.getCamera.rotation = Quaternion.Euler(modifiedEuler);
            }

            void OnLeftThumbstickClickActivation () {
                if (isRunActive) {
                    setRunActive(false);
                } else {
                    setRunActive(true);
                }
            }

            void OnLeftThumbstickClickDeactivation () {
                if (isRunActive) {
                    setRunActive(false);
                } else {
                    setRunActive(true);
                }
            }

            void OnRightThumbstickClickActivation () {
                HMDSimulator.getCamera.rotation = Quaternion.Euler(Vector3.zero);
                InputActionManager.setRightThumbstickClickActivated(false);
            }

            void OnRightThumbstickClickDeactivation () {

            }

            void OnMoveAvatarStart() {
                setHasMoved(false);
            }

            void moveAvatar (InputAction inputAction = null) {
                Vector3 moveDirection = Vector3.zero;
                float motorDamp = (1.0f + (forwardDampingRate * movementRate * Time.deltaTime));
                CharacterController characterController = HMDSimulator.getCamera.GetComponentInParent<CharacterController>();
                bool isGrounded = true;
                if (characterController != null) {
                    isGrounded = characterController.isGrounded;
                } else {
                    if (HMDSimulator.getCameraContainer.position.y > HMDSimulator.headHeight) {
                        isGrounded = false;
                    }
                }

                moveThrottle.x /= motorDamp;
                moveThrottle.y = (moveThrottle.y > 0.0f) ? (moveThrottle.y / motorDamp) : moveThrottle.y;
                moveThrottle.z /= motorDamp;

                moveDirection += moveThrottle * movementRate * Time.deltaTime;

                if (isGrounded == false && fallSpeed <= 0) {
                    fallSpeed = ((Physics.gravity.y * (gravityAdjustment * 0.002f)));
                } else if (isGrounded == false) {
                    fallSpeed += ((Physics.gravity.y * (gravityAdjustment * 0.002f)) * movementRate * Time.deltaTime);
                } else {
                    fallSpeed = 0f;
                }

                moveDirection.y += fallSpeed * movementRate * Time.deltaTime;

                Vector3 predictedXZ;
                Vector3 predictedWorldXZ;
                Vector3 predictedWorldLeftControllerXZ;
                Vector3 predictedWorldRightControllerXZ;
                Vector3 actualXZ;

                if (characterController != null) {
                    OVRPlayerController ovrPlayerController = HMDSimulator.getCamera.GetComponentInParent<OVRPlayerController>();
                    predictedXZ = Vector3.Scale((characterController.transform.localPosition + moveDirection), new Vector3(1, 0, 1));
                    predictedWorldXZ = Vector3.Scale((characterController.transform.position + moveDirection), new Vector3(1, 0, 1));
                    if (HMDSimulator.simulateControllers) {
                        predictedWorldLeftControllerXZ = Vector3.Scale((HMDSimulator.leftController.position + moveDirection), new Vector3(1, 0, 1));
                        predictedWorldRightControllerXZ = Vector3.Scale((HMDSimulator.rightController.position + moveDirection), new Vector3(1, 0, 1));
                        if (PlayAreaManager.isMovementBoundToPlayArea && PlayAreaManager.hasImmerseumPlayArea) {
                            if (PlayAreaManager.playArea.isWithinBoundary(predictedWorldXZ, predictedWorldLeftControllerXZ, predictedWorldRightControllerXZ) == false) {
                                if (characterController != null && ovrPlayerController != null) {
                                    ovrPlayerController.SetHaltUpdateMovement(true);
                                }
                                return;
                            }
                        }
                    }
                    if (PlayAreaManager.isMovementBoundToPlayArea && PlayAreaManager.hasImmerseumPlayArea) {
                        if (PlayAreaManager.playArea.isWithinBoundary(predictedWorldXZ) == false) {
                            if (characterController != null && ovrPlayerController != null) {
                                ovrPlayerController.SetHaltUpdateMovement(true);
                            }
                            return;
                        }
                    }
                    characterController.Move(moveDirection);
                    actualXZ = Vector3.Scale(characterController.transform.localPosition, new Vector3(1, 0, 1));
                } else {
                    predictedXZ = Vector3.Scale((HMDSimulator.getCamera.localPosition + moveDirection), new Vector3(1, 0, 1));
                    predictedWorldXZ = Vector3.Scale((HMDSimulator.getCamera.position + moveDirection), new Vector3(1, 0, 1));
                    if (HMDSimulator.simulateControllers) {
                        predictedWorldLeftControllerXZ = Vector3.Scale((HMDSimulator.leftController.position + moveDirection), new Vector3(1, 0, 1));
                        predictedWorldRightControllerXZ = Vector3.Scale((HMDSimulator.rightController.position + moveDirection), new Vector3(1, 0, 1));
                        if (PlayAreaManager.isMovementBoundToPlayArea && PlayAreaManager.hasImmerseumPlayArea) {
                            if (PlayAreaManager.playArea.isWithinBoundary(predictedWorldXZ, predictedWorldLeftControllerXZ, predictedWorldRightControllerXZ) == false) {
                                return;
                            }
                        }
                    }
                    if (PlayAreaManager.isMovementBoundToPlayArea && PlayAreaManager.hasImmerseumPlayArea) {
                        if (PlayAreaManager.playArea.isWithinBoundary(predictedWorldXZ) == false) {
                            return;
                        }
                    }
                    HMDSimulator.CameraRig.root.localPosition += moveDirection;
                    actualXZ = Vector3.Scale(HMDSimulator.getCamera.localPosition, new Vector3(1, 0, 1));
                }

                if (predictedXZ != actualXZ) {
                    moveThrottle += (actualXZ - predictedXZ) / (movementRate * Time.deltaTime);
                }

                setHasMoved(true);
            }

            void FixedUpdate() {
                if (hasMoved) {
                    EventManager.endMoveAvatar();
                }
            }

        }
    }
}