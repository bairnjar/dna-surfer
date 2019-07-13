using UnityEngine;

namespace Btkalman.Unity.Util {
    public class Floats {
        public static float Quantize(float f, float accuracy) {
            return accuracy * Mathf.Round(f / accuracy);
        }

        public static int Compare(float f, float g) {
            return f == g ? 0 : f < g ? -1 : 1;
        }
    }
}