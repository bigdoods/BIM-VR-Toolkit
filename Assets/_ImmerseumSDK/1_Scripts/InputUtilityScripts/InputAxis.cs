using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {
        /// <summary>
        ///   <para>Represents a virtual axis defined in <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Unity's Input Manager</see>.</para>
        ///   <para>By including this Input Axis in an <see cref="InputAction" />, you associate the input device with the action that you want the user's input to evoke.</para>
        /// </summary>
        public class InputAxis {
            /// <summary>
            ///   <para>Indicates the number of dimensions that the axis returns a value in.</para>
            /// </summary>
            /// <value>
            ///   <para>Returns an integer, typically either <strong>1</strong> or <strong>2</strong> depending on the input device the virtual axis is mapped to:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <para>Input Axes with <strong>1</strong> dimension return a floating point value accessed through the properties: <see cref="value" /> and <see cref="rawValue" />.</para>
            ///     </item>
            ///     <item>
            ///       <para>Input Axes with with <strong>2</strong> dimensions return <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> values through the properties: <see cref="value2D" /> and <see cref="rawValue2D" />.</para>
            ///     </item>
            ///   </list>
            /// </value>
            public int dimensions {
                get {
                    return 1;
                }
            }

            /// <summary>Determines which component from a 2-dimensional value to return.</summary>
            /// <value>True if the value to be returned is the X value ina  2-dimensional value.</value>
            public bool returnX = true;

            /// <summary>Determines the device type that the Input Axis' virtual axis maps to.</summary>
            /// <value>
            ///   <para>An <see cref="Immerseum.VRSimulator.InputType">InputType</see>, which can have any of the following values:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>Keyboard</strong> - if the virtual axis is mapped to keyboard input,</item>
            ///     <item>
            ///       <strong>Mouse</strong> - if the virtual axis is mapped to mouse movement,</item>
            ///     <item>
            ///       <strong>Oculus</strong> - if the virtual axis is mapped to an Oculus-device (e.g. Oculus Touch or Oculus Remote),</item>
            ///     <item>
            ///       <strong>SteamVR</strong> - if the virtual axis is mapped to a SteamVR-device (e.g. HTC Wands/Vive Wands, etc.),</item>
            ///     <item>
            ///       <strong>Gamepad</strong> - if the virtual axis is mapped to a gamepad (e.g. Xbox One Controller).</item>
            ///   </list>
            /// </value>
            public InputType inputType = InputType.Gamepad;

            /// <summary>
            ///   <para>The name given to the Input Axis.</para>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <strong>Be careful!</strong> To properly recognize input from the user's input devices, this value
            ///     must correspond to a <strong>Name</strong> given to a virtual axis in the <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Unity Input Manager</see>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A string corresponding to the <strong>Name</strong> property of a virtual axis defined in the <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Unity Input Manager</see>.</value>
            public string name;
            /// <summary>
            ///   <para>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of floating point values, where each floating
            /// point value represents an $$axis value$$ where the virtual axis can be considered to be $$at rest$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">The way some device drivers operate on certain operating systems (notably gamepads), a virtual axis
            ///     may actually have multiple $$at rest$$ values. That's why the Immerseum Input Axes support a list of possible $$at rest$$
            ///     values.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of floating point values.</value>
            public List<float> valueAtRestList = new List<float>();
            /// <summary>Determines whether the Input Axis has a minimum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Input Axis has a minimum value, otherwise <strong>false</strong>.</value>
            public bool hasMinimumValue = true;
            /// <summary>Determines whether the Input Axis has a maximum value.</summary>
            /// <value>
            ///   <strong>true</strong> if the Input Axis has a maximum value, otherwise <strong>false</strong>.</value>
            public bool hasMaximumValue = true;
            /// <summary>The maximum value that the virtual axis can return.</summary>
            /// <value>A floating point value</value>
            public float maximumValue = 1f;
            /// <summary>The maximum value that the virtual axis can return.</summary>
            /// <value>A floating point value</value>
            public float minimumValue = 1f;

            /// <summary>The $$axis value$$ returned from the Input Axis.</summary>
            /// <value>A floating point value of the virtual axis' current $$axis value$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown if this property were accessed but the Input Axis has more than 1 %dimension:dimensions%.</exception>
            public virtual float value {
                get {
                    if (dimensions == 1) {
                        return Input.GetAxis(name);
                    } else {
                        if (returnX) {
                            return value2D.x;
                        } else {
                            return value2D.y;
                        }
                    }
                }
            }

            /// <summary>The $$raw value$$ returned from the Input Axis.</summary>
            /// <value>A floating point value of the virtual axis' current $$raw value$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown if this property were accessed but the Input Axis has more than 1 %dimension:dimensions%.</exception>
            public virtual float rawValue {
                get {
                    if (dimensions == 1) {
                        return Input.GetAxisRaw(name);
                    } else {
                        if (returnX) {
                            return rawValue2D.x;
                        } else {
                            return rawValue2D.y;
                        }
                    }
                }
            }

            /// <summary>The $$axis value$$ returned from the Input Axis, expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> value of the virtual axis' current $$axis value$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown if this property were accessed but the Input Axis has only 1 %dimension:dimensions%.</exception>
            public virtual Vector2 value2D {
                get {
                    Debug.LogError("[ImmerseumSDK] Axis:" + name + " is 1-Dimensional. Cannot return Vector2. Use value().");
                    throw new System.DataMisalignedException("Axis is uni-dimensional. Multiple dimensions expected.");
                }
            }

            /// <summary>The $$raw value$$ returned from the Input Axis, expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>.</summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> value of the virtual axis' current $$raw value$$.</value>
            /// <exception caption="DataMisalignedException" cref="System.DataMisalignedException">Thrown if this property were accessed but the Input Axis has only 1 %dimension:dimensions%.</exception>
            public virtual Vector2 rawValue2D {
                get {
                    Debug.LogError("[ImmerseumSDK] Axis:" + name + " is 1-Dimensional. Cannot return Vector2. Use rawValue().");
                    throw new System.DataMisalignedException("Axis is uni-dimensional. Multiple dimensions expected.");
                }
            }

            /// <summary>Indicates whether the Input Axis is currently $$at rest$$.</summary>
            /// <value>
            ///   <strong>true</strong> if the Input Axis is currently $$at rest$$, otherwise <strong>false</strong>.</value>
            public bool isAtRest {
                get {
                    int n = valueAtRestList.Count;
                    for (int x = 0; x < n; x++) {
                        if (valueAtRestList[x] == value) {
                            return true;
                        }
                    }
                    return false;
                }
            }

            /// <summary>Indicates whether the Input Axis is currently at its <see cref="maximumValue">maximum value</see>.</summary>
            /// <value>If the Input Axis <see cref="hasMaximumValue">has a maximum value</see> and its current <see cref="value" /> is equal to that maximum value returns <strong>true</strong>. Otherwise,
            /// returns <strong>false</strong>.</value>
            public bool isAtMax {
                get {
                    if (value == maximumValue && hasMaximumValue) {
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>Indicates whether the Input Axis is currently at its <see cref="minimumValue">minimum value</see>.</summary>
            /// <value>If the Input Axis <see cref="hasMinimumValue">has a minimum value</see> and its current <see cref="value" /> is equal to that minimum value, returns <strong>true</strong>. Otherwise,
            /// returns <strong>false</strong>.</value>
            public bool isAtMin {
                get {
                    if (value == minimumValue && hasMinimumValue) {
                        return true;
                    }
                    return false;
                }
            }
        }
    }
}