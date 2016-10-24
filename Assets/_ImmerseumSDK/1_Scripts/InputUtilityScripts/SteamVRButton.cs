using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {
        /// <summary>
        ///   <para>Represents a virtual button accessible through the <see cref="!:https://www.assetstore.unity3d.com/en/#!/content/32647">SteamVR Plugin for
        /// Unity</see>, specifically for input devices that are supported by SteamVR but are not supported in the <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Unity Input Manager</see>.</para>
        ///   <para>By including this Input Button in an <see cref="InputAction" />, you associate the input device with the action that you want the user's input to evoke.</para>
        /// </summary>
        public class SteamVRButton : InputButton {
            /// <summary>A bitmask which identifies the button on the SteamVR input device.</summary>
            /// <value>
            ///   <para>A SteamVR_Controller.Button enum value. Accepts any of the following options:</para>
            ///   <list type="bullet">
            ///     <item><strong>System</strong> - indicates the system button on an HTC Wand/Vive Controller,</item>
            ///     <item><strong>ApplicationMenu</strong> - indicates the menu button on an HTC Wand / Vive Controller,</item>
            ///     <item>
            ///             <strong>Grip</strong> - indicates the Grip button on an
            ///     HTC Wand / Vive Controller,
            ///     </item>
            ///     <item>
            ///               <strong>Axis0</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>Axis1</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>Axis2</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>Axis3</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>Axis4</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>Touchpad</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>Trigger</strong> - TBD
            ///     </item>
            ///   </list>
            /// </value>
            public ulong buttonMask = SteamVR_Controller.ButtonMask.Trigger;

            /// <summary>Provides the index position of the SteamVR Input Device determined by <see cref="deviceRelation" />.</summary>
            /// <value>An integer value.</value>
            public int deviceIndex {
                get {
                    return SteamVR_Controller.GetDeviceIndex(deviceRelation);
                }
            }

            public override InputType buttonType {
                get {
                    return InputType.SteamVR;
                }
            }

            /// <summary>
            ///   <para>Indicates which SteamVR input device the Input Axis is associated with.</para>
            ///   <innovasys:widget type="Tip Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Because of the interchangability / swappability of position-tracked SteamVR input devices like the
            ///     HTC Wands/Vive Controllers, this field is used to identify a specific device based on its position relative to the HMD.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>
            ///   <para>A value from the <see cref="SteamVR_Controller.DeviceRelation" /> enum indicating which SteamVR input device to use for this Input Axis. Accepts the following
            /// values:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>First</strong> - indicates the first input device connected to the user's computer,</item>
            ///     <item>
            ///       <strong>FarthestLeft</strong> - indicates the SteamVR input device that is the furthest to the left of the $$HMD$$,</item>
            ///     <item>
            ///       <strong>FarthestRight</strong> - indicates the SteamVR input device that is the furthest to the right of the $$HMD$$,</item>
            ///     <item>
            ///       <strong>Leftmost</strong> [deprecated] - indicates the SteamVR input device that at some point is furthest to the left relative to the
            ///     $$HMD$$,</item>
            ///     <item>
            ///       <strong>Rightmost</strong> [deprecated] - indicates the SteamVR input device that at some poitn is furthest to the right relative to the
            ///     $$HMD$$.</item>
            ///   </list>
            ///   <innovasys:widget type="Caution Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <para>
            ///         <strong>Be careful!</strong> Because of a combination of legacy and ambiguous coding in the SteamVR plugin, it is recommended to use
            ///         <strong>FarthestLeft</strong> and <strong>FarthestRight</strong> in place of Leftmost and Rightmost.</para>
            ///       <para>Unexpected behaviors will likely result if you use <strong>Leftmost</strong> or <strong>Rightmost</strong>.</para>
            ///     </innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </value>
            public SteamVR_Controller.DeviceRelation deviceRelation = SteamVR_Controller.DeviceRelation.First;

            /// <summary>Indicates whether the SteamVR Input Device identified by <see cref="SteamVRAxis.deviceRelation">deviceRelation</see> currently is out of range.</summary>
            /// <value>
            ///   <strong>true</strong> if the SteamVR Input Device is currently Out of Range, otherwise <strong>false</strong>.</value>
            public bool isOutOfRange {
                get {
                    return SteamVR_Controller.Input(deviceIndex).outOfRange;
                }
            }

            /// <summary>Indicates whether the SteamVR Input Device identified by <see cref="SteamVRAxis.deviceRelation">deviceRelation</see> currently is connected.</summary>
            /// <value>
            ///   <strong>true</strong> if the SteamVR Input Device is currently connected, otherwise <strong>false</strong>.</value>
            public bool isConnected {
                get {
                    return SteamVR_Controller.Input(deviceIndex).connected;
                }
            }

            /// <summary>Indicates whether the SteamVR Input Device identified by <see cref="SteamVRAxis.deviceRelation">deviceRelation</see> currently is calibrating.</summary>
            /// <value>
            ///   <strong>true</strong> if the SteamVR Input Device is currently calibrating, otherwise <strong>false</strong>.</value>
            public bool isCalibrating {
                get {
                    return SteamVR_Controller.Input(deviceIndex).calibrating;
                }
            }

            /// <summary>Indicates whether the SteamVR Input Device identified by <see cref="SteamVRAxis.deviceRelation">deviceRelation</see> currently $$has positional tracking$$.</summary>
            /// <value>
            ///   <strong>true</strong> if the SteamVR device $$has positional tracking$$, otherwise <strong>false</strong>.</value>
            public bool hasTracking {
                get {
                    return SteamVR_Controller.Input(deviceIndex).hasTracking;
                }
            }

            public override bool isHeld {
                get {
                    if (HMDSimulator.isTrackingHands && isOutOfRange == false) {
                        return SteamVR_Controller.Input(deviceIndex).GetPress(buttonMask);
                    }
                    return false;
                }
            }

            public override bool isPressed {
                get {
                    if (HMDSimulator.isTrackingHands && isOutOfRange == false) {
                        return SteamVR_Controller.Input(deviceIndex).GetPressDown(buttonMask);
                    }
                    return false;
                }
            }

            public override bool isReleased {
                get {
                    if (HMDSimulator.isTrackingHands && isOutOfRange == false) {
                        return SteamVR_Controller.Input(deviceIndex).GetPressUp(buttonMask);
                    }
                    return false;
                }
            }
        }
    }
}