using UnityEngine;

namespace Btkalman.Unity.Util {
    public class Singleton {
        public static void Awake<T>(T newInstance, ref T preInstance)
                where T : MonoBehaviour {
            if (preInstance == null) {
                preInstance = newInstance;
            } else if (preInstance != newInstance) {
                GameObject.Destroy(preInstance);
                preInstance = newInstance;
            }
        }
    }
}