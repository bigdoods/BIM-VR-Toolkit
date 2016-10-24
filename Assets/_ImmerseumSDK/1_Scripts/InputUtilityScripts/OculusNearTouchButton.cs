using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {
        /// <summary>
        ///   <para>Represents a $$NearTouch$$ virtual button accessible through the <see cref="!:https://developer.oculus.com/downloads/game-engines/1.5.0/Oculus_Utilities_for_Unity_5/% ">Oculus Utilities for
        /// Unity</see>OVRInput% API.</para>
        ///   <para>By including this Input Button in an <see cref="InputAction" />, you associate the input device with the action that you want the user's input to evoke.</para>
        /// </summary>
        public class OculusNearTouchButton : OculusButton {
            /// <summary>Determines the particular <see cref="OVRInput.NearTouch" /> on the Oculus input device to associate with this Input Button.</summary>
            /// <value>See <see cref="OVRInput.NearTouch" /> for a detailed listing of acceptable values.</value>
            public new OVRInput.NearTouch buttonMask = OVRInput.NearTouch.Any;
            /// <summary>Determines the particular <see cref="OVRInput.RawNearTouch" /> on the Oculus input device to associate with this Input Button.</summary>
            /// <value>See <see cref="OVRInput.RawNearTouch" /> for a detailed listing of acceptable values.</value>
            public new OVRInput.RawNearTouch rawButtonMask = OVRInput.RawNearTouch.Any;
        }
    }
}