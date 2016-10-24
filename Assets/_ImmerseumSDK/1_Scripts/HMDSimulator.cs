#if !UNITY_5_4_OR_NEWER
#error The Immerseum SDK requires Unity v.5.4 or newer.
#endif

#if UNITY_EDITOR_OSX
#warning Oculus Rift support is extremely limited on MacOS. Certain functionality will not work in play mode, though you can build to Windows systems.
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR;

namespace Immerseum {
    namespace VRSimulator {
        /// <summary>Unity Primitive Types that can be used to simulate position-tracked controllers.</summary>
        public enum PrimitiveType { /// <summary>Unity <see cref="!:https://docs.unity3d.com/ScriptReference/PrimitiveType.Cube.html">Cube</see> primitive</summary>
            Cube, /// <summary>Unity <see cref="!:https://docs.unity3d.com/ScriptReference/PrimitiveType.Cube.html">Sphere</see> primitive</summary>
            Sphere, /// <summary>Unity <see cref="!:https://docs.unity3d.com/ScriptReference/PrimitiveType.Cube.html">Cylinder</see> primitive</summary>
            Cylinder, /// <summary>Unity <see cref="!:https://docs.unity3d.com/ScriptReference/PrimitiveType.Cube.html">Capsule</see> primitive</summary>
            Capsule, /// <summary>A custom prefab</summary>
            Custom
        }
        /// <summary>A set of standard predefined heights used when simulating an HMD.</summary>
        public enum HeightTargets { /// <summary>
                                    ///   <span style="FONT-SIZE: 13px; FONT-FAMILY: &quot;Segoe UI&quot;, Verdana, Arial; WHITE-SPACE: normal; WORD-SPACING: 0px; TEXT-TRANSFORM: none; FLOAT: none; FONT-WEIGHT: normal; COLOR: rgb(0,0,0); FONT-STYLE: normal; TEXT-ALIGN: left; ORPHANS: 2; WIDOWS: 2; DISPLAY: inline !important; LETTER-SPACING: normal; LINE-HEIGHT: normal; TEXT-INDENT: 0px; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px">
                                    /// This lets you set your own custom height to simulate whatever height you wish.</span>
                                    /// </summary>
            Custom, /// <summary>
                    ///   <span style="FONT-SIZE: 13px; FONT-FAMILY: &quot;Segoe UI&quot;, Verdana, Arial; WHITE-SPACE: normal; WORD-SPACING: 0px; TEXT-TRANSFORM: none; FLOAT: none; FONT-WEIGHT: normal; COLOR: rgb(0,0,0); FONT-STYLE: normal; TEXT-ALIGN: left; ORPHANS: 2; WIDOWS: 2; DISPLAY: inline !important; LETTER-SPACING: normal; LINE-HEIGHT: normal; TEXT-INDENT: 0px; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px">
                    /// This simulates a seated posture, significantly lower than the standing height mentioned above (set to approx. 1.06).</span>
                    /// </summary>
            Seated, /// <summary>
                    ///   <span style="FONT-SIZE: 13px; FONT-FAMILY: &quot;Segoe UI&quot;, Verdana, Arial; WHITE-SPACE: normal; WORD-SPACING: 0px; TEXT-TRANSFORM: none; FLOAT: none; FONT-WEIGHT: normal; COLOR: rgb(0,0,0); FONT-STYLE: normal; TEXT-ALIGN: left; ORPHANS: 2; WIDOWS: 2; DISPLAY: inline !important; LETTER-SPACING: normal; LINE-HEIGHT: normal; TEXT-INDENT: 0px; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px">
                    /// This simulates standing or room-scale VR. It takes the statistical average human height (1.775) and adjusts for stnadard eye level differences (setting eye
                    /// level to approximately 1.65).</span>
                    /// </summary>
            Standing
        }
        /// <summary>Indicates a family of HMD.</summary>
        public enum hmdFamilies { /// <summary>None</summary>
            None, /// <summary>Any SteamVR-compatible HMD.</summary>
            SteamVR, /// <summary>Any Oculus Rift-compatible HMD.</summary>
            Oculus, /// <summary>The PlayStationVR</summary>
            PlayStationVR
        }
        /// <summary>Used to indicate the camera rig packages that are present within the VR scene.</summary>
        public enum CameraRigPackages { /// <summary>Neither the SteamVR nor OculusRift camera rigs.</summary>
            None, /// <summary>Only the SteamVR camera rig.</summary>
            SteamVR, /// <summary>Only the Oculus Rift camera rig.</summary>
            OculusRift, /// <summary>Both the SteamVR and OculusRift camera rigs.</summary>
            Both
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>Singleton responsible for managing the simulation of a VR head-mounted display (HMD) and position-tracked controllers.</summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public class HMDSimulator : MonoBehaviour {
            /// <summary>The singleton Instance of the HMDSimulator class.</summary>
            public static HMDSimulator Instance { get; private set; }

            protected static bool _displayedConnectionWarningOnce = false;

            [SerializeField]
            protected bool _disableInStandaloneBuild = true;
            /// <summary>Indicates whether the VR Simulator should be disabled when running in a standalone build.</summary>
            /// <value>
            ///   <strong>true</strong> if the VR Simulator should be disabled in a standalone build, otherwise <strong>false</strong>.</value>
            public static bool disableInStandaloneBuild {
                get {
                    return Instance._disableInStandaloneBuild;
                }
            }

            /// <summary>   If true, displays Help Boxes in the VR Simulator Unity Inspector. </summary>
            [SerializeField]
            protected bool _displayHelpBoxes = true;

            /// <summary>   true to log diagnostic information to the console.</summary>
            [SerializeField]
            protected bool _logDiagnostics = false;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Gets a value indicating whether Diagnostic Information should be logged to the console.</summary>
            /// <value>
            ///   <strong>true</strong> if Diagnostic events should be logged to the console, <strong>false</strong> if not.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool logDiagnostics {
                get {
                    return Instance._logDiagnostics;
                }
            }


            /// <summary>   true to log input actions to the console. </summary>
            [SerializeField]
            protected bool _logInputActions = false;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Gets a value indicating whether <see cref="InputAction">InputAction</see> events should be logged to the console.</summary>
            /// <value>
            ///   <strong>true</strong> if <see cref="InputAction">InputAction</see> events should be logged to the console, <strong>false</strong> if not.</value>
            /// <seealso cref="InputAction">InputAction</seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool logInputActions {
                get {
                    return Instance._logInputActions;
                }
            }

