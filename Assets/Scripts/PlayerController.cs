using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController i;

    [SerializeField] private float m_turnSpeed = 1f;

    private Rigidbody2D m_rb;

    public int currentHealth;
    public int maxHealth = 5;

    public void Collect(Collectible collectible)
    {
        Debug.Log("Collected");
        if (collectible.collectibleType == COLLECTIBLETYPE.HEALTHUP)
        {
            ChangeHealth(1);
        }
        else if (collectible.collectibleType == COLLECTIBLETYPE.HEALTHDOWN)
        {
            ChangeHealth(-1);
        }
        else if (collectible.collectibleType == COLLECTIBLETYPE.NONE)
        {
        }
    }

    void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    private void Awake() {
        Singleton.Awake(this, ref i);
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        var cinema = GameObject.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        cinema.Follow = transform;
        currentHealth = maxHealth;
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
