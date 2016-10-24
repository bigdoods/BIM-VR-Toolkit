using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {
        /// <summary>
        ///   <para>Represents a virtual axis accessible through the <see cref="!:https://www.assetstore.unity3d.com/en/#!/content/32647">SteamVR Plugin for
        /// Unity</see>, specifically for input devices that are supported by SteamVR but are not supported in the <see cref="!:https://docs.unity3d.com/Manual/class-InputManager.html">Unity Input Manager</see>.</para>
        ///   <para>By including this Input Axis in an <see cref="InputAction" />, you associate the input device with the action that you want the user's input to evoke.</para>
        /// </summary>
        public class SteamVRAxis : InputAxis {
            /// <summary>A bitmask which identifies the button associated with the Input Axis.</summary>
            /// <value>
            ///   <para>A Valve.VR.EVRButtonId enum value. Accepts any of the following options:</para>
            ///   <list type="bullet">
            ///     <item><strong>k_EButton_System</strong> - indicates the system button on an HTC Wand/Vive Controller,</item>
            ///     <item><strong>k_EButton_ApplicationMenu</strong> - indicates the menu button on an HTC Wand / Vive Controller,</item>
            ///     <item><strong>k_EButton_DPad_Left</strong> - indicates the Left D-Pad button on an HTC Wand / Vive Controller, </item>
            ///     <item>
            ///               <strong>k_EButton_DPad_Up</strong> - indicates the Up D-Pad button on an HTC Wand / Vive Controller
            ///     </item>
            ///     <item>
            ///               <strong>k_EButton_DPad_Down</strong> - indicates the Down D-Pad button on an HTC Wand / Vive Controller,
            ///     </item>
            ///     <item>
            ///             <strong>k_EButton_Grip</strong> - indicates the Grip button on an
            ///     HTC Wand / Vive Controller,
            ///     </item>
            ///     <item>
            ///               <strong>k_EButton_A</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>k_EButton_Axis0</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>k_EButton_Axis1</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>k_EButton_Axis2</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>k_EButton_Axis3</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>k_EButton_Axis4</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>k_EButton_SteamVR_Touchpad</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>k_EButton_SteamVR_Trigger</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>k_EButton_Dashboard_Back</strong> - TBD
            ///     </item>
            ///     <item>
            ///               <strong>k_EButton_Max</strong> - TBD
            ///     </item>
            ///   </list>
            /// </value>
            public Valve.VR.EVRButtonId buttonMask = Valve.VR.EVRButtonId.k_EButton_Axis0;

            public new int dimensions {
                get {
                    return 2;
                }
            }

            /// <summary>Provides the index position of the SteamVR Input Device determined by <see cref="deviceRelation" />.</summary>
            /// <value>An integer value.</value>
            public int deviceIndex {
                get {
                    return SteamVR_Controller.GetDeviceIndex(deviceRelation);
                }
            }

            public new InputType inputType {
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

            public override Vector2 value2D {
                get {
                    return SteamVR_Controller.Input(deviceIndex).GetAxis(buttonMask);
                }
            }
        }
    }
}