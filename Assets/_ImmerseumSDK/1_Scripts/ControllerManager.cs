using UnityEngine;
using System.Collections;
using System;

namespace Immerseum {
    namespace VRSimulator {

        /// <summary>Standard predefined positions into which simulated controllers can be placed.</summary>
        public enum ControllerPositions { /// <summary>
                                          ///   <span style="FONT-SIZE: 13px; FONT-FAMILY: &quot;Segoe UI&quot;, Verdana, Arial; WHITE-SPACE: normal; WORD-SPACING: 0px; TEXT-TRANSFORM: none; FLOAT: none; FONT-WEIGHT: normal; COLOR: rgb(0,0,0); FONT-STYLE: normal; ORPHANS: 2; WIDOWS: 2; DISPLAY: inline !important; LETTER-SPACING: normal; LINE-HEIGHT: 18px; TEXT-INDENT: 0px; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px">
                                          /// Simulates "dangling at your waist". They start at more-or-less waist height (lower if seated) and slightly forward of your player avatar's implied body. This
                                          /// slight forward simulation is because players rarely have controllers at rest when in a VR experience, and this lets you more accurately see where your
                                          /// controllers are when in simulation mode.</span>
                                          /// </summary>
            Origin, /// <summary>
                    ///   <span style="FONT-SIZE: 13px; FONT-FAMILY: &quot;Segoe UI&quot;, Verdana, Arial; WHITE-SPACE: normal; WORD-SPACING: 0px; TEXT-TRANSFORM: none; FLOAT: none; FONT-WEIGHT: normal; COLOR: rgb(0,0,0); FONT-STYLE: normal; ORPHANS: 2; WIDOWS: 2; DISPLAY: inline !important; LETTER-SPACING: normal; LINE-HEIGHT: 18px; TEXT-INDENT: 0px; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px">
                    /// Simulates "resting on your desk/keyboard". They are at about torso-height and more substantially forward.</span>
                    /// </summary>
            Forward, /// <summary>
                     ///   <span style="FONT-SIZE: 13px; FONT-FAMILY: &quot;Segoe UI&quot;, Verdana, Arial; WHITE-SPACE: normal; WORD-SPACING: 0px; TEXT-TRANSFORM: none; FLOAT: none; FONT-WEIGHT: normal; COLOR: rgb(0,0,0); FONT-STYLE: normal; ORPHANS: 2; WIDOWS: 2; DISPLAY: inline !important; LETTER-SPACING: normal; LINE-HEIGHT: 18px; TEXT-INDENT: 0px; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px">
                     /// Simulates hands extended out from the shoulder.</span>
                     /// </summary>
            Reaching, /// <summary>
                      ///   <span style="FONT-SIZE: 13px; FONT-FAMILY: &quot;Segoe UI&quot;, Verdana, Arial; WHITE-SPACE: normal; WORD-SPACING: 0px; TEXT-TRANSFORM: none; FLOAT: none; FONT-WEIGHT: normal; COLOR: rgb(0,0,0); FONT-STYLE: normal; ORPHANS: 2; WIDOWS: 2; DISPLAY: inline !important; LETTER-SPACING: normal; LINE-HEIGHT: 18px; TEXT-INDENT: 0px; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px">
                      /// Simulates a "puglist's pose" with the hands near eye-level, with one (the primary) hand higher and more forward than the secondary hand.</span>
                      /// </summary>
            Boxer, /// <summary>
                   ///   <span style="FONT-SIZE: 13px; FONT-FAMILY: &quot;Segoe UI&quot;, Verdana, Arial; WHITE-SPACE: normal; WORD-SPACING: 0px; TEXT-TRANSFORM: none; FLOAT: none; FONT-WEIGHT: normal; COLOR: rgb(0,0,0); FONT-STYLE: normal; ORPHANS: 2; WIDOWS: 2; DISPLAY: inline !important; LETTER-SPACING: normal; LINE-HEIGHT: 18px; TEXT-INDENT: 0px; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px">
                   /// Uses whatever position automatically derives from the simulated controller's primitive (useful if you're using a custom prefab to simulate a
                   /// controller).</span>
                   /// </summary>
            Prefab
        }
        /// <summary>Indicates an axis direction in three-dimensional worldspace.</summary>
        public enum Direction { XAxis, YAxis, ZAxis }
        /// <summary>Indicates one of the ways hands can be prioritized.</summary>
        public enum Handedness { /// <summary>Left hand is prioritized.</summary>
            Left, /// <summary>Right hand is prioritized.</summary>
            Right, /// <summary>Both hands given equal priority.</summary>
            Ambidextrous
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>$$Singleton$$ that is used to manage position-tracked controllers that are simulated by the VRSimulator.</summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public class ControllerManager : MonoBehaviour {
            /// <summary>The Singleton Instance of the ControllerManager class.</summary>
            public static ControllerManager Instance { get; private set; }

            protected bool _areControllersSimulated = false;
            /// <summary>Indicates whether both left-hand and right-hand controllers have been simulated.</summary>
            /// <value>
            ///   <strong>true</strong> if both have been simulated, otherwise <strong>false</strong>.</value>
            public static bool areControllersSimulated {
                get {
                    return Instance._areControllersSimulated;
                }
            }

            [SerializeField]
            protected ControllerPositions _startingPosition = ControllerPositions.Forward;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets the position in which simulated controllers should start. </summary>
            /// <value>
            ///   <para>Returns one of the following values:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>
            ///         <see cref="ControllerPositions.Origin" />
            ///       </strong> sets the simulated controllers at waist-level and only slightly forward of the camera to simulate a
            ///     naturally dangling resting pose.</item>
            ///     <item>
            ///       <strong>
            ///         <see cref="ControllerPositions.Forward" />
            ///       </strong> sets the simulated controllers at slightly above waist-level and more forward so as to simulate a
            ///     keyboard-resting pose.</item>
            ///     <item>
            ///       <strong>
            ///         <see cref="ControllerPositions.Reaching" />
            ///       </strong> sets the simulated controllers at near-shoulder height and substantially forward, to simulate arms
            ///     reaching forward (Superman-pose).</item>
            ///     <item>
            ///       <strong>
            ///         <see cref="ControllerPositions.Boxer" />
            ///       </strong> sets the simulated controllers at different heights and different depths so as to simulate a classic
            ///     pugilist's pose.</item>
            ///     <item>
            ///       <strong>
            ///         <see cref="ControllerPositions.Prefab"/>
            ///       </strong> sets the simulated controllers at whatever default positions their primitives or prefabs
            ///     have.</item>
            ///   </list>
            /// </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static ControllerPositions startingPosition {
                get {
                    return Instance._startingPosition;
                }
            }

