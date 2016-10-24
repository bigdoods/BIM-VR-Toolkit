using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {
        public class KeyboardButton : InputButton {
            public new InputType buttonType {
                get {
                    return InputType.Keyboard;
                }
            }

            /// <summary>List of Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see> which fire this Input Action.</summary>
            /// <value>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of
            /// Unity <see cref="!:https://docs.unity3d.com/ScriptReference/KeyCode.html">KeyCodes</see>.</value>
            public List<KeyCode> keyList = new List<KeyCode>();

            /// <summary>Indicates whether the Input Button is $$held$$.</summary>
            /// <value>
            ///   <strong>true</strong> if the Input Button is $$held$$, otherwise <strong>false</strong>.</value>
            public override bool isHeld {
                get {
                    int n = keyList.Count;
                    for (int x = 0; x < n; x++) {
                        if (Input.GetKey(keyList[x])) {
                            return true;
                        }
                    }
                    return false;
                }
            }

            /// <summary>Indicates whether the Input Button is $$pressed$$.</summary>
            /// <value>
            ///   <strong>true</strong> if the Input Button is $$pressed$$, otherwise <strong>false</strong>.</value>
            public override bool isPressed {
                get {
                    int n = keyList.Count;
                    for (int x = 0; x < n; x++) {
                        if (Input.GetKeyDown(keyList[x])) {
                            return true;
                        }
                    }
                    return false;
                }
            }

            /// <summary>Indicates whether the Input Button is $$released$$.</summary>
            /// <value>
            ///   <strong>true</strong> if the Input Button is $$released$$, otherwise <strong>false</strong>.</value>
            public override bool isReleased {
                get {
                    int n = keyList.Count;
                    for (int x = 0; x < n; x++) {
                        if (Input.GetKeyUp(keyList[x])) {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }
    }
}