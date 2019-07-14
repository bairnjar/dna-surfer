using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public static PlayerController i;

    [HideInInspector] public int currentHealth;

    [SerializeField] private float m_turnSpeed = 1f;
    [SerializeField] private float m_dropAnchorSpeed = 1f;
    [SerializeField] private float m_dropAnchorFriction = 1f;
    [SerializeField] private int m_maxHealth = 5;
    [SerializeField] private int m_shallowsFriction = 4;
    [SerializeField] private float m_sweetSpotDamagePeriod = 1f;
    [SerializeField] private float m_windZeroRotation = 90f;
    [SerializeField] private float m_windBestRotation = 45f;
    [SerializeField] private float m_windSweetSpotRotation = 10f;
    [SerializeField] private float m_windSweetSpotSpeedMultiplierMin = 0.7f;
    [SerializeField] private float m_windSweetSpotSpeedMultiplierMax = 1.5f;
    [SerializeField] private float m_invincibilityTime = 2.0f;
    [SerializeField] private GameObject healthText;
    [SerializeField] private GameObject directionIndicator;

    private Rigidbody2D m_rb;

    private bool isInvincible;
    private float invincibleTimer;
    private bool m_isDropAnchor;
    private float m_frictionBeforeDropAnchor;
    private float m_previousDrag;
    private float m_sweetSpotDamageTimer;
    private Vector3 m_startPosition;

    public void Collect(Collectible collectible) {
        if (collectible.collectibleType == COLLECTIBLETYPE.HEALTHUP) {
            ChangeHealth(1);
        } else if (collectible.collectibleType == COLLECTIBLETYPE.HEALTHDOWN) {
            ChangeHealth(-1);
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        ChangeHealth(-1);
    }

    private void ChangeHealth(int amount) {
        if (amount < 0) {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = m_invincibilityTime;
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, m_maxHealth);
        if (currentHealth == 0) {
            FinishScreen.i.Lose();
            return;
        }
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
        m_startPosition = transform.position;
        Reset();
    }

    private void Reset() {
        currentHealth = m_maxHealth;
        m_rb.rotation = 0f;
        m_rb.velocity = Vector2.zero;
        transform.position = m_startPosition;
        m_isDropAnchor = false;
    }

    private void Update() {
        if (!FinishScreen.i.Visible()) {
            UpdateSailRotation();
            UpdateDropAnchor();
            UpdateWindForce();
            UpdateInvicibilityTimer();
        } else {
            UpdateTryAgain();
        }
    }

    private void UpdateSailRotation() {
        float h = Input.GetAxisRaw("Horizontal");
        m_rb.AddTorque(-h * m_turnSpeed * Time.deltaTime);
        directionIndicator.transform.rotation = Quaternion.Euler(0f, 0f, m_rb.rotation);
    }

    private void UpdateDropAnchor() {
        bool isDropAnchor = Input.GetButton("Submit");
        if (!isDropAnchor) {
            if (m_isDropAnchor) {
                // Stopped drop anchor.
                m_rb.drag = m_frictionBeforeDropAnchor;
                m_isDropAnchor = false;
            }
            return;
        } else {
            if (!m_isDropAnchor) {
                // Started drop anchor.
                m_frictionBeforeDropAnchor = m_rb.drag;
                m_rb.drag = m_dropAnchorFriction;
                m_isDropAnchor = true;
            }
        }

        // Drop anchor in the direction of the turn.
        float h = Input.GetAxisRaw("Horizontal");
        m_rb.AddTorque(-h * m_dropAnchorSpeed * Time.deltaTime);
    }

    private void UpdateInvicibilityTimer() {
        if (isInvincible) {
            GetComponentInChildren<SpriteRenderer>().color = Color.red;
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0) {
                isInvincible = false;
                GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }
        }
    }

    private void UpdateWindForce() {
        var wind = WindController.i;
        var windForce = wind.StregthDirection();
        var playerDirection = Quaternion.Euler(0, 0, m_rb.rotation) * Vector2.up;
        var rotationToWind = Quaternion.FromToRotation(wind.direction, playerDirection);
        var zRotation = Mathf.Min(
                Mathf.Abs(rotationToWind.eulerAngles.z),
                Mathf.Abs(360f - rotationToWind.eulerAngles.z));
        var playerForce = playerDirection * wind.strength;

        float sweetSpotFactor = 0f;
        float scale = 0f;
        if (zRotation < m_windSweetSpotRotation) {
            sweetSpotFactor = (m_windSweetSpotRotation - zRotation) / m_windSweetSpotRotation;
            scale = Mathf.Lerp(
                m_windSweetSpotSpeedMultiplierMin,
                m_windSweetSpotSpeedMultiplierMax,
                sweetSpotFactor);
            // Debug.LogFormat("sweet spot scale {0}", scale);

        } else if (zRotation < m_windBestRotation) {
            float zRotationOffset = zRotation - m_windSweetSpotRotation;
            float zRotationOffsetInto = m_windBestRotation - m_windSweetSpotRotation;
            scale = Mathf.Lerp(
                m_windSweetSpotSpeedMultiplierMin,
                1f,
                1f - ((zRotationOffsetInto - zRotationOffset) / zRotationOffsetInto));
            // Debug.LogFormat("best spot scale {0}", scale);
        } else if (zRotation < m_windZeroRotation) {
            float zRotationOffset = zRotation - m_windBestRotation;
            float zRotationOffsetInto = m_windZeroRotation - m_windBestRotation;
            scale = Mathf.Lerp(
                0f,
                1f,
                (zRotationOffsetInto - zRotationOffset) / zRotationOffsetInto);
            // Debug.LogFormat("zero spot scale {0}", scale);
        }

        CinemachineController.i.cameraShake = sweetSpotFactor;
        if (sweetSpotFactor > 0) {
            m_sweetSpotDamageTimer += Time.deltaTime;
            if (m_sweetSpotDamageTimer >= m_sweetSpotDamagePeriod) {
                ChangeHealth(-1);
                m_sweetSpotDamageTimer = 0f;
            }
        } else {
            m_sweetSpotDamageTimer = 0f;
        }

        m_rb.AddForce(scale * playerForce * Time.deltaTime);
    }

    private void UpdateTryAgain() {
        if (Input.GetButtonDown("Submit")) {
            Reset();
            FinishScreen.i.Hide();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.name.Equals("shallows")) {
            m_previousDrag = m_rb.drag;
            float newDrag = m_rb.drag + m_shallowsFriction;
            m_rb.drag = newDrag;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.name.Equals("shallows")) {

            m_rb.drag = m_previousDrag;

        }
    }
}
