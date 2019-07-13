using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController i;

    [SerializeField] private float m_turnSpeed = 1f;

    private Rigidbody2D m_rb;

    public void Collect(Collectible collectible) {
        Debug.Log("Collected");
    }

    private void Awake() {
        Singleton.Awake(this, ref i);
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        var cinema = GameObject.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        cinema.Follow = transform;
    }

    private void Update() {
        UpdateRotation();
        UpdateWindForce();
    }

    private void UpdateRotation() {
        float h = Input.GetAxisRaw("Horizontal");
        transform.Rotate(Vectors.Z(-h * m_turnSpeed * Time.deltaTime));
    }

    private void UpdateWindForce() {
        var wind = WindController.i;
        var force = wind.StregthDirection() * Time.deltaTime;
        m_rb.AddForce(force);
    }
}
