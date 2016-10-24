using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {

        /// <summary>
        ///   <para>Singleton used to manage <see cref="InputAction">InputActions</see>.</para>
        ///   <para>The primary purpose of the InputActionManager is to store a list of the registered <see cref="InputAction">InputActions</see> and to evaluate whether a registered
        /// <see cref="InputAction" /> has been received from the user.</para>
        ///   <para>Every frame, the InputActionManager checks to see which <see cref="InputAction">InputActions</see> were received during the last frame (if any). If an
        /// <see cref="InputAction" /> was received from the user, the InputActionManager then fires the <see cref="EventManager.OnInputActionStart">EventManager.OnInputActionStart</see> event.</para>
        /// </summary>
        /// <seealso cref="InputAction">InputAction</seealso>
        public class InputActionManager : MonoBehaviour {

#if UNITY_EDITOR
            static UnityEditor.SerializedObject _serializedTemplate;
            static UnityEditor.SerializedObject _serializedInputManager;

            private static UnityEditor.SerializedObject loadSerializedTemplate (string filePath) {
                string[] guidArray = UnityEditor.AssetDatabase.FindAssets(filePath);
                if (guidArray.Length == 0) {
                    _serializedTemplate = new UnityEditor.SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath(filePath)[0]);
                }
                return _serializedTemplate;
            }

            private static UnityEditor.SerializedObject loadSerializedInputManager () {
                string[] guidArray = UnityEditor.AssetDatabase.FindAssets("ProjectSettings/InputManager.asset");
                if (guidArray.Length == 0) {
                    _serializedInputManager = new UnityEditor.SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
                }
                return _serializedInputManager;
            }
#endif

            /// <summary>The singleton Instance of thi class that exists within your VR Scene.</summary>
            public static InputActionManager Instance { get; private set; }

            protected bool _areInputActionsInitialized = false;
            /// <summary>Indicates whether input actions have completed their initialization.</summary>
            /// <value>
            ///   <strong>true</strong> if input actions are initialized, otherwise <strong>false</strong></value>
            public static bool areInputActionsInitialized {
                get {
                    return Instance._areInputActionsInitialized;
                }
            }

            /// <summary>
            ///   <para>Indicates whether the default <see cref="!:Immerseum Movement Mapping">Immerseum Movement Mappings</see> should be applied.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">This property is configured in the <strong>Unity Editor</strong>, and is actually edited within the
            ///        <strong><see cref="MovementManager">MovementManager</see></strong>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>
            ///   <strong>true</strong> if registered <see cref="InputAction">InputActions</see> should be mapped to movement using the default <see cref="!:Immerseum Movement Mapping">Immerseum Movement Mapping</see>, <strong>false</strong> if you will be supplying your own movement mapping.</value>
            /// <seealso cref="!:Immerseum Movement Mapping"></seealso>
            /// <seealso cref="!:Configuring Input &amp; Movement"></seealso>
            /// <seealso cref="!:Immerseum Default Input Actions"></seealso>
            public static bool useImmerseumDefaults {
                get {
                    return MovementManager.useDefaultInputActions;
                }
            }

            [SerializeField]
            protected bool _createImmerseumDefaults = true;
            /// <summary>
            ///   <para>Indicates whether the VRSimulator should create the default Immerseum <see cref="InputAction">InputActions</see>. For a list of
            /// default <see cref="InputAction">InputActions</see>, please refer to:<strong></strong><see cref="!:Immerseum Default Input Actions">Immerseum Default Input Actions</see>.</para>
            ///   <innovasys:widget type="Tip Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">This property is configured within the <strong>Unity Editor</strong>. Please see
            ///     <strong>Configuring Input and Movement</strong>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>
            ///   <strong>true</strong> if Immerseum's <see cref="!:Immerseum Default Input Actions">default InputActions</see> should be created during
            /// initialization and registered, <strong>false</strong> if not.</value>
            public static bool createImmerseumDefaults {
                get {
                    return Instance._createImmerseumDefaults;
                }
            }

            [SerializeField]
            protected bool _areInputAxesDefined = false;

            /// <summary>Indicates whether the input axes used by Immerseum's default input actions are defined in the project's <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Input Manager</see>.</summary>
            /// <value>
            ///   <strong>true</strong> if Immerseum's input axes are defined in the project's <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Input Manager</see>, otherwise <strong>false</strong>.</value>
            protected static bool areInputAxesDefined {
                get {
                    return Instance._areInputAxesDefined;
                }
            }

            protected List<InputAction> _inputActionList = new List<InputAction>();
            /// <summary>
            ///   <para>The list of <see cref="InputAction">InputActions</see> registered with the InputActionManager.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <para>The contents of this list can only be modified by <see cref="VR%20Simulator.CSharp~Immerseum.VRSimulator.InputActionManager~addInputAction.html">addInputAction</see> and <see cref="VR%20Simulator.CSharp~Immerseum.VRSimulator.InputActionManager~removeInputAction.html">removeInputAction</see>.</para>
            ///       <para>It is also recommended that you use <see cref="VR%20Simulator.CSharp~Immerseum.VRSimulator.InputActionManager~getInputAction.html">getInputAction</see> and <see cref="VR%20Simulator.CSharp~Immerseum.VRSimulator.InputActionManager~isActionRegistered.html">isActionRegistered</see> to check its contents.</para>
            ///     </innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A list of <see cref="InputAction" /> objects.</value>
            public static List<InputAction> inputActionList {
                get {
                    return Instance._inputActionList;
                }
            }

            /// <summary>Indicates the number of <see cref="InputAction">InputActions</see> on the <see cref="InputActionManager.inputActionList">inputActionList</see>.</summary>
            /// <value>An integer representing the number of <see cref="InputAction">InputActions</see> on the <see cref="InputActionManager.inputActionList">inputActionList</see>.</value>
            public static int inputActions {
                get {
                    return Instance._inputActionList.Count;
                }
            }

            protected bool _isCancelButtonActivated = false;
            /// <summary>Indicates whether the Cancel Button is in an activated state (i.e. has been toggled on).</summary>
            /// <value>
            ///   <strong>true</strong> if activated, <strong>false</strong> if not</value>
            public static bool isCancelButtonActivated {
                get {
                    return Instance._isCancelButtonActivated;
                }
            }
            /// <summary>Toggles the activated state of the Cancel Button.</summary>
            /// <param name="value">
            ///   <strong>true</strong> to indicate activated, <strong>false</strong> to indicate inactive.</param>
            public static void setCancelButtonActivated(bool value) {
                Instance._isCancelButtonActivated = value;
            }

            protected bool _isLeftBumperActivated = false;
            /// <summary>Indicates whether the Left Bumper is in an activated state (i.e. has been toggled on).</summary>
            /// <value>
            ///   <strong>true</strong> if activated, <strong>false</strong> if not</value>
            public static bool isLeftBumperActivated {
                get {
                    return Instance._isLeftBumperActivated;
                }
            }
            /// <summary>Toggles the activated state of the Left Bumper.</summary>
            /// <param name="value">
            ///   <strong>true</strong> to indicate activated, <strong>false</strong> to indicate inactive.</param>
            public static void setLeftBumperActivated(bool value) {
                Instance._isLeftBumperActivated = value;
            }

            protected bool _isRightBumperActivated = false;
            /// <summary>Indicates whether the Right Bumper is in an activated state (i.e. has been toggled on).</summary>
            /// <value>
            ///   <strong>true</strong> if activated, <strong>false</strong> if not</value>
            public static bool isRightBumperActivated {
                get {
                    return Instance._isRightBumperActivated;
                }
            }
            /// <summary>Toggles the activated state of the Right Bumper.</summary>
            /// <param name="value">
            ///   <strong>true</strong> to indicate activated, <strong>false</strong> to indicate inactive.</param>
            public static void setRightBumperActivated(bool value) {
                Instance._isRightBumperActivated = value;
            }

            protected bool _isLeftThumbstickClickActivated = false;
            /// <summary>Indicates whether the Left Thumbstick Click is in an activated state (i.e. has been toggled on).</summary>
            /// <value>
            ///   <strong>true</strong> if activated, <strong>false</strong> if not</value>
            public static bool isLeftThumbstickClickActivated {
                get {
                    return Instance._isLeftThumbstickClickActivated;
                }
            }
            /// <summary>Toggles the activated state of the Left Thumbstick Click.</summary>
            /// <param name="value">
            ///   <strong>true</strong> to indicate activated, <strong>false</strong> to indicate inactive.</param>
            public static void setLeftThumbstickClickActivated(bool value) {
                Instance._isLeftThumbstickClickActivated = value;
            }

            protected bool _isRightThumbstickClickActivated = false;
            /// <summary>Indicates whether the Right Thumbstick Click is in an activated state (i.e. has been toggled on).</summary>
            /// <value>
            ///   <strong>true</strong> if activated, <strong>false</strong> if not</value>
            public static bool isRightThumbstickClickActivated {
                get {
                    return Instance._isRightThumbstickClickActivated;
                }
            }
            /// <summary>Toggles the activated state of the Right Thumbstick Click.</summary>
            /// <param name="value">
            ///   <strong>true</strong> to indicate activated, <strong>false</strong> to indicate inactive.</param>
            public static void setRightThumbstickClickActivated(bool value) {
                Instance._isRightThumbstickClickActivated = value;
            }

            protected bool _isTertiaryButtonActivated = false;
            /// <summary>Indicates whether the Tertiary Button is in an activated state (i.e. has been toggled on).</summary>
            /// <value>
            ///   <strong>true</strong> if activated, <strong>false</strong> if not</value>
            public static bool isTertiaryButtonActivated {
                get {
                    return Instance._isTertiaryButtonActivated;
                }
            }
            /// <summary>Toggles the activated state of the Tertiary Button.</summary>
            /// <param name="value">
            ///   <strong>true</strong> to indicate activated, <strong>false</strong> to indicate inactive.</param>
            public static void setTertiaryButtonActivated(bool value) {
                Instance._isTertiaryButtonActivated = value;
            }

            protected bool _isViewButtonActivated = false;
            /// <summary>Indicates whether the View Button is in an activated state (i.e. has been toggled on).</summary>
            /// <value>
            ///   <strong>true</strong> if activated, <strong>false</strong> if not</value>
            public static bool isViewButtonActivated {
                get {
                    return Instance._isViewButtonActivated;
                }
            }
            /// <summary>Toggles the activated state of the View Button.</summary>
            /// <param name="value">
            ///   <strong>true</strong> to indicate activated, <strong>false</strong> to indicate inactive.</param>
            public static void setViewButtonActivated(bool value) {
                Instance._isViewButtonActivated = value;
            }

            protected bool _isSelectionButtonActivated = false;
            /// <summary>Indicates whether the Selection Button is in an activated state (i.e. has been toggled on).</summary>
            /// <value>
            ///   <strong>true</strong> if activated, <strong>false</strong> if not</value>
            public static bool isSelectionButtonActivated {
                get {
                    return Instance._isSelectionButtonActivated;
                }
            }
            /// <summary>Toggles the activated state of the Selection Button.</summary>
            /// <param name="value">
            ///   <strong>true</strong> to indicate activated, <strong>false</strong> to indicate inactive.</param>
            public static void setSelectionButtonActivated(bool value) {
                Instance._isSelectionButtonActivated = value;
            }

            protected bool _isSecondaryButtonActivated = false;
            /// <summary>Indicates whether the Secondary Button is in an activated state (i.e. has been toggled on).</summary>
            /// <value>
            ///   <strong>true</strong> if activated, <strong>false</strong> if not</value>
            public static bool isSecondaryButtonActivated {
                get {
                    return Instance._isSecondaryButtonActivated;
                }
            }
            /// <summary>Toggles the activated state of the Secondary Button.</summary>
            /// <param name="value">
            ///   <strong>true</strong> to indicate activated, <strong>false</strong> to indicate inactive.</param>
            public static void setSecondaryButtonActivated(bool value) {
                Instance._isSecondaryButtonActivated = value;
            }

            protected bool _isPauseButtonActivated = false;
            /// <summary>Indicates whether the Pause Button is in an activated state (i.e. has been toggled on).</summary>
            /// <value>
            ///   <strong>true</strong> if activated, <strong>false</strong> if not</value>
            public static bool isPauseButtonActivated {
                get {
                    return Instance._isPauseButtonActivated;
                }
            }
            /// <summary>Toggles the activated state of the Pause Button.</summary>
            /// <param name="value">
            ///   <strong>true</strong> to indicate activated, <strong>false</strong> to indicate inactive.</param>
            public static void setPauseButtonActivated(bool value) {
                Instance._isPauseButtonActivated = value;
            }


            /// <summary>Returns whether an <see cref="InputAction" /> whose <see cref="InputAction.name" /> matches <strong>name</strong> is registered (i.e. contained in <see cref="InputActionManager.inputActionList" />).</summary>
            /// <param name="name">A string corresponding to <see cref="InputAction.name" />.</param>
            /// <returns>
            ///   <strong>true</strong> if an <see cref="InputAction" /> with matching <see cref="InputAction.name">name</see> is present on
            /// the <see cref="InputActionManager.inputActionList">inputActionList</see>, <strong>false</strong> if not.</returns>
            /// <example>
            ///   <code title="Example" description="Checks whether &quot;myInputAction&quot; is registered (which it is), and then checks whether &quot;otherInputAction&quot; is registered (it is not)." lang="CS">
            /// InputAction myInputAction = new InputAction();
            /// myInputAction.name = "myInputAction";
            ///  
            /// InputAction otherInputAction = newInputAction();
            /// otherInputAction.name = "otherInputAction";
            ///  
            /// InputActionManager.addInputAction(myInputAction);
            ///  
            /// bool isMyInputActionRegistered = InputActionManager.isActionRegistered("myInputAction");
            /// // Returns true.
            ///  
            /// bool isOtherInputActionRegistered = InputActionManager.isActionRegistered("otherInputAction");
            /// // Returns false.</code>
            /// </example>
            public static bool isActionRegistered (string name) {
                int n = inputActionList.Count;
                for (int x = 0; x < n; x++) {
                    if (inputActionList[x].name == name) {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>Adds an <see cref="InputAction" /> to the managed list. This is equivalent to "teaching" the system a new <see cref="InputAction" />.</summary>
            /// <param name="action">
            ///   <para>A properly defined <see cref="InputAction" />.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">The <strong><see cref="InputAction.name" /></strong> property must be unique - if it is not, the method will
            ///     return <strong>false</strong>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </param>
            /// <returns>
            ///   <strong>true</strong> if the <see cref="InputAction" /> was successfully added to <see cref="InputActionManager.inputActionList" />, <strong>false</strong> if not.</returns>
            /// <example>
            ///   <code title="Example" description="Registers the %InputAction% &quot;myInputAction&quot; with the InputActionManager." lang="CS">
            /// InputAction actionToRegister = new InputAction();
            /// actionToRegister.name = "myInputAction";
            /// // Other code to configure the InputAction goes here.
            ///  
            /// bool success = InputActionManager.addInputAction(actionToRegister);</code>
            /// </example>
            public static bool addInputAction (InputAction action) {
                if (isActionRegistered(action.name) == false) {
                    Instance._inputActionList.Add(action);
                    return true;
                }
                return false;
            }

            /// <summary>Removes the <see cref="InputAction" /> provided from the list of registered <see cref="InputAction">InputActions</see> contained in <see cref="InputActionManager.inputActionList">inputActionList</see>.</summary>
            /// <param name="action">The <see cref="InputAction" /> to remove.</param>
            /// <returns>
            ///   <strong>true</strong> if the <see cref="InputAction" /> was found and removed successfully, <strong>false</strong> if it not.</returns>
            /// <example>
            ///   <code title="Example" description="Removes InputAction &quot;myInputAction&quot; from the %inputActionList:InputActionManager.inputActionList%." lang="CS">
            /// InputAction myInputAction;
            ///  
            /// bool wasRemoved = removeInputAction(myInputAction);</code>
            /// </example>
            public static bool removeInputAction (InputAction action) {
                int n = inputActionList.Count;
                for (int x = 0; x < n; x++) {
                    if (inputActionList[x].name == action.name) {
                        inputActionList.RemoveAt(x);
                        return true;
                    }
                }
                return false;
            }
            /// <summary>Removes the <see cref="InputAction" /> at the list index provided from the <see cref="InputActionManager.inputActionList">inputActionList</see>.</summary>
            /// <param name="x">The index position that should be removed from the <see cref="InputActionManager.inputActionList">inputActionList</see>.</param>
            /// <returns>
            ///   <strong>true</strong> if successful, <strong>false</strong> if not.</returns>
            /// <example>
            ///   <code title="Example" description="Removes the %InputAction% from index position 2 in the %inputActionList:InputActionManager.inputActionList%." lang="CS">
            /// bool wasRemoved = removeInputAction(2);</code>
            /// </example>
            public static bool removeInputAction (int x) {
                if (x <= inputActionList.Capacity && x >= 0) {
                    inputActionList.RemoveAt(x);
                    return true;
                }
                return false;
            }
            /// <summary>Removes the <see cref="InputAction" /> with the <strong>name</strong> provided from the <see cref="InputActionManager.inputActionList">inputActionlist</see>.</summary>
            /// <param name="name">The string <strong>name</strong> property of the <see cref="InputAction" /> to remove.</param>
            /// <returns>
            ///   <strong>true</strong> if successful, <strong>false</strong> if not.</returns>
            /// <example>
            ///   <code title="Example" description="Removes the InputAction with name &quot;myInputAction&quot; from the %inputActionList:InputActionManager.inputActionList%" lang="CS">
            /// bool wasRemoved = InputActionManager.removeInputAction("myInputAction");</code>
            /// </example>
            public static bool removeInputAction (string name) {
                int n = inputActionList.Count;
                for (int x = 0; x < n; x++) {
                    if (inputActionList[x].name == name) {
                        inputActionList.RemoveAt(x);
                        return true;
                    }
                }
                return false;
            }

            /// <summary>Indicates whether the input axes used by Immerseum's default Input Actions are defined in the project's <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Input Manager</see>.</summary>
            /// <value>
            ///   <strong>true</strong> if Immerseum's set of input axes are defined, otherwise <strong>false</strong>.</value>
            public static bool areImmerseumAxesDefined {
                get {
                    if (areInputAxesDefined && areInputActionsInitialized) {
                        return true;
                    }
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
#if UNITY_EDITOR
                        if (isAxisDefined(axisNameList[x]) == false) {
                            Instance._areInputAxesDefined = false;
                            return false;
                        }
#else
                        try {
                            float axisValue = Input.GetAxis(axisNameList[x]);
                        } catch {
                            Instance._areInputActionsInitialized = false;
                            return false;
                        }
#endif
                    }
                    Instance._areInputAxesDefined = true;
                    return true;
                }
            }

            private static bool isAxisDefined (string axisName) {
#if UNITY_EDITOR
                _serializedInputManager = loadSerializedInputManager();
                UnityEditor.SerializedProperty axesProperty = _serializedInputManager.FindProperty("m_Axes");

                axesProperty.Next(true);
                axesProperty.Next(true);
                while (axesProperty.Next(false)) {
                    UnityEditor.SerializedProperty axis = axesProperty.Copy();
                    axis.Next(true);
                    if (axis.stringValue == axisName) {
                        return true;
                    }
                }
                return false;
#else
                if (HMDSimulator.logDiagnostics) {
                    Debug.LogWarning("[ImmerseumSDK] Attempted to access InputManager settings outside of the Unity Editor.");
                }
                try {
                    float axisValue = Input.GetAxis(axisName);
                    return true;
                } catch {
                    return false;
                }                
                return false;
#endif
            }


            void OnEnable () {
                EventManager.OnInitializeInputActions += OnInitializeInputActions;
                EventManager.OnInitializeInputActionsEnd += OnInitializeInputActionsEnd;
            }

            void OnDisable () {
                EventManager.OnInitializeInputActions -= OnInitializeInputActions;
                EventManager.OnInitializeInputActionsEnd -= OnInitializeInputActionsEnd;
            }

            void setupInputManager () {
#if UNITY_EDITOR
                _serializedTemplate = loadSerializedTemplate("Assets/_ImmerseumSDK/Resources/InputManagerTemplate.dat");
                UnityEditor.SerializedProperty templateAxesProperty = _serializedTemplate.FindProperty("m_Axes");

                _serializedInputManager = loadSerializedInputManager();
                UnityEditor.SerializedProperty axesProperty = _serializedInputManager.FindProperty("m_Axes");

                axesProperty.arraySize = templateAxesProperty.arraySize;

                _serializedInputManager.CopyFromSerializedProperty(templateAxesProperty);

                _serializedInputManager.ApplyModifiedProperties();
#else
                if (HMDSimulator.logDiagnostics) {
                    Debug.LogError("[ImmerseumSDK] Attempted to setup the InputManager outside of the Unity Editor.");
                }
#endif

                if (createImmerseumDefaults) {
                    createDefaultInputActions();
                }
            }

            void OnInitializeInputActions () {
                if (createImmerseumDefaults) {
                    if (areImmerseumAxesDefined) {
                        if (HMDSimulator.logDiagnostics) {
                            Debug.Log("[ImmerseumSDK] InputManager settings support default Input Actions.");
                        }

                        createDefaultInputActions();
                    } else {
#if UNITY_EDITOR
                        if (UnityEditor.EditorUtility.DisplayDialog("InputManager Settings", "You're using Immerseum's default Input Actions, but your InputManager settings don't support them. Would you like to use Immerseum's default InputManager Settings instead? (since you are in Play Mode, these settings will be temporary)", "Yes, update InputManager.", "No, disable Input Actions.")) {
                            setupInputManager();
                        } else {
                            Debug.LogError("[ImmerseumSDK] Your project's InputManager settings do not support Immerseum's default Input Actions. Therefore, to prevent errors, we are disabling Immerseum's default Input Actions.");
                            Instance._createImmerseumDefaults = false;
                        }
#else
                        Debug.LogError("[ImmerseumSDK] Your project's InputManager settings do not support Immerseum's default Input Actions. Therefore, to prevent errors, we are disabling Immerseum's default Input Actions.");
                        Instance._createImmerseumDefaults = false;
#endif
                    }
                } else {
                    EventManager.endInitializeInputActions();
                }
            }

            void OnInitializeInputActionsEnd () {
                Instance._areInputActionsInitialized = true;
            }

            /// <summary>Indicates whether the key provided has been $$pressed$$ during the frame.</summary>
            /// <overloads>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Can also accept an array or comma-delimited list of Unity
            ///     <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>. Will return <strong>true</strong> if one or more of the keys was
            ///     $$pressed$$.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </overloads>
            /// <param name="key">The Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</see> to check for. Can also accept an array or comma-delimited list of Unity
            /// <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>.</param>
            /// <returns>
            ///   <strong>true</strong> if one or more of the keys indicated was $$pressed$$, <strong>false</strong> if not.</returns>
            /// <example>
            ///   <innovasys:widget layout="block" type="Authoring Note" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">TODO: Add example for Single Key and Multiple Key</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </example>
            public static bool isKeyPressed (KeyCode key) {
                if (Input.GetKeyDown(key)) {
                    return true;
                } else {
                    return false;
                }

            }
            public static bool isKeyPressed (params KeyCode[] key) {
                bool _keyPressed = false;
                int keys = key.Length;

                for (int x = 0; x < keys; x++) {
                    if (Input.GetKeyDown(key[x])) {
                        _keyPressed = true;
                    }
                }
                return _keyPressed;
            }

            /// <summary>Indicates whether the key provided has been $$held$$ down during the frame.</summary>
            /// <overloads>
            ///   <innovasys:widget layout="block" type="Note Box" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Can also accept an array or comma-delimited list of Unity
            ///     <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>. Will return <strong>true</strong> if one or more of the keys was
            ///     $$held$$.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </overloads>
            /// <param name="key">The Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</see> to check for. Can also accept an array or comma-delimited list of Unity
            /// <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>.</param>
            /// <returns>
            ///   <strong>true</strong> if one or more of the keys indicated was $$held$$, <strong>false</strong> if not.</returns>
            /// <example>
            ///   <innovasys:widget type="Authoring Note" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">TODO: Add example for Single Key and Multiple Key</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </example>
            public static bool isKeyHeld (KeyCode key) {
                if (Input.GetKey(key)) {
                    return true;
                }
                return false;
            }
            public static bool isKeyHeld (params KeyCode[] key) {
                bool _keyHeld = false;
                int keys = key.Length;

                for (int x = 0; x < keys; x++) {
                    if (Input.GetKey(key[x])) {
                        _keyHeld = true;
                    }
                }
                return _keyHeld;
            }

            /// <summary>Indicates whether the key provided has been $$pressed$$ this frame and then $$held$$ throughout.</summary>
            /// <overloads>
            ///   <innovasys:widget layout="block" type="Note Box" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Can also accept an array or comma-delimited list of Unity
            ///     <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>. Will return <strong>true</strong> if one or more of the keys was $$pressed$$ and
            ///     $$held$$.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </overloads>
            /// <param name="key">The Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</see> to check for. Can also accept an array or comma-delimited list of Unity
            /// <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>.</param>
            /// <returns>
            ///   <strong>true</strong> if one or more of the keys indicated was $$pressed$$ and $$held$$, <strong>false</strong> if not.</returns>
            /// <example>
            ///   <innovasys:widget type="Authoring Note" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">TODO: Add example for Single Key and Multiple Key</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </example>
            public static bool isKeyPressedAndHeld (KeyCode key) {
                if (isKeyHeld(key) == true && isKeyPressed(key) == true) {
                    return true;
                }
                return false;
            }
            public static bool isKeyPressedAndHeld (params KeyCode[] keys) {
                if (isKeyHeld(keys) == true && isKeyPressed(keys) == true) {
                    return true;
                }
                return false;
            }

            /// <summary>Indicates if the key provided has been $$released$$ this frame.</summary>
            /// <overloads>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Can also accept an array or comma-delimited list of Unity
            ///     <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>. Will return <strong>true</strong> if one or more of the keys was
            ///     $$released$$.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </overloads>
            /// <param name="key">The Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</see> to check for. Can also accept an array or comma-delimited list of Unity
            /// <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>.</param>
            /// <returns>
            ///   <strong>true</strong> if one or more of the keys indicated was $$released$$, <strong>false</strong> if not.</returns>
            /// <example>
            ///   <innovasys:widget layout="block" type="Authoring Note" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">TODO: Add example for Single Key and Multiple Key</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </example>
            public static bool isKeyReleased (KeyCode key) {
                if (Input.GetKeyUp(key)) {
                    return true;
                }
                return false;
            }
            public static bool isKeyReleased (params KeyCode[] keys) {
                bool _keyReleased = false;
                int numberOfKeys = keys.Length;

                for (int x = 0; x < numberOfKeys; x++) {
                    if (Input.GetKeyUp(keys[x])) {
                        _keyReleased = true;
                    }
                }
                return _keyReleased;
            }

            /// <summary>
            ///   <para>Indicates whether the key indicated has been clicked (pushed down and released within the same frame).</para>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <strong>Be careful!</strong> It is very rare for a key to actually be $$clicked$$ since it is very
            ///     hard to push down and release a key in the space of one frame. You might want to consider <see cref="InputActionManager.isKeyPressed">isKeyPressed</see>
            ///     instead.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <overloads>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Can also accept an array or comma-delimited list of Unity
            ///     <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>. Will return <strong>true</strong> if one or more of the keys was
            ///     $$clicked$$.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </overloads>
            /// <param name="key">The Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCode</see> to check for. Can also accept an array or comma-delimited list of Unity
            /// <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>.</param>
            /// <returns>
            ///   <strong>true</strong> if one or more of the keys indicated was $$clicked$$, <strong>false</strong> if not.</returns>
            /// <example>
            ///   <innovasys:widget layout="block" type="Authoring Note" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">TODO: Add example for Single Key and Multiple Key</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </example>
            public static bool isKeyClicked (KeyCode key) {
                if (isKeyPressed(key) == true && isKeyReleased(key) == true) {
                    return true;
                }
                return false;
            }
            public static bool isKeyClicked (params KeyCode[] keys) {
                if (isKeyPressed(keys) == true && isKeyReleased(keys) == true) {
                    return true;
                }
                return false;
            }

            /// <overloads>
            ///   <innovasys:widget layout="block" type="Note Box" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Can also accept an array or comma-delimited list of integers for buttons. Will return
            ///     <strong>true</strong> if one or more of the buttons was $$clicked$$.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </overloads>
            /// <summary>Indicates whether the mouse button provided was $$pressed$$.</summary>
            /// <param name="button">
            ///   <para>An integer indicating the Mouse Button to check for. Accepts:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>0</strong> for the left-hand mouse button.</item>
            ///     <item>
            ///       <strong>1</strong> for the right-hand mouse button.</item>
            ///     <item>
            ///       <strong>2</strong> for the center mouse button (not all mice have this).</item>
            ///   </list>
            /// </param>
            /// <returns>
            ///   <strong>true</strong> if the button was $$pressed$$, <strong>false</strong> if not.</returns>
            /// <example>
            ///   <innovasys:widget type="Authoring Note" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">TODO: Add example for Single Key and Multiple Key</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </example>
            public static bool isMousePressed (int button) {
                if (Input.GetMouseButtonDown(button)) {
                    return true;
                }
                return false;
            }
            public static bool isMousePressed (params int[] buttons) {
                bool _isPressed = false;
                int numberOfButtons = buttons.Length;

                for (int x = 0; x < numberOfButtons; x++) {
                    if (Input.GetMouseButtonDown(buttons[x])) {
                        _isPressed = true;
                    }
                }
                return _isPressed;
            }

            /// <summary>Indicates whether the mouse button provided was $$held$$.</summary>
            /// <overloads>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Can also accept an array or comma-delimited list of integers for buttons. Will return
            ///     <strong>true</strong> if one or more of the buttons was $$held$$.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </overloads>
            /// <param name="button">
            ///   <para>An integer indicating the Mouse Button to check for. Accepts:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>0</strong> for the left-hand mouse button.</item>
            ///     <item>
            ///       <strong>1</strong> for the right-hand mouse button.</item>
            ///     <item>
            ///       <strong>2</strong> for the center mouse button (not all mice have this).</item>
            ///   </list>
            /// </param>
            /// <returns>
            ///   <strong>true</strong> if the button was $$held$$, <strong>false</strong> if not.</returns>
            /// <example>
            ///   <innovasys:widget layout="block" type="Authoring Note" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">TODO: Add example for Single Key and Multiple Key</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </example>
            public static bool isMouseHeld (int button) {
                if (Input.GetMouseButton(button)) {
                    return true;
                }
                return false;
            }
            public static bool isMouseHeld (params int[] buttons) {
                bool _isHeld = false;
                int numberOfButtons = buttons.Length;

                for (int x = 0; x < numberOfButtons; x++) {
                    if (Input.GetMouseButton(buttons[x])) {
                        _isHeld = true;
                    }
                }
                return _isHeld;
            }

            /// <overloads>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Can also accept an array or comma-delimited list of integers for buttons. Will return
            ///     <strong>true</strong> if one or more of the buttons was $$clicked$$.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </overloads>
            /// <summary>Indicates whether the mouse button provided was $$pressed$$ and then $$held$$ throughout the frame.</summary>
            /// <param name="button">
            ///   <para>An integer indicating the Mouse Button to check for. Accepts:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>0</strong> for the left-hand mouse button.</item>
            ///     <item>
            ///       <strong>1</strong> for the right-hand mouse button.</item>
            ///     <item>
            ///       <strong>2</strong> for the center mouse button (not all mice have this).</item>
            ///   </list>
            /// </param>
            /// <returns>
            ///   <strong>true</strong> if the button was $$pressed$$ and $$held$$, <strong>false</strong> if not.</returns>
            /// <example>
            ///   <innovasys:widget layout="block" type="Authoring Note" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">TODO: Add example for Single Key and Multiple Key</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </example>
            public static bool isMousePressedAndHeld (int button) {
                if (isMousePressed(button) == true && isMouseHeld(button) == true) {
                    return true;
                }
                return false;
            }
            public static bool isMousePressedAndHeld (params int[] buttons) {
                if (isMousePressed(buttons) == true && isMouseHeld(buttons) == true) {
                    return true;
                }
                return false;
            }

            /// <overloads>
            ///   <innovasys:widget layout="block" type="Note Box" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Can also accept an array or comma-delimited list of integers for buttons. Will return
            ///     <strong>true</strong> if one or more of the buttons was $$clicked$$.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </overloads>
            /// <summary>Indicates whether the mouse button provided was $$released$$.</summary>
            /// <param name="button">
            ///   <para>An integer indicating the Mouse Button to check for. Accepts:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>0</strong> for the left-hand mouse button.</item>
            ///     <item>
            ///       <strong>1</strong> for the right-hand mouse button.</item>
            ///     <item>
            ///       <strong>2</strong> for the center mouse button (not all mice have this).</item>
            ///   </list>
            /// </param>
            /// <returns>
            ///   <strong>true</strong> if the button was $$released$$, <strong>false</strong> if not.</returns>
            /// <example>
            ///   <innovasys:widget type="Authoring Note" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">TODO: Add example for Single Key and Multiple Key</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </example>
            public static bool isMouseReleased (int button) {
                if (Input.GetMouseButtonUp(button)) {
                    return true;
                }
                return false;
            }
            public static bool isMouseReleased (params int[] buttons) {
                bool _isReleased = false;
                int numberOfButtons = buttons.Length;

                for (int x = 0; x < numberOfButtons; x++) {
                    if (Input.GetMouseButtonUp(buttons[x])) {
                        _isReleased = true;
                    }
                }
                return _isReleased;
            }

            /// <summary>Indicates whether the mouse button provided was $$clicked$$.</summary>
            /// <overloads>
            ///   <innovasys:widget layout="block" type="Note Box" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Can also accept an array or comma-delimited list of integers for buttons. Will return
            ///     <strong>true</strong> if one or more of the buttons was $$clicked$$.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </overloads>
            /// <param name="button">
            ///   <para>An integer indicating the Mouse Button to check for. Accepts:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>0</strong> for the left-hand mouse button.</item>
            ///     <item>
            ///       <strong>1</strong> for the right-hand mouse button.</item>
            ///     <item>
            ///       <strong>2</strong> for the center mouse button (not all mice have this).</item>
            ///   </list>
            /// </param>
            /// <returns>
            ///   <strong>true</strong> if the button was $$clicked$$, <strong>false</strong> if not.</returns>
            /// <example>
            ///   <innovasys:widget type="Authoring Note" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">TODO: Add example for Single Key and Multiple Key</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </example>
            public static bool isMouseClicked (int button) {
                if (isMousePressed(button) == true && isMouseReleased(button) == true) {
                    return true;
                }
                return false;
            }
            public static bool isMouseClicked (params int[] buttons) {
                if (isMousePressed(buttons) == true && isMouseReleased(buttons) == true) {
                    return true;
                }
                return false;
            }



            void createDefaultInputActions () {
                // Gamepad: Start
                // Keyboard: Escape
                InputAction togglePauseMenu = createTogglePauseMenuAction();
                togglePauseMenu.registerAction();

                // Gamepad: Back
                // Keyboard: Tab
                InputAction toggleView = createToggleView();
                toggleView.registerAction();

                // Gamepad: Right/Left Trigger based on MovementManager configuration.
                // Keyboard: Left Control
                // Mouse: Left Mouse Button
                InputAction togglePrimaryTrigger = createToggleTrigger();
                togglePrimaryTrigger.registerAction();

                // Gamepad: Right/Left Trigger based on MovementManager cnofiguration.
                // Keyboard: Left Alt
                // Mouse: Right Mouse Button
                InputAction toggleSecondaryTrigger = createToggleTrigger("toggleSecondaryTrigger", true);
                toggleSecondaryTrigger.registerAction();

                // Gamepad: RightThumbstick
                // Keyboard: F, Numpad5
                // Mouse: Center Button (2)
                InputAction toggleRightThumbstickClick = createToggleRightThumbstickClick();
                toggleRightThumbstickClick.registerAction();

                // Gamepad: LeftThumbstick
                // Keyboard: C, Left Shift
                InputAction toggleLeftThumbstickClick = createToggleLeftThumbstickClick();
                toggleLeftThumbstickClick.registerAction();

                // Gamepad: Right Bumper
                // Keyboard: E, Numpad9
                InputAction toggleRightBumper = createToggleRightBumper();
                toggleRightBumper.registerAction();

                // Gamepad: Left Bumper
                // Keyboard: Q, Numpad7
                InputAction toggleLeftBumper = createToggleLeftBumper();
                toggleLeftBumper.registerAction();

                // Gamepad: A
                // Keyboard: Enter, NumpadEnter, Space
                InputAction toggleSelectionButton = createToggleSelectionButton();
                toggleSelectionButton.registerAction();

                // Gamepad: B
                // Keyboard: X, NumpadPlus, Backspace
                InputAction toggleCancelButton = createToggleCancelButton();
                toggleCancelButton.registerAction();

                // Gamepad: X
                // Keyboard: Page Up, RightShift, Numpad0, CapsLock
                InputAction toggleSecondaryButton = createToggleSecondaryButton();
                toggleSecondaryButton.registerAction();

                //Gamepad: Y
                // Keyboard: Numpad/, 2, K
                InputAction toggleTertiaryButton = createToggleTertiaryButton();
                toggleTertiaryButton.registerAction();

                // Gamepad: Left Thumbstick X, D-Pad X
                // Keyboard: A, D, Left Arrow, Right Arrow, Numpad4, Numpad6
                InputAction xAxisMovement = createXAxisMovement();
                xAxisMovement.registerAction();

                // Gamepad: Left Thumbstick Y, D-Pad Y
                // Keyboard: W, S, Up Arrow, Down Arrow, Numpad8, Numpad 2
                InputAction zAxisMovement = createZAxisMovement();
                zAxisMovement.registerAction();

                // Gamepad: Right Thumstick Y
                // Mouse: Vertical
                InputAction pitchRotation = createPitchRotation();
                pitchRotation.registerAction();

                // Gamepad: Right Thumbstick X
                // Mouse: Horizontal
                InputAction yawRotation = createYawRotation();
                yawRotation.registerAction();

                EventManager.endInitializeInputActions();
            }

            void Awake () {
                if (Instance != null && Instance != this) {
                    Destroy(gameObject);
                }

                Instance = this;
            }

            // Use this for initialization
            void Start () {
                EventManager.initializeInputActions();
            }

            // Update is called once per frame
            void Update () {
                if (areInputActionsInitialized) {
                    int n = inputActions;
                    for (int x = 0; x < n; x++) {
                        if (inputActionList[x].hasFired) {
                            EventManager.startInputAction(inputActionList[x]);
                        }
                    }
                }
            }

            /// <summary>Returns the registered <see cref="InputAction" /> where <see cref="InputAction.name" /> matches the parameter string <strong>name</strong>.</summary>
            /// <param name="name">
            ///   <para>A string with the <strong>name</strong> of the <see cref="InputAction" /> to return.</para>
            /// </param>
            /// <returns>
            ///   <para>If an <see cref="InputAction" /> with a matching <see cref="InputAction.name" /> exists within <see cref="InputActionManager.inputActionList" />, returns that <strong><see cref="InputAction" /></strong>.</para>
            ///   <para>Otherwise, returns <strong>null</strong>.</para>
            /// </returns>
            /// <seealso cref="P:Immerseum.VRSimulator.InputActionManager.inputActionList">inputActionList</seealso>
            /// <example>
            ///   <code title="Example" description="Returns an %InputAction% named &quot;myInputAction&quot;." lang="CS">
            /// InputAction returnedAction = InputActionManager.getInputAction("myInputAction");</code>
            /// </example>
            public static InputAction getInputAction (string name) {
                int n = inputActions;
                for (int x = 0; x < n; x++) {
                    if (inputActionList[x].name == name) {
                        return inputActionList[x];
                    }
                }
                return null;
            }

            InputAction createTogglePauseMenuAction () {
                InputAction togglePauseMenu = new InputAction();
                togglePauseMenu.name = "togglePauseMenu";
                togglePauseMenu.internalDescription = "Fired when the user presses a pause button on an input device.";

                togglePauseMenu.pressedKeyList.Add(KeyCode.Escape);

                InputButton startButton = new InputButton();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                startButton.name = "Gamepad_Start_Windows";
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                startButton.name = "Gamepad_Start_MacOS";
#endif
                startButton.internalDescription = "The start button on the XBox One Controller.";

                togglePauseMenu.pressedButtonList.Add(startButton);


                SteamVRButton wandMenuButtonLeft = new SteamVRButton();
                wandMenuButtonLeft.name = "WandMenuButtonLeft";
                wandMenuButtonLeft.internalDescription = "The menu button on the left-hand HTC Wand.";
                wandMenuButtonLeft.buttonMask = SteamVR_Controller.ButtonMask.ApplicationMenu;
                wandMenuButtonLeft.deviceRelation = SteamVR_Controller.DeviceRelation.FarthestLeft;

                SteamVRButton wandMenuButtonRight = new SteamVRButton();
                wandMenuButtonRight.name = "WandMenuButtonRight";
                wandMenuButtonRight.internalDescription = "The menu button on the right-hand HTC wand.";
                wandMenuButtonRight.buttonMask = SteamVR_Controller.ButtonMask.ApplicationMenu;
                wandMenuButtonRight.deviceRelation = SteamVR_Controller.DeviceRelation.FarthestRight;

                togglePauseMenu.pressedButtonList.Add(wandMenuButtonLeft);
                togglePauseMenu.pressedButtonList.Add(wandMenuButtonRight);

                OculusTouchButton oculusRightThumbstick = new OculusTouchButton();
                oculusRightThumbstick.name = "oculusRightThumbstick";
                oculusRightThumbstick.internalDescription = "The right thumbstick click action.";
                oculusRightThumbstick.controllerMask = OVRInput.Controller.RTouch;
                oculusRightThumbstick.buttonMask = OVRInput.Touch.PrimaryThumbstick;

                togglePauseMenu.pressedButtonList.Add(oculusRightThumbstick);

                return togglePauseMenu;
            }

            InputAction createToggleView () {
                InputAction toggleView = new InputAction();
                toggleView.name = "toggleView";
                toggleView.internalDescription = "Fired when the user presses the View button on their gamepad or the Tab button on their keyboard.";

                toggleView.pressedKeyList.Add(KeyCode.Tab);

                InputButton viewButton = new InputButton();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                viewButton.name = "Gamepad_View_Windows";
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                viewButton.name = "Gamepad_View_MacOS";
#endif
                viewButton.internalDescription = "The View button on the XBox One Controller.";

                toggleView.pressedButtonList.Add(viewButton);

                return toggleView;
            }

            InputAction createToggleTrigger (string name = "togglePrimaryTrigger", bool flipFromPrimary = false) {
                InputAction togglePrimaryTrigger = new InputAction();
                togglePrimaryTrigger.name = name;
                togglePrimaryTrigger.internalDescription = "Fired when the trigger is pulled.";

                MouseButton mouseButton = new MouseButton();
                if (flipFromPrimary == false) {
                    togglePrimaryTrigger.pressedKeyList.Add(KeyCode.LeftControl);
                    togglePrimaryTrigger.pressedKeyList.Add(KeyCode.LeftCommand);

                    mouseButton.name = "Left Mouse Button";
                    mouseButton.buttonIndex = 0;
                } else {
                    togglePrimaryTrigger.pressedKeyList.Add(KeyCode.LeftAlt);

                    mouseButton.name = "Right Mouse Button";
                    mouseButton.buttonIndex = 1;
                }

                togglePrimaryTrigger.pressedButtonList.Add(mouseButton);

                SteamVRHairTrigger wandTriggerRight = new SteamVRHairTrigger();
                wandTriggerRight.name = "wandTriggerRight";
                if (flipFromPrimary == false) {
                    wandTriggerRight.deviceRelation = SteamVR_Controller.DeviceRelation.FarthestRight;
                } else {
                    wandTriggerRight.deviceRelation = SteamVR_Controller.DeviceRelation.FarthestLeft;
                }
                wandTriggerRight.buttonMask = SteamVR_Controller.ButtonMask.Trigger;

                togglePrimaryTrigger.pressedButtonList.Add(wandTriggerRight);

                OculusButton oculusRemoteButton = new OculusButton();
                oculusRemoteButton.name = "oculusRemoteOne";
                oculusRemoteButton.controllerMask = OVRInput.Controller.Remote;
                oculusRemoteButton.buttonMask = OVRInput.Button.One;

                togglePrimaryTrigger.pressedButtonList.Add(oculusRemoteButton);

                OculusInputAxis oculusTriggerAxisRight = new OculusInputAxis();
                oculusTriggerAxisRight.name = "rightIndexTrigger";
                oculusTriggerAxisRight.controllerMask = OVRInput.Controller.RTouch;
                oculusTriggerAxisRight.axis1D = OVRInput.Axis1D.PrimaryIndexTrigger;
                oculusTriggerAxisRight.rawAxis1D = OVRInput.RawAxis1D.RIndexTrigger;
                oculusTriggerAxisRight.valueAtRestList.Add(0f);
                oculusTriggerAxisRight.maximumValue = 1f;
                oculusTriggerAxisRight.minimumValue = 0f;
                oculusTriggerAxisRight.hasMaximumValue = true;
                oculusTriggerAxisRight.hasMinimumValue = true;

                OculusInputAxis oculusTriggerAxisLeft = new OculusInputAxis();
                oculusTriggerAxisLeft.name = "leftIndexTrigger";
                oculusTriggerAxisLeft.controllerMask = OVRInput.Controller.LTouch;
                oculusTriggerAxisLeft.axis1D = OVRInput.Axis1D.PrimaryIndexTrigger;
                oculusTriggerAxisLeft.rawAxis1D = OVRInput.RawAxis1D.LIndexTrigger;
                oculusTriggerAxisLeft.valueAtRestList.Add(0f);
                oculusTriggerAxisLeft.maximumValue = 1f;
                oculusTriggerAxisLeft.minimumValue = 0f;
                oculusTriggerAxisLeft.hasMaximumValue = true;
                oculusTriggerAxisLeft.hasMinimumValue = true;

                if (flipFromPrimary == false) {
                    togglePrimaryTrigger.rightTriggerAxisList.Add(oculusTriggerAxisRight);
                } else {
                    togglePrimaryTrigger.leftTriggerAxisList.Add(oculusTriggerAxisLeft);
                }

                InputAxis gamepadTriggerAxis = new VRSimulator.InputAxis();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                gamepadTriggerAxis.minimumValue = 0f;
                gamepadTriggerAxis.maximumValue = 1f;
                gamepadTriggerAxis.valueAtRestList.Add(0f);
                if (flipFromPrimary == false) {
                    if (MovementManager.primaryGamepadTrigger == Hands.Right) {
                        gamepadTriggerAxis.name = "Gamepad_RightTrigger_Windows";
                        togglePrimaryTrigger.rightTriggerAxisList.Add(gamepadTriggerAxis);
                    } else {
                        gamepadTriggerAxis.name = "Gamepad_LeftTrigger_Windows";
                        togglePrimaryTrigger.leftTriggerAxisList.Add(gamepadTriggerAxis);
                    }
                } else {
                    if (MovementManager.primaryGamepadTrigger == Hands.Right) {
                        gamepadTriggerAxis.name = "Gamepad_LeftTrigger_Windows";
                        togglePrimaryTrigger.leftTriggerAxisList.Add(gamepadTriggerAxis);
                    } else {
                        gamepadTriggerAxis.name = "Gamepad_RightTrigger_Windows";
                        togglePrimaryTrigger.rightTriggerAxisList.Add(gamepadTriggerAxis);
                    }
                }
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                gamepadTriggerAxis.minimumValue = -1f;
                gamepadTriggerAxis.maximumValue = 1f;
                gamepadTriggerAxis.valueAtRestList.Add(0f);
                gamepadTriggerAxis.valueAtRestList.Add(-1f);
                if (flipFromPrimary == false) {
                    if (MovementManager.primaryGamepadTrigger == Hands.Right) {
                        gamepadTriggerAxis.name = "Gamepad_RightTrigger_MacOS";
                        togglePrimaryTrigger.rightTriggerAxisList.Add(gamepadTriggerAxis);
                    } else {
                        gamepadTriggerAxis.name = "Gamepad_LeftTrigger_MacOS";
                        togglePrimaryTrigger.leftTriggerAxisList.Add(gamepadTriggerAxis);
                    }
                } else {
                    if (MovementManager.primaryGamepadTrigger == Hands.Right) {
                        gamepadTriggerAxis.name = "Gamepad_LeftTrigger_MacOS";
                        togglePrimaryTrigger.leftTriggerAxisList.Add(gamepadTriggerAxis);
                    } else {
                        gamepadTriggerAxis.name = "Gamepad_RightTrigger_MacOS";
                        togglePrimaryTrigger.rightTriggerAxisList.Add(gamepadTriggerAxis);
                    }
                }
#endif

                return togglePrimaryTrigger;
            }

            InputAction createToggleRightThumbstickClick () {
                InputAction toggleThumbstickClick = new InputAction();
                toggleThumbstickClick.name = "toggleRightThumbstickClick";
                toggleThumbstickClick.internalDescription = "Fired when the user presses down on the Right Thumbstick on their gamepad, or presses F or Numpad5 on their keyboard, or presses the center button on their mouse.";

                toggleThumbstickClick.pressedKeyList.Add(KeyCode.F);
                toggleThumbstickClick.pressedKeyList.Add(KeyCode.Keypad5);

                InputButton gamepadButton = new InputButton();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                gamepadButton.name = "Gamepad_RStickClick_Windows";
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                gamepadButton.name = "Gamepad_RStickClick_MacOS";
#endif
                toggleThumbstickClick.pressedButtonList.Add(gamepadButton);

                MouseButton mouseButton = new MouseButton();
                mouseButton.name = "Center Mouse Button";
                mouseButton.buttonIndex = 2;

                toggleThumbstickClick.pressedButtonList.Add(mouseButton);

                SteamVRButton wandRightGrip = new SteamVRButton();
                wandRightGrip.name = "WandRightGrip";
                wandRightGrip.internalDescription = "The grip button on the right-hand HTC wand.";
                wandRightGrip.buttonMask = SteamVR_Controller.ButtonMask.Grip;
                wandRightGrip.deviceRelation = SteamVR_Controller.DeviceRelation.FarthestRight;

                toggleThumbstickClick.pressedButtonList.Add(wandRightGrip);

                OculusTouchButton oculusRightThumbstick = new OculusTouchButton();
                oculusRightThumbstick.name = "oculusRightThumbstick";
                oculusRightThumbstick.internalDescription = "The right thumbstick click action.";
                oculusRightThumbstick.controllerMask = OVRInput.Controller.RTouch;
                oculusRightThumbstick.buttonMask = OVRInput.Touch.PrimaryThumbstick;

                toggleThumbstickClick.pressedButtonList.Add(oculusRightThumbstick);

                return toggleThumbstickClick;
            }

            InputAction createToggleLeftThumbstickClick () {
                InputAction toggleThumbstickClick = new InputAction();
                toggleThumbstickClick.name = "toggleLeftThumbstickClick";
                toggleThumbstickClick.internalDescription = "Fired when the user presses down on the Left Thumbstick on their gamepad, or presses C or Numpad9 on their keyboard.";

                toggleThumbstickClick.pressedKeyList.Add(KeyCode.C);
                toggleThumbstickClick.pressedKeyList.Add(KeyCode.LeftShift);

                InputButton gamepadButton = new InputButton();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                gamepadButton.name = "Gamepad_LStickClick_Windows";
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                gamepadButton.name = "Gamepad_LStickClick_MacOS";
#endif
                toggleThumbstickClick.pressedButtonList.Add(gamepadButton);

                SteamVRButton wandLeftGrip = new SteamVRButton();
                wandLeftGrip.name = "WandLeftGrip";
                wandLeftGrip.internalDescription = "The grip button on the Left-hand HTC wand.";
                wandLeftGrip.buttonMask = SteamVR_Controller.ButtonMask.Grip;
                wandLeftGrip.deviceRelation = SteamVR_Controller.DeviceRelation.FarthestLeft;

                toggleThumbstickClick.pressedButtonList.Add(wandLeftGrip);

                OculusTouchButton oculusLeftThumbstick = new OculusTouchButton();
                oculusLeftThumbstick.name = "oculusLeftThumbstick";
                oculusLeftThumbstick.internalDescription = "The Left thumbstick click action.";
                oculusLeftThumbstick.controllerMask = OVRInput.Controller.LTouch;
                oculusLeftThumbstick.buttonMask = OVRInput.Touch.PrimaryThumbstick;

                toggleThumbstickClick.pressedButtonList.Add(oculusLeftThumbstick);


                return toggleThumbstickClick;
            }

            InputAction createToggleRightBumper () {
                InputAction toggleBumper = new VRSimulator.InputAction();
                toggleBumper.name = "toggleRightBumper";
                toggleBumper.internalDescription = "Fired when the user presses the right bumper on their gamepad, or the E key on their keyboard or the 9 key on their numeric keypad.";

                toggleBumper.pressedKeyList.Add(KeyCode.E);
                toggleBumper.pressedKeyList.Add(KeyCode.Keypad9);

                InputButton gamepadButton = new VRSimulator.InputButton();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                gamepadButton.name = "Gamepad_RBumper_Windows";
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                gamepadButton.name = "Gamepad_RBumper_MacOS";
#endif
                toggleBumper.pressedButtonList.Add(gamepadButton);

                return toggleBumper;
            }

            InputAction createToggleLeftBumper () {
                InputAction toggleBumper = new VRSimulator.InputAction();
                toggleBumper.name = "toggleLeftBumper";
                toggleBumper.internalDescription = "Fired when the user presses the left bumper on their gamepad, or the Q key on their keyboard or the 7 key on their numeric keypad.";

                toggleBumper.pressedKeyList.Add(KeyCode.Q);
                toggleBumper.pressedKeyList.Add(KeyCode.Keypad7);

                InputButton gamepadButton = new VRSimulator.InputButton();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                gamepadButton.name = "Gamepad_LBumper_Windows";
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                gamepadButton.name = "Gamepad_LBumper_MacOS";
#endif
                toggleBumper.pressedButtonList.Add(gamepadButton);

                return toggleBumper;
            }

            InputAction createToggleSelectionButton () {
                InputAction toggleSelectionButton = new InputAction();
                toggleSelectionButton.name = "toggleSelectionButton";
                toggleSelectionButton.internalDescription = "Fired when the user presses the A button on their gamepad, or the Enter or Space keys on their keyboard.";

                toggleSelectionButton.pressedKeyList.Add(KeyCode.KeypadEnter);
                toggleSelectionButton.pressedKeyList.Add(KeyCode.Space);
                toggleSelectionButton.pressedKeyList.Add(KeyCode.Return);

                InputButton gamepadButton = new VRSimulator.InputButton();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                gamepadButton.name = "Gamepad_A_Windows";
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                gamepadButton.name = "Gamepad_A_MacOS";
#endif
                toggleSelectionButton.pressedButtonList.Add(gamepadButton);

                SteamVRButton touchpadButtonLeft = new SteamVRButton();
                touchpadButtonLeft.name = "touchpadButtonLeft";
                touchpadButtonLeft.buttonMask = SteamVR_Controller.ButtonMask.Touchpad;
                touchpadButtonLeft.deviceRelation = SteamVR_Controller.DeviceRelation.FarthestLeft;

                toggleSelectionButton.pressedButtonList.Add(touchpadButtonLeft);

                SteamVRButton touchpadButtonRight = new SteamVRButton();
                touchpadButtonRight.name = "touchpadButtonRight";
                touchpadButtonRight.buttonMask = SteamVR_Controller.ButtonMask.Touchpad;
                touchpadButtonRight.deviceRelation = SteamVR_Controller.DeviceRelation.FarthestRight;

                toggleSelectionButton.pressedButtonList.Add(touchpadButtonRight);

                OculusButton oculusButtonA = new OculusButton();
                oculusButtonA.name = "oculusButtonA";
                oculusButtonA.buttonMask = OVRInput.Button.One;
                oculusButtonA.controllerMask = OVRInput.Controller.Touch;
                oculusButtonA.rawButtonMask = OVRInput.RawButton.A;

                toggleSelectionButton.pressedButtonList.Add(oculusButtonA);
                return toggleSelectionButton;
            }

            InputAction createToggleCancelButton () {
                InputAction toggleCancelButton = new InputAction();
                toggleCancelButton.name = "toggleCancelButton";
                toggleCancelButton.internalDescription = "Fired when the user presses the B button on their gamepad, or the X, Plus, or Backspace keys on their keyboard.";

                toggleCancelButton.pressedKeyList.Add(KeyCode.X);
                toggleCancelButton.pressedKeyList.Add(KeyCode.KeypadPlus);
                toggleCancelButton.pressedKeyList.Add(KeyCode.Backspace);

                InputButton gamepadButton = new VRSimulator.InputButton();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                gamepadButton.name = "Gamepad_B_Windows";
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                gamepadButton.name = "Gamepad_B_MacOS";
#endif
                toggleCancelButton.pressedButtonList.Add(gamepadButton);

                OculusButton oculusRemoteCancel = new OculusButton();
                oculusRemoteCancel.name = "oculusRemoteCancel";
                oculusRemoteCancel.controllerMask = OVRInput.Controller.Remote;
                oculusRemoteCancel.buttonMask = OVRInput.Button.Back;
                oculusRemoteCancel.rawButtonMask = OVRInput.RawButton.Back;

                toggleCancelButton.pressedButtonList.Add(oculusRemoteCancel);

                OculusTouchButton oculusTouchCancel = new OculusTouchButton();
                oculusTouchCancel.name = "oculusTouchCancel";
                oculusTouchCancel.controllerMask = OVRInput.Controller.Touch;
                oculusTouchCancel.buttonMask = OVRInput.Touch.Two;
                oculusTouchCancel.rawButtonMask = OVRInput.RawTouch.B;

                toggleCancelButton.pressedButtonList.Add(oculusTouchCancel);

                return toggleCancelButton;
            }

            InputAction createToggleSecondaryButton () {
                InputAction toggleSecondaryButton = new InputAction();
                toggleSecondaryButton.name = "toggleSecondaryButton";
                toggleSecondaryButton.internalDescription = "Fired when the user presses the X button on their gamepad, or the Page Up, Right Shift, Keypad0, or CapsLock keys on their keyboard.";

                toggleSecondaryButton.pressedKeyList.Add(KeyCode.PageUp);
                toggleSecondaryButton.pressedKeyList.Add(KeyCode.Keypad0);
                toggleSecondaryButton.pressedKeyList.Add(KeyCode.RightShift);
                toggleSecondaryButton.pressedKeyList.Add(KeyCode.CapsLock);

                InputButton gamepadButton = new VRSimulator.InputButton();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                gamepadButton.name = "Gamepad_X_Windows";
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                gamepadButton.name = "Gamepad_X_MacOS";
#endif
                toggleSecondaryButton.pressedButtonList.Add(gamepadButton);

                OculusTouchButton oculusTouchSecondary = new OculusTouchButton();
                oculusTouchSecondary.name = "oculusTouchSecondary";
                oculusTouchSecondary.controllerMask = OVRInput.Controller.Touch;
                oculusTouchSecondary.buttonMask = OVRInput.Touch.Three;
                oculusTouchSecondary.rawButtonMask = OVRInput.RawTouch.X;

                toggleSecondaryButton.pressedButtonList.Add(oculusTouchSecondary);

                return toggleSecondaryButton;
            }

            InputAction createToggleTertiaryButton () {
                InputAction toggleTertiaryButton = new InputAction();
                toggleTertiaryButton.name = "toggleTertiaryButton";
                toggleTertiaryButton.internalDescription = "Fired when the user presses the Y button on their gamepad, or the Numpad / 2, or K keys on their keyboard.";

                toggleTertiaryButton.pressedKeyList.Add(KeyCode.KeypadDivide);
                toggleTertiaryButton.pressedKeyList.Add(KeyCode.Alpha2);
                toggleTertiaryButton.pressedKeyList.Add(KeyCode.K);

                InputButton gamepadButton = new VRSimulator.InputButton();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                gamepadButton.name = "Gamepad_Y_Windows";
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                gamepadButton.name = "Gamepad_Y_MacOS";
#endif
                toggleTertiaryButton.pressedButtonList.Add(gamepadButton);

                OculusTouchButton oculusTouchTertiary = new OculusTouchButton();
                oculusTouchTertiary.name = "oculusTouchTertiary";
                oculusTouchTertiary.controllerMask = OVRInput.Controller.Touch;
                oculusTouchTertiary.buttonMask = OVRInput.Touch.Four;
                oculusTouchTertiary.rawButtonMask = OVRInput.RawTouch.Y;

                toggleTertiaryButton.pressedButtonList.Add(oculusTouchTertiary);

                return toggleTertiaryButton;
            }

            InputAction createXAxisMovement () {
                InputAction xAxisMovement = new InputAction();
                xAxisMovement.name = "xAxisMovement";
                xAxisMovement.internalDescription = "Fired when the user moves the left thumbstick on the gamepad right or left, presses or holds A, D, Left, Right, Keypad4, Keypad6 on their keyboard.";

                if (MovementManager.isStrafeEnabled) {
                    KeyboardButton leftKeys = new KeyboardButton();
                    leftKeys.isNegativeValue = true;
                    leftKeys.keyList.Add(KeyCode.Keypad4);

                    xAxisMovement.heldButtonList.Add(leftKeys);

                    KeyboardButton rightKeys = new KeyboardButton();
                    rightKeys.isNegativeValue = false;
                    rightKeys.keyList.Add(KeyCode.Keypad6);

                    xAxisMovement.heldButtonList.Add(rightKeys);

                    InputAxis keyboardAxis = new VRSimulator.InputAxis();
                    keyboardAxis.hasMaximumValue = false;
                    keyboardAxis.hasMinimumValue = false;
                    keyboardAxis.valueAtRestList.Add(0f);
                    keyboardAxis.name = "Horizontal";

                    xAxisMovement.xAxisList.Add(keyboardAxis);

                    OculusButton remoteDpadLeft = new OculusButton();
                    remoteDpadLeft.isNegativeValue = true;
                    remoteDpadLeft.name = "remoteDpadLeft";
                    remoteDpadLeft.controllerMask = OVRInput.Controller.Remote;
                    remoteDpadLeft.buttonMask = OVRInput.Button.DpadLeft;
                    remoteDpadLeft.rawButtonMask = OVRInput.RawButton.DpadLeft;

                    xAxisMovement.heldButtonList.Add(remoteDpadLeft);

                    OculusButton remoteDpadRight = new OculusButton();
                    remoteDpadRight.name = "remoteDpadRight";
                    remoteDpadRight.controllerMask = OVRInput.Controller.Remote;
                    remoteDpadRight.buttonMask = OVRInput.Button.DpadRight;
                    remoteDpadRight.rawButtonMask = OVRInput.RawButton.DpadRight;

                    xAxisMovement.heldButtonList.Add(remoteDpadRight);

                }

                InputAxis gamepadStickAxis = new InputAxis();
                gamepadStickAxis.hasMaximumValue = false;
                gamepadStickAxis.hasMinimumValue = false;
                gamepadStickAxis.valueAtRestList.Add(0f);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                gamepadStickAxis.name = "Gamepad_LThumbstickX_Windows";
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                gamepadStickAxis.name = "Gamepad_LThumbstickX_MacOS";
#endif

                xAxisMovement.xAxisList.Add(gamepadStickAxis);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                InputAxis gamepadDpadAxis = new InputAxis();
                gamepadDpadAxis.name = "Gamepad_DPadX_Windows";
                gamepadDpadAxis.hasMaximumValue = false;
                gamepadDpadAxis.hasMinimumValue = false;
                gamepadDpadAxis.valueAtRestList.Add(0f);

                xAxisMovement.xAxisList.Add(gamepadDpadAxis);
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                InputButton gamepadDpadLeftButton = new InputButton();
                gamepadDpadLeftButton.name = "Gamepad_DPadLeft_MacOS";

                xAxisMovement.heldButtonList.Add(gamepadDpadLeftButton);

                InputButton gamepadDpadRightButton = new InputButton();
                gamepadDpadRightButton.name = "Gamepad_DPadRight_MacOS";

                xAxisMovement.heldButtonList.Add(gamepadDpadRightButton);
#endif

                OculusInputAxis leftTouchX = new OculusInputAxis();
                leftTouchX.name = "leftTouchX";
                leftTouchX.returnX = true;
                leftTouchX.controllerMask = OVRInput.Controller.LTouch;
                leftTouchX.axis2D = OVRInput.Axis2D.PrimaryThumbstick;
                leftTouchX.rawAxis2D = OVRInput.RawAxis2D.LThumbstick;
                leftTouchX.valueAtRestList.Add(0f);

                xAxisMovement.xAxisList.Add(leftTouchX);

                return xAxisMovement;
            }

            InputAction createZAxisMovement () {
                InputAction zAxisMovement = new InputAction();
                zAxisMovement.name = "zAxisMovement";
                zAxisMovement.internalDescription = "Fired when the user moves the Left thumbstick on the gamepad Up or Down, presses or holds W, S, Down, Up, Keypad8, Keypad2 on their keyboard.";

                KeyboardButton forwardKeys = new KeyboardButton();
                forwardKeys.isNegativeValue = false;
                forwardKeys.keyList.Add(KeyCode.Keypad8);

                zAxisMovement.heldButtonList.Add(forwardKeys);

                KeyboardButton backwardKeys = new KeyboardButton();
                backwardKeys.isNegativeValue = true;
                backwardKeys.keyList.Add(KeyCode.Keypad2);

                zAxisMovement.heldButtonList.Add(backwardKeys);

                InputAxis gamepadStickAxis = new InputAxis();
                gamepadStickAxis.name = "Vertical";
                gamepadStickAxis.hasMaximumValue = false;
                gamepadStickAxis.hasMinimumValue = false;
                gamepadStickAxis.valueAtRestList.Add(0f);

                zAxisMovement.zAxisList.Add(gamepadStickAxis);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                InputAxis gamepadDpadAxis = new InputAxis();
                gamepadDpadAxis.name = "Gamepad_DPadY_Windows";
                gamepadDpadAxis.hasMaximumValue = false;
                gamepadDpadAxis.hasMinimumValue = false;
                gamepadDpadAxis.valueAtRestList.Add(0f);

                zAxisMovement.zAxisList.Add(gamepadDpadAxis);
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                InputButton gamepadDpadDownButton = new InputButton();
                gamepadDpadDownButton.name = "Gamepad_DPadDown_MacOS";

                zAxisMovement.heldButtonList.Add(gamepadDpadDownButton);

                InputButton gamepadDpadUpButton = new InputButton();
                gamepadDpadUpButton.name = "Gamepad_DPadUp_MacOS";

                zAxisMovement.heldButtonList.Add(gamepadDpadUpButton);
#endif

                OculusButton remoteDpadDown = new OculusButton();
                remoteDpadDown.isNegativeValue = true;
                remoteDpadDown.name = "remoteDpadDown";
                remoteDpadDown.controllerMask = OVRInput.Controller.Remote;
                remoteDpadDown.buttonMask = OVRInput.Button.DpadDown;
                remoteDpadDown.rawButtonMask = OVRInput.RawButton.DpadDown;

                zAxisMovement.heldButtonList.Add(remoteDpadDown);

                OculusButton remoteDpadUp = new OculusButton();
                remoteDpadUp.name = "remoteDpadUp";
                remoteDpadUp.controllerMask = OVRInput.Controller.Remote;
                remoteDpadUp.buttonMask = OVRInput.Button.DpadUp;
                remoteDpadUp.rawButtonMask = OVRInput.RawButton.DpadUp;

                zAxisMovement.heldButtonList.Add(remoteDpadUp);


                OculusInputAxis leftTouchZ = new OculusInputAxis();
                leftTouchZ.name = "leftTouchZ";
                leftTouchZ.returnX = false;
                leftTouchZ.controllerMask = OVRInput.Controller.LTouch;
                leftTouchZ.axis2D = OVRInput.Axis2D.PrimaryThumbstick;
                leftTouchZ.rawAxis2D = OVRInput.RawAxis2D.LThumbstick;
                leftTouchZ.valueAtRestList.Add(0f);


                zAxisMovement.zAxisList.Add(leftTouchZ);

                return zAxisMovement;
            }

            InputAction createPitchRotation () {
                InputAction pitchRotation = new InputAction();
                pitchRotation.name = "pitchRotation";
                pitchRotation.internalDescription = "Fired when the user moves the Right thumbstick on the gamepad Up or Down or moves their mouse vertically.";

                InputAxis gamepadAxis = new InputAxis();
                gamepadAxis.inputType = InputType.Gamepad;
                gamepadAxis.hasMaximumValue = false;
                gamepadAxis.hasMinimumValue = false;
                gamepadAxis.valueAtRestList.Add(0f);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                gamepadAxis.name = "Gamepad_RThumbstickY_Windows";
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                gamepadAxis.name = "Gamepad_RThumbstickY_MacOS";
#endif

                pitchRotation.pitchAxisList.Add(gamepadAxis);

                InputAxis mouseAxis = new InputAxis();
                mouseAxis.inputType = InputType.Mouse;
                mouseAxis.hasMaximumValue = false;
                mouseAxis.hasMinimumValue = false;
                mouseAxis.valueAtRestList.Add(0f);
                mouseAxis.name = "Mouse Y";

                pitchRotation.pitchAxisList.Add(mouseAxis);

                return pitchRotation;
            }

            InputAction createYawRotation () {
                InputAction yawRotation = new InputAction();
                yawRotation.name = "yawRotation";
                yawRotation.internalDescription = "Fired when the user moves the Right thumbstick on the gamepad Left or Right or moves their mouse horizontally.";

                InputAxis gamepadAxis = new InputAxis();
                gamepadAxis.inputType = InputType.Gamepad;
                gamepadAxis.hasMaximumValue = false;
                gamepadAxis.hasMinimumValue = false;
                gamepadAxis.valueAtRestList.Add(0f);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                gamepadAxis.name = "Gamepad_RThumbstickX_Windows";
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                gamepadAxis.name = "Gamepad_RThumbstickX_MacOS";
#endif

                yawRotation.yawAxisList.Add(gamepadAxis);

                InputAxis mouseAxis = new InputAxis();
                mouseAxis.inputType = InputType.Mouse;
                mouseAxis.hasMaximumValue = false;
                mouseAxis.hasMinimumValue = false;
                mouseAxis.valueAtRestList.Add(0f);
                mouseAxis.name = "Mouse X";

                yawRotation.yawAxisList.Add(mouseAxis);

                if (MovementManager.isStrafeEnabled == false) {
                    KeyboardButton leftKeys = new KeyboardButton();
                    leftKeys.isNegativeValue = true;
                    leftKeys.keyList.Add(KeyCode.Keypad4);

                    yawRotation.heldButtonList.Add(leftKeys);

                    KeyboardButton rightKeys = new KeyboardButton();
                    rightKeys.isNegativeValue = false;
                    rightKeys.keyList.Add(KeyCode.Keypad6);

                    yawRotation.heldButtonList.Add(rightKeys);

                    InputAxis keyboardAxis = new InputAxis();
                    keyboardAxis.inputType = InputType.Keyboard;
                    keyboardAxis.hasMaximumValue = false;
                    keyboardAxis.hasMinimumValue = false;
                    keyboardAxis.valueAtRestList.Add(0f);
                    keyboardAxis.name = "Horizontal";

                    yawRotation.yawAxisList.Add(keyboardAxis);

                    OculusButton remoteDpadLeft = new OculusButton();
                    remoteDpadLeft.isNegativeValue = true;
                    remoteDpadLeft.name = "remoteDpadLeft";
                    remoteDpadLeft.controllerMask = OVRInput.Controller.Remote;
                    remoteDpadLeft.buttonMask = OVRInput.Button.DpadLeft;
                    remoteDpadLeft.rawButtonMask = OVRInput.RawButton.DpadLeft;

                    yawRotation.heldButtonList.Add(remoteDpadLeft);

                    OculusButton remoteDpadRight = new OculusButton();
                    remoteDpadRight.name = "remoteDpadRight";
                    remoteDpadRight.controllerMask = OVRInput.Controller.Remote;
                    remoteDpadRight.buttonMask = OVRInput.Button.DpadRight;
                    remoteDpadRight.rawButtonMask = OVRInput.RawButton.DpadRight;

                    yawRotation.heldButtonList.Add(remoteDpadRight);
                }

                return yawRotation;
            }

        }
    }
}