using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public static PlayerController i;

    public int currentHealth;

    [SerializeField] private float m_turnSpeed = 1f;
    [SerializeField] private int m_maxHealth = 5;

    private Rigidbody2D m_rb;

    public GameObject healthText;
    public GameObject directionIndicator;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    public void Collect(Collectible collectible) {
        if (collectible.collectibleType == COLLECTIBLETYPE.HEALTHUP) {
            ChangeHealth(1);
        } else if (collectible.collectibleType == COLLECTIBLETYPE.HEALTHDOWN) {
            ChangeHealth(-1);
        } else if (collectible.collectibleType == COLLECTIBLETYPE.NONE) {
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        Debug.Log("COLLIDED");
        ChangeHealth(-1);
    }

    void ChangeHealth(int amount) {
        if (amount < 0) {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, m_maxHealth);
        Debug.Log(currentHealth + "/" + m_maxHealth);
        healthText.GetComponent<Text>().text = "HEALTH : " + currentHealth;
    }

    private void Awake() {
        Singleton.Awake(this, ref i);
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        var cinema = GameObject.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        cinema.Follow = transform;
        currentHealth = m_maxHealth;
    }

    private void Update() {
        if (!FinishScreen.i.Visible()) {
            UpdateRotation();
            UpdateWindForce();
            updateInvicibilityTimer();
        } else {
            UpdateTryAgain();
        }
    }

    private void UpdateRotation() {
        float h = Input.GetAxisRaw("Horizontal");
        m_rb.AddTorque(-h * m_turnSpeed * Time.deltaTime);
        directionIndicator.transform.rotation = Quaternion.Euler(0f, 0f, m_rb.rotation);
    }

    private void updateInvicibilityTimer() {
        if (isInvincible) {
            Debug.Log("invulnerable, turn red");
            GetComponentInChildren<SpriteRenderer>().color = Color.red;
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0) {
                isInvincible = false;
                Debug.Log("vulnerable, turn white");
                GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }
        }
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

    private void UpdateTryAgain() {
        if (Input.GetButtonDown("Submit")) {
            currentHealth = m_maxHealth;
            m_rb.rotation = 0f;
            m_rb.velocity = Vector2.zero;
            transform.position = Vector2.zero;
            FinishScreen.i.Hide();
        }
    }
}
