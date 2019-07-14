using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour {
    public static WindController i;

    [SerializeField] private float m_initialStrength = 1f;
    [SerializeField] private GameObject windArrow;

    public Vector2 direction { get; private set; }
    public float strength { get; private set; }

    public Vector2 StregthDirection() {
        return direction * strength;
    }

    private void Awake() {
        Singleton.Awake(this, ref i);
    }

    private void Start() {
        direction = Vector2.up;
        strength = m_initialStrength;
    }
}
