using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {
        /// <summary>
        ///   <para>Represents a virtual axis accessible through the <see cref="!:https://developer.oculus.com/downloads/game-engines/1.5.0/Oculus_Utilities_for_Unity_5/% ">Oculus Utilities for
        /// Unity</see>OVRInput% API.</para>
        ///   <para>By including this Input Axis in an <see cref="InputAction" />, you associate the input device with the action that you want the user's input to evoke.</para>
        /// </summary>
        public class OculusInputAxis : InputAxis {
            [Range(1, 2)]
            public new int dimensions = 1;

            public new InputType inputType {
                get {
                    return InputType.Oculus;
                }
            }

            /// <summary>Determines the particular input device / controller type from which input along this Input Axis should be captured.</summary>
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
            ///       <strong>Be careful!</strong> While the Oculus Utilities for Unity API does support gamepad input
            ///     from the XBox One Controller, the Immerseum SDK recommends that you use the more universal / cross-platform supported Immerseum InputActions to
            ///     define your gamepad configuration instead of the OVR API.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </value>
            public OVRInput.Controller controllerMask = OVRInput.Controller.Active;

            private OVRInput.Axis1D _previousAxis1D = OVRInput.Axis1D.Any;
            private OVRInput.Axis2D _previousAxis2D = OVRInput.Axis2D.Any;

            protected OVRInput.Axis1D _axis1D = OVRInput.Axis1D.Any;
            /// <summary>The bitmask by which this 1-dimensional input axis is accessed through the %Oculus Utilities for
            /// Unity:https://developer.oculus.com/downloads/game-engines/1.5.0/Oculus_Utilities_for_Unity_5/% API.</summary>
            /// <value>An <see cref="OVRInput.Axis1D" /> enum value which represents a bitmask corresponding to one of the hard-coded 1-dimensional input axes supported by Oculus devices.</value>
            public OVRInput.Axis1D axis1D {
                get {
                    return this._axis1D;
                }
                set {
                    if (value != this._axis1D) {
                        toggleAxis1D(value);
                    }
                }
            }

            protected OVRInput.Axis2D _axis2D = OVRInput.Axis2D.None;
            /// <summary>The bitmask by which this 2-dimensional input axis is accessed through the %Oculus Utilities for
            /// Unity:https://developer.oculus.com/downloads/game-engines/1.5.0/Oculus_Utilities_for_Unity_5/% API.</summary>
            public OVRInput.Axis2D axis2D {
                get {
                    return this._axis2D;
                }
                set {
                    if (value != this._axis2D) {
                        toggleAxis2D(value);
                    }
                }
            }

            /// <summary>
            ///   <para>Toggles the Oculus Axis defined by the <strong>axisMask</strong> parameter so as to associate the Input Axis with an Oculus Touch Input Axis.</para>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <strong>Be careful!</strong> Oculus devices do not use virtual inputs or virtual mappings managed
            ///     through the <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Unity Input Manager</see>. Instead, they are accessed
            ///     directly through the <see cref="!:https://developer.oculus.com/downloads/game-engines/1.5.0/Oculus_Utilities_for_Unity_5/">Oculus
            ///     Utilities for Unity</see>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <param name="axisMask">An <see cref="OVRInput.Axis1D" /> enum value which represents a bitmask corresponding to one of the hard-coded 1-dimensional input axes supported by Oculus devices.</param>
            public void toggleAxis1D(OVRInput.Axis1D axisMask) {
                if (axisMask != OVRInput.Axis1D.None) {
                    _previousAxis1D = this.axis1D;
                    _previousAxis2D = this.axis2D;
                    _axis2D = OVRInput.Axis2D.None;
                    _axis1D = axisMask;
                } else {
                    _previousAxis1D = this.axis1D;
                    _axis1D = OVRInput.Axis1D.None;
                    _axis2D = _previousAxis2D;
                }
            }
            /// <summary>
            ///   <para>Toggles the Oculus Axis defined by the <strong>axisMask</strong> parameter so as to associate the Input Axis with an Oculus Touch Input Axis.</para>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <strong>Be careful!</strong> Oculus devices do not use virtual inputs or virtual mappings managed
            ///     through the <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Unity Input Manager</see>. Instead, they are accessed directly through the %Oculus
            ///     Utilities for Unity:https://developer.oculus.com/downloads/game-engines/1.5.0/Oculus_Utilities_for_Unity_5/%.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            ///   <para></para>
            /// </summary>
            /// <param name="axisMask">An <see cref="OVRInput.Axis2D" /> enum value which represents a bitmask corresponding to one of the hard-coded 2-dimensional input axes supported by Oculus devices.</param>
            public void toggleAxis2D(OVRInput.Axis2D axisMask) {
                if (axisMask != OVRInput.Axis2D.None) {
                    _previousAxis2D = this.axis2D;
                    _previousAxis1D = this.axis1D;
                    _axis1D = OVRInput.Axis1D.None;
                    _axis2D = axisMask;
                } else {
                    _previousAxis2D = this.axis2D;
                    _axis2D = OVRInput.Axis2D.None;
                    _axis1D = this._previousAxis1D;
                }
            }

            private OVRInput.RawAxis1D _previousRawAxis1D = OVRInput.RawAxis1D.Any;
            private OVRInput.RawAxis2D _previousRawAxis2D = OVRInput.RawAxis2D.None;

            protected OVRInput.RawAxis1D _rawAxis1D = OVRInput.RawAxis1D.Any;
            /// <summary>The bitmask by which this 1-dimensional raw input axis is accessed through the %Oculus Utilities for
            /// Unity:https://developer.oculus.com/downloads/game-engines/1.5.0/Oculus_Utilities_for_Unity_5/% API.</summary>
            public OVRInput.RawAxis1D rawAxis1D {
                get {
                    return this._rawAxis1D;
                }
                set {
                    if (value != this._rawAxis1D) {
                        toggleRawAxis1D(value);
                    }
                }
            }

            protected OVRInput.RawAxis2D _rawAxis2D = OVRInput.RawAxis2D.None;
            /// <summary>The bitmask by which this 2-dimensional raw input axis is accessed through the %Oculus Utilities for
            /// Unity:https://developer.oculus.com/downloads/game-engines/1.5.0/Oculus_Utilities_for_Unity_5/% API.</summary>
            public OVRInput.RawAxis2D rawAxis2D {
                get {
                    return this._rawAxis2D;
                }
                set {
                    if (value != this._rawAxis2D) {
                        toggleRawAxis2D(value);
                    }
                }
            }

            /// <summary>
            ///   <para>Toggles the Oculus Axis defined by the <strong>axisMask</strong> parameter so as to associate the Input Axis with an Oculus Touch Raw Input Axis.</para>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <strong>Be careful!</strong> Oculus devices do not use virtual inputs or virtual mappings managed
            ///     through the <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Unity Input Manager</see>. Instead, they are accessed directly through the %Oculus
            ///     Utilities for Unity:https://developer.oculus.com/downloads/game-engines/1.5.0/Oculus_Utilities_for_Unity_5/%.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <param name="axisMask">An <see cref="OVRInput.RawAxis1D" /> enum value which represents a bitmask corresponding to one of the hard-coded 1-dimensional raw input axes supported by Oculus devices.</param>
            public void toggleRawAxis1D(OVRInput.RawAxis1D axisMask) {
                if (axisMask != OVRInput.RawAxis1D.None) {
                    _previousRawAxis1D = this._rawAxis1D;
                    _previousRawAxis2D = this._rawAxis2D;
                    _rawAxis2D = OVRInput.RawAxis2D.None;
                    _rawAxis1D = axisMask;
                } else {
                    _previousRawAxis1D = this._rawAxis1D;
                    _rawAxis1D = OVRInput.RawAxis1D.None;
                    _rawAxis2D = this._previousRawAxis2D;
                }
            }
            /// <summary>
            ///   <para>Toggles the Oculus Axis defined by the <strong>axisMask</strong> parameter so as to associate the Input Axis with an Oculus Touch Raw Input Axis.</para>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <strong>Be careful!</strong> Oculus devices do not use virtual inputs or virtual mappings managed
            ///     through the <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Unity Input Manager</see>. Instead, they are accessed directly through the %Oculus
            ///     Utilities for Unity:https://developer.oculus.com/downloads/game-engines/1.5.0/Oculus_Utilities_for_Unity_5/%.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <param name="axisMask">An <see cref="OVRInput.RawAxis1D" /> enum value which represents a bitmask corresponding to one of the hard-coded 1-dimensional raw input axes supported by Oculus devices.</param>
            public void toggleRawAxis2D(OVRInput.RawAxis2D axisMask) {
                if (axisMask != OVRInput.RawAxis2D.None) {
                    _previousRawAxis2D = this.rawAxis2D;
                    _previousRawAxis1D = this.rawAxis1D;
                    _rawAxis1D = OVRInput.RawAxis1D.None;
                    _rawAxis2D = axisMask;
                } else {
                    _previousRawAxis2D = this.rawAxis2D;
                    _rawAxis2D = OVRInput.RawAxis2D.None;
                    _rawAxis1D = this._previousRawAxis1D;
                }
            }

            public override float value {
                get {
                    if (this.dimensions == 1) {
                        return OVRInput.Get(axis1D, controllerMask);
                    } else {
                        if (returnX) {
                            return value2D.x;
                        } else {
                            return value2D.y;
                        }
                    }
                }
            }

            public override Vector2 value2D {
                get {
                    if (this.dimensions == 2) {
                        return OVRInput.Get(axis2D, controllerMask);
                    } else {
                        Debug.LogError("[ImmerseumSDK] Axis:" + name + " is 1-Dimensional. Cannot return Vector2. Use value().");
                        throw new System.DataMisalignedException("Axis is uni-dimensional. Multiple dimensions expected.");
                    }
                }
            }

            public override float rawValue {
                get {
                    if (this.dimensions == 1) {
                        return OVRInput.Get(rawAxis1D, controllerMask);
                    } else {
                        if (returnX) {
                            return rawValue2D.x;
                        } else {
                            return rawValue2D.y;
                        }
                    }
                }
            }

            public override Vector2 rawValue2D {
                get {
                    if (this.dimensions == 2) {
                        return OVRInput.Get(rawAxis2D, controllerMask);
                    } else {
                        Debug.LogError("[ImmerseumSDK] Axis:" + name + " is 1-Dimensional. Cannot return Vector2. Use rawValue().");
                        throw new System.DataMisalignedException("Axis is uni-dimensional. Multiple dimensions expected.");
                    }
                }
            }
        }
    }
}