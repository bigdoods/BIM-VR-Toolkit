using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {
        /// <summary>
        ///   <para>Represents a $$CapacitiveTouch$$ virtual button accessible through the <see cref="!:https://developer.oculus.com/downloads/game-engines/1.5.0/Oculus_Utilities_for_Unity_5/% ">Oculus Utilities for
        /// Unity</see>OVRInput% API.</para>
        ///   <para>By including this Input Button in an <see cref="InputAction" />, you associate the input device with the action that you want the user's input to evoke.</para>
        /// </summary>
        public class OculusTouchButton : OculusButton {
            /// <summary>Determines the particular <see cref="OVRInput.Touch" /> on the Oculus input device to associate with this Input Button.</summary>
            /// <value>See <see cref="OVRInput.Touch" /> for a detailed listing of acceptable values.</value>
            public new OVRInput.Touch buttonMask = OVRInput.Touch.Any;
            /// <summary>Determines the particular <see cref="OVRInput.RawTouch" /> on the Oculus input device to associate with this Input Button.</summary>
            /// <value>See <see cref="OVRInput.RawTouch" /> for a detailed listing of acceptable values.</value>
            public new OVRInput.RawTouch rawButtonMask = OVRInput.RawTouch.Any;
        }
    }
}