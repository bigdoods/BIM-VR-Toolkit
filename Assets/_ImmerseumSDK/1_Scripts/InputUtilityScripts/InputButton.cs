using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {

        public enum InputType { Keyboard, Mouse, Oculus, SteamVR, Gamepad, None }

        /// <summary>
        ///   <para>Represents a virtual button defined in <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Unity's Input Manager</see>.</para>
        ///   <para>By including this Input Button in an <see cref="InputAction" /> (on one of the Button lists), you associate the input device with the action that you want the user's
        /// input to evoke.</para>
        /// </summary>
        public class InputButton {
            /// <summary>Determines the device type that the Input Button's virtual button maps to.</summary>
            /// <value>
            ///   <para>An <see cref="Immerseum.VRSimulator.InputType">InputType</see>, which can have any of the following values:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>Keyboard</strong> - if the virtual button is mapped to keyboard input,</item>
            ///     <item>
            ///       <strong>Mouse</strong> - if the virtual button is mapped to mouse movement,</item>
            ///     <item>
            ///       <strong>Oculus</strong> - if the virtual button is mapped to an Oculus-device (e.g. Oculus Touch or Oculus Remote),</item>
            ///     <item>
            ///       <strong>SteamVR</strong> - if the virtual button is mapped to a SteamVR-device (e.g. HTC Wands/Vive Wands, etc.),</item>
            ///     <item>
            ///       <strong>Gamepad</strong> - if the virtual button is mapped to a gamepad (e.g. Xbox One Controller).</item>
            ///   </list>
            /// </value>
            public virtual InputType buttonType {
                get {
                    return InputType.Gamepad;
                }
            }

            /// <summary>Determines whether the button's boolean firing value is converted to a negative or a positive number if true.</summary>
            /// <value>True returns a negative number when fired, false returns a positive.</value>
            public bool isNegativeValue = false;

            /// <summary>
            ///   <para>The name given to the Input Button.</para>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <strong>Be careful!</strong> To properly recognize input from the user's input devices, if
            ///     <span class="Code">buttonType == Mouse</span> or <span class="Code">buttonType == Gamepad</span>, this value must correspond to a <strong>Name</strong>
            ///     given to a virtual axis in the <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Unity Input Manager</see>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>
            ///   <para>If <span class="Code">buttonType == Mouse</span> or <span class="Code">buttonType == Gamepad</span>:</para>
            ///   <blockquote style="MARGIN-RIGHT: 0px" dir="ltr">
            ///     <para>A string corresponding to the <strong>Name</strong> property of a virtual axis defined in the <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Unity Input
            ///     Manager</see>.</para>
            ///   </blockquote>
            ///   <para>Otherwise, a unique name given to the Input Button.</para>
            /// </value>
            public string name;
            /// <summary>A brief description of the Input Button - may be useful for notation and validation.</summary>
            /// <value>A string containing the internal description.</value>
            public string internalDescription;


            /// <summary>Indicates whether the Input Button is on a mouse.</summary>
            /// <value>
            ///   <strong>true</strong> if <span class="Code">buttonType == Keyboard</span>, otherwise <strong>false</strong>.</value>
            public bool isKeyboardButton {
                get {
                    return buttonType == InputType.Keyboard;
                }
            }

            /// <summary>Indicates whether the Input Button is on a mouse.</summary>
            /// <value>
            ///   <strong>true</strong> if <span class="Code">buttonType == Mouse</span>, otherwise <strong>false</strong>.</value>
            public bool isMouseButton {
                get {
                    return buttonType == InputType.Mouse;
                }
            }

            /// <summary>Indicates whether the Input Button is on an Oculus input device.</summary>
            /// <value>
            ///   <strong>true</strong> if <span class="Code">buttonType == Oculus</span>, otherwise <strong>false</strong>.</value>
            public bool isOculusButton {
                get {
                    return buttonType == InputType.Oculus;
                }
            }

            /// <summary>Indicates whether the Input Button is on a SteamVR input device.</summary>
            /// <value>
            ///   <strong>true</strong> if <span class="Code">buttonType == SteamVR</span>, otherwise <strong>false</strong>.</value>
            public bool isSteamVRButton {
                get {
                    return buttonType == InputType.SteamVR;
                }
            }

            /// <summary>Indicates whether the Input Button is on a gamepad.</summary>
            /// <value>
            ///   <strong>true</strong> if <span class="Code">buttonType == Gamepad</span>, otherwise <strong>false</strong>.</value>
            public bool isGamepadButton {
                get {
                    return buttonType == InputType.Gamepad;
                }
            }

            public virtual float value {
                get {
                    if (isHeld || isPressed || isPressedAndHeld || isReleased || isClicked) {
                        if (isNegativeValue) {
                            return -1f;
                        } else {
                            return 1f;
                        }
                    }
                    return 0f;
                }
            }

            /// <summary>Indicates whether the Input Button is $$held$$.</summary>
            /// <value>
            ///   <strong>true</strong> if the Input Button is $$held$$, otherwise <strong>false</strong>.</value>
            public virtual bool isHeld {
                get {
                    return Input.GetButton(name);
                }
            }

            /// <summary>Indicates whether the Input Button is $$pressed$$.</summary>
            /// <value>
            ///   <strong>true</strong> if the Input Button is $$pressed$$, otherwise <strong>false</strong>.</value>
            public virtual bool isPressed {
                get {
                    return Input.GetButtonDown(name);
                }
            }

            /// <summary>Indicates whether the Input Button is $$released$$.</summary>
            /// <value>
            ///   <strong>true</strong> if the Input Button is $$released$$, otherwise <strong>false</strong>.</value>
            public virtual bool isReleased {
                get {
                    return Input.GetButtonUp(name);
                }
            }

            /// <summary>Indicates whether the Input Button is $$pressed$$ and then $$held$$.</summary>
            /// <value>
            ///   <strong>true</strong> if the Input Button is $$pressed$$ and $$held$$, otherwise <strong>false</strong>.</value>
            public virtual bool isPressedAndHeld {
                get {
                    return isPressed && isHeld;
                }
            }

            /// <summary>
            ///   <para>Indicates whether the Input Button has been $$clicked$$.</para>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <strong>Be careful!</strong> It is very difficult to actually "click" a button (press it down and
            ///     release it) in the small window of a single frame. Instead, it is usually wiser to use <see cref="InputButton.isReleased">isReleased</see> or
            ///     <see cref="InputButton.isPressed">isPressed</see>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>
            ///   <strong>true</strong> if the Input Button was $$clicked$$, <strong>false</strong> if not.</value>
            public virtual bool isClicked {
                get {
                    return isPressed && isReleased;
                }
            }

        }

    }
}