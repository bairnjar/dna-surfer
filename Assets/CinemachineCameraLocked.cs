using UnityEngine;
using Cinemachine;

namespace Btkalman.Cinemachine
{
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class CinemachineCameraLocked : CinemachineExtension
    {
        [Tooltip("Position to lock each axis to")]
        public Vector3 m_positionLock = Vector3.zero;
        [Tooltip("Set to a non-zero value for each axis to enable Position Lock")]
        public Vector3 m_enablePositionLock = Vector3.zero;
        [Tooltip("Lock orthographic camera size relative to the screen aspect")]
        public float m_orthoLockWidth = 0f;

        float screenRatio = 0f;

        private void Start()
        {
            if (m_orthoLockWidth > 0)
            {
                screenRatio = (float)Screen.height / (float)Screen.width;
                UpdateOrthoSize();
            }
        }

        protected override void PostPipelineStageCallback(
                CinemachineVirtualCameraBase vcam,
                CinemachineCore.Stage stage,
                ref CameraState state,
                float deltaTime)
        {
            if (m_orthoLockWidth > 0)
            {
                float currentScreenRatio = (float)Screen.height / (float)Screen.width;
                if (currentScreenRatio != screenRatio)
                {
                    screenRatio = currentScreenRatio;
                    UpdateOrthoSize();
                }
            }

            if (stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;
                if (m_enablePositionLock.x != 0)
                {
                    pos.x = m_positionLock.x;
                }
                if (m_enablePositionLock.y != 0)
                {
                    pos.y = m_positionLock.y;
                }
                if (m_enablePositionLock.z != 0)
                {
                    pos.z = m_positionLock.z;
                }
                state.RawPosition = pos;
            }
        }

        private void UpdateOrthoSize()
        {
            GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 13.5f * screenRatio / 2.0f;
        }
    }
}
