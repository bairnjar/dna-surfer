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
        m_rb.AddTorque(-h * m_turnSpeed * Time.deltaTime);
    }

    private void UpdateWindForce() {
        // Wind translated into our fake sailing simulation where:
        // * rotation of the boat is where the boat will sail.
        // * force on the boat is scaled such that 45deg to the
        // wind is max force, parallel with the wind is 50%
        // force, and perpendicular is 0%. We could use some
        // fancy math for this but linear is easier.
        var wind = WindController.i;
        var windForce = wind.StregthDirection();
        var playerDirection = (Quaternion.Euler(0, 0, m_rb.rotation) * Vector2.up).normalized;
        var rotationToWind = Quaternion.FromToRotation(wind.direction, playerDirection);
        var zRotation = Mathf.Min(
                Mathf.Abs(rotationToWind.eulerAngles.z),
                Mathf.Abs(360f - rotationToWind.eulerAngles.z));
        var playerForce = playerDirection * wind.strength;
        if (zRotation <= 45f) {
            playerForce *= 1f - ((45f - zRotation) / 90f);
        } else if (zRotation <= 90f) {
            playerForce *= 1f - ((45f - zRotation) / 45f);
        } else {
            playerForce = Vector2.zero;
        }
        m_rb.AddForce(playerForce * Time.deltaTime);
    }
}
