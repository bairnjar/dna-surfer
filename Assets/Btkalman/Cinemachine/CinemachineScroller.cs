using UnityEngine;
using Cinemachine;

namespace Btkalman.Cinemachine
{
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class CinemachineScroller : CinemachineExtension, CinemachineReset.Listener
    {
        private float m_y = 0f;

        protected override void PostPipelineStageCallback(
                CinemachineVirtualCameraBase vcam,
                CinemachineCore.Stage stage,
                ref CameraState state,
                float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;
                if (pos.y < m_y)
                {
                    pos.y = m_y;
                }
                else
                {
                    m_y = pos.y;
                }
                state.RawPosition = pos;
            }
        }

        // CinemachineReset.Listener
        public void OnCinemachineReset()
        {
            m_y = 0;
        }
    }
}
