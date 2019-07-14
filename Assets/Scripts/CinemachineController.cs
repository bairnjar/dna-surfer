using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineController : MonoBehaviour {
    public static CinemachineController i;

    [SerializeField] private float m_maxCameraShake = 0.1f;
    [SerializeField] private float m_cameraShakeDelay = 0.05f;

    private CinemachineCameraOffset m_cameraOffset;

    [HideInInspector] public float cameraShake;

    private float m_cameraShakeTimer = 0f;

    private void Awake() {
        Singleton.Awake(this, ref i);
        m_cameraOffset = GetComponentInChildren<CinemachineCameraOffset>();
    }

    private void Start() {
        cameraShake = 0;
    }

    private void Update() {
        UpdateCameraShake();
    }

    private void UpdateCameraShake() {
        if (cameraShake == 0) {
            m_cameraOffset.m_Offset = Vector3.zero;
            return;
        }

        m_cameraShakeTimer += Time.deltaTime;
        if (m_cameraShakeTimer < m_cameraShakeDelay) {
            return;
        }

        m_cameraShakeTimer = 0f;
        float cameraShakeAmount = cameraShake * m_maxCameraShake;
        m_cameraOffset.m_Offset = new Vector3(
            Random.Range(0f, cameraShakeAmount),
            Random.Range(0f, cameraShakeAmount),
            0f);
        Debug.Log(m_cameraOffset.m_Offset);
    }
}