using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Immerseum {
    namespace VRSimulator {
        public class SplineTester : MonoBehaviour {

            Color color = new Color(0, 255, 255, 255);
            float borderThickness = 0.15f;

            void Start () {
                Vector3[] verticesArray = new Vector3[4];
                verticesArray[0] = new Vector3(1.35f, 1f, 0.15f);
                verticesArray[1] = new Vector3(-1.35f, 1f, -1.15f);
                verticesArray[2] = new Vector3(-1.35f, 1f, 1.15f);
                verticesArray[3] = new Vector3(1.35f, 1f, 1.15f);

                renderLine(verticesArray);
            }

            protected void renderLine (Vector3[] verticesArray) {
                List<Vector3> _vertexList = new List<Vector3>();
                int n = verticesArray.Length;
                for (int x = 0; x < n; x++) {
                    _vertexList.Add(verticesArray[x]);
                }
                if (verticesArray[n - 1] != verticesArray[0]) {
                    _vertexList.Add(verticesArray[0]);
                }

                for (int x = 1; x < n; x++) {
                    bool isRightAngleTurn = Shapes.isRightAngleTurn(_vertexList[x], _vertexList[x + 1]);
                    Vector3[] bisectingVertexArray = new Vector3[3];
                    if (isRightAngleTurn) {
                        bisectingVertexArray = Shapes.getBisectingVertices(_vertexList[x - 1], _vertexList[x], _vertexList[x + 1]);
                        _vertexList[x] = bisectingVertexArray[0];
                        _vertexList.Insert(x, bisectingVertexArray[1]);
                    }
                }

                int m = _vertexList.Count;
                Vector3[] _finalVertexArray = _vertexList.ToArray();

                LineRenderer playAreaLineRenderer = gameObject.AddComponent<LineRenderer>();
                playAreaLineRenderer.SetColors(color, color);
                playAreaLineRenderer.SetWidth(borderThickness, borderThickness);
                playAreaLineRenderer.SetVertexCount(m);
                playAreaLineRenderer.SetPositions(_finalVertexArray);

            }

        }
    }
}