            #region Environment Reference

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Determines which VR plugins have been imported into the scene based on the camera rig objects
            /// that are present and active in the scene hierarchy.
            /// </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private CameraRigPackages _packagesPresent;

            #endregion

            #region Controller Configuration
            /// <summary>   true to simulate position-tracked controllers. </summary>
            [SerializeField]
            protected bool _simulateControllers = false;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Indicates whether to simulate position-tracked controllers or not.</summary>
            /// <value>
            ///   <strong>true</strong> if controllers should be simulated, <strong>false</strong> if not.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool simulateControllers {
                get {
                    return Instance._simulateControllers;
                }
            }

            /// <summary>   The primitive to use when simulating position-tracked controllers. </summary>
            [SerializeField]
            protected PrimitiveType _controllerPrimitive = PrimitiveType.Sphere;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>If <see cref="simulateControllers" /> is set to <strong>true</strong>, will return the Unity primitive that will be used to simulate position-tracked controllers.</summary>
            /// <value>
            ///   <para>Supports the following options:</para>
            ///   <para></para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>Cube</strong> applies the Unity-standard <see cref="!:https://docs.unity3d.com/ScriptReference/PrimitiveType.Cube.html">Cube</see> primitive.</item>
            ///     <item>
            ///       <strong>Sphere</strong> applies the Unity-standard <see cref="!:https://docs.unity3d.com/ScriptReference/PrimitiveType.Sphere.html">Sphere</see> primitive.</item>
            ///     <item>
            ///       <strong>Cylinder</strong> applies the Unity-standard <see cref="!:https://docs.unity3d.com/ScriptReference/PrimitiveType.Cylinder.html">Cylinder</see> primitive.</item>
            ///     <item>
            ///       <strong>Capsule</strong> applies the Unity-standard <see cref="!:https://docs.unity3d.com/ScriptReference/PrimitiveType.Capsule.html">Capsule</see> primitive.</item>
            ///     <item>
            ///       <strong>Custom</strong> applies a custom primitive, based on the references indicated by <see cref="leftControllerPrefab" /> and <see cref="rightControllerPrefab" />.</item>
            ///   </list>
            /// </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static PrimitiveType controllerPrimitive {
                get {
                    return Instance._controllerPrimitive;
                }
            }

            [SerializeField]
            [Range(0, 5)]
            protected float _primitiveScaling = 0.15f;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>If <see cref="simulateControllers" /> is set to <strong>true</strong> and <see cref="controllerPrimitive" /> is not <strong>Custom</strong>, returns the scaling factor to apply to the
            /// controller primitive.</summary>
            /// <value>
            /// The primitive scaling factor expressed as a floating point value, where 1 is equal to the
            /// primitive's default local scale.
            /// </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static float primitiveScaling {
                get {
                    return Mathf.Abs(Instance._primitiveScaling);
                }
            }

            /// <summary>   The prefab that will be used to simulate the left-hand position-tracked controller.</summary>
            [SerializeField]
            protected GameObject _leftControllerPrefab;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            ///   <para>Returns the $$prefab$$ <see cref="!:https://docs.unity3d.com/Manual/class-GameObject.html">GameObject</see> that will be used to
            /// simulate the left-hand controller. Can be any prefab you wish.</para>
            /// </summary>
            /// <remarks>Only applies if <see cref="simulateControllers" /> is true and <see cref="controllerPrimitive" /> is Custom.</remarks>
            /// <value>Any $$prefab$$ <see cref="!:https://docs.unity3d.com/Manual/class-GameObject.html">GameObject</see> found in your Unity project folder.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static GameObject leftControllerPrefab {
                get {
                    return Instance._leftControllerPrefab;
                }
            }

            /// <summary>   The prefab that will be used to simulate the right-hand position-tracked controller. </summary>
            [SerializeField]
            protected GameObject _rightControllerPrefab;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            ///   <para>Returns the $$prefab$$ <see cref="!:https://docs.unity3d.com/Manual/class-GameObject.html">GameObject</see> that will be used to
            /// simulate the right-hand controller. Can be any prefab you wish.</para>
            /// </summary>
            /// <remarks>Only applies if <see cref="simulateControllers" /> is true and <see cref="controllerPrimitive" /> is Custom.</remarks>
            /// <value>Any $$prefab$$ <see cref="!:https://docs.unity3d.com/Manual/class-GameObject.html">GameObject</see> found in your Unity project folder.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static GameObject rightControllerPrefab {
                get {
                    return Instance._rightControllerPrefab;
                }
            }

            /// <summary>   A reference to the instance of the left-hand controller (real or simulated). </summary>
            protected Transform _leftController;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The instance of the simulated left-hand controller in your scene. </summary>
            /// <remarks>   If there are no simulated controllers, will be null. </remarks>
            /// <value>The <see cref="!:https://docs.unity3d.com/Manual/class-Transform.html">Transform</see> of the left-hand controller.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static Transform leftController {
                get {
                    return Instance._leftController;
                }
            }

            /// <summary>   A reference to the instance of the right-hand controller (real or simulated). </summary>
            protected Transform _rightController;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The instance of the simulated right-hand controller in your scene. </summary>
            /// <remarks>   If there are no simulated controllers, will be null. </remarks>
            /// <value>The <see cref="!:https://docs.unity3d.com/Manual/class-Transform.html">Transform</see> of the right-hand controller.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static Transform rightController {
                get {
                    return Instance._rightController;
                }
            }
            #endregion

            #region Camera Configuration
            /// <summary>   true if Camera Initialization has been completed. </summary>
            protected bool _isCameraInitializationComplete = false;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Gets a value indicating whether the Camera Initialization process has been completed.
            /// </summary>
            /// <value>
            ///   <strong>true</strong> if camera initialization has been completed, <strong>false</strong> if not.</value>
            /// <seealso cref="P:Immerseum.VRSimulator.HMDSimulator.cameraInitializationTimeout">cameraInitializationTimeout</seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool isCameraInitializationComplete {
                get {
                    return Instance._isCameraInitializationComplete;
                }
            }

            /// <summary>   The number of seconds to wait before Camera Initialization gives up trying to find a connected HMD. </summary>
            [SerializeField]
            [Range(0.1f, 3f)]
            protected float _cameraInitializationTimeout = 2f;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets the number of seconds that the system should attempt to establish a connection with an HMD. </summary>
            ///
            /// <value> A floating point value that indicates the number of seconds that the system should spend trying to establish a connection to an HMD. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static float cameraInitializationTimeout {
                get {
                    return Instance._cameraInitializationTimeout;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Indicates whether a supported Camera Rig <see cref="!:https://docs.unity3d.com/Manual/class-GameObject.html">GameObject</see> is present within
            /// the scene or not.</summary>
            /// <value>
            ///   <strong>true</strong> if a <see cref="CameraRig" /> is present, <strong>false</strong> if not.</value>
            /// <seealso cref="P:Immerseum.VRSimulator.HMDSimulator.CameraRig">CameraRig</seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool isCameraRigPresent {
                get {
                    if (GameObject.Find("[CameraRig]") != null || GameObject.Find("OVRCameraRig") != null) {
                        return true;
                    } else {
                        return false;
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Indicates whether a supported VR Camera package is present in the scene hierarchy.</summary>
            /// <value>
            ///   <strong>true</strong> if an acceptable camera rig is present in the VR scene, <strong>false</strong> if not.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            protected static bool _isAcceptableCameraRigPresent {
                get {
                    if (Instance._packagesPresent != CameraRigPackages.None) {
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>   The camera rig. </summary>
            [SerializeField]
            protected Transform _CameraRig;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>The <see cref="!:https://docs.unity3d.com/Manual/class-Transform.html">Transform</see> of the <see cref="!:https://docs.unity3d.com/Manual/class-GameObject.html">GameObject</see> you are using as your VR CameraRig.</summary>
            /// <remarks>
            ///   <para>The VR CameraRig is <see cref="GameObject" /> that typically represents the rig (collection of GameObjects) that includes/represents your camera. Typically, the CameraRig will contain underlying objects that actually represent (or instantiate) the camera itself.</para>
            /// </remarks>
            /// <value>The <see cref="!:https://docs.unity3d.com/Manual/class-Transform.html">Transform</see> of a <see cref="!:https://docs.unity3d.com/Manual/class-GameObject.html">GameObject</see> in the scene hierarchy. Must be either a SteamVR plugin "[CameraRig]" or
            /// Oculus Rift "OVRCameraRig".</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static Transform CameraRig {
                get {
                    return Instance._CameraRig;
                }
            }

            /// <summary>Indicates whether the scene or character controller is using the HMD's stored profile data.</summary>
            /// <value>
            ///   <strong>true</strong> if stored profile data is being used, <strong>false</strong> if not.</value>
            public static bool isUsingProfileData {
                get {
                    if (isUsingSteamRig == false) {
                        if (CameraRig.GetComponentInParent<OVRPlayerController>() != null) {
                            return CameraRig.GetComponentInParent<OVRPlayerController>().useProfileData;
                        }
                    }
                    return false;
                }
            }

            /// <summary>   The target height. </summary>
            [SerializeField]
            protected HeightTargets _heightTarget;
            /// <summary>
            /// Indicates the camera height that you wish to simulate.
            /// </summary>
            /// <remarks>If set to Custom, camera height is set to <see cref="headHeight" />.</remarks>
            /// <value>
            ///   <para>Accepts one of the following values:</para>
            ///   <list type="bullet">
            ///     <item>Standing</item>
            ///     <item>Seated</item>
            ///     <item>Custom</item>
            ///   </list>
            /// </value>
            /// <seealso cref="headHeight"></seealso>
            /// <seealso cref="getDefaultHeadHeight(HeightTargets)"></seealso>
            /// <seealso cref="T:Immerseum.VRSimulator.HeightTargets"></seealso>
            public static HeightTargets heightTarget {
                get {
                    return Instance._heightTarget;
                }
            }

            /// <summary>   Height of the custom head. </summary>
            [SerializeField]
            [Range(0f, 5f)]
            protected float _customHeadHeight;

            /// <summary>Indicates whether the scene is actively making use of the <strong>OVRPlayerController</strong> prefab from the Oculus Utilities for Unity.</summary>
            /// <value>
            ///   <strong>true</strong> if the scene is using the <strong>OVRPlayerController</strong> prefab from the Oculus Utilities ffor Unity, <strong>false</strong> if
            /// not.</value>
            public static bool isUsingOVRPlayerController {
                get {
                    if (CameraRig.root.GetComponent<OVRPlayerController>() != null) {
                        return true;
                    }
                    return false;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Returns the height (in meters / unity units) of the camera's position.
            /// </summary>
            /// <value>A floating point value that indicates the height (Y) of the camera's position.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static float headHeight {
                get {
                    if (isHMDConnected) {
                        switch (hmdFamily) {
                            case hmdFamilies.SteamVR:
                                return CameraRig.position.y;
                            case hmdFamilies.Oculus:
                                if (CameraRig.root.GetComponent<OVRPlayerController>() != null) {
                                    if (CameraRig.root.GetComponent<OVRPlayerController>().useProfileData) {
                                        return OVRManager.profile.eyeHeight;
                                    }
                                }
                                break;
                            case hmdFamilies.PlayStationVR:
                                Debug.LogError("[ImmerseumSDK] PlayStation VR is not yet supported.");
                                goto case hmdFamilies.SteamVR;
                            default:
                                goto case hmdFamilies.SteamVR;
                        }
                    }
                    switch (heightTarget) {
                        case HeightTargets.Custom:
                            return Instance._customHeadHeight;
                        case HeightTargets.Seated:
                            return 1.06f;
                        case HeightTargets.Standing:
                            return 1.755f;
                        default:
                            goto case HeightTargets.Standing;
                    }
                }
            }
            #endregion

            #region Environment Context
            /// <summary>   The time at which the <see cref="Start"/> method began execution. </summary>
            protected static float _startTime = 0f;

            /// <summary>   The hmd family. </summary>
            protected hmdFamilies _hmdFamily = hmdFamilies.None;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>The family of $$HMD$$ which is currently connected.</summary>
            /// <value>
            ///   <para>Returns one of the following values:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>
            ///         <see cref="hmdFamilies.None" />
            ///       </strong> if no HMD is connected.</item>
            ///     <item>
            ///       <strong>
            ///         <see cref="hmdFamilies.Oculus" />
            ///       </strong> if an Oculus Rift is connected.</item>
            ///     <item>
            ///       <strong>
            ///         <see cref="hmdFamilies.SteamVR" />
            ///       </strong> if a SteamVR-compatible (e.g. Vive, OSVR, etc.) HMD is connected.</item>
            ///     <item>
            ///       <strong>
            ///         <see cref="hmdFamilies.PlayStationVR" />
            ///       </strong> if a PlayStationVR is connected (note: not currently supported).</item>
            ///   </list>
            /// </value>
            /// <seealso cref="T:Immerseum.VRSimulator.hmdFamilies"></seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static hmdFamilies hmdFamily {
                get {
                    return Instance._hmdFamily;
                }
            }

            /// <summary>   true if the user's system is currently positionally-tracking the user's hands. </summary>
            protected bool _isTrackingHands = false;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            ///   <para>Indicates whether the user's VR system is positionally tracking their hands (i.e. by way of a positionally-tracked handheld controller, such as the HTC
            /// Wands or the Oculus Touch).</para>
            /// </summary>
            /// <value>
            ///   <para>If <strong>true</strong>, indicates that the user is currently using positionally-tracked handheld controllers (e.g. the HTC Wands, the Oculus Touch,
            /// etc).</para>
            ///   <para>If <strong>false</strong>, indicates that there are no positionally-tracked handheld controllers connected to the user's VR system.</para>
            /// </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool isTrackingHands {
                get {
                    return Instance._isTrackingHands;
                }
            }

            /// <summary>   true if an Oculus Remote is connected. </summary>
            protected bool _isRemoteConnected = false;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Indicates whether the user has an Oculus Remote connected to their computer.</summary>
            /// <value>If <strong>true</strong>, indicates that there is an Oculus Remote connected to the user's VR system.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool isRemoteConnected {
                get {
                    return Instance._isRemoteConnected;
                }
            }

            protected bool _isGamepadConnected = false;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Indicates whether the user has a gamepad connected to the platform. </summary>
            /// <value>If <strong>true</strong>, indicates that there is a gamepad connected to the user's VR system. Otherwise, returns <strong>false</strong>.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool isGamepadConnected {
                get {
                    return Instance._isGamepadConnected;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Indicates whether there is a VR Head-Mounted Display (HMD) connected to the user's system. </summary>
            ///
            /// <value> true if an HMD is connected, false if not. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool isHMDConnected {
                get {
                    if (VRSettings.enabled == true) {
                        if (VRDevice.isPresent) {
                            return true;
                        }
                    } else {
                        if (SteamVR.connected[0] == true) {
                            return true;
                        } else if (OVRManager.isHmdPresent) {
                            return true;
                        }
                    }
                    return false;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Indicates whether there is an Oculus Rift $$HMD$$ connected to the user's system.</summary>
            /// <value>
            ///   <strong>true</strong> if an Oculus Rift is connected, <strong>false</strong> if not.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool isOculusHMDConnected {
                get {
                    if (OVRManager.isHmdPresent == true) {
                        return true;
                    }
                    return false;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Indicates whether a PlayStation VR is connected to the user's system.
            /// </summary>
            /// <value>
            ///   <strong>true</strong> if a PlayStation VR is connected, <strong>false</strong> if not.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool isPlaystationVRHMDConnected {
                get {
                    if (VRDevice.isPresent == true && VRSettings.loadedDeviceName == "PlayStationVR") {
                        return true;
                    }
                    return false;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Indicates whether a SteamVR $$HMD$$ (e.g. an HTC Vive) is connected to the user's system.</summary>
            /// <value>
            ///   <strong>true</strong> if a SteamVR HMD (e.g. an HTC Vive) is connected, <strong>false</strong> if not.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool isSteamVRHMDConnected {
                get {
                    if (isHMDConnected && isOculusHMDConnected == false && isPlaystationVRHMDConnected == false) {
                        return true;
                    }
                    return false;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary> Indicates whether the scene (and HMD Simulator) is using a SteamVR CameraRig. </summary>
            /// <value>
            ///   <strong>true</strong> if the scene is using a SteamVR <see cref="CameraRig" />, otherwise <strong>false</strong>.</value>
            /// <seealso cref="CameraRig"></seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool isUsingSteamRig {
                get {
                    try {
                        if (CameraRig.name == "[CameraRig]") {
                            return true;
                        } else {
                            return false;
                        }
                    } catch {
                        return false;
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets the <see cref="Transform" /> of the Camera Container. </summary>
            /// <value>The <see cref="!:https://docs.unity3d.com/Manual/class-Transform.html">Transform</see> of the <see cref="!:https://docs.unity3d.com/Manual/class-GameObject.html">GameObject</see> which contains the VR Camera.</value>
            /// <seealso cref="CameraRig"></seealso>
            /// <seealso cref="getCamera"></seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static Transform getCameraContainer {
                get {
                    if (isUsingSteamRig == false) {
                        return GameObject.Find("Camera Container").transform;
                    } else {
                        return CameraRig;
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Returns the <see cref="Transform" /> of the camera <see cref="GameObject" />. </summary>
            /// <value>The <see cref="!:https://docs.unity3d.com/Manual/class-Transform.html">Transform</see> of the Camera being used in this scene.</value>
            /// <seealso cref="CameraRig"></seealso>
            /// <seealso cref="getCameraContainer"></seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static Transform getCamera {
                get {
                    if (isCameraInitializationComplete) {
                        if (isUsingSteamRig) {
                            try {
                                return GameObject.Find("Camera (head)").transform;
                            }
                            catch (System.NullReferenceException) {
                                if (HMDSimulator._displayedConnectionWarningOnce == false) {
                                    Debug.LogWarning("[ImmerseumSDK] Camera (head) object was not found, so falling back to Camera (eye). This happens in Unity 5.4 when SteamVR is on and the Vive is plugged in, or if you have changed the name of the Camera (head) game object.");
                                    HMDSimulator._displayedConnectionWarningOnce = true;
                                }
                                return GameObject.Find("Camera (eye)").transform;
                            }
                        } else {
                            try {
                                return CameraRig;
                            } catch (UnassignedReferenceException e) {
                                Debug.LogError("[ImmerseumSDK] For some reason, the CameraRig is set to null. Something must have gone wrong somewhere.");
                                throw e;
                            }
                        }
                    } else {
                        throw new System.Exception("[ImmerseumSDK] Tried to do something with the Camera before initialization has finished. Wait until complete, then try again.");
                    }
                }
            }

            protected static float _standingAdjustment {
                get {
                    if (heightTarget == HeightTargets.Standing) {
                        return 0.495f;
                    }
                    return 0.195f;
                }
            }
            #endregion

            void Awake() {
                if (Instance != null && Instance != this) {
                    Destroy(gameObject);
                }

                Instance = this;
            }

            void OnEnable() {
                EventManager.OnInitializeHMDSimulator += OnInitializeHMDSimulator;
                EventManager.OnInitializeCameraRig += OnInitializeCameraRig;
                EventManager.OnInitializeCameraRigEnd += OnInitializeCameraRigEnd;
                EventManager.OnDisableVRSimulator += OnDisableVRSimulator;
            }

            void OnDisable() {
                EventManager.OnInitializeHMDSimulator -= OnInitializeHMDSimulator;
                EventManager.OnInitializeCameraRig -= OnInitializeCameraRig;
                EventManager.OnInitializeCameraRigEnd -= OnInitializeCameraRigEnd;
                EventManager.OnDisableVRSimulator -= OnDisableVRSimulator;
            }

            void OnDisableVRSimulator() {
                gameObject.SetActive(false);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Initializes the VR Simulator. </summary>
            ///
            /// <exception cref="PackageNotFoundException"> Thrown when one of the third-party packages that the ImmerseumSDK requires is not found. </exception>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void Start() {
                if (disableInStandaloneBuild) {
#if !UNITY_EDITOR
                    EventManager.disableVRSimulator();
                    if (logDiagnostics) {
                        Debug.LogWarning("[ImmerseumSDK] Running in standalone mode with the VR Simulator has been disabled.");
                    }
                    return;
#endif
                }
                if (!isCameraRigPresent) {
                    Debug.LogError("[ImmerseumSDK] No Camera Rig found in the scene.");
                }

                EventManager.initializeHMDSimulator();

                if (_isAcceptableCameraRigPresent) {
                    determineHMDFamily();

                    determineHandInputMode();

                    EventManager.initializeCameraRig();
                } else {
                    throw new PackageNotFoundException("[ImmerseumSDK] FATAL ERROR. Neither the SteamVR plugin ([CameraRig]) nor Oculus Rift (OVRCameraRig) cameras were found in your scene.");
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Initializes the HMD Simulator. </summary>
            ///
            /// <exception cref="ArgumentNullException">    Thrown when <see cref="simulateControllers"/> is
            ///                                             true, <see cref="controllerPrimitive"/> is set to
            ///                                             Custom, and either <see cref="leftControllerPrefab"/>
            ///                                             or <see cref="rightControllerPrefab"/> are null. </exception>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void OnInitializeHMDSimulator() {
                GameObject steamVR = GameObject.Find("[CameraRig]");
                GameObject oculusRift = GameObject.Find("OVRCameraRig");
                GameObject ovrPlayerController = GameObject.Find("OVRPlayerController");

                if (steamVR == null && oculusRift == null) {
                    _packagesPresent = CameraRigPackages.None;
                } else if (steamVR != null && oculusRift != null) {
                    _packagesPresent = CameraRigPackages.Both;
                } else if (steamVR != null && oculusRift == null) {
                    _packagesPresent = CameraRigPackages.SteamVR;
                } else {
                    _packagesPresent = CameraRigPackages.OculusRift;
                }

                if (ovrPlayerController != null) {
                    if (OVRManager.profile.eyeHeight == 0f) {
                        ovrPlayerController.GetComponent<OVRPlayerController>().useProfileData = false;
                    }
                }


                if (leftControllerPrefab == null && simulateControllers && controllerPrimitive == PrimitiveType.Custom) {
                    throw new System.ArgumentNullException("[ImmerseumSDK] / HMDSimulator: Controller Simulation is turned on using a Custom Controller, but Left Controller Prefab has not been set.");
                } else if (rightControllerPrefab == null && simulateControllers && controllerPrimitive == PrimitiveType.Custom) {
                    throw new System.ArgumentNullException("[ImmerseumSDK]/HMDSimulator: Controller Simulation is turned on using a Custom Controller, but Right Controller Prefab has not been set.");
                }

                if (leftControllerPrefab == null && !simulateControllers) {
                    Debug.LogWarning("[ImmerseumSDK]/HMDSimulator: Left Controller Prefab has not been set.");
                }
                if (rightControllerPrefab == null && !simulateControllers) {
                    Debug.LogWarning("[ImmerseumSDK]/HMDSimulator: Right Controller Prefab has not been set.");
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Initializes the CameraRig. </summary>
            ///
            /// <remarks>
            /// After some preliminary housekeeping, hands off initialization to <see cref="initializeCameraCoroutine"/>, which
            /// continues initialization.
            /// </remarks>
            ///
            /// <exception cref="NullReferenceException">   Thrown when no supported <see cref="CameraRig"/>
            ///                                             was indicated or found in the scene. </exception>
            ///
            /// <seealso cref="initializeCameraCoroutine"/>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void OnInitializeCameraRig() {
                StartCoroutine(initializeCameraCoroutine());
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary> Completes initialization of the VR Camera, either when a connection to an HMD is detected or when <see cref="cameraInitializationTimeout"/> has been reached. </summary>
            ///
            /// <returns> <para>A yield instruction to wait for (0.1f) seconds while <see cref="cameraInitializationTimeout"/> has not yet occured, and a recognized HMD has not yet been detected.</para>
            ///           <para>Upon completion, sets <see cref="_isCameraInitializationComplete"/> to true and hands off to <see cref="setSteamCameraRigInitialPosition(GameObject, GameObject)"/> or <see cref="setOculusCameraRigInitialPosition"/>.</para> </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            IEnumerator initializeCameraCoroutine() {
                float elapsedTime = Time.time - _startTime;
                GameObject steamVR = GameObject.Find("[CameraRig]");
                GameObject oculusRift = GameObject.Find("OVRCameraRig");

                GameObject steamVRCamera = GameObject.Find("Camera (head)");

                while (isCameraInitializationComplete == false && elapsedTime < cameraInitializationTimeout) {
                    elapsedTime = Time.time - _startTime;
                    if (isHMDConnected) {
                        EventManager.endInitializeCameraRig();
                    }
                    yield return new WaitForSeconds(0.1f);
                }

                if (logDiagnostics) {
                    Debug.Log("[ImmerseumSDK] Camera Initialization completed in: " + Time.time + "s");
                }
                EventManager.endInitializeCameraRig();

                if (isHMDConnected == false) {
                    if (CameraRig == null) {
                        if (Instance._packagesPresent == CameraRigPackages.Both || Instance._packagesPresent == CameraRigPackages.SteamVR) {
                            setCameraRig(steamVR.transform);
                        } else if (Instance._packagesPresent == CameraRigPackages.OculusRift) {
                            setCameraRig(oculusRift.transform);
                        } else {
                            throw new System.NullReferenceException("[ImmerseumSDK] No supported CameraRig found / indicated.");
                        }
                    } else if (CameraRig.name == "OVRPlayerController") {
                        oculusRift = CameraRig.FindChild("OVRCameraRig").gameObject;
                        setCameraRig(oculusRift.transform);
                    }
                } else {
                    if (isSteamVRHMDConnected) {
                        if (Instance._packagesPresent == CameraRigPackages.Both || Instance._packagesPresent == CameraRigPackages.SteamVR) {
                            setCameraRig(steamVR.transform);
                        } else {
                            Debug.LogError("[ImmerseumSDK] SteamVR HMD detected, but no corresponding CameraRig found / indicated.");
                            throw new System.NullReferenceException("No supported CameraRig found / indicated.");
                        }
                    } else if (isOculusHMDConnected) {
                        if (Instance._packagesPresent == CameraRigPackages.Both || Instance._packagesPresent == CameraRigPackages.OculusRift) {
                            setCameraRig(oculusRift.transform);
                        } else {
                            Debug.LogError("[ImmerseumSDK] Oculus HMD detected, but no corresponding CameraRig found / indicated.");
                            throw new System.NullReferenceException("No supported CameraRig found / indicated.");
                        }
                    } else {
                        Debug.LogError("[ImmerseumSDK] Neither SteamVR nor Oculus HMD Connected");
                    }
                }

                if (isUsingSteamRig) {
                    if (oculusRift != null) {
                        oculusRift.transform.root.gameObject.SetActive(false);
                    }
                } else {
                    if (steamVR != null) {
                        steamVR.SetActive(false);
                        GameObject.Find("[SteamVR]").SetActive(false);
                    }
                }

                if (isUsingSteamRig) {
                    if (logDiagnostics) {
                        Debug.Log("[ImmerseumSDK] Using Steam VR ([SteamVR]).");
                    }
                    setSteamCameraRigInitialPosition(steamVRCamera);
                } else {
                    if (logDiagnostics) {
                        Debug.Log("[ImmerseumSDK] Using Oculus Rift (OVR).");
                    }
                    setOculusCameraRigInitialPosition();
                }
            }

            void OnInitializeCameraRigEnd() {
                Instance._isCameraInitializationComplete = true;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Sets CameraRig initial position. </summary>
            ///
            /// <param name="steamVRCamera">            The <see cref="GameObject"/> of the SteamVR Camera. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void setSteamCameraRigInitialPosition(GameObject steamVRCamera) {
                GameObject steamVRCameraContainer = new GameObject();
                steamVRCameraContainer.name = "Camera Container";
                Vector3 cameraPosition = new Vector3(CameraRig.position.x, headHeight, CameraRig.position.z);
                Vector3 cameraRigPosition = Vector3.zero;

                if (isHMDConnected == false) {
                    cameraRigPosition = new Vector3(cameraPosition.x, 1.06f, cameraPosition.z);
                    CameraRig.position = cameraRigPosition;
                    steamVRCameraContainer.transform.SetParent(CameraRig);
                    steamVRCameraContainer.transform.localPosition = new Vector3(0f, _standingAdjustment, 0f);
                    steamVRCamera.transform.SetParent(steamVRCameraContainer.transform);
                    steamVRCamera.transform.localPosition = Vector3.zero;
                } else {
                    steamVRCameraContainer.transform.SetParent(CameraRig);
                    if (steamVRCamera != null) {
                        steamVRCamera.transform.SetParent(steamVRCameraContainer.transform);
                    } else {
                        if (HMDSimulator._displayedConnectionWarningOnce == false) {
                            Debug.LogWarning("[ImmerseumSDK] Unable to find a Camera (head) object, so falling back to Camera (eye). This happens in Unity 5.4 when SteamVR is running and the Vive is plugged in, or if you have changed the name of the Camera (head) gameobject.");
                            HMDSimulator._displayedConnectionWarningOnce = true;
                        }
                        GameObject.Find("Camera (eye)").transform.SetParent(steamVRCameraContainer.transform);
//                        throw new System.Exception("[ImmerseumSDK] Unable to find a Camera (head) object. This happens in Unity 5.4 when SteamVR is running and the Vive is plugged in, or if you have changed the name of the Camera (head) gameobject.");
                    }
                }
                EventManager.initializeControllers();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Sets CameraRig initial position. </summary>
            ///
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void setOculusCameraRigInitialPosition() {

                GameObject cameraContainer = new GameObject();
                cameraContainer.name = "Camera Container";
                Vector3 cameraPosition = new Vector3(0, headHeight, 0);
                float containerX = 0f;
                float containerY = getDefaultHeadHeight(heightTarget);
                float containerZ = 0f;

                Transform rootTransform;

                float rootY = 0f;

                if (CameraRig.parent == null) {
                    cameraContainer.transform.SetParent(null);
                    containerX = CameraRig.position.x;
                    containerZ = CameraRig.position.z;
                    containerY = 1.06f;
                    rootTransform = cameraContainer.transform;
                } else {
                    cameraContainer.transform.SetParent(CameraRig.parent);
                    rootTransform = cameraContainer.transform.root;
                    rootTransform.position = new Vector3(CameraRig.position.x, 1.06f, CameraRig.position.z);
                    rootY = CameraRig.parent.position.y;
                    containerY = rootY;
                }

                cameraContainer.transform.localPosition = new Vector3(containerX, containerY - rootY, containerZ);

                cameraPosition.y = getDefaultHeadHeight(heightTarget) - rootTransform.position.y;

                getCamera.SetParent(cameraContainer.transform);
                if (isHMDConnected == false) {
                    cameraPosition = new Vector3(cameraPosition.x, cameraPosition.y, cameraPosition.z);
                    getCamera.localPosition = cameraPosition;
                } else {
                    if (CameraRig.GetComponentInParent<OVRPlayerController>() == null) {
                        getCamera.localPosition = Vector3.zero;
                    }
                }

                EventManager.initializeControllers();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Sets the value of <see cref="CameraRig" />.</summary>
            /// <param name="cameraRigTransform">The <see cref="!:https://docs.unity3d.com/Manual/class-Transform.html">Transform</see> of the <see cref="CameraRig" />.</param>
            /// <seealso cref="CameraRig"></seealso>
            /// <example>
            /// The following examples set the <see cref="CameraRig" /> for SteamVR and Oculus Rift respectively:
            /// <code title="SteamVR" description="Finds the default SteamVR CameraRig and configures the HMDSimulator to use it." groupname="C#" lang="C#">
            /// GameObject cameraRigGameObject = GameObject.Find("[CameraRig]");
            /// Transform cameraRigTransform = cameraRigGameObject.transform;
            ///  
            /// HMDSimulator.setCameraRig(cameraRigTransform);</code><code title="Oculus Rift" description="Finds the default Oculus Rift Utilities for Unity camera rig and sets it as the HMDSimulator's CameraRig." groupname="C#" lang="C#">
            /// GameObject oculusRiftCameraRig = GameObject.Find("OVRCameraRig");
            /// Transform cameraRigTransform = oculusRiftCameraRig.transform;
            ///  
            /// HMDSimulator.setCameraRig(cameraRigTransform);</code></example>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static void setCameraRig(Transform cameraRigTransform) {
                Instance._CameraRig = cameraRigTransform;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Assigns a supplied $$prefab$$ as the prefab to use when simulating an indicated position-tracked controller.</summary>
            /// <param name="controller">Integer which indicates the controller. Accepts either <strong>0</strong> (left-hand) or <strong>1</strong> (right-hand).</param>
            /// <param name="prefab">(Optional) The prefab to apply to the indicated <paramref name="controller" />. If <strong>null</strong>, clears the indicated controller prefab reference.</param>
            /// <exception caption="" cref="IndexOutOfRangeException"> Thrown when an unrecognized value is passed for &lt;paramref name="controller" /&gt;. </exception>
            /// <example>
            ///   <code title="Setting a Prefab" description="Instantiates a prefab and assigns it to the simulated right-hand controller." lang="C#">
            /// GameObject prefabObject = (GameObject)Instantiate(Resources.load("prefabName"));
            ///  
            /// setControllerPrefab(1, prefabObject);</code>
            ///   <code title="Clearing Controller" description="Clears the simulated right-hand controller's prefab." lang="C#">
            /// setControllerPrefab(1, null);</code>
            /// </example>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static void setControllerPrefab(int controller, GameObject prefab = null) {
                if (controller == 0) {
                    Instance._leftControllerPrefab = prefab;
                } else if (controller == 1) {
                    Instance._rightControllerPrefab = prefab;
                } else {
                    Debug.LogError("[ImmerseumSDK]/HMDSimulator: Unrecognized controller index.");
                    throw new System.IndexOutOfRangeException();
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Clears the values of both <see cref="leftControllerPrefab" /> and <see cref="rightControllerPrefab" />.
            /// </summary>
            /// <example>
            ///   <code title="Example" description="Clears the value of both left-hand and right-hand simulated controllers." lang="C#">
            /// setControllerPrefab();</code>
            /// </example>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static void setControllerPrefab() {
                Instance._leftControllerPrefab = null;
                Instance._rightControllerPrefab = null;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Assigns a supplied $$prefab$$ as the prefab to use when simulating both position-tracked controllers.</summary>
            /// <param name="prefab">   The prefab to apply to both controllers. </param>
            /// <remarks>   Erin, 7/20/2016. </remarks>
            /// <example>
            ///   <code title="Setting a Prefab" description="Instantiates the prefab and applies it to both simulated controllers." lang="C#">
            /// GameObject prefabObject = (GameObject)Instantiate(Resources.load("prefabName"));
            ///  
            /// setControllerPrefab(prefabObject);</code>
            ///   <code title="Clearing the Prefab" description="Clears the prefab from both controllers." lang="C#">
            /// setControllerPrefab(null);</code>
            /// </example>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static void setControllerPrefab(GameObject prefab) {
                Instance._leftControllerPrefab = prefab;
                Instance._rightControllerPrefab = prefab;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Sets <paramref name="controllerObject" /> as the object for the simulated controller indicated by the value of <paramref name="controller" />. </summary>
            /// <param name="controller">Integer which indicates the controller. Accepts either <strong>0</strong> (left-hand) or <strong>1</strong> (right-hand).</param>
            /// <param name="controllerObject"> The object to be used as the indicated controller. </param>
            /// <exception caption="" cref="IndexOutOfRangeException"> Thrown when &lt;paramref name="controller" /&gt; is not 0 (left-hand) or 1 (right-hand). </exception>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static void setControllers(int controller, GameObject controllerObject) {
                Vector3 originalLocalScale = controllerObject.transform.localScale;
                GameObject cameraContainer = GameObject.Find("Camera Container");
                Transform parentTransform = cameraContainer.transform.parent;
                if (parentTransform == null) {
                    parentTransform = cameraContainer.transform;
                }
                if (controller == 0) {
                    Instance._leftController = controllerObject.transform;
                    leftController.SetParent(parentTransform);
                    leftController.localScale = originalLocalScale;
                    leftController.localPosition = Vector3.zero;
                } else if (controller == 1) {
                    Instance._rightController = controllerObject.transform;
                    rightController.SetParent(parentTransform);
                    rightController.localScale = originalLocalScale;
                    rightController.localPosition = Vector3.zero;
                } else {
                    Debug.LogError("[ImmerseumSDK]/HMDSimulator: Unrecognized controller index.");
                    throw new System.IndexOutOfRangeException();
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Determines the <see cref="hmdFamily"/> connected to the user's system. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private void determineHMDFamily() {
                if (logDiagnostics) {
                    Debug.Log("Detemrining HMD Family");
                }
                if (isSteamVRHMDConnected == true) {
                    if (logDiagnostics) {
                        Debug.Log("SteamVR is connected");
                    }
                    _hmdFamily = hmdFamilies.SteamVR;
                } else if (isOculusHMDConnected == true) {
                    if (logDiagnostics) {
                        Debug.Log("Oculus Rift is connected");
                    }
                    _hmdFamily = hmdFamilies.Oculus;
                } else if (isPlaystationVRHMDConnected == true) {
                    if (logDiagnostics) {
                        Debug.Log("PlayStationVR is connected");
                    }
                    _hmdFamily = hmdFamilies.PlayStationVR;
                } else {
                    _hmdFamily = hmdFamilies.None;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Determines the value of <see cref="isTrackingHands"/>, <see cref="isRemoteConnected"/>, and <see cref="isGamepadConnected"/>. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void determineHandInputMode() {
                if (isSteamVRHMDConnected == true) {
                    uint leftControllerIndex = SteamVR.instance.hmd.GetTrackedDeviceIndexForControllerRole(Valve.VR.ETrackedControllerRole.LeftHand);
                    uint rightControllerIndex = SteamVR.instance.hmd.GetTrackedDeviceIndexForControllerRole(Valve.VR.ETrackedControllerRole.RightHand);

                    if (SteamVR.instance.hmd.IsTrackedDeviceConnected(leftControllerIndex) || SteamVR.instance.hmd.IsTrackedDeviceConnected(rightControllerIndex)) {
                        _isTrackingHands = true;
                        if (logDiagnostics) {
                            Debug.Log("[ImmerseumSDK] SteamVR: Hand-tracking controllers active.");
                        }
                    } else {
                        _isTrackingHands = false;
                        if (logDiagnostics) {
                            Debug.Log("[ImmerseumSDK] SteamVR: No hand-tracking controllers active.");
                        }
                    }
                } else if (isOculusHMDConnected == true) {
                    if (OVRInput.GetControllerPositionTracked(OVRInput.Controller.LTouch) || OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch) || OVRInput.GetControllerPositionTracked(OVRInput.Controller.Touch)) {
                        _isTrackingHands = true;
                        if (logDiagnostics) {
                            Debug.Log("[ImmerseumSDK] Oculus: Hand-tracking controllers active.");
                        }
                    } else {
                        _isTrackingHands = false;
                        if (logDiagnostics) {
                            Debug.Log("[ImmerseumSDK] Oculus: No hand-tracking controllers active.");
                        }
                    }
                } else if (isPlaystationVRHMDConnected == true) {
                    // TODO: IM-35: Implement PlayStation VR support.
                    _isTrackingHands = false;
                } else {
                    _isTrackingHands = false;
                    if (logDiagnostics) {
                        Debug.LogWarning("[ImmerseumSDK]: No HMD connected, so no hand-tracking controllers active.");
                    }
                }

                if (isOculusHMDConnected == true) {
                    if (OVRInput.GetControllerPositionTracked(OVRInput.Controller.Remote)) {
                        _isRemoteConnected = true;
                        if (logDiagnostics) {
                            Debug.Log("[ImmerseumSDK] Oculus: Remote detected.");
                        }
                    }

                    if (OVRInput.GetControllerPositionTracked(OVRInput.Controller.Gamepad)) {
                        _isGamepadConnected = true;
                        if (logDiagnostics) {
                            Debug.Log("[ImmerseumSDK] Oculus: No gamepad detected.");
                        }
                    } else {
                        _isGamepadConnected = false;
                        if (logDiagnostics) {
                            Debug.Log("[ImmerseumSDK] Oculus: No gamepad detected.");
                        }
                    }
                } else {
                    string[] joystickNames = Input.GetJoystickNames();
                    if (joystickNames.Length > 0 && joystickNames[0] != "") {
                        _isGamepadConnected = true;
                        if (logDiagnostics) {
                            Debug.Log("[ImmerseumSDK] Gamepad detected: " + joystickNames[0]);
                        }
                    } else {
                        _isGamepadConnected = false;
                        if (logDiagnostics) {
                            Debug.Log("[ImmerseumSDK] No gamepad detected.");
                        }
                    }
                }
            }

            protected virtual void Update() {

            }

            /// <summary>Indicates whether the $$Camera Rig$$ is related to the <see cref="!:https://docs.unity3d.com/Manual/class-Transform.html">Transform</see> passed
            /// to this method.</summary>
            /// <param name="otherTransform">The <see cref="!:https://docs.unity3d.com/Manual/class-Transform.html">Transform</see> whose tree in the scene hierarchy is checked for the
            /// $$Camera Rig$$.</param>
            /// <returns>
            ///   <strong>true</strong> if the $$Camera Rig$$ is related to the <strong>otherTransform</strong>, otherwise <strong>false</strong></returns>
            public static bool isCameraRigInTransform(Transform otherTransform) {
                Transform rootTransform;
                if (otherTransform.root != null) {
                    rootTransform = otherTransform.root;
                } else {
                    rootTransform = otherTransform;
                }

                if (rootTransform.FindChild(CameraRig.name) != null) {
                    return true;
                }
                return false;
            }

            /// <summary>Indicates whether the $$Camera Rig$$ or its relatives in the hierarchy have a <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> attached.</summary>
            /// <value>
            ///   <strong>true</strong> if either the $$Camera Rig$$ or any of its relatives in the scene hierarchy have a <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> attached, otherwise <strong>false</strong>.</value>
            public static bool hasCollider {
                get {
                    Transform rootTransform = CameraRig.root;
                    Collider[] rootColliderArray = (Collider[])rootTransform.GetComponents<Collider>();
                    if (rootColliderArray.Length > 0) {
                        return true;
                    }
                    Collider[] childColliderArray = (Collider[])rootTransform.GetComponentsInChildren<Collider>();
                    if (childColliderArray.Length > 0) {
                        return true;
                    }
                    return false;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Returns the default Head Height for given <see cref="HeightTargets" /> value.</summary>
            /// <param name="forPosition">  The <see cref="HeightTargets" /> value for which the head height value should be returned. </param>
            /// <returns>   A floating point value that represents the height in meters (or unity units) of the simulated user's head. </returns>
            /// <seealso cref="T:Immerseum.VRSimulator.HeightTargets"></seealso>
            /// <example>
            /// The following example retrieves the default head height for the <strong>Standing</strong><see cref="HeightTargets" />.
            /// <code title="Example" description="" lang="C#">
            /// float defaultHeadHeight = HMDSimulator.getDefaultHeadHeight(HeightTargets.Standing);
            /// // Returns: 1.65f;</code></example>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static float getDefaultHeadHeight(HeightTargets forPosition) {
                if (isOculusHMDConnected) {
                    if (CameraRig.root.GetComponent<OVRPlayerController>() != null) {
                        if (CameraRig.root.GetComponent<OVRPlayerController>().useProfileData) {
                            Debug.LogWarning("Eye Height: " + OVRManager.profile.eyeHeight);
                            return OVRManager.profile.eyeHeight;
                        }
                    }
                }
                switch (forPosition) {
                    case HeightTargets.Custom:
                        return Instance._customHeadHeight;
                    case HeightTargets.Seated:
                        return 1.06f;
                    case HeightTargets.Standing:
                        return 1.755f;
                    default:
                        goto case HeightTargets.Standing;
                }
            }
        }
    }
}

