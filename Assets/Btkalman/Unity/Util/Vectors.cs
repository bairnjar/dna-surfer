using UnityEngine;

namespace Btkalman.Unity.Util {
    public class Vectors {
        public static Vector3 Abs(Vector3 position) {
            return new Vector3(
                Mathf.Abs(position.x),
                Mathf.Abs(position.y),
                Mathf.Abs(position.z));
        }

        public static Vector2 Round(Vector2 position) {
            return new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
        }

        public static Vector2 Quantize(Vector2 position, float accuracy) {
            return new Vector2(
                Floats.Quantize(position.x, accuracy),
                Floats.Quantize(position.y, accuracy));
        }

        public static int Compare(Vector2 a, Vector2 b) {
            int compare = Floats.Compare(a.x, b.x);
            if (compare == 0) {
                compare = Floats.Compare(a.y, b.y);
            }
            return compare;
        }

        public static Vector3 X(float x, Vector3 v) {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 X(float x) {
            return new Vector3(x, 0, 0);
        }

        public static Vector3 Y(float y, Vector3 v) {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 Y(float y) {
            return new Vector3(0, y, 0);
        }

        public static Vector3 Z(float z, Vector3 v) {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector3 Z(float z) {
            return new Vector3(0, 0, z);
        }
    }
}