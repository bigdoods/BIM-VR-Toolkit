using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Immerseum {

    /// <summary>A set of static methods that can be applied to any geometric shape.</summary>
    public class Shapes {
        /// <summary>Determines whether the two points provided are at right angles to each other (i.e. generate a sharp corner).</summary>
        /// <param name="point">The <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> coordinates of the first point.</param>
        /// <param name="nextPoint">The <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> coordinates of the second point.</param>
        /// <param name="approximate">If <strong>true</strong>, defines a right angle as being 85 - 95 degrees (inclusive). If <strong>false</strong>, defines a right angle as being precisely 90
        /// degrees. Defaults to <strong>false</strong>.</param>
        /// <returns>
        ///   <strong>true</strong> if the points are at right angles to each other, otherwise <strong>false</strong>.</returns>
        public static bool isRightAngleTurn(Vector3 point, Vector3 nextPoint, bool approximate = false) {
            float degrees = Mathf.Abs(Vector3.Angle(point, nextPoint));
            if (approximate) {
                if (degrees >= 85 || degrees <= 95) {
                    return true;
                }
            } else {
                if (degrees == 90) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Given a sequence of three points, inserts a new point between the last two points.</summary>
        /// <param name="previousPoint">The <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> position of the first point in the sequence.</param>
        /// <param name="point">The <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> position of the point after which a new point should be
        /// inserted.</param>
        /// <param name="nextPoint">The <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> position of the point before which a new point should be
        /// inserted.</param>
        /// <param name="adjustmentFactor">The percentage of the distance between the point and previous point that the point should be moved back (towards the previous point), expressed as a float.</param>
        /// <returns>An array of three <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> positions, where the first position
        /// corresponds to <strong>point</strong> (adjusted so as to make room) and the third corresponds to the <strong>nextPoint</strong>.</returns>
        public static Vector3[] getBisectingVertices(Vector3 previousPoint, Vector3 point, Vector3 nextPoint, float adjustmentFactor = 0.05f) {
            Vector3[] vertexArray = new Vector3[3];
            float distanceToPrevious = Vector3.Distance(previousPoint, point);

            vertexArray[0] = point * (1 - (distanceToPrevious * adjustmentFactor));
            vertexArray[1] = point;
            vertexArray[2] = nextPoint;

            return vertexArray;
        }

        /// <summary>
        ///   <innovasys:widget type="Warning Box" layout="block" xmlns:innovasys="http://www.innovasys.com/widgets">
        ///     <innovasys:widgetproperty layout="block" name="Content">This functionality has <strong>not</strong> been robustly tested.</innovasys:widgetproperty>
        ///   </innovasys:widget>
        ///   <para>Given an array of <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> values representing the vertices of a Mesh, returns an array of triangle
        /// indices that correspond to a set of triangle faces that "fill" the plane.</para>
        /// </summary>
        /// <param name="vertices">An array of <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points that represent the vertices of a Mesh.</param>
        /// <returns>An array of triangle indices to use when procedurally generating a 2D (planar) mesh.</returns>
        public static int[] getDelaunayTriangulationArray(Vector3[] vertices) {
            int n = vertices.Length;
            float xmin = vertices[0].x;
            float zmin = vertices[0].z;
            float xmax = xmin;
            float zmax = zmin;

            for (int x = 0; x < n; x++) {
                if (vertices[x].x < xmin) {
                    xmin = vertices[x].x;
                } else if (vertices[x].x > xmax) {
                    xmax = vertices[x].x;
                }
                if (vertices[x].z < zmin) {
                    zmin = vertices[x].z;
                } else if (vertices[x].z > zmax) {
                    zmax = vertices[x].z;
                }
            }

            float dx = xmax - xmin;
            float dz = zmax - zmin;
            float dmax = (dx > dz) ? dx : dz;
            float xmid = (xmax + xmin) * 0.5f;
            float zmid = (zmax + zmin) * 0.5f;

            Vector2[] expandedXZ = new Vector2[3 + n];
            for (int x = 0; x < n; x++) {
                expandedXZ[x] = new Vector2(vertices[x].x, vertices[x].z);
            }

            expandedXZ[n] = new Vector2((xmid - 2 * dmax), (zmid - dmax));
            expandedXZ[n + 1] = new Vector2(xmid, (zmid + 2 * dmax));
            expandedXZ[n + 2] = new Vector2((xmid + 2 * dmax), (zmid - dmax));

            List<Triangle> triangleList = new List<Triangle>();
            triangleList.Add(new Triangle(n, n + 1, n + 2));
            for (int x = 0; x < n; x++) {
                List<Edge> edgeList = new List<Edge>();
                for (int c = 0; c < triangleList.Count; c++) {
                    if (inCircle(expandedXZ[x],
                                 expandedXZ[triangleList[c].p1],
                                 expandedXZ[triangleList[c].p2],
                                 expandedXZ[triangleList[c].p3])) {
                        edgeList.Add(new Edge(triangleList[c].p1, triangleList[c].p2));
                        edgeList.Add(new Edge(triangleList[c].p2, triangleList[c].p3));
                        edgeList.Add(new Edge(triangleList[c].p3, triangleList[c].p1));
                        triangleList.RemoveAt(c);
                        c--;
                    }
                }
                if (x >= n) {
                    continue;
                }
                for (int c = edgeList.Count - 2; c >= 0; c--) {
                    for (int d = edgeList.Count - 1; d >= c + 1; d--) {
                        if (edgeList[c].Equals(edgeList[d])) {
                            edgeList.RemoveAt(d);
                            edgeList.RemoveAt(c);
                            d--;
                            continue;
                        }
                    }
                }
                int edgesN = edgeList.Count;
                for (int c = 0; c < edgesN; c++) {
                    triangleList.Add(new Triangle(edgeList[c].p1, edgeList[c].p2, x));
                }
                edgeList.Clear();
                edgeList = null;
            }
            int trianglesN = triangleList.Count - 1;
            for (int x = trianglesN; x >= 0; x--) {
                if (triangleList[x].p1 >= n || triangleList[x].p2 >= n || triangleList[x].p3 >= n) {
                    triangleList.RemoveAt(x);
                }
            }
            triangleList.TrimExcess();
            int arraySize = triangleList.Count * 3;
            int[] triangleArray = new int[arraySize];
            for (int x = 0; x < triangleList.Count; x++) {
                triangleArray[3 * x] = triangleList[x].p1;
                triangleArray[3 * x + 1] = triangleList[x].p2;
                triangleArray[3 * x + 2] = triangleList[x].p3;
            }

            return triangleArray;
        }

        static bool inCircle(Vector2 p, Vector2 p1, Vector2 p2, Vector2 p3) {
            if (Mathf.Abs(p1.y - p2.y) < float.Epsilon && Mathf.Abs(p2.y - p3.y) < float.Epsilon) {
                return false;
            }
            float m1;
            float m2;
            float mx1;
            float mx2;
            float my1;
            float my2;
            float xc;
            float yc;
            if (Mathf.Abs(p2.y - p1.y) < float.Epsilon) {
                m2 = -(p3.x - p2.x) / (p3.y - p2.y);
                mx2 = (p2.x + p3.x) * 0.5f;
                my2 = (p2.y + p3.y) * 0.5f;
                xc = (p2.x + p1.x) * 0.5f;
                yc = m2 * (xc - mx2) + my2;
            } else if (Mathf.Abs(p3.y - p2.y) < float.Epsilon) {
                m1 = -(p2.x - p1.x) / (p2.y - p1.y);
                mx1 = (p1.x + p2.x) * 0.5f;
                my1 = (p1.y + p2.y) * 0.5f;
                xc = (p3.x + p2.x) * 0.5f;
                yc = m1 * (xc - mx1) + my1;
            } else {
                m1 = -(p2.x - p1.x) / (p2.y - p1.y);
                m2 = -(p3.x - p2.x) / (p3.y - p2.y);
                mx1 = (p1.x + p2.x) * 0.5f;
                mx2 = (p2.x + p3.x) * 0.5f;
                my1 = (p1.y + p2.y) * 0.5f;
                my2 = (p2.y + p3.y) * 0.5f;
                xc = (m1 * mx1 - m2 * mx2 + my2 - my1) / (m1 - m2);
                yc = m1 * (xc - mx1) + my1;
            }
            float dx = p2.x - xc;
            float dy = p2.y - yc;
            float rsqr = dx * dx + dy * dy;
            dx = p.x - xc;
            dy = p.y - yc;
            double drsqr = dx * dx + dy * dy;

            return (drsqr <= rsqr);
        }
    }

    /// <summary>Defines a Triangle as a set of indices corresponding to the vertex array of a Mesh.</summary>
    public struct Triangle {
        public int p1;
        public int p2;
        public int p3;
        public Triangle(int point1, int point2, int point3) {
            p1 = point1;
            p2 = point2;
            p3 = point3;
        }
    }

    /// <summary>An edge of a triangle.</summary>
    public class Edge {
        /// <summary>The index of the first point of the Edge.</summary>
        public int p1;
        /// <summary>The index of the second point of the Edge.</summary>
        public int p2;
        public Edge(int point1, int point2) {
            p1 = point1;
            p2 = point2;
        }
        public Edge() : this(0, 0) { }
        public bool Equals(Edge other) {
            return ((this.p1 == other.p2) && (this.p2 == other.p1)) || ((this.p1 == other.p1) && (this.p2 == other.p2));
        }
    }

    /// <summary>A Rectangle that is defined by a set of vertices.</summary>
    public class Rectangle {
        public Vector3 outerA;
        public Vector3 outerB;
        public Vector3 outerC;
        public Vector3 outerD;

        public Vector3 innerA;
        public Vector3 innerB;
        public Vector3 innerC;
        public Vector3 innerD;

        /// <summary>Collection of eight <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points that define the Outer and Inner boundaries of the Rectangle in
        /// clockwise order starting from top-left (user's perspective: forward-right).</summary>
        /// <value>Array of eight <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points that define the Outer and Inner boundary of the Rectangle. The first
        /// four values represent the Outer boundary. The second four represent the Inner.</value>
        public Vector3[] verticesArray {
            get {
                Vector3[] _verticesArray = new Vector3[8];
                outerA = _verticesArray[0];
                outerB = _verticesArray[1];
                outerC = _verticesArray[2];
                outerD = _verticesArray[3];
                innerA = _verticesArray[4];
                innerB = _verticesArray[5];
                innerC = _verticesArray[6];
                innerD = _verticesArray[7];

                return _verticesArray;
            }
        }

        /// <summary>Collection of four <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points that define the Outer boundary of the Rectangle in clockwise
        /// order starting from top-left (user's perspective: forward-right).</summary>
        /// <value>Array of four <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points that define the Outer boundary of the Rectangle.</value>
        public Vector3[] outerVerticesArray {
            get {
                Vector3[] _outerVerticesArray = new Vector3[4];
                outerA = _outerVerticesArray[0];
                outerB = _outerVerticesArray[1];
                outerC = _outerVerticesArray[2];
                outerD = _outerVerticesArray[3];

                return _outerVerticesArray;
            }
        }

        /// <summary>Collection of four <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points that define the Inner boundary of the Rectangle in clockwise order
        /// starting from top-left (user's perspective: forward-right).</summary>
        /// <value>Array of four <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points that define the Inner boundary of the
        /// Rectangle.</value>
        public Vector3[] innerVerticesArray {
            get {
                Vector3[] _innerVerticesArray = new Vector3[4];
                innerA = _innerVerticesArray[0];
                innerB = _innerVerticesArray[1];
                innerC = _innerVerticesArray[2];
                innerD = _innerVerticesArray[3];

                return _innerVerticesArray;
            }
        }

        public Rectangle(Vector3 outer_A, Vector3 outer_B, Vector3 outer_C, Vector3 outer_D, Vector3 inner_A, Vector3 inner_B, Vector3 inner_C, Vector3 inner_D) {
            outerA = outer_A;
            outerB = outer_B;
            outerC = outer_C;
            outerD = outer_D;

            innerA = inner_A;
            innerB = inner_B;
            innerC = inner_C;
            innerD = inner_D;
        }
        public Rectangle() : this(Vector3.zero, Vector3.right, new Vector3(1, 0, -1), Vector3.back, new Vector3(0.33f, 0, 0), new Vector3(0.66f, 0, 0), new Vector3(0.66f, 0, -0.66f), new Vector3(0.33f, 0, -0.66f)) { }
        public Rectangle(List<Vector3> vertices) : this(vertices.ToArray()) { }
        public Rectangle(Vector3[] vertices) {
            setVertices(vertices);
        }

        /// <summary>Extracts those vertices from a provided array that correspond to the corners of an Inner boundary rectangle.</summary>
        /// <param name="_verticesArray">An array of 8 vertices expressed as <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points in worldspace.</param>
        /// <returns>An array of four <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points in worldspace that represent the Inner corners of the boundary
        /// rectangle (starting from the top-left/player forward-left corner and proceeding clockwise).</returns>
        public static Vector3[] getInnerVertices(Vector3[] _verticesArray) {
            List<Vector3> _verticesList = splitInnerOuterVertices(_verticesArray, false);
            return _verticesList.ToArray();
        }
        /// <summary>Extracts those vertices from a provided array that correspond to the corners of an Outer boundary rectangle.</summary>
        /// <param name="_verticesArray">An array of 8 vertices expressed as <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points in worldspace.</param>
        /// <returns>An array of four <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points in worldspace that represent the Inner corners of the boundary
        /// rectangle (starting from the top-left/player forward-left corner and proceeding clockwise).</returns>
        public static Vector3[] getOuterVertices(Vector3[] _verticesArray) {
            List<Vector3> _verticesList = splitInnerOuterVertices(_verticesArray, true);
            return _verticesList.ToArray();
        }

        /// <summary>Sorts a collection of vertices into a collection that contains Outer vertices that define an exterior boundary and Inner vertices that define an interior
        /// boundary.</summary>
        /// <param name="_verticesArray">An array of 8 vertices expressed as <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points in worldspace.</param>
        /// <returns>A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of eight <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points in worldspace, where the first
        /// four values represent the Outer corners of the boundary rectangle (starting from the top-left/player forward-left corner and proceeding clockwise) and the
        /// second four values represent the Inner corners of the boundary rectangle.</returns>
        public static List<Vector3> getInnerOuterVerticesList(Vector3[] _verticesArray) {
            Vector3[] _innerVerticesList = getInnerVertices(_verticesArray);
            Vector3[] _outerVerticesList = getOuterVertices(_verticesArray);

            List<Vector3> _verticesList = new List<Vector3>();
            int n = _outerVerticesList.Length;
            int m = _innerVerticesList.Length;

            for (int x = 0; x < n; x++) {
                _verticesList.Add(_outerVerticesList[x]);
            }
            for (int x = 0; x < m; x++) {
                _verticesList.Add(_innerVerticesList[x]);
            }

            return _verticesList;
        }
        /// <summary>Sorts a collection of vertices into a collection that contains Outer vertices that define an exterior boundary and Inner vertices that define an interior
        /// boundary.</summary>
        /// <param name="_verticesArray">An array of 8 vertices expressed as <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points in worldspace.</param>
        /// <returns>An array of eight <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points in worldspace, where the first four
        /// values represent the Outer corners of the boundary rectangle (starting from the top-left/player forward-left corner and proceeding clockwise) and the second
        /// four values represent the Inner corners of the boundary rectangle.</returns>
        public static Vector3[] getInnerOuterVertices(Vector3[] _verticesArray) {
            List<Vector3> _verticesList = getInnerOuterVerticesList(_verticesArray);

            return _verticesList.ToArray();
        }

        static List<Vector3> splitInnerOuterVertices(Vector3[] _verticesArray, bool returnOuter) {
            int n = _verticesArray.Length;
            float[] xArray = new float[n];
            float[] zArray = new float[n];

            for (int c = 0; c < n; c++) {
                xArray[c] = _verticesArray[c].x;
                zArray[c] = _verticesArray[c].z;
            }

            float minX = xArray.Min();
            float maxX = xArray.Max();
            float minZ = zArray.Min();
            float maxZ = zArray.Max();

            List<Vector3> outerVerticesList = new List<Vector3>();
            List<Vector3> innerVerticesList = new List<Vector3>();

            for (int c = 0; c < n; c++) {
                if ((_verticesArray[c].x == minX || _verticesArray[c].x == maxX) && (_verticesArray[c].z == minZ || _verticesArray[c].z == maxZ)) {
                    outerVerticesList.Add(_verticesArray[c]);
                } else {
                    innerVerticesList.Add(_verticesArray[c]);
                }
            }

            if (returnOuter) {
                return outerVerticesList;
            } else {
                return innerVerticesList;
            }
        }

        /// <summary>Sets the values of the Rectangle's defining Outer and Inner vertex boundaries.</summary>
        /// <param name="vertices">A <see cref="!:https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx">List</see> of <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points in worldspace that represent the corner's of the Rectangle's Outer
        /// and Inner boundaries. The first four values must represent the Outer boundaries' corners in clockwise order starting from top-left (user forward-left). The
        /// second four values must represent the Inner boundaries' corners in clockwise order from top-left (user forward-left).</param>
        public void setVertices(List<Vector3> vertices) {
            setVertices(vertices.ToArray());
        }
        /// <summary>Sets the values of the Rectangle's defining Outer and Inner vertex boundaries.</summary>
        /// <param name="vertices">An array of <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points in worldspace that represent the corner's
        /// of the Rectangle's Outer and Inner boundaries. The first four values must represent the Outer boundaries' corners in clockwise order starting from top-left
        /// (user forward-left). The second four values must represent the Inner boundaries' corners in clockwise order from top-left (user forward-left).</param>
        public void setVertices(Vector3[] vertices) {
            int n = vertices.Length;
            if (n != 8) {
                throw new System.ArgumentException(("[ImmerseumSDK] Expected 8 vertices, but only received " + n.ToString()));
            }
            outerA = vertices[0];
            outerB = vertices[1];
            outerC = vertices[2];
            outerD = vertices[3];
            innerA = vertices[4];
            innerB = vertices[5];
            innerC = vertices[6];
            innerD = vertices[7];
        }

        /// <summary>Sorts an array of points in worldspace so that they in clockwise order starting from top-left (user's perspective: forward-left).</summary>
        /// <param name="_vertexArray">An array of four <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points.</param>
        /// <returns>An array of four <see cref="!:https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> points.</returns>
        public static Vector3[] sortFourPointsClockwise(Vector3[] _vertexArray) {
            float[] xArray = new float[4];
            float[] zArray = new float[4];

            for (int x = 0; x < 4; x++) {
                xArray[x] = _vertexArray[x].x;
                zArray[x] = _vertexArray[x].z;
            }

            float minX = xArray.Min();
            float maxX = xArray.Max();
            float minZ = zArray.Min();
            float maxZ = zArray.Max();

            Vector3[] sortedArray = new Vector3[4];
            for (int x = 0; x < 4; x++) {
                if (_vertexArray[x].x == minX && _vertexArray[x].z == maxZ) {
                    sortedArray[0] = _vertexArray[x];
                } else if (_vertexArray[x].x == maxX && _vertexArray[x].z == maxZ) {
                    sortedArray[1] = _vertexArray[x];
                } else if (_vertexArray[x].x == maxX && _vertexArray[x].z == minZ) {
                    sortedArray[2] = _vertexArray[x];
                } else {
                    sortedArray[3] = _vertexArray[x];
                }
            }

            return sortedArray;
        }

        /// <summary>Sorts both the Outer and Inner vertices that define the rectangle in clockwise order from top-left (user's perspective: forward-left).</summary>
        public void sortVerticesClockwise() {
            sortVerticesClockwise(true);
            sortVerticesClockwise(false);
        }
        /// <summary>Sorts either the Outer or Inner vertices that define the rectangle in clockwise order from top-left (user's perspective: forward-left).</summary>
        /// <param name="sortOuter">If <strong>true</strong>, sorts the Outer vertices. If <strong>false</strong>, sorts the Inner vertices.</param>
        public void sortVerticesClockwise(bool sortOuter) {
            float[] xArray = new float[4];
            float[] zArray = new float[4];

            for (int x = 0; x < 4; x++) {
                if (sortOuter) {
                    xArray[x] = outerVerticesArray[x].x;
                    zArray[x] = outerVerticesArray[x].z;
                } else {
                    xArray[x] = innerVerticesArray[x].x;
                    zArray[x] = innerVerticesArray[x].z;
                }
            }

            float minX = xArray.Min();
            float maxX = xArray.Max();
            float maxZ = zArray.Max();

            Vector3[] unsortedArray = new Vector3[4];
            if (sortOuter) {
                unsortedArray = outerVerticesArray;
            } else {
                unsortedArray = innerVerticesArray;
            }

            for (int x = 0; x < 4; x++) {
                if (unsortedArray[x].x == minX && unsortedArray[x].z == maxZ) {
                    if (sortOuter) {
                        outerA = unsortedArray[x];
                    } else {
                        innerA = unsortedArray[x];
                    }
                } else if (unsortedArray[x].x == maxX && unsortedArray[x].z == maxZ) {
                    if (sortOuter) {
                        outerB = unsortedArray[x];
                    } else {
                        innerB = unsortedArray[x];
                    }
                } else if (unsortedArray[x].x == minX && unsortedArray[x].z == maxZ) {
                    if (sortOuter) {
                        outerC = unsortedArray[x];
                    } else {
                        innerC = unsortedArray[x];
                    }
                } else {
                    if (sortOuter) {
                        outerD = unsortedArray[x];
                    } else {
                        innerD = unsortedArray[x];
                    }
                }
            }
        }

    }

}