            protected static Vector3[] _defaultOriginOffset = new Vector3[2];
            protected static Quaternion[] _defaultOriginRotation = new Quaternion[2];

            protected static Vector3[] _defaultForwardOffset = new Vector3[2];
            protected static Quaternion[] _defaultForwardRotation = new Quaternion[2];

            protected static Vector3[] _defaultReachingOffset = new Vector3[2];
            protected static Quaternion[] _defaultReachingRotation = new Quaternion[2];

            protected static Vector3[] _defaultBoxerOffset = new Vector3[2];
            protected static Quaternion[] _defaultBoxerRotation = new Quaternion[2];

            protected static Quaternion QUATERNION_ZERO = Quaternion.Euler(Vector3.zero);

            protected static Vector3[] _originalPrefabOffset = new Vector3[2];
            protected static Quaternion[] _originalPrefabRotation = new Quaternion[2];

            /// <summary>   The primary/dominant hand. </summary>
            [SerializeField]
            protected Handedness _primaryHand = Handedness.Left;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary> Returns the primary/dominant hand.  </summary>
            /// <value>
            ///   <para>Returns one of the following values:</para>
            ///   <para></para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>
            ///         <see cref="Handedness.Left" />
            ///       </strong> (default)</item>
            ///     <item>
            ///       <see cref="Handedness.Right" />
            ///     </item>
            ///     <item>
            ///       <see cref="Handedness.Ambidextrous" />
            ///     </item>
            ///   </list>
            ///   <para></para>
            /// </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static Handedness primaryHand {
                get {
                    return Instance._primaryHand;
                }
            }

            protected float _seatedAdjustment = -0.3f;
            /// <summary>   The height by which simulated controllers are adjusted, expressed as m (or unity units). </summary>
            public static float seatedAdjustment {
                get {
                    if (HMDSimulator.heightTarget == HeightTargets.Standing) {
                        return 0f;
                    }
                    return Instance._seatedAdjustment;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Returns <strong><see cref="HMDSimulator.primitiveScaling" /></strong>.</summary>
            /// <value> The value of <see cref="HMDSimulator.primitiveScaling" />. </value>
            /// <seealso cref="HMDSimulator.primitiveScaling">HMDSimulator.primitiveScaling</seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            protected static float primitiveScaling {
                get {
                    return HMDSimulator.primitiveScaling;
                }
            }

            /// <summary>   Default speed at which simulated controllers move (measured in m / s). </summary>
            [SerializeField]
            [Range(0.1f, 10f)]
            protected float _movementSpeed = 1f;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Returns the speed at which simulated controllers naturally move. </summary>
            /// <value> The movement speed expressed in meters / second. </value>
            /// <seealso cref="M:Immerseum.VRSimulator.ControllerManager.moveController">moveController()</seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static float movementSpeed {
                get {
                    return Instance._movementSpeed;
                }
            }

            #region Collider Configuration
            [SerializeField]
            protected bool _applyCollider = false;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            ///   <para>Indicates whether a <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> should be applied to and enabled on simulated controllers.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Because <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> components are
            ///     automatically applied to primitives, if they do not apply to a particular controller (or if <see cref="applyCollider" /> is <strong>false</strong>) they will be
            ///     disabled where appropriate.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>
            ///   <strong>true</strong> if <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> should be applied to simulated
            /// controllers, otherwise <strong>false</strong>.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool applyCollider {
                get { return Instance._applyCollider; }
            }

