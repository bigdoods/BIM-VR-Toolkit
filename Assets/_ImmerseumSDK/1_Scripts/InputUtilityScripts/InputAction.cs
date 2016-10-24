using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {

        /// <summary>Indicates an input device from which an input can be captured.</summary>
        public enum InputSources { /// <summary>Standard keyboard</summary>
            Keyboard, /// <summary>Standard mouse</summary>
            Mouse, /// <summary>Supported gamepads (e.g. XBox One Controller).</summary>
            Gamepad, /// <summary>HTC/Vive Wand (Steam VR Controller)</summary>
            Wand, /// <summary>Oculus Touch</summary>
            OculusTouch, /// <summary>Oculus Remote</summary>
            OculusRemote, /// <summary>Something else / unrecognized.</summary>
            Other
        }

        /// <summary>
        ///   <para>Represents one or more inputs from a user. If the InputAction is registered with the <see cref="InputActionManager" />, then when one or more of the InputAction's input
        /// methods is received, the InputAction will be fired (and any methods listening for it will be executed).</para>
        ///   <innovasys:widget type="Tip Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
        ///     <innovasys:widgetproperty layout="block" name="Content">
        ///       <para>A good way to think of an InputAction is like something the user can do. For example:</para>
        ///       <list type="bullet">
        ///         <item>the user can "move forward", or;</item>
        ///         <item>the user can "toggle the pause menu", or;</item>
        ///         <item>the user can "pull the primary trigger".</item>
        ///       </list>
        ///       <para>The user "does" each of these actions using their input devices (e.g. moving a thumbstick, pressing a button, pulling the trigger, etc.). An
        ///         InputAction defines the action the user can take and identifies the specific inputs which trigger that action.</para>
        ///     </innovasys:widgetproperty>
        ///   </innovasys:widget>
        ///   <para>The VRSimulator supports Immerseum's <see cref="!:Immerseum Default Input Actions">default set of InputActions</see>, which correspond to Immerseum's <see cref="!:Immerseum Movement Mapping">default Movement
        /// Mappings</see>. For more information on defining your own Input Actions and movement mappings, please see:</para>
        ///   <list type="bullet">
        ///     <item>
        ///       <see cref="Configuring Input &amp;amp; Movement">Configuring Input &amp; Movement</see>, and;</item>
        ///     <item>
        ///       <see cref="!:Custom Input Handling">Custom Input Handling</see>, and;</item>
        ///     <item>
        ///       <see cref="!:Custom InputAction Mapping">Custom InputAction Mapping</see>.</item>
        ///   </list>
        /// </summary>
        /// <seealso cref="!:Custom Input Handling"></seealso>
        /// <seealso cref="!:Configuring Input &amp; Movement"></seealso>
        /// <seealso cref="!:Custom InputAction Mapping"></seealso>
        public class InputAction {

            /// <summary>Returns the <strong>name</strong> property of the InputAction.</summary>
            /// <returns>The <strong>name</strong> value of the InputAction.</returns>
            public override string ToString() {
                return "InputAction:" + name;
            }
            public override bool Equals(object o) {
                if (o == null) return false;
                InputAction objAsInputAction = o as InputAction;
                if (objAsInputAction == null) return false;
                else return Equals(objAsInputAction);
            }
            public bool Equals(InputAction other) {
                if (other == null) return false;
                return (this.name == other.name);
            }
            public override int GetHashCode() {
                return this.name.GetHashCode();
            }
            public static bool operator ==(InputAction A, InputAction B) {
                if (System.Object.ReferenceEquals(A, B)) {
                    return true;
                }

                if (((object)A == null) || ((object)B == null)) {
                    return false;
                }

                return A.name == B.name;
            }
            public static bool operator !=(InputAction A, InputAction B) {
                return !(A == B);
            }

            protected string _name;
            /// <summary>The unique name given to the Input Action.</summary>
            /// <value>String containing the unique name of the Input Action.</value>
            /// <exception caption="ArgumentException" cref="System.ArgumentException">Thrown if an Input Action with an identical name is already registered with the %InputActionManager%.</exception>
            public string name {
                get {
                    return this._name;
                }
                set {
                    if (InputActionManager.isActionRegistered(value)) {
                        Debug.LogError("[ImmerseumSDK] InputAction is already registered. Modify its properties instead.");
                        throw new System.ArgumentException("InputAction is already registered. Modify its properties instead.");
                    } else {
                        this._name = value;
                    }
                }
            }

            /// <summary>A textual description of the Input Action - may be useful for documentation and validation.</summary>
            public string internalDescription;

            /// <summary>List of Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see> which fire this Input Action when $$pressed$$.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of
            /// Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>.</value>
            public List<KeyCode> pressedKeyList = new List<KeyCode>();
            /// <summary>List of Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see> which fire this Input Action when $$released$$.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of
            /// Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>.</value>
            public List<KeyCode> releasedKeyList = new List<KeyCode>();
            /// <summary>List of Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see> which fire this Input Action when $$held$$.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>.</value>
            public List<KeyCode> heldKeyList = new List<KeyCode>();
            /// <summary>List of Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see> which fire this Input Action when $$pressed$$ and $$held$$.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of
            /// Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>.</value>
            public List<KeyCode> pressedAndHeldKeyList = new List<KeyCode>();

            /// <summary>A list of <see cref="InputAxis">Input Axes</see> that are intended to manipulate the user's position on the X-Axis in worldspace when moved.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of <see cref="InputAxis">Input
            /// Axes</see>.</value>
            public List<InputAxis> xAxisList = new List<InputAxis>();
            /// <summary>A list of <see cref="InputAxis">Input Axes</see> that are intended to manipulate the user's position along the Y-Axis in worldspace when moved.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of <see cref="InputAxis">Input
            /// Axes</see>.</value>
            public List<InputAxis> yAxisList = new List<InputAxis>();
            /// <summary>A list of <see cref="InputAxis">Input Axes</see> that are intended to manipulate the user's position along the Z-Axis in worldspace when moved.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of <see cref="InputAxis">Input
            /// Axes</see>.</value>
            public List<InputAxis> zAxisList = new List<InputAxis>();
            /// <summary>A list of <see cref="InputAxis">Input Axes</see> that are intended to manipulate the Right Trigger when moved.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of <see cref="InputAxis">Input
            /// Axes</see>.</value>
            public List<InputAxis> rightTriggerAxisList = new List<InputAxis>();
            /// <summary>A list of <see cref="InputAxis">Input Axes</see> that are intended to manipulate the Left Trigger when moved.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of <see cref="InputAxis">Input
            /// Axes</see>.</value>
            public List<InputAxis> leftTriggerAxisList = new List<InputAxis>();

            /// <summary>A list of <see cref="InputAxis">Input Axes</see> that are intended to manipulate the camera Pitch when moved.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of <see cref="InputAxis">Input
            /// Axes</see>.</value>
            public List<InputAxis> pitchAxisList = new List<InputAxis>();
            /// <summary>A list of <see cref="InputAxis">Input Axes</see> that are intended to manipulate the camera's Yaw when moved.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of <see cref="InputAxis">Input
            /// Axes</see>.</value>
            public List<InputAxis> yawAxisList = new List<InputAxis>();

            /// <summary>List of <see cref="InputButton">Input Buttons</see> which fire this Input Action when they are $$pressed$$.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of <see cref="InputButton">Input
            /// Buttons</see>.</value>
            public List<InputButton> pressedButtonList = new List<InputButton>();
            /// <summary>List of <see cref="InputButton">Input Buttons</see> which fire this Input Action when they are $$released$$.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of <see cref="InputButton">Input
            /// Buttons</see>.</value>
            public List<InputButton> releasedButtonList = new List<InputButton>();
            /// <summary>List of <see cref="InputButton">Input Buttons</see> which fire this Input Action when they are $$held$$.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of <see cref="InputButton">Input Buttons</see>.</value>
            public List<InputButton> heldButtonList = new List<InputButton>();
            /// <summary>List of <see cref="InputButton">Input Buttons</see> which fire this Input Action when they are $$pressed$$ and $$held$$.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of <see cref="InputButton">Input Buttons</see>.</value>
            public List<InputButton> pressedAndHeldButtonList = new List<InputButton>();

            /// <summary>Indicates whether a defined $$input key$$ for this Input Action is $$pressed$$ in the most-recent frame.</summary>
            /// <value>
            ///   <strong>true</strong> if one or more $$input keys$$ defined for this Input Action are $$pressed$$, otherwise <strong>false</strong>.</value>
            public bool isKeyPressed {
                get {
                    if (InputActionManager.isKeyPressed(pressedKeyList.ToArray())) {
                        return true;
                    }
                    return false;
                }
            }
            /// <summary>Indicates whether a defined $$input key$$ for this Input Action is $$released$$ in the most-recent frame.</summary>
            /// <value>
            ///   <strong>true</strong> if one or more $$input keys$$ defined for this Input Action are $$released$$, otherwise <strong>false</strong>.</value>
            public bool isKeyReleased {
                get {
                    if (InputActionManager.isKeyReleased(releasedKeyList.ToArray())) {
                        return true;
                    }
                    return false;
                }
            }
            /// <summary>Indicates whether a defined $$input key$$ for this Input Action is $$held$$ in the most-recent frame.</summary>
            /// <value>
            ///   <strong>true</strong> if one or more $$input keys$$ defined for this Input Action are $$held$$, otherwise <strong>false</strong>.</value>
            public bool isKeyHeld {
                get {
                    if (InputActionManager.isKeyHeld(heldKeyList.ToArray())) {
                        return true;
                    }
                    return false;
                }
            }
            /// <summary>Indicates whether a defined $$input key$$ for this Input Action is $$pressed$$ and $$held$$ in the most-recent frame.</summary>
            /// <value>
            ///   <strong>true</strong> if one or more $$input keys$$ defined for this Input Action are $$pressed$$ and $$held$$, otherwise <strong>false</strong>.</value>
            public bool isKeyPressedAndHeld {
                get {
                    if (InputActionManager.isKeyPressedAndHeld(pressedAndHeldKeyList.ToArray())) {
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>Indicates whether a defined <see cref="InputButton" /> for this Input Action is $$pressed$$ in the most-recent frame.</summary>
            /// <value>
            ///   <strong>true</strong> if one or more <see cref="InputButton">Input Buttons</see> defined for this Input Action are $$pressed$$, otherwise <strong>false</strong>.</value>
            public bool isButtonPressed {
                get {
                    int n = pressedButtonList.Count;
                    for (int x = 0; x < n; x++) {
                        if (pressedButtonList[x].isPressed) {
                            return true;
                        }
                    }
                    return false;
                }
            }
            /// <summary>Indicates whether a defined <see cref="InputButton" /> for this Input Action is $$held$$ in the most-recent frame.</summary>
            /// <value>
            ///   <strong>true</strong> if one or more <see cref="InputButton">Input Buttons</see> defined for this Input Action are $$held$$, otherwise <strong>false</strong>.</value>
            public bool isButtonHeld {
                get {
                    int n = heldButtonList.Count;
                    for (int x = 0; x < n; x++) {
                        if (heldButtonList[x].isHeld) {
                            return true;
                        }
                    }
                    return false;
                }
            }
            /// <summary>Indicates whether a defined <see cref="InputButton" /> for this Input Action is $$pressed$$ and $$held$$ in the most-recent frame.</summary>
            /// <value>
            ///   <strong>true</strong> if one or more <see cref="InputButton">Input Buttons</see> defined for this Input Action are $$pressed$$ and $$held$$, otherwise
            /// <strong>false</strong>.</value>
            public bool isButtonPressedAndHeld {
                get {
                    int n = pressedAndHeldButtonList.Count;
                    for (int x = 0; x < n; x++) {
                        if (pressedAndHeldButtonList[x].isPressedAndHeld) {
                            return true;
                        }
                    }
                    return false;
                }
            }
            /// <summary>Indicates whether a defined <see cref="InputButton" /> for this Input Action is $$released$$ in the most-recent frame.</summary>
            /// <value>
            ///   <strong>true</strong> if one or more <see cref="InputButton">Input Buttons</see> defined for this Input Action are $$released$$, otherwise <strong>false</strong>.</value>
            public bool isButtonReleased {
                get {
                    int n = releasedButtonList.Count;
                    for (int x = 0; x < n; x++) {
                        if (releasedButtonList[x].isReleased) {
                            return true;
                        }
                    }
                    return false;
                }
            }

            public float buttonValue {
                get {
                    if (hasButtonFired == false) {
                        return 0f;
                    }
                    if (isButtonPressed) {
                        int n = pressedButtonList.Count;
                        for (int x = 0; x < n; x++) {
                            if (pressedButtonList[x].value != 0f) {
                                return pressedButtonList[x].value;
                            }
                        }
                    }
                    if (isButtonHeld) {
                        int n = heldButtonList.Count;
                        for (int x = 0; x < n; x++) {
                            if (heldButtonList[x].value != 0f) {
                                return heldButtonList[x].value;
                            }
                        }
                    }
                    if (isButtonPressedAndHeld) {
                        int n = pressedAndHeldButtonList.Count;
                        for (int x = 0; x < n; x++) {
                            if (pressedAndHeldButtonList[x].value != 0f) {
                                return pressedAndHeldButtonList[x].value;
                            }
                        }
                    }
                    if (isButtonReleased) {
                        int n = releasedButtonList.Count;
                        for (int x = 0; x < n; x++) {
                            if (releasedButtonList[x].value != 0f) {
                                return releasedButtonList[x].value;
                            }
                        }
                    }
                    return 0f;
                }
            }

            bool isAxisAtRest(InputAxis axis) {
                return axis.isAtRest;
            }
            bool isAxisAtRest(params InputAxis[] axisArray) {
                int n = axisArray.Length;
                bool _isAtRest = true;
                for (int x = 0; x < n; x++) {
                    if (axisArray[x].isAtRest == false) {
                        _isAtRest = false;
                    }
                }
                return _isAtRest;
            }

            bool isAxisAtMax(InputAxis axis) {
                return axis.isAtMax;
            }
            bool isAxisAtMax(params InputAxis[] axisArray) {
                int n = axisArray.Length;
                bool _isAtMax = false;
                for (int x = 0; x < n; x++) {
                    if (axisArray[x].isAtMax) {
                        _isAtMax = true;
                    }
                }
                return _isAtMax;
            }

            bool isAxisAtMin(InputAxis axis) {
                return axis.isAtMin;
            }
            bool isAxisAtMin(params InputAxis[] axisArray) {
                int n = axisArray.Length;
                bool _isAtMin = false;
                for (int x = 0; x < n; x++) {
                    if (axisArray[x].isAtMin) {
                        _isAtMin = true;
                    }
                }
                return _isAtMin;
            }

            /// <summary>Indicates whether the $$input X-Axis$$ is currently at its resting value.</summary>
            /// <value>
            ///   <strong>true</strong> if the X-Axis value is at its resting value, otherwise <strong>false</strong>.</value>
            public bool isXAxisAtRest {
                get {
                    return (isAxisAtRest(xAxisList.ToArray()));
                }
            }
            /// <summary>Indicates whether the $$input Y-Axis$$ is currently at its resting value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Y-Axis value is at its resting value, otherwise <strong>false</strong>.</value>
            public bool isYAxisAtRest {
                get {
                    return (isAxisAtRest(yAxisList.ToArray()));
                }
            }
            /// <summary>Indicates whether the $$input Z-Axis$$ is currently at its resting value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Z-Axis value is at its resting value, otherwise <strong>false</strong>.</value>
            public bool isZAxisAtRest {
                get {
                    return isAxisAtRest(zAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$Right Trigger Axis$$ is currently at its resting value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Right Trigger Axis value is at its resting value, otherwise <strong>false</strong>.</value>
            public bool isRightTriggerAxisAtRest {
                get {
                    return isAxisAtRest(rightTriggerAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$Left Trigger Axis$$ is currently at its resting value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Left Trigger Axis value is at its resting value, otherwise <strong>false</strong>.</value>
            public bool isLeftTriggerAxisAtRest {
                get {
                    return isAxisAtRest(leftTriggerAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$Pitch Axis$$ is currently at its resting value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Pitch Axis value is at its resting value, otherwise <strong>false</strong>.</value>
            public bool isPitchAxisAtRest {
                get {
                    return isAxisAtRest(pitchAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$Yaw Axis$$ is currently at its resting value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Yaw Axis value is at its resting value, otherwise <strong>false</strong>.</value>
            public bool isYawAxisAtRest {
                get {
                    return isAxisAtRest(yawAxisList.ToArray());
                }
            }

            /// <summary>Indicates whether the $$input X-Axis$$ is currently at its maximum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the X-Axis value is at its maximum allowable value, otherwise <strong>false</strong>.</value>
            public bool isXAxisAtMax {
                get {
                    return isAxisAtMax(xAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$input Y-Axis$$ is currently at its maximum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Y-Axis value is at its maximum allowable value, otherwise <strong>false</strong>.</value>
            public bool isYAxisAtMax {
                get {
                    return isAxisAtMax(yAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$input Z-Axis$$ is currently at its maximum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Z-Axis value is at its maximum allowable value, otherwise <strong>false</strong>.</value>
            public bool isZAxisAtMax {
                get {
                    return isAxisAtMax(zAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$Right Trigger Axis$$ is currently at its maximum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Right Trigger Axis' value is at its maximum allowable value, otherwise <strong>false</strong>.</value>
            public bool isRightTriggerAxisAtMax {
                get {
                    return isAxisAtMax(rightTriggerAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$Left Trigger Axis$$ is currently at its maximum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Left Trigger Axis' value is at its maximum allowable value, otherwise <strong>false</strong>.</value>
            public bool isLeftTriggerAxisAtMax {
                get {
                    return isAxisAtMax(leftTriggerAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$Pitch Axis$$ is currently at its maximum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Pitch Axis' value is at its maximum allowable value, otherwise <strong>false</strong>.</value>
            public bool isPitchAxisAtMax {
                get {
                    return isAxisAtMax(pitchAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$Yaw Axis$$ is currently at its maximum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Yaw Axis' value is at its maximum allowable value, otherwise <strong>false</strong>.</value>
            public bool isYawAxisAtMax {
                get {
                    return isAxisAtMax(yawAxisList.ToArray());
                }
            }

            /// <summary>Indicates whether the $$input X-Axis$$ is currently at its minimum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the X-Axis value is at its minimum allowable value, otherwise <strong>false</strong>.</value>
            public bool isXAxisAtMin {
                get {
                    return isAxisAtMin(xAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$input Y-Axis$$ is currently at its minimum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Y-Axis value is at its minimum allowable value, otherwise <strong>false</strong>.</value>
            public bool isYAxisAtMin {
                get {
                    return isAxisAtMin(yAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$input Z-Axis$$ is currently at its minimum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Z-Axis value is at its minimum allowable value, otherwise <strong>false</strong>.</value>
            public bool isZAxisAtMin {
                get {
                    return isAxisAtMin(zAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$Right Trigger Axis$$ is currently at its minimum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Right Trigger Axis value is at its minimum allowable value, otherwise <strong>false</strong>.</value>
            public bool isRightTriggerAxisAtMin {
                get {
                    return isAxisAtMin(rightTriggerAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$Left Trigger Axis$$ is currently at its minimum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Left Trigger Axis value is at its minimum allowable value, otherwise <strong>false</strong>.</value>
            public bool isLeftTriggerAxisAtMin {
                get {
                    return isAxisAtMin(leftTriggerAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$Pitch Axis$$ is currently at its minimum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Pitch Axis value is at its minimum allowable value, otherwise <strong>false</strong>.</value>
            public bool isPitchAxisAtMin {
                get {
                    return isAxisAtMin(pitchAxisList.ToArray());
                }
            }
            /// <summary>Indicates whether the $$Yaw Axis$$ is currently at its minimum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Yaw Axis value is at its minimum allowable value, otherwise <strong>false</strong>.</value>
            public bool isYawAxisAtMin {
                get {
                    return isAxisAtMin(yawAxisList.ToArray());
                }
            }

            /// <summary>
            ///   <para>Returns the type of input device from which the $$axis value$$ is being returned.</para>
            ///   <innovasys:widget type="Tip Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">When an InputAction $$axis$$ has multiple <see cref="InputAxis">Input Axes</see> defined, the ImmerseumSDK
            ///     always returns the $$axis value$$ with the highest value or greatest magnitude.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <param name="axisArray">
            ///   <para>An array of <see cref="InputAxis" /> objects.</para>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <strong>Be careful!</strong> The axis list properties in the InputAction object are all stored as
            ///     <strong>Lists</strong>. Therefore, be sure to convert them to <strong>Arrays</strong> before passing them to this method using the <span class="Code">&lt;axis-list&gt;.ToArray()</span> method.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </param>
            /// <returns>
            ///   <para>Returns the <see cref="InputType" /> of the device from which the $$axis value$$ was returned. Returns one of the following values:</para>
            ///   <list type="bullet">
            ///     <item>InputType.Keyboard</item>
            ///     <item>InputType.Mouse</item>
            ///     <item>InputType.Oculus</item>
            ///     <item>InputType.SteamVR</item>
            ///     <item>InputType.Gamepad</item>
            ///     <item>InputType.None</item>
            ///   </list>
            /// </returns>
            /// <example>
            ///   <code title="Example" description="Returns the %InputType% from which the Right Trigger is being captured, and from which the X-Axis is being captured." lang="C#">
            /// InputType xAxisSource = getAxisValue(xAxisList.ToArray());
            ///  
            /// InputType rightTriggerSource = getAxisValue(rightTriggerAxisList.ToArray());</code>
            /// </example>
            public InputType getAxisValueSource(params InputAxis[] axisArray) {
                int n = axisArray.Length;
                float currentValue = getAxisValue(axisArray);

                for (int x = 0; x < n; x++) {
                    if (axisArray[x].value == currentValue) {
                        return axisArray[x].inputType;
                    }
                }
                return InputType.None;
            }

            float getAxisValue(InputAxis axis) {
                return axis.value;
            }
            float getAxisValue(params InputAxis[] axisArray) {
                int n = axisArray.Length;
                if (n < 1) {
                    return 0f;
                }
                float[] valueArray = new float[n];
                float[] absValueArray = new float[n];
                int maxIndex = new int();

                for (int x = 0; x < n; x++) {
                    absValueArray[x] = Mathf.Abs(axisArray[x].value);
                    valueArray[x] = axisArray[x].value;
                }
                maxIndex = Array.IndexOf(absValueArray, Mathf.Max(absValueArray));
                return valueArray[maxIndex];
            }

            float getAxisRawValue(InputAxis axis) {
                return axis.rawValue;
            }
            float getAxisRawValue(params InputAxis[] axisArray) {
                int n = axisArray.Length;
                float[] valueArray = new float[n];
                float[] absValueArray = new float[n];
                int maxIndex = new int();
                for (int x = 0; x < n; x++) {
                    absValueArray[x] = Mathf.Abs(axisArray[x].rawValue);
                    valueArray[x] = axisArray[x].rawValue;
                }
                maxIndex = Array.IndexOf(absValueArray, Mathf.Max(absValueArray));
                return valueArray[maxIndex];
            }

            Vector2 getAxisValue2D(InputAxis axis) {
                return axis.value2D;
            }
            Vector2 getAxisValue2D(params InputAxis[] axisArray) {
                int n = axisArray.Length;
                Vector2[] valueArray = new Vector2[n];
                float[] valueMagnitudeArray = new float[n];
                float maxMagnitude = new float();
                int valueIndex = new int();

                for (int x = 0; x < n; x++) {
                    valueArray[x] = axisArray[x].value2D;
                    valueMagnitudeArray[x] = Mathf.Abs(axisArray[x].value2D.sqrMagnitude);
                }
                maxMagnitude = Mathf.Max(valueMagnitudeArray);
                valueIndex = Array.IndexOf(valueMagnitudeArray, maxMagnitude);
                return valueArray[valueIndex];
            }

            Vector2 getAxisRawValue2D(InputAxis axis) {
                return axis.rawValue2D;
            }
            Vector2 getAxisRawValue2D(params InputAxis[] axisArray) {
                int n = axisArray.Length;
                Vector2[] valueArray = new Vector2[n];
                float[] valueMagnitudeArray = new float[n];
                float maxMagnitude = new float();
                int valueIndex = new int();

                for (int x = 0; x < n; x++) {
                    valueArray[x] = axisArray[x].rawValue2D;
                    valueMagnitudeArray[x] = Mathf.Abs(axisArray[x].rawValue2D.sqrMagnitude);
                }
                maxMagnitude = Mathf.Max(valueMagnitudeArray);
                valueIndex = Array.IndexOf(valueMagnitudeArray, maxMagnitude);
                return valueArray[valueIndex];
            }

            /// <summary>
            ///   <para>The value from the $$input X-Axis$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$input X-Axis$$, then returns the
            ///     greatest value from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            ///   <para></para>
            /// </summary>
            /// <value>The $$axis value$$ expressed as a floating point number.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called on an input axis that is multi-dimensional.</exception>
            public float xAxisValue {
                get {
                    return getAxisValue(xAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The value from the $$input Y-Axis$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$input Y-Axis$$, then returns the
            ///     greatest value from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            ///   <para></para>
            /// </summary>
            /// <value>The $$axis value$$ expressed as a floating point number.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called on an input axis that is multi-dimensional.</exception>
            public float yAxisValue {
                get {
                    return getAxisValue(yAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The value from the $$input Z-Axis$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$input Z-Axis$$, then returns the
            ///     greatest value from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            ///   <para></para>
            /// </summary>
            /// <value>The $$axis value$$ expressed as a floating point number.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called on an input axis that is multi-dimensional.</exception>
            public float zAxisValue {
                get {
                    return getAxisValue(zAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The value from the $$Right Trigger Axis$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$Right Trigger Axis$$, then returns the
            ///     greatest value from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            ///   <para></para>
            /// </summary>
            /// <value>The $$axis value$$ expressed as a floating point number.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called on an input axis that is multi-dimensional.</exception>
            public float rightTriggerAxisValue {
                get {
                    return getAxisValue(rightTriggerAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The value from the $$Left Trigger Axis$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$Left Trigger Axis$$, then returns the
            ///     greatest value from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            ///   <para></para>
            /// </summary>
            /// <value>The $$axis value$$ expressed as a floating point number.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called on an input axis that is multi-dimensional.</exception>
            public float leftTriggerAxisValue {
                get {
                    return getAxisValue(leftTriggerAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The value from the $$Pitch Axis$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$Left Trigger Axis$$, then returns the
            ///     greatest value from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            ///   <para></para>
            /// </summary>
            /// <value>The $$axis value$$ expressed as a floating point number.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called on an input axis that is multi-dimensional.</exception>
            public float pitchAxisValue {
                get {
                    return getAxisValue(pitchAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The value from the $$Yaw Axis$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$Left Trigger Axis$$, then returns the
            ///     greatest value from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            ///   <para></para>
            /// </summary>
            /// <value>The $$axis value$$ expressed as a floating point number.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called on an input axis that is multi-dimensional.</exception>
            public float yawAxisValue {
                get {
                    return getAxisValue(yawAxisList.ToArray());
                }
            }

            /// <summary>
            ///   <para>The $$raw value$$ from the $$input X-Axis$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$Left Trigger Axis$$, then returns the
            ///     greatest $$raw value$$ from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A floating point value representing the $$raw value$$ from the $$input X-Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis that is multi-dimensional.</exception>
            public float xAxisRawValue {
                get {
                    return getAxisRawValue(xAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$raw value$$ from the $$input Y-Axis$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$Left Trigger Axis$$, then returns the
            ///     greatest $$raw value$$ from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A floating point value representing the $$raw value$$ from the $$input Y-Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis that is multi-dimensional.</exception>
            public float yAxisRawValue {
                get {
                    return getAxisRawValue(yAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$raw value$$ from the $$input Z-Axis$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$Left Trigger Axis$$, then returns the
            ///     greatest $$raw value$$ from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A floating point value representing the $$raw value$$ from the $$input Z-Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis that is multi-dimensional.</exception>
            public float zAxisRawValue {
                get {
                    return getAxisRawValue(zAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$raw value$$ from the $$Right Trigger Axis$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$Left Trigger Axis$$, then returns the
            ///     greatest $$raw value$$ from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A floating point value representing the $$raw value$$ from the $$Right Trigger Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis that is multi-dimensional.</exception>
            public float rightTriggerAxisRawValue {
                get {
                    return getAxisRawValue(rightTriggerAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$raw value$$ from the $$Left Trigger Axis$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$Left Trigger Axis$$, then returns the
            ///     greatest $$raw value$$ from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A floating point value representing the $$raw value$$ from the $$Left Trigger Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis that is multi-dimensional.</exception>
            public float leftTriggerAxisRawValue {
                get {
                    return getAxisRawValue(leftTriggerAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$raw value$$ from the Pitch Axis$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$Left Trigger Axis$$, then returns the
            ///     greatest $$raw value$$ from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A floating point value representing the $$raw value$$ from the $$Pitch Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis that is multi-dimensional.</exception>
            public float pitchAxisRawValue {
                get {
                    return getAxisRawValue(pitchAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$raw value$$ from the $$Yaw Axis$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$Left Trigger Axis$$, then returns the
            ///     greatest $$raw value$$ from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A floating point value representing the $$raw value$$ from the $$Yaw Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis that is multi-dimensional.</exception>
            public float yawAxisRawValue {
                get {
                    return getAxisRawValue(yawAxisList.ToArray());
                }
            }

            /// <summary>
            ///   <para>The $$axis value$$ from the $$input X-Axis$$ expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined, then returns the $$axis
            ///     value$$ with the greatest magnitude from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> containing the $$axis value$$ from the $$input X-Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis which is not expressed in two dimensions.</exception>
            public Vector2 xAxisValue2D {
                get {
                    return getAxisValue2D(xAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$axis value$$ from the $$input Y-Axis$$ expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined, then returns the $$axis
            ///     value$$ with the greatest magnitude from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> containing the $$axis value$$ from the $$input Y-Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis which is not expressed in two dimensions.</exception>
            public Vector2 yAxisValue2D {
                get {
                    return getAxisValue2D(yAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$axis value$$ from the $$input Z-Axis$$ expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined, then returns the $$axis
            ///     value$$ with the greatest magnitude from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> containing the $$axis value$$ from the $$input Z-Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis which is not expressed in two dimensions.</exception>
            public Vector2 zAxisValue2D {
                get {
                    return getAxisValue2D(zAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$axis value$$ from the $$Right Trigger Axis$$ expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined, then returns the $$axis
            ///     value$$ with the greatest magnitude from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> containing the $$axis value$$ from the $$Right Trigger Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis which is not expressed in two dimensions.</exception>
            public Vector2 rightTriggerAxisValue2D {
                get {
                    return getAxisValue2D(rightTriggerAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$axis value$$ from the $$Left Trigger Axis$$ expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined, then returns the $$axis
            ///     value$$ with the greatest magnitude from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> containing the $$axis value$$ from the $$Left Trigger Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis which is not expressed in two dimensions.</exception>
            public Vector2 leftTriggerAxisValue2D {
                get {
                    return getAxisValue2D(leftTriggerAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$axis value$$ from the $$Pitch Axis$$ expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined, then returns the $$axis
            ///     value$$ with the greatest magnitude from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> containing the $$axis value$$ from the $$Pitch Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis which is not expressed in two dimensions.</exception>
            public Vector2 pitchAxisValue2D {
                get {
                    return getAxisValue2D(pitchAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$axis value$$ from the $$Yaw Axis$$ expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined, then returns the $$axis
            ///     value$$ with the greatest magnitude from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> containing the $$axis value$$ from the $$Yaw Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis which is not expressed in two dimensions.</exception>
            public Vector2 yawAxisValue2D {
                get {
                    return getAxisValue2D(yawAxisList.ToArray());
                }
            }

            /// <summary>
            ///   <para>The $$raw value$$ from the $$input X-Axis$$ expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined, then returns $$raw
            ///     value$$ with the greatest magnitude from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> containing the $$raw value$$ from the $$input X-Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis which is not expressed in two dimensions.</exception>
            public Vector2 xAxisRawValue2D {
                get {
                    return getAxisRawValue2D(xAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$raw value$$ from the $$input Y-Axis$$ expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined, then returns $$raw
            ///     value$$ with the greatest magnitude from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> containing the $$raw value$$ from the $$input Y-Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis which is not expressed in two dimensions.</exception>
            public Vector2 yAxisRawValue2D {
                get {
                    return getAxisRawValue2D(yAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$raw value$$ from the $$input Z-Axis$$ expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined, then returns $$raw
            ///     value$$ with the greatest magnitude from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> containing the $$raw value$$ from the $$input Z-Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis which is not expressed in two dimensions.</exception>
            public Vector2 zAxisRawValue2D {
                get {
                    return getAxisRawValue2D(zAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$raw value$$ from the $$Right Trigger Axis$$ expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined, then returns $$raw
            ///     value$$ with the greatest magnitude from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> containing the $$raw value$$ from the $$Right Trigger Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis which is not expressed in two dimensions.</exception>
            public Vector2 rightTriggerAxisRawValue2D {
                get {
                    return getAxisRawValue2D(rightTriggerAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$raw value$$ from the $$Left Trigger Axis$$ expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined for the $$Left Trigger Axis$$, then returns $$raw
            ///     value$$ with the greatest magnitude from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> containing the $$raw value$$ from the $$Left Trigger Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis which is not expressed in two dimensions.</exception>
            public Vector2 leftTriggerAxisRawValue2D {
                get {
                    return getAxisRawValue2D(leftTriggerAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$raw value$$ from the $$Pitch Axis$$ expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined, then returns $$raw
            ///     value$$ with the greatest magnitude from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> containing the $$raw value$$ from the $$Pitch Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis which is not expressed in two dimensions.</exception>
            public Vector2 pitchAxisRawValue2D {
                get {
                    return getAxisRawValue2D(pitchAxisList.ToArray());
                }
            }
            /// <summary>
            ///   <para>The $$raw value$$ from the $$Yaw Axis$$ expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">If multiple <see cref="InputAxis">Input Axes</see> are defined, then returns $$raw
            ///     value$$ with the greatest magnitude from defined Input Axes.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> containing the $$raw value$$ from the $$Yaw Axis$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown when this property is called for an Input Axis which is not expressed in two dimensions.</exception>
            public Vector2 yawAxisRawValue2D {
                get {
                    return getAxisRawValue2D(yawAxisList.ToArray());
                }
            }

            /// <summary>Indicates whether the InputAction has one or more <see cref="InputAxis">Input Axes</see> defined for any of its available axes.</summary>
            /// <value>
            ///   <strong>true</strong> if the InputAction has one or more <see cref="InputAxis" /> included in any of its available axes, <strong>false</strong> if not.</value>
            public bool hasAxis {
                get {
                    return (xAxisList.Count > 0 || yAxisList.Count > 0 || zAxisList.Count > 0 || rightTriggerAxisList.Count > 0 || leftTriggerAxisList.Count > 0 || pitchAxisList.Count > 0 || yawAxisList.Count > 0);
                }
            }
            /// <summary>Indicates that the Input Action has one or more $$input keys$$ defined.</summary>
            /// <value>
            ///   <strong>true</strong> if the Input Action has one or more $$input keys$$ defined, otherwise <strong>false</strong>.</value>
            public bool hasKey {
                get {
                    return (pressedKeyList.Count > 0 || heldKeyList.Count > 0 || pressedAndHeldKeyList.Count > 0 || releasedKeyList.Count > 0);
                }
            }
            /// <summary>Indicates the Input Action has one or more <see cref="InputButton">Input Buttons</see> defined.</summary>
            /// <value>
            ///   <strong>true</strong> if the Input Action has one or more <see cref="InputButton">InputButtons</see> defined, otherwise <strong>false</strong>.</value>
            public bool hasButton {
                get {
                    return (pressedButtonList.Count > 0 || heldButtonList.Count > 0 || pressedAndHeldButtonList.Count > 0 || releasedButtonList.Count > 0);
                }
            }

            /// <summary>Indicates whether one or more of Input Action's defined axes has fired (i.e. was not at rest).</summary>
            /// <value>
            ///   <strong>true</strong> if one or more of the Input Action's defined axes was not at rest, otherwise <strong>false</strong>.</value>
            public bool hasAxisFired {
                get {
                    if (hasAxis) {
                        if (isXAxisAtRest && isYAxisAtRest && isZAxisAtRest && isRightTriggerAxisAtRest && isLeftTriggerAxisAtRest && isPitchAxisAtRest && isYawAxisAtRest) {
                            return false;
                        } else {
                            return true;
                        }
                    }
                    return false;
                }
            }
            /// <summary>Indicates whether an $$input key$$ defined for this Input Action has been fired.</summary>
            /// <value>
            ///   <strong>true</strong> if a defined $$input key$$ has been fired, otherwise <strong>false</strong>.</value>
            public bool hasKeyFired {
                get {
                    if (hasKey) {
                        return (isKeyPressed || isKeyHeld || isKeyPressedAndHeld || isKeyReleased);
                    }
                    return false;
                }
            }
            /// <summary>Indicates one of the Input Action's defined <see cref="InputButton">InputButtons</see> has fired.</summary>
            /// <value>
            ///   <strong>true</strong> if one of the Input Action's defined <see cref="InputButton">InputButtons</see> has fired, otherwise <strong>false</strong>.</value>
            public bool hasButtonFired {
                get {
                    if (hasButton) {
                        return (isButtonPressed || isButtonHeld || isButtonPressedAndHeld || isButtonReleased);
                    }
                    return false;
                }
            }
            /// <summary>Indicates that one or more of the inputs that kick off this Input Action were received from the user.</summary>
            /// <value>
            ///   <strong>true</strong> if the user's input devices generated any of the defined inputs for this Input Action, otherwise <strong>false</strong>.</value>
            public bool hasFired {
                get {
                    if (hasAxisFired) {
                        return true;
                    }
                    if (hasButtonFired) {
                        return true;
                    }
                    if (hasKeyFired) {
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>
            ///   <para>Registers the InputAction with the <see cref="InputActionManager" />, thus supporting it in your VR Scene.</para>
            ///   <para>Once an InputAction has been registered, the <see cref="InputActionManager" /> will fire the <see cref="EventManager.OnInputActionStart" /> event whenever the InputAction is received
            /// from the user's input devices.</para>
            /// </summary>
            /// <returns>
            ///   <strong>true</strong> if successful, <strong>false</strong> if not.</returns>
            public bool registerAction() {
                bool wasSuccess = false;
                wasSuccess = InputActionManager.addInputAction(this);
                return wasSuccess;
            }
            /// <summary>Removes the InputAction from the list of registered InputActions recognized/monitored by the <see cref="InputActionManager" />.</summary>
            /// <returns>
            ///   <strong>true</strong> if successful, <strong>false</strong> if not.</returns>
            public bool deregisterAction() {
                bool wasSuccess = false;
                wasSuccess = InputActionManager.removeInputAction(this);
                return wasSuccess;
            }
        }
    }
}
