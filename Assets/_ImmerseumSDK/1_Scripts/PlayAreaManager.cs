using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {

        /// <summary>Indicates which objects movement input actually moves.</summary>
        public enum InputMoveTarget {
            /// <summary>Movement input moves only the camera/controllers, which leaves the $$camera rig$$ and its related Play Area in its original position. Furthermore, the camera
            /// cannot go beyond the bounds of the Play Area.</summary>
            CameraOnly,
            /// <summary>Movement input moves both the camera/controllers <strong>and</strong> the play area (which is attached to the $$camera rig$$).</summary>
            CameraAndPlayArea
        }

        /// <summary>Indicates the configured size of the Play Area.</summary>
        public enum PlayAreaSize {
            /// <summary>The Play Area's width and depth are irrelevant because they do not apply.</summary>
            NotApplicable,
            /// <summary>The Play Area's width and depth are determined by the HMD's calibration values.</summary>
            Calibrated,
            /// <summary>Indicates a Play Area that is 4m x 3m.</summary>
            _400x300,
            /// <summary>
            ///   <para>Indicates a Play Area that is 3m x 2.25m.</para>
            /// </summary>
            _300x225,
            /// <summary>Indicates a Play Area that is 2m x 1.5m.</summary>
            _200x150,
            /// <summary>The Play Area's width and depth are explicitly determined by the scene designer.</summary>
            Custom
        }

        /// <summary>Indicates when to display the Play Area.</summary>
        public enum PlayAreaDisplayTrigger {
            /// <summary>The Play Area should always be displayed.</summary>
            Always,
            /// <summary>The Play Area should be displayed when the user is near the Play Area's bounds.</summary>
            OnApproach,
            /// <summary>The Play Area should be displayed when a specific <see cref="InputAction">Input Action</see> is detected.</summary>
            OnInputAction,
            /// <summary>The Play Area should never be displayed.</summary>
            Never
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Singleton responsible for controlling the display and behavior of the Immerseum Play Area. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public class PlayAreaManager : MonoBehaviour {
            /// <summary>The singleton Instance of this class.</summary>
            public static PlayAreaManager Instance { get; private set; }

            #region HMD Play Area
            /// <summary>Indicates whether an $$HMD Play Area$$ is included in the scene or calculated by an attached HMD.</summary>
            /// <value>
            ///   <strong>true</strong> if there is an $$HMD Play Area$$ in the scene, <strong>false</strong> if not.</value>
            public static bool hasHMDPlayArea {
                get {
                    if (hasSteamPlayArea || hasOculusPlayArea) {
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>Indicates whether there is a Steam-generated $$HMD Play Area$$ (or its data) present in the scene.</summary>
            /// <value>
            ///   <strong>true</strong> if there is a Steam-generated $$HMD Play Area$$ (or its data) in the scene, otherwise <strong>false</strong>.</value>
            public static bool hasSteamPlayArea {
                get {
                    if (HMDSimulator.isUsingSteamRig) {
                        if (HMDSimulator.CameraRig.GetComponent<SteamVR_PlayArea>() != null) {
                            return true;
                        }
                    }
                    return false;
                }
            }

            /// <summary>Indicates whether there is an Oculus-generated $$HMD Play Area$$ in the scene.</summary>
            /// <value>
            ///   <strong>true</strong> if there is an Oculus-generated $$HMD Play Area$$ (or its applicable data) available in the scene, otherwise <strong>false</strong>.</value>
            public static bool hasOculusPlayArea {
                get {
                    if (HMDSimulator.isUsingSteamRig == false) {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                        if (OVRManager.boundary.GetConfigured()) {
                            return true;
                        }
#endif
                    }
                    return false;
                }
            }


            /// <summary>The <see cref="!:https://docs.unity3d.com/Manual/class-Transform.html">Transform</see> to which the SteamVR_PlayArea component is attached.</summary>
            /// <value>The <see cref="!:https://docs.unity3d.com/Manual/class-Transform.html">Transform</see> to which the SteamVR_PlayArea component is attached. If
            /// no SteamVR_PlayArea component is present, returns <strong>null</strong>.</value>
            public static Transform getSteamPlayAreaTransform {
                get {
                    if (hasSteamPlayArea) {
                        return HMDSimulator.CameraRig.GetComponent<SteamVR_PlayArea>().transform;
                    }
                    return null;
                }
            }

            /// <summary>Returns the SteamVR_PlayArea component of the $$HMD Play Area$$.</summary>
            /// <value>The <strong>SteamVR_PlayArea</strong> component of the SteamVR $$Camera Rig$$ if present, otherwise returns <strong>null</strong>.</value>
            public static SteamVR_PlayArea getSteamPlayArea {
                get {
                    if (hasSteamPlayArea) {
                        return HMDSimulator.CameraRig.GetComponent<SteamVR_PlayArea>();
                    }
                    return null;
                }
            }

            /// <summary>Returns the <see cref="PlayAreaSize">PlayAreaSize</see> that is applied to the $$HMD Play Area$$.</summary>
            /// <value>The <see cref="PlayAreaSize" /> that is applied to the $$HMD Play Area$$.</value>
            public static PlayAreaSize getHMDPlayAreaSize {
                get {
                    if (hasSteamPlayArea) {
                        SteamVR_PlayArea steamPlayArea = getSteamPlayArea;
                        switch (steamPlayArea.size) {
                            case SteamVR_PlayArea.Size.Calibrated:
                                if (HMDSimulator.isHMDConnected) {
                                    return PlayAreaSize.Calibrated;
                                } else {
                                    if (HMDSimulator.logDiagnostics) {
                                        Debug.LogWarning("[ImmerseumSDK] Steam Play Area is sized as Calibrated, however calibration is unavailable without an HMD connected. Play Area will not be displayed / respected.");
                                    }
                                    return PlayAreaSize.NotApplicable;
                                }
                            case SteamVR_PlayArea.Size._200x150:
                                return PlayAreaSize._200x150;
                            case SteamVR_PlayArea.Size._300x225:
                                return PlayAreaSize._300x225;
                            case SteamVR_PlayArea.Size._400x300:
                                return PlayAreaSize._400x300;
                        }
                    } else if (hasOculusPlayArea) {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                        Vector3 playAreaDimensions = OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.OuterBoundary);
                        if (playAreaDimensions.x == 2.0f && playAreaDimensions.z == 1.5f) {
                            return PlayAreaSize._200x150;
                        } else if (playAreaDimensions.x == 3.0f && playAreaDimensions.z == 2.25f) {
                            return PlayAreaSize._300x225;
                        } else if (playAreaDimensions.x == 4.0f && playAreaDimensions.z == 3.0f) {
                            return PlayAreaSize._400x300;
                        }
                        return PlayAreaSize.Calibrated;
#else
                        if (HMDSimulator.logDiagnostics) {
                            Debug.LogWarning("[ImmerseumSDK] The Oculus Rift Boundary is only supported on Windows. Falling back to default Play Area size of 3m x 2.25m.");
                        }
                        return PlayAreaSize._300x225;
#endif
                    }
                    return PlayAreaSize.NotApplicable;
                }
            }

            /// <summary>Indicates whether the $$HMD Play Area$$'s size has been calibrated to an attached HMD.</summary>
            /// <value>
            ///   <strong>true</strong> if the $$HMD Play Area$$'s size has been calibrated using an attached HMD and the calibration data is available, otherwise returns
            /// <strong>false</strong>.</value>
            public static bool isHMDPlayAreaSizeCalibrated {
                get {
                    if (hasSteamPlayArea && HMDSimulator.isHMDConnected) {
                        if (getSteamPlayArea.size == SteamVR_PlayArea.Size.Calibrated) {
                            return true;
                        }
                    } else if (hasOculusPlayArea) {
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>Returns a list of the vertices exposed by the $$HMD Play Area$$.</summary>
            /// <value>A list of <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> values, where each value represents a vertex in
            /// worldspace returned by the $$HMD Play Area$$.</value>
            public static List<Vector3> getHMDPlayAreaVerticesList {
                get {
                    List<Vector3> vertices = new List<Vector3>();

                    if (hasSteamPlayArea) {
                        vertices = Utilities.convertArrayToList(getSteamPlayArea.vertices);
                    } else if (hasOculusPlayArea) {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                        vertices = Utilities.convertArrayToList(OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea));
#else
                        if (HMDSimulator.logDiagnostics) {
                            Debug.LogWarning("[ImmerseumSDK] Oculus Rift is not supported on MacOS. Returning an empty list.");
                        }
#endif
                    }
                    return vertices;
                }
            }
            #endregion

            /// <summary>
            /// Indicates whether the Immerseum SDK has generated a play area for display in the scene.
            /// </summary>
            /// <value>
            ///   <strong>true</strong> if the Play Area has been generated, <strong>false</strong> if not.</value>
            public static bool hasImmerseumPlayArea {
                get {
                    if (Instance._playArea != null) {
                        return true;
                    }
                    return false;
                }
            }

            protected PlayArea _playArea;
            /// <summary>
            /// The instance of the Play Area component applied to the Immerseum Play Area object.
            /// </summary>
            /// <value>
            ///   The instance of the Play Area component applied to the Immerseum Play Area object.</value>
            public static PlayArea playArea {
                get {
                    return Instance._playArea;
                }
            }

            /// <summary>
            /// Sets the instance of the Play Area object that is assigned to the Immerseum Play Area.
            /// </summary>
            /// <param name="playAreaInstance">The PlayArea object that is assigned to the Immerseum Play Area.</param>
            public static void setPlayArea(PlayArea playAreaInstance) {
                Instance._playArea = playAreaInstance;
            }

            bool hasHitPlayAreaBounds {
                get {
                    if (hasImmerseumPlayArea && HMDSimulator.hasCollider == false) {
                        if (playArea.isWithinBoundary() == false) {
                            return true;
                        }
                    }
                    if (hasImmerseumPlayArea && playArea.isWithinBoundary() == false) {
                        return true;
                    }
                    return false;
                }
            }

            #region Play Area Behavior
            [SerializeField]
            protected InputMoveTarget _inputMoveTarget = InputMoveTarget.CameraAndPlayArea;
            /// <summary>Indicates the objects that are moved in response to user input.</summary>
            /// <value>The %%InputMoveTarget:InputMoveTarget%% value that indicates the objects to move in response to user input.</value>
            protected static InputMoveTarget inputMoveTarget {
                get {
                    return Instance._inputMoveTarget;
                }
            }

            /// <summary>
            /// Sets whether the user's movement is bound to the size of their play area, either determined by their HMD configuration or by the scene designer.
            /// </summary>
            /// <param name="value">Boolean value that determines the value of <see cref="isMovementBoundToPlayArea">isMovementBoundToPlayArea</see>.</param>
            /// <param name="target">The %%InputMoveTarget:InputMoveTarget%% that determines which objects should be moved in response to user input.</param>
            public void setInputMoveTarget(InputMoveTarget target) {
                Instance._inputMoveTarget = target;
            }

            /// <summary>Indicates whether the user's movement is currently constrained to the play area, i.e. movement input will not take the user's camera outside of the play area as determined by their HMD configuration (for room-scale configurations) or by the scene designer (for seated/standing configurations).</summary>
            /// <value>
            ///   <strong>true</strong> if the player cannot leave the play area and movement moves the camera object, <strong>false</strong> if movement actually moves the play
            /// area (with the camera centered within that play area).</value>
            public static bool isMovementBoundToPlayArea {
                get {
                    if (inputMoveTarget == InputMoveTarget.CameraOnly) {
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>
            /// Sets whether the user's movement is bound to the size of their play area, either determined by their HMD configuration or by the scene designer.
            /// </summary>
            /// <param name="value">Boolean value that determines the value of <see cref="isMovementBoundToPlayArea">isMovementBoundToPlayArea</see>.</param>
            public static void setIsMovementBoundToPlayArea(bool value) {
                if (value == true) {
                    Instance._inputMoveTarget = InputMoveTarget.CameraOnly;
                } else {
                    Instance._inputMoveTarget = InputMoveTarget.CameraAndPlayArea;
                }
            }

            /// <summary>Indicates whether the $$Immerseum Play Area$$ should be calculated and built in the scene.</summary>
            /// <value>
            ///   <strong>True</strong> if the $$Immerseum Play Area$$ should be calculated / processed within the scene, <strong>false</strong> if not.</value>
            public static bool shouldPlayAreaBeApplied {
                get {
                    if (playAreaDisplayTrigger != PlayAreaDisplayTrigger.Never) {
                        return true;
                    }
                    return false;
                }
            }

            [SerializeField]
            protected PlayAreaDisplayTrigger _playAreaDisplayTrigger = PlayAreaDisplayTrigger.OnApproach;
            /// <summary>Indicates when the $$Immerseum Play Area$$ should be rendered/displayed within the scene.</summary>
            /// <value>The <see cref="PlayAreaDisplayTrigger">PlayAreaDisplayTrigger</see> which causes a display of the $$Immerseum Play Area$$.</value>
            public static PlayAreaDisplayTrigger playAreaDisplayTrigger {
                get {
                    return Instance._playAreaDisplayTrigger;
                }
            }

            /// <summary>
            /// Sets the value of <see cref="playAreaDisplayTrigger">playAreaDisplayTrigger</see>.
            /// </summary>
            /// <param name="value"><see cref="PlayAreaDisplayTrigger">PlayAreaDisplayTrigger</see> value to apply.</param>
            public static void setPlayAreaDisplayTrigger(PlayAreaDisplayTrigger value) {
                Instance._playAreaDisplayTrigger = value;
            }

            /// <summary>
            /// Indicates whether the Play Area should always be displayed in the scene.
            /// </summary>
            /// <value>
            ///   <strong>true</strong> if the $$Immerseum Play Area$$ should always be displayed, <strong>false</strong> if not.</value>
            public static bool isPlayAreaDisplayedAlways {
                get {
                    return playAreaDisplayTrigger == PlayAreaDisplayTrigger.Always;
                }
            }

            /// <summary>Indicates whether the $$Immerseum Play Area$$ should be displayed when the user approaches its bounds.</summary>
            /// <value>
            ///   <strong>true</strong> if the $$Immerseum Play Area$$ should be displayed when the user approaches its bounds, <strong>false</strong> if not.</value>
            public static bool isPlayAreaDisplayedOnApproach {
                get {
                    return playAreaDisplayTrigger == PlayAreaDisplayTrigger.OnApproach;
                }
            }

            /// <summary>
            /// Indicates whether the Play Area should be displayed given the distance supplied.
            /// </summary>
            /// <param name="distance">A distance from the Play Area's bounds.</param>
            /// <returns>
            ///   <strong>True</strong> if the Play Area should be displayed, otherwise <strong>False</strong>.</returns>
            public static bool doesPlayAreaDisplayAtDistance(float distance) {
                return (playAreaDisplayTrigger == PlayAreaDisplayTrigger.OnApproach && playAreaDisplayOnApproachDistance >= distance);
            }

            /// <summary>Indicates whether the Play Area should be toggled (displayed or hidden) on an %%InputAction:InputAction%%.</summary>
            /// <returns>
            ///   <strong>True</strong> if the Play Area should be displayed on an %%InputAction:InputAction%%, otherwise <strong>False</strong>.</returns>
            public static bool doesPlayAreaDisplayOnInputAction() {
                return playAreaDisplayTrigger == PlayAreaDisplayTrigger.OnInputAction;
            }
            /// <summary>Indicates whether the Play Area should be toggled (displayed or hidden) on an %%InputAction:InputAction%%.</summary>
            /// <param name="name">The specific name of the %%InputAction:InputAction%% to check.</param>
            /// <returns>
            ///   <strong>True</strong> if an %%InputAction:InputAction%% with a matching <strong>name</strong> should toggle the Play Area's display, otherwise
            /// <strong>False</strong>.</returns>
            public static bool doesPlayAreaDisplayOnInputAction(string name) {
                return (playAreaDisplayTrigger == PlayAreaDisplayTrigger.OnInputAction && playAreaDisplayOnInputAction == name);
            }

            [SerializeField]
            protected string _playAreaDisplayOnInputAction;
            /// <summary>Indicates the name of the <see cref="InputAction">InputAction</see> that toggles the $$Immerseum Play Area$$.</summary>
            /// <value>The name of the <see cref="InputAction">InputAction</see> that - when fired - toggles the display of the $$Immerseum Play Area$$.</value>
            public static string playAreaDisplayOnInputAction {
                get {
                    return Instance._playAreaDisplayOnInputAction;
                }
            }

            /// <summary>
            /// Sets the value of <see cref="playAreaDisplayOnInputAction">playAreaDisplayOnInputAction</see> which determines which %%InputAction:InputAction%% toggles the display of the Play Area.
            /// </summary>
            /// <param name="value">The name property of the %%InputAction:InputAction%% which should toggle the display of the Play Area.</param>
            public static void setPlayAreaDisplayOnInputAction(string value) {
                Instance._playAreaDisplayOnInputAction = value;
            }

            [SerializeField]
            [Range(0, 100)]
            protected float _playAreaDisplayOnApproachDistance = 0.25f;
            /// <summary>Indicates the distance at which the $$Immerseum Play Area$$ should be displayed. When the distance between the Play Area's bounds and the
            /// camera/controllers is less than this value, the Immerseum Play Area is displayed.</summary>
            /// <value>The distance (in Unity worldspace units, equivalent to meters) at which the $$Immerseum Play Area$$ should be displayed.</value>
            public static float playAreaDisplayOnApproachDistance {
                get {
                    return Instance._playAreaDisplayOnApproachDistance;
                }
            }

            /// <summary>
            /// Sets the value of <see cref="playAreaDisplayOnApproachDistance">playAreaDisplayOnApproachDistance</see>.
            /// </summary>
            /// <param name="value">The distance from the Play Area bounds which - when crossed by the camera/controllers - toggles the display of the Play Area.</param>
            public static void setPlayAreaDisplayOnApproachDistance(float value) {
                Instance._playAreaDisplayOnApproachDistance = Mathf.Abs(value);
            }

            #endregion

            #region Design Properties
            [SerializeField]
            protected PlayAreaSize _playAreaSize = PlayAreaSize.NotApplicable;
            /// <summary>
            /// Indicates the size of the Play Area, in terms of width (x-axis) and depth (z-axis).
            /// </summary>
            /// <value>The <see cref="PlayAreaSize">PlayAreaSize</see> value that indicates the dimensions of the $$Immerseum Play Area$$.</value>
            public static PlayAreaSize playAreaSize {
                get {
                    if (playAreaDisplayTrigger == PlayAreaDisplayTrigger.Never) {
                        return PlayAreaSize.NotApplicable;
                    }
                    if (isHMDPlayAreaSizeCalibrated) {
                        return PlayAreaSize.Calibrated;
                    }
                    return Instance._playAreaSize;
                }
            }
            /// <summary>Sets the value of <see cref="playAreaSize">playAreaSize</see> and propagates that information to the scene.</summary>
            /// <param name="size">The dimensions of the Play Area to apply.</param>
            public void setPlayAreaSize(PlayAreaSize size) {
                Instance._playAreaSize = size;
                if (hasImmerseumPlayArea) {
                    _playArea.setPlayAreaSize(size);
                }
            }

            [SerializeField]
            protected Vector2 _customPlayAreaDimensions = Vector2.zero;
            /// <summary>
            /// Indicates the custom dimensions (width / depth) of the Play Area.
            /// </summary>
            /// <value>A <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> value where the width is stored in the <strong>x</strong>
            /// position and the depth is stored in the <strong>y</strong> position.</value>
            public static Vector2 customPlayAreaDimensions {
                get {
                    return Instance._customPlayAreaDimensions;
                }
            }

            /// <summary>Returns the width (X) and depth (Z) of the Play Area as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>
            /// value.</summary>
            /// <returns>The width/depth of the Play Area expressed as a <see cref="!:https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see>, with the
            /// width (x-axis) stored in the <strong>x</strong> position and the depth (z-axis) stored in the <strong>y</strong> position.</returns>
            public static Vector2 getPlayAreaDimensions() {
                switch (playAreaSize) {
                    case PlayAreaSize.NotApplicable:
                        return Vector2.zero;
                    case PlayAreaSize._200x150:
                        return new Vector2(2.00f, 1.50f);
                    case PlayAreaSize._300x225:
                        return new Vector2(3.00f, 2.25f);
                    case PlayAreaSize._400x300:
                        return new Vector2(4.00f, 3.00f);
                    case PlayAreaSize.Custom:
                        return customPlayAreaDimensions;
                    case PlayAreaSize.Calibrated:
                        if (HMDSimulator.logDiagnostics) {
                            Debug.LogWarning("[ImmerseumSDK] Play area is calibrated to the user's room. Use getHMDPlayAreaVerticesList instead.");
                        }
                        throw new System.Exception("Play area is calibrated to the user's room, which means it cannot return a 2-dimensional vector. Use getHMDPlayAreaVerticesList instead.");
                }
                return Vector2.zero;
            }

            [SerializeField]
            [Range(0, 100)]
            protected float _playAreaBorderThickness = 0.15f;
            /// <summary>
            /// Indicates the thickness of the Play Area's border when rendering without a Material.
            /// </summary>
            /// <value>The thickness (in Unity worldspace measurements, equivalent to meters) of the $$Immerseum Play Area$$'s border.</value>
            public static float playAreaBorderThickness {
                get {
                    return Instance._playAreaBorderThickness;
                }
            }

            /// <summary>
            /// Sets the value of the <see cref="playAreaBorderThickness">playAreaBorderThickness</see>.
            /// </summary>
            /// <param name="value">The thickness to apply to the Play Area's border.</param>
            public void setPlayAreaBorderThickness(float value) {
                Instance._playAreaBorderThickness = Mathf.Abs(value);
            }

            [SerializeField]
            protected Color _playAreaColor = new Color(0, 255, 255, 255);
            /// <summary>
            /// Indicates the color to apply to the Play Area.
            /// </summary>
            /// <value>The <see cref="!:https://docs.unity3d.com/ScriptReference/Color.html">Color</see> to apply to the $$Immerseum Play Area$$.</value>
            public static Color playAreaColor {
                get {
                    return Instance._playAreaColor;
                }
            }

            /// <summary>
            /// Sets the Color to apply to the Play Area.
            /// </summary>
            /// <param name="value">The Color to apply.</param>
            public void setPlayAreaColor(Color value) {
                Instance._playAreaColor = value;
            }

            [SerializeField]
            protected Material _playAreaMaterial;
            /// <summary>Indicates the <see cref="!:https://docs.unity3d.com/ScriptReference/Material.html">Material</see> to apply to the $$Immerseum Play Area$$.</summary>
            /// <value>The <see cref="!:https://docs.unity3d.com/ScriptReference/Material.html">Material</see> to apply to the $$Immerseum Play Area$$.</value>
            public static Material playAreaMaterial {
                get {
                    return Instance._playAreaMaterial;
                }
            }

            /// <summary>
            /// Sets the Material to apply to the Play Area.
            /// </summary>
            /// <param name="value">The Material to apply to the Play Area.</param>
            public void setPlayAreaMaterial(Material value) {
                Instance._playAreaMaterial = value;
            }
            #endregion

            void Awake() {
                if (Instance != null && Instance != this) {
                    Destroy(gameObject);
                }

                Instance = this;
            }

            void OnEnable() {
                EventManager.OnApplyCollider += OnApplyCollider;
                EventManager.OnInitializePlayArea += OnInitializePlayArea;
                EventManager.OnInputActionStart += OnInputActionStart;
                EventManager.OnMoveAvatarEnd += OnMoveAvatarEnd;
            }

            void OnDisable() {
                EventManager.OnApplyCollider -= OnApplyCollider;
                EventManager.OnInitializePlayArea -= OnInitializePlayArea;
                EventManager.OnInputActionStart -= OnInputActionStart;
                EventManager.OnMoveAvatarEnd -= OnMoveAvatarEnd;
            }

            void OnApplyCollider(int controller, bool removeCollider) {
                if (ControllerManager.areControllersSimulated) {
                    EventManager.initializePlayArea(); 
                }
            }

            PlayArea createPlayAreaInstance() {
                GameObject playAreaPrefab = (GameObject)Instantiate(Resources.Load("[immerseumPlayArea]", typeof(GameObject)));
                PlayArea playAreaInstance = playAreaPrefab.GetComponent<PlayArea>();
                playAreaInstance.borderThickness = playAreaBorderThickness;
                playAreaInstance.setMaterial(playAreaMaterial, playAreaColor);
                if (playAreaSize == PlayAreaSize.Calibrated && HMDSimulator.isHMDConnected == false) {
                    if (HMDSimulator.logDiagnostics) {
                        Debug.LogWarning("[ImmerseumSDK] Play Area calibration is not available when there is no headset attached. Defaulting to 3m x 2.25m Play Area.");
                    }
                    setPlayAreaSize(PlayAreaSize._300x225);
                }
                playAreaInstance.setPlayAreaSize(playAreaSize);

                playAreaInstance.applyColliders();

                return playAreaInstance;
            }

            void OnInitializePlayArea() {
                if (shouldPlayAreaBeApplied) {
                    PlayArea playAreaInstance = createPlayAreaInstance();

                    if (isMovementBoundToPlayArea == false) {
                        if (HMDSimulator.isUsingSteamRig) {
                            playAreaInstance.setParent(HMDSimulator.CameraRig);
                        } else {
                            playAreaInstance.setParent(HMDSimulator.CameraRig.root);
                        }
                    }

                    setPlayArea(playAreaInstance);

                    if (playAreaDisplayTrigger == PlayAreaDisplayTrigger.OnInputAction) {
                        playArea.gameObject.SetActive(false);
                    } else if (playAreaDisplayTrigger == PlayAreaDisplayTrigger.OnApproach && _playArea.getDistance() > playAreaDisplayOnApproachDistance) {
                        playArea.gameObject.SetActive(false);
                    }

                    EventManager.endInitializePlayArea();
                }
            }

            void OnInputActionStart(InputAction inputAction) {
                if (doesPlayAreaDisplayOnInputAction(inputAction.name) && hasImmerseumPlayArea) {
                    if (playArea.isVisible) {
                        playArea.gameObject.SetActive(false);
                    } else {
                        playArea.gameObject.SetActive(true);
                    }
                }
            }

            bool toggleDisplayAtDistance() {
                float distance = playArea.getDistance();
                return toggleDisplayAtDistance(distance);
            }

            bool toggleDisplayAtDistance(float distance) {
                if (doesPlayAreaDisplayAtDistance(distance)) {
                    playArea.gameObject.SetActive(true);
                    return true;
                }
                playArea.gameObject.SetActive(false);
                return false;
            }

            void OnMoveAvatarEnd() {
                if (playAreaDisplayTrigger == PlayAreaDisplayTrigger.OnApproach && hasImmerseumPlayArea) {
                    toggleDisplayAtDistance();
                }
            }

        }
    }
}