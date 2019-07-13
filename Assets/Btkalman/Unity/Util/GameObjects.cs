using UnityEngine;

namespace Btkalman.Unity.Util {
    public class GameObjects {
        public static T GetComponentWithTagInChildren<T>(GameObject obj, string tag) {
            foreach (var t in obj.GetComponentsInChildren<T>()) {
                if ((t as Component).tag == tag) {
                    return t;
                }
            }
            return default(T);
        }

        public static T GetComponentWithTagInChildren<T>(Component c, string tag) {
            return GetComponentWithTagInChildren<T>(c.gameObject, tag);
        }
    }
}