            [SerializeField]
            protected Handedness _applyColliderHand = Handedness.Ambidextrous;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            ///   <para>If <see cref="applyCollider" /> is true, then indicates which simulated controller(s) should receive an enabled
            /// <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Because <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> components are automatically applied to primitives, if they do not apply to a particular controller (or if <see cref="applyCollider" /> is
            ///     <strong>false</strong>) they will be disabled where appropriate.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>
            ///   <para>Returns one of the following values:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>
            ///         <see cref="Handedness.Ambidextrous" />
            ///       </strong> applies a <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see>
            ///     to both simulated controllers (<see cref="HMDSimulator.leftController" /> and <see cref="HMDSimulator.rightController" />).</item>
            ///     <item>
            ///       <strong>
            ///         <see cref="Handedness.Left" />
            ///       </strong> applies a <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> to the
            ///     <see cref="HMDSimulator.leftController" />.</item>
            ///     <item>
            ///       <strong>
            ///         <see cref="Handedness.Right" />
            ///       </strong> applies a <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> to the
            ///     <see cref="HMDSimulator.rightController" />.</item>
            ///   </list>
            /// </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static Handedness applyColliderHand {
                get { return Instance._applyColliderHand; }
            }

            /// <summary>   true if the simulated controller's collider is a Trigger. </summary>
            [SerializeField]
            protected bool _isTrigger = false;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            ///   <para>Indicates whether the <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> applied to simulated controller(s)
            /// should be flagged as a $$trigger$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Only applies if <see cref="applyCollider" /> is <strong>true</strong>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>
            ///   <strong>true</strong> if the simulated controller's <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> is
            /// a $$trigger$$, <strong>false</strong> if not.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool isTrigger {
                get {
                    return Instance._isTrigger;
                }
            }

            /// <summary>   The collider physic material. </summary>
            [SerializeField]
            protected PhysicMaterial _colliderPhysicMaterial;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            ///   <para>Indicates the <see cref="!:https://docs.unity3d.com/Manual/class-PhysicMaterial.html">Physic Material</see> applied to the <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> component(s) applied to the simulated controller(s).</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <para>Only applies if <see cref="applyCollider" /> is <strong>true</strong>.</para>
            ///     </innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>The <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">collider</see><see cref="!:https://docs.unity3d.com/Manual/class-PhysicMaterial.html">PhysicMaterial</see>.</value>
            /// <seealso cref="P:Immerseum.VRSimulator.ControllerManager.applyCollider">ControllerManager.applyCollider</seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static PhysicMaterial colliderPhysicMaterial {
                get {
                    return Instance._colliderPhysicMaterial;
                }
            }

            /// <summary>   The collider center. </summary>
            [SerializeField]
            protected Vector3 _colliderCenter = Vector3.zero;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Returns the center of the <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> applied to the simulated
            /// controller(s).</summary>
            /// <value>The <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> position of the <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider's</see> center.</value>
            /// <seealso cref="P:Immerseum.VRSimulator.ControllerManager.applyCollider">applyCollider</seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static Vector3 colliderCenter {
                get {
                    return Instance._colliderCenter;
                }
            }

            /// <summary>   The radius of the simulated controller(s)' <see cref="Collider"/>. </summary>
            [SerializeField]
            protected float _colliderRadius = 0.5f;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            ///   <para>Returns the radius of the simulated controller(s)' <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see>.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <para>Only applies if:</para>
            ///       <list type="bullet">
            ///         <item>
            ///           <see cref="applyCollider" /> is <strong>true</strong>, and;</item>
            ///         <item>
            ///           <see cref="HMDSimulator.controllerPrimitive" /> is not <strong>Cube</strong> and;</item>
            ///         <item>
            ///           <see cref="HMDSimulator.controllerPrimitive" /> is not <strong>Custom</strong>.</item>
            ///       </list>
            ///     </innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A floating point value indicating the radius of the  <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> expressed in meters/unity units.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static float colliderRadius {
                get {
                    return Instance._colliderRadius;
                }
            }

            /// <summary>   Size of the collider. </summary>
            [SerializeField]
            protected Vector3 _colliderSize = Vector3.one;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Returns the scale of the <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> applied to the simulated controller(s).</summary>
            /// <value>
            ///   <para>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> value that indicates the size of the <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see>, where <span class="Code">(1, 1, 1)</span> is equal to the simulated
            /// controller's size.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <para>Only applies if:</para>
            ///       <list type="bullet">
            ///         <item>
            ///           <see cref="applyCollider" /> is <strong>true</strong>, and;</item>
            ///         <item>
            ///           <see cref="HMDSimulator.controllerPrimitive" /> is <strong>Cube</strong>.</item>
            ///       </list>
            ///     </innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static Vector3 colliderSize {
                get {
                    return Instance._colliderSize;
                }
            }

            /// <summary>   Height of the collider applied to the simulated controller(s). </summary>
            [SerializeField]
            protected float _colliderHeight = 2;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            ///   <para>Returns the height of the <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> applied to the simulated
            /// controller(s).</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">
            ///       <para>Only applies if:</para>
            ///       <list type="bullet">
            ///         <item>
            ///           <see cref="applyCollider" /> is <strong>true</strong> and</item>
            ///         <item>
            ///           <see cref="HMDSimulator.controllerPrimitive" /> is <strong>Cylinder</strong> or <strong>Capsule</strong>.</item>
            ///       </list>
            ///     </innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>A floating point value that expresses the height of the <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> in
            /// meters (or unity units).</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static float colliderHeight {
                get { return Instance._colliderHeight; }
            }

            /// <summary>   The collider direction applied to the simulated controller(s). </summary>
            [SerializeField]
            protected Direction _colliderDirection = Direction.YAxis;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            ///   <para>Returns the direction of the <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> applied to the simulated
            /// controller(s).</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Only applies if <see cref="applyCollider" /> is <strong>true</strong>
            ///     and <see cref="HMDSimulator.controllerPrimitive" /> is <strong>Capsule</strong> or <strong>Cylinder</strong>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>
            ///   <para>The direction of the <see cref="!:https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> expressed as a <see cref="Direction" />.
            /// Accepts one of the following values:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>
            ///         <see cref="Direction.XAxis" />
            ///       </strong>
            ///     </item>
            ///     <item>
            ///       <strong>
            ///         <see cref="Direction.YAxis" />
            ///       </strong> (default)</item>
            ///     <item>
            ///       <strong>
            ///         <see cref="Direction.ZAxis" />
            ///       </strong>
            ///     </item>
            ///   </list>
            /// </value>
            /// <seealso cref="P:Immerseum.VRSimulator.ControllerManager.applyCollider">applyCollider</seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static Direction colliderDirection {
                get { return Instance._colliderDirection; }
            }
            #endregion

            #region Rigidbody Configuration
            /// <summary>   true to apply rigidbody to simulated controllers. </summary>
            [SerializeField]
            protected bool _applyRigidbody = false;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Indicates whether a <see cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</see> should be applied to the simulated
            /// controller(s).</summary>
            /// <value>
            ///   <strong>true</strong> if a <see cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</see> should be applied to the
            /// simulated controller(s), <strong>false</strong> if not.</value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool applyRigidbody {
                get { return Instance._applyRigidbody; }
            }

            /// <summary>   The simulated controller(s) to which a <see cref="Rigidbody"/> should be applied. </summary>
            [SerializeField]
            protected Handedness _applyRigidbodyHand = Handedness.Ambidextrous;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>If <see cref="applyRigidbody" /> is <strong>true</strong>, then indicates which simulated controller(s) should receive a <see cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</see>.</summary>
            /// <value>
            ///   <para>Returns one of the following values:</para>
            ///   <list type="bullet">
            ///     <item>
            ///       <strong>
            ///         <see cref="Handedness.Ambidextrous" />
            ///       </strong> applies a <see cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</see> to both simulated controllers (<see cref="HMDSimulator.leftController" /> and <see cref="HMDSimulator.rightController" />).</item>
            ///     <item>
            ///       <strong>
            ///         <see cref="Handedness.Left" />
            ///       </strong> applies a <see cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</see> to
            ///     the <see cref="HMDSimulator.leftController" />.</item>
            ///     <item>
            ///       <strong>
            ///         <see cref="Handedness.Right" />
            ///       </strong> applies a <see cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</see> to
            ///     the <see cref="HMDSimulator.rightController" />.</item>
            ///   </list>
            /// </value>
            /// <seealso cref="P:Immerseum.VRSimulator.ControllerManager.applyRigidbody">applyRigidbody</seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static Handedness applyRigidbodyHand {
                get { return Instance._applyRigidbodyHand; }
            }

            /// <summary>   true the simulated controller(s) should be kinematic. </summary>
            [SerializeField]
            protected bool _isKinematic = false;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            ///   <para>Indicates whether the simulated controller(s)' <see cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</see>
            /// component(s) should be $$kinematic$$.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Only applies if <see cref="applyRigidbody" /> is <strong>true</strong>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </summary>
            /// <value>
            ///   <strong>true</strong> if the <see cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</see> applied should
            /// be $$kinematic$$, <strong>false</strong> if not.</value>
            /// <seealso cref="Rigidbody.isKinematic"></seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool isKinematic {
                get { return Instance._isKinematic; }
            }

            /// <summary>   The mass applied to the applicable <see cref="Rigidbody"/>. </summary>
            [SerializeField]
            protected float _rbMass = 1f;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Returns the mass of the <see cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</see> applied to the indicated
            /// controller(s).</summary>
            /// <remarks>   Only applies if <see cref="applyRigidbody" /> is set to true.</remarks>
            /// <value> A mass expressed in kilograms (kg) as a floating point number. </value>
            /// <seealso cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody-mass.html">UNITY3D: Rigidbody.mass</seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static float rbMass {
                get { return Instance._rbMass; }
            }

            /// <summary>   The drag applied to the <see cref="Rigidbody"/> applied to simulated controller(s). </summary>
            [SerializeField]
            protected float _rbDrag = 0f;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Returns the Drag Factor applied to the simulated controller(s)' <see cref="Rigidbody" /> component(s). </summary>
            /// <value>
            ///   <para>The <see cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</see> component(s)' Drag Factor.</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Only applies if <see cref="applyRigidbody" /> is <strong>true</strong>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </value>
            /// <seealso cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody-drag.html">UNITY3D: Rigidbody.drag</seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static float rbDrag {
                get { return Instance._rbDrag; }
            }

            [SerializeField]
            protected float _rbAngularDrag = 0.05f;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Returns the Angular Drag Factor applied to the <see cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</see>
            /// component(s) applied to the simulated controller(s).</summary>
            /// <value>
            ///   <para>The Angular Drag applied to the <see cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</see> component(s) attached
            /// to the simulated controller(s).</para>
            ///   <innovasys:widget type="Note Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
            ///     <innovasys:widgetproperty layout="block" name="Content">Only applies if <see cref="applyRigidbody" /> is <strong>true</strong>.</innovasys:widgetproperty>
            ///   </innovasys:widget>
            /// </value>
            /// <seealso cref="Rigidbody.angularDrag"></seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static float rbAngularDrag {
                get { return Instance._rbAngularDrag; }
            }

            /// <summary>   true to apply Use Gravity to the <see cref="Rigidbody"/> attached to the simulated controller(s). </summary>
            [SerializeField]
            protected bool _rbUseGravity = true;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Returns the value of Use Gravity applied to the <see cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</see>
            /// component(s) attached to the simulated controller(s).</summary>
            /// <remarks>   Only applies if <see cref="applyRigidbody" /> is set to true.</remarks>
            /// <value>
            ///   <strong>true</strong> if the <see cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</see> component should be subject
            /// to gravity, <strong>false</strong> if not.</value>
            /// <seealso cref="!:https://docs.unity3d.com/ScriptReference/Rigidbody-useGravity.html">UNITY3D:Rigidbody.useGravity</seealso>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static bool rbUseGravity {
                get { return Instance._rbUseGravity; }
            }
            #endregion

            void OnEnable() {
                EventManager.OnInitializeControllers += OnInitializeControllers;
                EventManager.OnInstantiateControllerEnd += OnInstantiateControllerEnd;
                EventManager.OnInstantiateController += OnInstantiateController;
                EventManager.OnApplyCollider += OnApplyCollider;
                EventManager.OnApplyColliderEnd += OnApplyColliderEnd;
                EventManager.OnApplyRigidbody += OnApplyRigidbody;
                EventManager.OnPositionController += OnPositionController;
                EventManager.OnMoveControllerStart += OnMoveControllerStart;
                EventManager.OnMoveControllerStart_Vector += OnMoveControllerStart_Vector;
            }

            void OnDisable() {
                EventManager.OnInitializeControllers -= OnInitializeControllers;
                EventManager.OnInstantiateControllerEnd -= OnInstantiateControllerEnd;
                EventManager.OnInstantiateController -= OnInstantiateController;
                EventManager.OnApplyCollider -= OnApplyCollider;
                EventManager.OnApplyColliderEnd -= OnApplyColliderEnd;
                EventManager.OnApplyRigidbody -= OnApplyRigidbody;
                EventManager.OnPositionController -= OnPositionController;
                EventManager.OnMoveControllerStart -= OnMoveControllerStart;
                EventManager.OnMoveControllerStart_Vector -= OnMoveControllerStart_Vector;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Defines the initial positions of the simulated controller(s). </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void setControllerPositions() {
                float originHeightOffsetLeft;
                float originHeightOffsetRight;
                float forwardHeightOffsetLeft;
                float forwardHeightOffsetRight;
                float boxerHeightOffsetLeft;
                float boxerHeightOffsetRight;
                float reachingHeightOffsetLeft;
                float reachingHeightOffsetRight;

                float xCameraOffset = 0f;
                float yCameraOffset = 0f;
                float zCameraOffset = 0f;

                if (HMDSimulator.isHMDConnected) {
                    xCameraOffset += HMDSimulator.getCamera.position.x;
                    if (HMDSimulator.isUsingSteamRig || HMDSimulator.isUsingProfileData) {
                        yCameraOffset += 0f;
                    } else {
                        yCameraOffset += -1.755f;
                        switch (startingPosition) {
                            case ControllerPositions.Origin:
                                yCameraOffset *= 0.68f;
                                break;
                            case ControllerPositions.Forward:
                                yCameraOffset *= 0.68f;
                                break;
                            case ControllerPositions.Boxer:
                                yCameraOffset *= 0.68f;
                                break;
                            case ControllerPositions.Reaching:
                                yCameraOffset *= 0.68f;
                                break;
                            case ControllerPositions.Prefab:
                                break;
                        }
                    }
                    zCameraOffset += HMDSimulator.getCamera.position.z;
                } else {
                    xCameraOffset += 0f;
                    yCameraOffset += -HMDSimulator.getCameraContainer.root.position.y;
                    zCameraOffset += 0f;
                }

                originHeightOffsetLeft = 0.88f + seatedAdjustment;
                originHeightOffsetRight = originHeightOffsetLeft;

                forwardHeightOffsetLeft = 1.27f + seatedAdjustment;
                forwardHeightOffsetRight = forwardHeightOffsetLeft;

                reachingHeightOffsetLeft = 1.47f + seatedAdjustment;
                reachingHeightOffsetRight = reachingHeightOffsetLeft;

                if (primaryHand == Handedness.Left) {
                    boxerHeightOffsetLeft = 1.49f + seatedAdjustment;
                    boxerHeightOffsetRight = 1.38f + seatedAdjustment;
                } else {
                    boxerHeightOffsetLeft = 1.38f + seatedAdjustment;
                    boxerHeightOffsetRight = 1.49f + seatedAdjustment;
                }

                _defaultOriginOffset[0] = new Vector3(-0.24f + xCameraOffset, originHeightOffsetLeft + yCameraOffset, 0.25f + zCameraOffset);
                _defaultOriginOffset[1] = new Vector3(0.24f + xCameraOffset, originHeightOffsetRight + yCameraOffset, 0.25f + zCameraOffset);

                _defaultForwardOffset[0] = new Vector3(-0.24f + xCameraOffset, forwardHeightOffsetLeft + yCameraOffset, 0.43f + zCameraOffset);
                _defaultForwardOffset[1] = new Vector3(0.24f + xCameraOffset, forwardHeightOffsetRight + yCameraOffset, 0.43f + zCameraOffset);

                if (primaryHand == Handedness.Right) {
                    _defaultBoxerOffset[0] = new Vector3(-0.1f + xCameraOffset, boxerHeightOffsetLeft + yCameraOffset, 0.25f + zCameraOffset);
                    _defaultBoxerOffset[1] = new Vector3(0.05f + xCameraOffset, boxerHeightOffsetRight + yCameraOffset, 0.34f + zCameraOffset);
                } else {
                    _defaultBoxerOffset[0] = new Vector3(-0.05f + xCameraOffset, boxerHeightOffsetLeft + yCameraOffset, 0.34f + zCameraOffset);
                    _defaultBoxerOffset[1] = new Vector3(0.1f + xCameraOffset, boxerHeightOffsetRight + yCameraOffset, 0.25f + zCameraOffset);
                }

                _defaultReachingOffset[0] = new Vector3(-0.15f + xCameraOffset, reachingHeightOffsetLeft + yCameraOffset, 0.65f + zCameraOffset);
                _defaultReachingOffset[1] = new Vector3(0.15f + xCameraOffset, reachingHeightOffsetRight + yCameraOffset, 0.65f + zCameraOffset);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Defines the initial rotations of the simulated controller(s). </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void setControllerRotations() {
                _defaultOriginRotation[0] = _defaultOriginRotation[1] = QUATERNION_ZERO;
                _defaultForwardRotation[0] = _defaultForwardRotation[1] = QUATERNION_ZERO;
                _defaultBoxerRotation[0] = _defaultBoxerRotation[1] = QUATERNION_ZERO;
                _defaultReachingRotation[0] = _defaultReachingRotation[1] = QUATERNION_ZERO;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Initializes the simulated controllers if <see cref="HMDSimulator.simulateControllers"/> is set to true. Hands off control to the <see cref="initializeControllersCoroutine"/>.  </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void OnInitializeControllers() {
                if (HMDSimulator.simulateControllers) {
                    StartCoroutine(initializeControllersCoroutine());
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Waits for camera initialization to complete, and then instantiates and initializes the simulated controllers as configured in <see cref="HMDSimulator"/> and <see cref="ControllerManager"/>. </summary>
            /// <returns>   Yields in a loop for (0.1f) seconds until camera initializationis complete (i.e. while <see cref="HMDSimulator.isCameraInitializationComplete"/> is false).</returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            IEnumerator initializeControllersCoroutine() {
                while (HMDSimulator.isCameraInitializationComplete == false) {
                    yield return new WaitForSeconds(0.1f);
                }

                setControllerPositions();
                setControllerRotations();

                if (applyCollider && applyColliderHand == Handedness.Ambidextrous) {
                    EventManager.instantiateController(0, false);
                    EventManager.instantiateController(1, false);
                } else if (applyCollider && applyColliderHand == Handedness.Left) {
                    EventManager.instantiateController(0, false);
                    EventManager.instantiateController(1, true);
                } else if (applyCollider && applyColliderHand == Handedness.Right) {
                    EventManager.instantiateController(0, true);
                    EventManager.instantiateController(1, false);
                } else {
                    EventManager.instantiateController(0, true);
                    EventManager.instantiateController(1, true);
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Instantiates the controller indicated by the <paramref name="controller"/> parameter and disables the attached <see cref="Collider"/> component if <paramref name="removeCollider"/> is true. </summary>
            ///
            /// <exception cref="NullReferenceException">   Thrown if either <see cref="HMDSimulator.leftControllerPrefab"/> or <see cref="HMDSimulator.rightControllerPrefab"/> is null. </exception>
            ///
            /// <param name="controller">       The controller to instantiate. Accepts either 0 for left-hand or 1 for right-hand. </param>
            /// <param name="removeCollider">   (Optional) true to remove collider. Defaults to false. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void OnInstantiateController(int controller, bool removeCollider = false) {
                GameObject[] controllers = new GameObject[2];
                switch (HMDSimulator.controllerPrimitive) {
                    case PrimitiveType.Cube:
                        HMDSimulator.setControllerPrefab();
                        controllers[controller] = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
                        controllers[controller].transform.localScale *= primitiveScaling;
                        break;
                    case PrimitiveType.Capsule:
                        HMDSimulator.setControllerPrefab();
                        controllers[controller] = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Capsule);
                        controllers[controller].transform.localScale *= primitiveScaling;
                        break;
                    case PrimitiveType.Cylinder:
                        HMDSimulator.setControllerPrefab();
                        controllers[controller] = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cylinder);
                        controllers[controller].transform.localScale *= primitiveScaling;
                        break;
                    case PrimitiveType.Sphere:
                        HMDSimulator.setControllerPrefab();
                        controllers[controller] = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Sphere);
                        controllers[controller].transform.localScale *= primitiveScaling;
                        break;
                    case PrimitiveType.Custom:
                        if (controller == 0 && HMDSimulator.leftControllerPrefab != null) {
                            controllers[0] = GameObject.Instantiate(HMDSimulator.leftControllerPrefab) as GameObject;
                            _originalPrefabOffset[0] = controllers[0].transform.position;
                            _originalPrefabRotation[0] = controllers[0].transform.rotation;
                        } else if (controller == 0 && HMDSimulator.leftControllerPrefab == null) {
                            Debug.LogError("[ImmerseumSDK]/HMDSimulator: Missing Left Controller Prefab.");
                            throw new System.NullReferenceException();
                        } else if (controller == 1 && HMDSimulator.rightControllerPrefab != null) {
                            controllers[1] = GameObject.Instantiate(HMDSimulator.rightControllerPrefab) as GameObject;
                            _originalPrefabOffset[0] = controllers[1].transform.position;
                            _originalPrefabRotation[0] = controllers[1].transform.rotation;
                        } else if (controller == 1 && HMDSimulator.rightControllerPrefab == null) {
                            Debug.LogError("[ImmerseumSDK]/HMDSimulator: Missing Right Controller Prefab.");
                            throw new System.NullReferenceException();
                        }
                        break;
                    default:
                        goto case PrimitiveType.Sphere;
                }

                if (controller == 0) {
                    controllers[0].name = "Left Controller";
                } else {
                    controllers[1].name = "Right Controller";
                }

                HMDSimulator.setControllers(controller, controllers[controller]);
                EventManager.endInstantiateController(controller, removeCollider);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Initiates the <see cref="EventManager.OnApplyCollider"/> event. </summary>
            ///
            /// <param name="controller">       The controller to instantiate. Accepts either 0 for left-hand
            ///                                 or 1 for right-hand. </param>
            /// <param name="removeCollider">   (Optional) true to remove collider. Defaults to false. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void OnInstantiateControllerEnd(int controller, bool removeCollider = false) {
                if (HMDSimulator.leftController != null && HMDSimulator.rightController != null) {
                    Instance._areControllersSimulated = true;
                }
                EventManager.applyCollider(controller, removeCollider);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Applies the <see cref="Collider"/> to the <paramref name="controller"/> based on the configuration in <see cref="ControllerManager"/>. </summary>
            /// <remarks>   Upon conclusion, fires the <see cref="EventManager.OnApplyColliderEnd"/> event.  </remarks>
            ///
            /// <exception cref="IndexOutOfRangeException"> Thrown when the the value of <paramref name="controller"/> is not 0 or 1. </exception>
            ///
            /// <param name="controller">       The controller to instantiate. Accepts either 0 for left-hand
            ///                                 or 1 for right-hand. </param>
            /// <param name="removeCollider">   (Optional) true to remove collider. Defaults to false. </param>
            /// <seealso cref="applyCollider"/>
            /// <seealso cref="colliderHeight"/>
            /// <seealso cref="colliderPhysicMaterial"/>
            /// <seealso cref="colliderRadius"/>
            /// <seealso cref="colliderCenter"/>
            /// <seealso cref="colliderDirection"/>
            /// <seealso cref="colliderSize"/>
            /// <seealso cref="isTrigger"/>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void OnApplyCollider(int controller, bool removeCollider = false) {
                Collider collider;
                Rigidbody rb;

                if (controller == 0) {
                    collider = HMDSimulator.leftController.GetComponent<Collider>();
                    rb = HMDSimulator.leftController.GetComponent<Rigidbody>();
                } else if (controller == 1) {
                    collider = HMDSimulator.rightController.GetComponent<Collider>();
                    rb = HMDSimulator.rightController.GetComponent<Rigidbody>();
                } else {
                    Debug.LogError("[ImmerseumSDK]/HMDSimulator: Unrecognized controller index.");
                    throw new System.IndexOutOfRangeException();
                }

                if (removeCollider) {
                    if (collider != null) {
                        collider.enabled = false;
                    }
                    if (rb != null) {
                        rb.isKinematic = true;
                    }
                } else {
                    collider.material = colliderPhysicMaterial;
                    collider.isTrigger = isTrigger;

                    switch (HMDSimulator.controllerPrimitive) {
                        case PrimitiveType.Sphere:
                            collider.GetComponent<SphereCollider>().center = colliderCenter;
                            collider.GetComponent<SphereCollider>().radius = colliderRadius;
                            break;
                        case PrimitiveType.Capsule:
                            collider.GetComponent<CapsuleCollider>().center = colliderCenter;
                            collider.GetComponent<CapsuleCollider>().radius = colliderRadius;
                            collider.GetComponent<CapsuleCollider>().height = colliderHeight;
                            switch (colliderDirection) {
                                case Direction.XAxis:
                                    collider.GetComponent<CapsuleCollider>().direction = 0;
                                    break;
                                case Direction.YAxis:
                                    collider.GetComponent<CapsuleCollider>().direction = 1;
                                    break;
                                case Direction.ZAxis:
                                    collider.GetComponent<CapsuleCollider>().direction = 2;
                                    break;
                                default:
                                    Debug.LogError("[ImmerseumSDK] Unidentified collider direction applied.");
                                    break;
                            }
                            break;
                        case PrimitiveType.Cube:
                            collider.GetComponent<BoxCollider>().center = colliderCenter;
                            collider.GetComponent<BoxCollider>().size = colliderSize;
                            break;
                        case PrimitiveType.Cylinder:
                            goto case PrimitiveType.Capsule;
                        case PrimitiveType.Custom:
                            break;
                        default:
                            goto case PrimitiveType.Sphere;
                    }
                }
                EventManager.endApplyCollider(controller, removeCollider);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Fires the <see cref="EventManager.OnApplyRigidbody"/> and the <see cref="EventManager.OnPositionController"/> events</summary>
            ///
            /// <param name="controller">       The controller to instantiate. Accepts either 0 for left-hand
            ///                                 or 1 for right-hand. </param>
            /// <param name="removeCollider">   (Optional) true to remove collider. Defaults to false. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void OnApplyColliderEnd(int controller, bool removeCollider = false) {
                EventManager.applyRigidbody(controller);
                EventManager.positionController(controller, startingPosition);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Attaches a <see cref="Rigidbody"/> component to the indicated <paramref name="controller"/> based on the configuration in <see cref="ControllerManager"/>.  </summary>
            ///
            /// <remarks>   Only applies if <see cref="applyRigidbody"/> is true. </remarks>
            ///
            /// <exception cref="ArgumentException">    Thrown when the value of <paramref name="controller"/> is not 0 or 1. </exception>
            ///
            /// <param name="controller">   The controller to instantiate. Accepts either 0 for left-hand or
            ///                             1 for right-hand. </param>
            /// <seealso cref="applyRigidbody"/>
            /// <seealso cref="applyRigidbodyHand"/>
            /// <seealso cref="rbMass"/>
            /// <seealso cref="rbDrag"/>
            /// <seealso cref="rbAngularDrag"/>
            /// <seealso cref="rbUseGravity"/>
            /// <seealso cref="isKinematic"/>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void OnApplyRigidbody(int controller) {
                Transform controllerTransform = HMDSimulator.leftController.transform;
                Rigidbody rb;

                if (controller == 0) {
                    controllerTransform = HMDSimulator.leftController.transform;
                } else if (controller == 1) {
                    controllerTransform = HMDSimulator.rightController.transform;
                } else {
                    Debug.LogError("[ImmerseumSDK] Unrecognized Controller Index.");
                    throw new ArgumentException("Unrecognized Controller Index.");
                }

                rb = controllerTransform.GetComponent<Rigidbody>();

                if (applyRigidbody && applyRigidbodyHand == Handedness.Ambidextrous) {
                    if (rb == null) {
                        rb = controllerTransform.gameObject.AddComponent<Rigidbody>();
                    }
                    rb.isKinematic = _isKinematic;
                    rb.mass = _rbMass;
                    rb.drag = _rbDrag;
                    rb.angularDrag = _rbAngularDrag;
                    rb.useGravity = _rbUseGravity;
                } else if (applyRigidbody && applyRigidbodyHand == Handedness.Left && controller == 0) {
                    if (rb == null) {
                        rb = controllerTransform.gameObject.AddComponent<Rigidbody>();
                    }
                    rb.isKinematic = _isKinematic;
                    rb.mass = _rbMass;
                    rb.drag = _rbDrag;
                    rb.angularDrag = _rbAngularDrag;
                    rb.useGravity = _rbUseGravity;
                } else if (applyRigidbody && applyRigidbodyHand == Handedness.Right && controller == 1) {
                    if (rb == null) {
                        rb = controllerTransform.gameObject.AddComponent<Rigidbody>();
                    }
                    rb.isKinematic = _isKinematic;
                    rb.mass = _rbMass;
                    rb.drag = _rbDrag;
                    rb.angularDrag = _rbAngularDrag;
                    rb.useGravity = _rbUseGravity;
                } else {
                    if (rb != null) {
                        rb.isKinematic = true;
                        rb.detectCollisions = false;
                    }
                }
                EventManager.endApplyRigidbody(controller);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Positions the <paramref name="controller"/> in the <paramref name="targetPosition"/> indicated.</summary>
            /// <remarks>   When finished, fires the <see cref="EventManager.OnPositionControllerEnd"/> event.   </remarks>
            ///
            /// <exception cref="IndexOutOfRangeException"> Thrown when <paramref name="controller"/> is not 0 or 1. </exception>
            ///
            /// <param name="controller">       The controller to instantiate. Accepts either 0 for left-hand
            ///                                 or 1 for right-hand. </param>
            /// <param name="targetPosition">   The target position for the controller to occupy, expressed as a <see cref="ControllerPositions"/>. </param>
            /// <seealso cref="ControllerPositions"/>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void OnPositionController(int controller, ControllerPositions targetPosition) {
                switch (targetPosition) {
                    case ControllerPositions.Origin:
                        if (controller == 0) {
                            HMDSimulator.leftController.position += _defaultOriginOffset[0];
                            HMDSimulator.leftController.localRotation = _defaultOriginRotation[0];
                        } else if (controller == 1) {
                            HMDSimulator.rightController.position += _defaultOriginOffset[1];
                            HMDSimulator.rightController.localRotation = _defaultOriginRotation[1];
                        } else {
                            Debug.LogError("[ImmerseumSDK]/HMDSimulator: Unrecognized controller index.");
                            throw new System.IndexOutOfRangeException();
                        }
                        break;
                    case ControllerPositions.Forward:
                        if (controller == 0) {
                            HMDSimulator.leftController.position += _defaultForwardOffset[0];
                            HMDSimulator.leftController.localRotation = _defaultForwardRotation[0];
                        } else if (controller == 1) {
                            HMDSimulator.rightController.position += _defaultForwardOffset[1];
                            HMDSimulator.rightController.localRotation = _defaultForwardRotation[1];
                        } else {
                            Debug.LogError("[ImmerseumSDK]/HMDSimulator: Unrecognized controller index.");
                            throw new System.IndexOutOfRangeException();
                        }
                        break;
                    case ControllerPositions.Boxer:
                        if (controller == 0) {
                            HMDSimulator.leftController.position += _defaultBoxerOffset[0];
                            HMDSimulator.leftController.localRotation = _defaultBoxerRotation[0];
                        } else if (controller == 1) {
                            HMDSimulator.rightController.position += _defaultBoxerOffset[1];
                            HMDSimulator.rightController.localRotation = _defaultBoxerRotation[1];
                        } else {
                            Debug.LogError("[ImmerseumSDK]/HMDSimulator: Unrecognized controller index.");
                            throw new System.IndexOutOfRangeException();
                        }
                        break;
                    case ControllerPositions.Reaching:
                        if (controller == 0) {
                            HMDSimulator.leftController.position += _defaultReachingOffset[0];
                            HMDSimulator.leftController.localRotation = _defaultReachingRotation[0];
                        } else if (controller == 1) {
                            HMDSimulator.rightController.position += _defaultReachingOffset[1];
                            HMDSimulator.rightController.localRotation = _defaultReachingRotation[1];
                        } else {
                            Debug.LogError("[ImmerseumSDK]/HMDSimulator: Unrecognized controller index.");
                            throw new System.IndexOutOfRangeException();
                        }
                        break;
                    case ControllerPositions.Prefab:
                        break;
                    default:
                        goto case ControllerPositions.Origin;
                }
                EventManager.endPositionController(controller, targetPosition);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Starts the process of moving <paramref name="controller"/> to the <paramref name="targetPosition"/>.</summary>
            ///
            /// <remarks>   When finished, fires the <see cref="EventManager.OnMoveControllerStart_Vector"/> event.</remarks>
            ///
            /// <param name="controller">       The controller to instantiate. Accepts either 0 for left-hand
            ///                                 or 1 for right-hand. </param>
            /// <param name="targetPosition">   The target position for the controller to occupy, expressed
            ///                                 as a <see cref="ControllerPositions"/>. </param>
            /// <seealso cref="ControllerPositions"/>
            /// <seealso cref="EventManager.OnMoveControllerStart_Vector"/>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void OnMoveControllerStart(int controller, ControllerPositions targetPosition) {
                Quaternion targetRotation = new Quaternion();
                Vector3 targetOffset = new Vector3();
                switch (targetPosition) {
                    case ControllerPositions.Origin:
                        targetOffset = _defaultOriginOffset[controller];
                        targetRotation = _defaultOriginRotation[controller];
                        break;
                    case ControllerPositions.Forward:
                        targetOffset = _defaultForwardOffset[controller];
                        targetRotation = _defaultForwardRotation[controller];
                        break;
                    case ControllerPositions.Boxer:
                        targetOffset = _defaultBoxerOffset[controller];
                        targetRotation = _defaultBoxerRotation[controller];
                        break;
                    case ControllerPositions.Reaching:
                        targetOffset = _defaultReachingOffset[controller];
                        targetRotation = _defaultReachingRotation[controller];
                        break;
                    case ControllerPositions.Prefab:
                        targetOffset = _originalPrefabOffset[controller];
                        targetRotation = _originalPrefabRotation[controller];
                        break;
                }
                EventManager.moveController(controller, targetOffset, targetRotation, true);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Moves the <paramref name="controller"/> by the <paramref name="targetOffset"/> and adjusts its rotation to <paramref name="targetRotation"/>.</summary>
            ///
            /// <remarks>   <para>By default, moves the <paramref name="controller"/> by <paramref name="targetOffset"/> relative to the controller's <code>localPosition</code>. However, if <paramref name="relativeToOrigin"/> is true, moves the controller relative to the origin at (0,0,0).</para>
            ///
            /// <exception cref="ArgumentException">    Thrown when one <paramref name="controller"/> is not 0 or 1. </exception>
            ///
            /// <param name="controller">       The controller to instantiate. Accepts either 0 for left-hand
            ///                                 or 1 for right-hand. </param>
            /// <param name="targetOffset">     A <see cref="Vector3"/> of the coordinate offset by which the controller's position should be moved. </param>
            /// <param name="targetRotation">   A <see cref="Quaternion"/> of the rotation to which the controller should be rotated. </param>
            /// <param name="relativeToOrigin"> (Optional) true to move the controller relative to the origin <code>(0,0,0)</code> instead of the controller's <code>localPosition</code>. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            void OnMoveControllerStart_Vector(int controller, Vector3 targetOffset, Quaternion targetRotation, bool relativeToOrigin = true) {
                Transform controllerTransform = HMDSimulator.leftController.transform;
                Vector3 targetPosition = new Vector3();

                if (controller == 0) {
                    controllerTransform = HMDSimulator.leftController.transform;
                } else if (controller == 1) {
                    controllerTransform = HMDSimulator.rightController.transform;
                } else {
                    Debug.LogError("[ImmerseumSDK] Unrecognized Controller Index.");
                    throw new ArgumentException("Unrecognized Controller Index");
                }

                if (relativeToOrigin) {
                    targetPosition = controllerTransform.parent.localPosition + targetOffset;
                } else {
                    targetPosition = controllerTransform.localPosition + targetOffset;
                }
                StartCoroutine(_moveController(controller, controllerTransform, targetPosition, targetRotation));
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Moves the <paramref name="controllerTransform"/> to <paramref name="targetPosition"/> and <paramref name="targetRotation"/>. </summary>
            /// 
            /// <remarks>   <para>Yields and waits for fixed update before proceeding. Movement speed is determined by <see cref="movementSpeed"/>.</para>
            ///             <para>Upon completion, fires the <see cref="EventManager.OnMoveControllerEnd"/> event.</para> </remarks>
            ///
            /// <param name="controllerIndex">      Zero-based index of the controller. Accepts 0 for left-hand and 1 for right-hand. </param>
            /// <param name="controllerTransform">  The <see cref="Transform"/> of the controller being moved.</param>
            /// <param name="targetPosition">       The target position for the controller to occupy,
            ///                                     expressed as a <see cref="Vector3"/>. </param>
            /// <param name="targetRotation">       A <see cref="Quaternion"/>
            ///                                     of the rotation to which the controller should be
            ///                                     rotated. </param>
            ///
            /// <returns>   Yields and waits for fixed update before proceeding, loops until the the controller has reached its target position and rotation. </returns>
            /// <seealso cref="EventManager.OnMoveControllerEnd"/>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            IEnumerator _moveController(int controllerIndex, Transform controllerTransform, Vector3 targetPosition, Quaternion targetRotation) {
                float startTime = Time.time;
                float elapsedTime = 0f;
                bool isComplete = false;
                float step = movementSpeed * Time.deltaTime;

                while (isComplete == false) {
                    elapsedTime = Time.time - startTime;
                    step = movementSpeed * Time.deltaTime;

                    controllerTransform.localPosition = Vector3.MoveTowards(controllerTransform.localPosition, targetPosition, step);

                    if (controllerTransform.rotation != targetRotation) {
                        controllerTransform.rotation = Quaternion.RotateTowards(controllerTransform.rotation, targetRotation, 0.1f);
                    }
                    if (controllerTransform.localPosition == targetPosition && (controllerTransform.rotation == targetRotation || elapsedTime >= movementSpeed)) {
                        isComplete = true;
                        break;
                    }
                    yield return new WaitForFixedUpdate();
                }
                EventManager.endMoveController(controllerIndex);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Moves the <paramref name="controller" /> to the <paramref name="targetPosition" /> and adjusts its rotation to <paramref name="targetRotation" />.</summary>
            /// <param name="controller">The controller to instantiate. Accepts either <strong>0</strong> for left-hand or <strong>1</strong> for right-hand.</param>
            /// <param name="targetPosition">A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> of the coordinates by which each of the controller's three
            /// axes should be offset (moved).</param>
            /// <param name="targetRotation">A <see cref="!:https://docs.unity3d.com/ScriptReference/Quaternion.html">Quaternion</see> of the rotation to which the controller should be
            /// rotated.</param>
            /// <remarks>   Fires the <see cref="EventManager.OnMoveControllerStart_Vector" /> event.</remarks>
            /// <example>
            ///   <code title="Example" description="Moves the right-hand controller forward by 30cm (0.3m)." lang="CS">
            /// Debug.Log(HMDSimulator.leftController.position);
            /// Vector3 moveOffset = new Vector3(0,0,0.3f);
            ///  
            /// ControllerManager.moveController(1, moveOffset);
            ///  
            /// Debug.Log(HMDSimulator.leftController.position);
            /// // Outputs the same position as above, but with the Z-value increased by 0.3f.</code>
            /// </example>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static void moveController(int controller, Vector3 targetPosition, Quaternion targetRotation) {
                EventManager.moveController(controller, targetPosition, targetRotation);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>Moves the <see cref="controller" /> to the <see cref="targetPosition" /> while retaining its current rotation.</summary>
            /// <param name="controller">The controller to move. Accepts either <strong>0</strong> for left-hand or <strong>1</strong> for right-hand.</param>
            /// <param name="targetPosition">   The target position for the controller to occupy, expressed
            ///                                 as a <see cref="ControllerPositions" />
            ///                                 . </param>
            /// <remarks>
            /// Fires the <see cref="EventManager.OnMoveControllerStart" />
            /// event.
            /// </remarks>
            /// <example>
            ///   <code title="Example" description="Moves the right-hand controller to the Reaching position." lang="CS">
            /// ControllerManager.moveController(1, ControllerPositions.Reaching);</code>
            /// </example>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static void moveController(int controller, ControllerPositions targetPosition) {
                EventManager.moveController(controller, targetPosition);
            }

            void Awake() {
                if (Instance != null && Instance != this) {
                    Destroy(gameObject);
                }

                Instance = this;
            }

            // Use this for initialization
            void Start() {

            }

            // Update is called once per frame
            void Update() {

            }
        }
    }
}