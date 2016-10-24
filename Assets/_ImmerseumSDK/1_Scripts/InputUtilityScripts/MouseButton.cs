using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {

        /// <summary>
        ///   <para>Represents a virtual button defined in <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Unity's Input Manager</see> that corresponds to a mouse
        /// button.</para>
        ///   <para>By including this Input Button in an <see cref="InputAction" /> (on one of the Button lists), you associate the input device with the action that you want the user's
        /// input to evoke.</para>
        /// </summary>
        public class MouseButton : InputButton {
            /// <summary>The index (integer) of the mouse button associated with this Input Button.</summary>
            /// <value>
            ///   <para>An integer value. Accepts one of the following values:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>0 </strong>- the left mouse button</item>
            ///     <item>1 - the right mouse button</item>
            ///     <item>
            ///       <strong>2</strong> - the middle mouse button</item>
            ///   </list>
            /// </value>
            [Range(0f, 2f)]
            public int buttonIndex = 0;

            public new InputType buttonType {
                get {
                    return InputType.Mouse;
                }
            }

            public override bool isHeld {
                get {
                    return Input.GetMouseButton(buttonIndex);
                }
            }

            public override bool isPressed {
                get {
                    return Input.GetMouseButtonDown(buttonIndex);
                }
            }

            public override bool isReleased {
                get {
                    return Input.GetMouseButtonUp(buttonIndex);
                }
            }

            public override bool isPressedAndHeld {
                get {
                    return Input.GetMouseButtonDown(buttonIndex) && Input.GetMouseButton(buttonIndex);
                }
            }

            public override bool isClicked {
                get {
                    return Input.GetMouseButtonDown(buttonIndex) && Input.GetMouseButtonUp(buttonIndex);
                }
            }
        }
    }
}