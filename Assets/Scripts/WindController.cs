using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour {
    public static WindController i;

    [SerializeField] private float m_strength = 1f;
    [SerializeField] private float m_rotateSpeed = 1f;

    private HashSet<Rigidbody2D> m_rbs = new HashSet<Rigidbody2D>();
    private Vector2 m_direction;

    public void Register(Rigidbody2D rb) {
        m_rbs.Add(rb);
    }

    private void Awake() {
        Singleton.Awake(this, ref i);
    }

    private void Start() {
        m_direction = Vector2.up;
    }

    private void Update() {
        var rotation = Quaternion.Euler(0f, 0f, m_rotateSpeed * Time.deltaTime);
        m_direction = rotation * m_direction;
        float force = m_strength * Time.deltaTime;
        foreach (var rb in m_rbs) {
            rb.AddForce(m_direction * force);
        }
    }
}
