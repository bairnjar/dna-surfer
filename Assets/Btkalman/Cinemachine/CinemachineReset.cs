using UnityEngine;
using Cinemachine;

namespace Btkalman.Cinemachine {
    public class CinemachineReset {
        public interface Listener {
            void OnCinemachineReset();
        }

        public interface Before {
            void Reset();
        }

        private class BeforeImpl : Before {
            public CinemachineVirtualCameraBase vcam;
            public Transform follow;
            public Vector2 followPosition;

            public void Reset() {
                CinemachineReset.Reset(this);
            }
        }

        public static Before BeforeReset(int index) {
            var before = new BeforeImpl();
            before.vcam = CinemachineCore.Instance.GetVirtualCamera(index);
            if (!before.vcam) {
                return before;
            }

            before.follow = before.vcam.Follow;
            if (before.follow) {
                before.followPosition = before.follow.position;
            }
            return before;
        }

        public static void Reset(Before before) {
            var beforeImpl = (BeforeImpl)before;
            var vcam = beforeImpl.vcam;
            if (!vcam) {
                return;
            }

            foreach (var component in vcam.GetComponents<Component>()) {
                var componentListener = component as Listener;
                if (componentListener != null) {
                    componentListener.OnCinemachineReset();
                }
            }

            var follow = beforeImpl.follow;
            if (follow == vcam.Follow) {
                vcam.OnTargetObjectWarped(follow, -beforeImpl.followPosition);
            }
        }
    }
}