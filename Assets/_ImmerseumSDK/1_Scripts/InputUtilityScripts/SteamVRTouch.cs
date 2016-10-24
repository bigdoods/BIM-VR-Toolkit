using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {
        /// <summary>
        ///   <para>Represents a virtual $$Touch$$ button accessible through the <see cref="!:https://www.assetstore.unity3d.com/en/#!/content/32647">SteamVR Plugin for Unity</see>, specifically
        /// for input devices that are supported by SteamVR but are not supported in the <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Unity Input Manager</see>.</para>
        ///   <para>By including this Input Button in an <see cref="InputAction" />, you associate the input device with the action that you want the user's input to evoke.</para>
        /// </summary>
        public class SteamVRTouch : SteamVRButton {
            public override bool isHeld {
                get {
                    if (HMDSimulator.isTrackingHands && isOutOfRange == false) {
                        return SteamVR_Controller.Input(deviceIndex).GetTouch(buttonMask);
                    }
                    return false;
                }
            }

            public override bool isPressed {
                get {
                    if (HMDSimulator.isTrackingHands && isOutOfRange == false) {
                        return SteamVR_Controller.Input(deviceIndex).GetTouchDown(buttonMask);
                    }
                    return false;
                }
            }

            public override bool isReleased {
                get {
                    if (HMDSimulator.isTrackingHands && isOutOfRange == false) {
                        return SteamVR_Controller.Input(deviceIndex).GetTouchUp(buttonMask);
                    }
                    return false;
                }
            }
        }
    }
}
