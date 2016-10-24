using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Immerseum {
    public class Utilities {

        public static List<Vector3> convertArrayToList(Vector3[] array) {
            List<Vector3> list = new List<Vector3>();

            int n = array.Length;
            if (n > 0) {
                for (int x = 0; x < n; x++) {
                    list.Add(array[x]);
                }
            }

            return list;
        }

    }
}
