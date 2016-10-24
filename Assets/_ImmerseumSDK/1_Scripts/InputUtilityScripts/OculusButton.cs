using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {
        /// <summary>
        ///   <para>Represents a virtual button accessible through the <see cref="!:https://developer.oculus.com/downloads/game-engines/1.5.0/Oculus_Utilities_for_Unity_5/% ">Oculus Utilities for
        /// Unity</see>OVRInput% API.</para>
        ///   <para>By including this Input Button in an <see cref="InputAction" />, you associate the input device with the action that you want the user's input to evoke.</para>
        /// </summary>
        public class OculusButton : InputButton {
            /// <summary>Determines  the particular <see cref="OVRInput.Button" /> on the Oculus input device to associate with this Input Button.</summary>
            /// <value>
            ///   <para>An <see cref="OVRInput.Button" /> value that identifies the physical button on the Oculus input device associated with this Input Button. Accepts one of the following
            /// values:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>None</strong>
            ///     </item>
            ///     <item>
            ///       <strong>One</strong> - indicates the A button on the Right-hand Touch and the X-button on the left-hand Touch</item>
            ///     <item>
            ///       <strong>Two</strong> - indicates the B button on the Right-hand Touch and the Y-button on the left-hand Touch</item>
            ///     <item>
            ///       <strong>Three</strong> - not applicable</item>
            ///     <item>
            ///       <strong>Four</strong> - not applicable</item>
            ///     <item>
            ///       <strong>Start</strong> - not applicable</item>
            ///     <item>
            ///       <strong>Back</strong> - not applicable</item>
            ///     <item>
            ///       <strong>PrimaryShoulder</strong> - not applicable</item>
            ///     <item>
            ///       <strong>PrimaryIndexTrigger</strong> - indicates the LIndexTrigger on the left-hand Touch or the RIndexTrigger on the right-hand Touch</item>
            ///     <item>
            ///       <strong>PrimaryHandTrigger</strong> - indicates the LHandTrigger on the left-hand Touch or the RHandTrigger on the right-hand Touch</item>
            ///     <item>
            ///       <strong>PrimaryThumbstick</strong> - indicates the LThumbstick on the left-hand Touch, or the RThumbstick on the right-hand Touch</item>
            ///     <item>
            ///       <strong>PrimaryThumbstickUp</strong> - indicates LThumbstickUp on the left-hand Touch or the RThumbstickUp on the right-hand Touch</item>
            ///     <item>
            ///       <strong>PrimaryThumbstickDown</strong> - indicates LThumbstickDown on the left-hand Touch or RThumbstickDown on the right-hand Touch</item>
            ///     <item>
            ///       <strong>PrimaryThumbstickLeft</strong> - indicates LThumbstickLeft on the left-hand Touch or RThumbstickLeft on the right-hand Touch</item>
            ///     <item>
            ///       <strong>PrimaryThumbstickRight</strong> - indicates LThumbstickRight on the left-hand Touch or RThumbstickRight on the right-hand Touch</item>
            ///     <item>
            ///       <strong>SecondaryShoulder</strong> - not applicable</item>
            ///     <item>
            ///       <strong>SecondaryIndexTrigger</strong> - TBD</item>
            ///     <item>
            ///       <strong>SecondaryHandTrigger</strong> - TBD</item>
            ///     <item>
            ///       <strong>SecondaryThumbstick</strong> - TBD</item>
            ///     <item>
            ///       <strong>SecondaryThumbstickUp</strong> - TBD</item>
            ///     <item>
            ///       <strong>SecondaryThumbstickDown</strong> - TBD</item>
            ///     <item>
            ///       <strong>SecondaryThumbstickLeft</strong> - TBD</item>
            ///     <item>
            ///       <strong>SecondaryThumbstickRight</strong> - TBD</item>
            ///     <item>
            ///       <strong>DpadUp</strong> - Not applicable</item>
            ///     <item>
            ///       <strong>DpadDown</strong> - Not Applicable</item>
            ///     <item>
            ///       <strong>DpadLeft</strong> - not Applicable</item>
            ///     <item>
            ///       <strong>DpadRight</strong> - not applicable</item>
            ///     <item>
            ///       <strong>Up</strong> - indicates LThumbstickUp on the left-hand Touch, RThumbstickUp on the right-hand Touch</item>
            ///     <item>
            ///       <strong>Down</strong> - indicates LThumbstickDown on the left-hand Touch, RThumbstickDown on the right-hand Touch</item>
            ///     <item>
            ///       <strong>Left</strong> - indicates LThumbstickLeft on the left-hand Touch, RThumbstickLeft on the right-hand Touch</item>
            ///     <item>
            ///       <strong>Right</strong> - indicates LThumbstickRight on the left-hand Touch, RThumbstickRight on the right-hand Touch</item>
            ///     <item>
            ///       <strong>Any</strong> - indicates any input</item>
            ///   </list>
            /// </value>
            public OVRInput.Button buttonMask = OVRInput.Button.Any;
            /// <summary>Determines  the particular <see cref="OVRInput.RawButton" /> on the Oculus input device to associate with this Input Button.</summary>
            /// <value>See <see cref="OVRInput.RawButton" /> for a detailed listing of acceptable values.</value>
            public OVRInput.RawButton rawButtonMask = OVRInput.RawButton.Any;

            /// <summary>Determines the particular input device / controller type from which input along this Input Button should be captured.</summary>
            /// <value>
            ///   <para>A bitmask value from %OVRInput.Controller %indicating a particular Oculus-compatible controller. Accepts one of the following values:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>All</strong> - indicates all possible Oculus input devices,</item>
            ///     <item>
            ///       <strong>Active</strong> - indicates those Oculus input devices currently connected to the user's system,</item>
            ///     <item>
            ///       <strong>Remote</strong> - indicates the Oculus Remote,</item>
            ///     <item>
            ///       <strong>LTouch</strong> - indicates the left-hand Oculus Touch,</item>
            ///     <item>
            ///       <strong>RTouch</strong> - indicates the right-hand Oculus Touch,</item>
            ///     <item>
            ///       <strong>Touch</strong> - indicates either the Left-hand Oculus Touch or the Right-hand Oculus Touch,</item>
            ///     <item>
            ///       <strong>Gamepad</strong> - indicates an XBox One Controller, or;</item>
            ///     <item>
            ///       <strong>None</strong> - indicates none of the supported input devices.</item>
            ///   </list>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <strong>Be careful!</strong> While the <see cref="!:https://developer.oculus.com/downloads/game-engines/1.5.0/Oculus_Utilities_for_Unity_5/">Oculus Utilities for Unity</see> API does support
            ///     gamepad input from the XBox One Controller, the Immerseum SDK recommends that you use the more universal / cross-platform supported Immerseum InputActions
            ///     to define your gamepad configuration instead of the <see cref="OVRInput" /> API.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </value>
            public OVRInput.Controller controllerMask = OVRInput.Controller.Active;

            public override InputType buttonType {
                get {
                    return InputType.Oculus;
                }
            }

            public override bool isHeld {
                get {
                    return OVRInput.Get(buttonMask, controllerMask);
                }
            }
            public bool isRawHeld {
                get {
                    return OVRInput.Get(rawButtonMask, controllerMask);
                }
            }

            public override bool isPressed {
                get {
                    return OVRInput.GetDown(buttonMask, controllerMask);
                }
            }
            public bool isRawPressed {
                get {
                    return OVRInput.GetDown(rawButtonMask, controllerMask);
                }
            }

            public override bool isReleased {
                get {
                    return OVRInput.GetUp(buttonMask, controllerMask);
                }
            }
            public bool isRawReleased {
                get {
                    return OVRInput.GetUp(rawButtonMask, controllerMask);
                }
            }

            public override bool isPressedAndHeld {
                get {
                    return isPressed && isHeld;
                }
            }
            public bool isRawPressedAndHeld {
                get {
                    return isRawPressed && isRawHeld;
                }
            }

            public override bool isClicked {
                get {
                    return isPressed && isReleased;
                }
            }
            public bool isRawClicked {
                get {
                    return isRawPressed && isRawReleased;
                }
            }
        }

    }
}