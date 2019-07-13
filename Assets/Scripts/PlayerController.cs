using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController i;

    [SerializeField] private float m_turnSpeed = 1f;

    private Rigidbody2D m_rb;

    private void Awake() {
        i = this;
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        WindController.i.Register(m_rb);
    }

    private void Update() {
        UpdateRotation();
    }

    private void UpdateRotation() {
        float h = Input.GetAxisRaw("Horizontal");
        transform.Rotate(Vectors.Z(-h * m_turnSpeed * Time.deltaTime));
    }
}
