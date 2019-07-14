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
    [SerializeField] private float m_speedBoost = 1000f;
    [SerializeField] private GameObject healthText;
    [SerializeField] private GameObject directionIndicator;
    [SerializeField] private GameObject sweetSpotIndicator;

    private Rigidbody2D m_rb;

    private bool isInvincible;
    private bool isBoostAvailable;
    private float invincibleTimer;
    private bool m_isDropAnchor;
    private float m_sweetSpotDamageTimer;
    private Vector3 m_startPosition;
    private Quaternion m_startRotation;
    private float m_startDrag;
    private Dictionary<string, float> m_dragMods =
        new Dictionary<string, float>();

    public void Collect(Collectible collectible) {
        switch (collectible.collectibleType) {
            case COLLECTIBLETYPE.HEALTHUP:
                ChangeHealth(1);
                break;
            case COLLECTIBLETYPE.HEALTHDOWN:
                ChangeHealth(-1);
                break;
            case COLLECTIBLETYPE.COIN:
                ScoreManager.i.CollectCoin();
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        ChangeHealth(-1);
    }

    public void SetHealth(int health) {
        ChangeHealth(health - currentHealth);
    }

    public void ChangeHealth(int amount) {
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
        m_startRotation = Quaternion.Euler(0, 0, 60f);
        m_startDrag = m_rb.drag;
        Reset(true);
    }

    private void Reset(bool start = false) {
        currentHealth = m_maxHealth;
        m_rb.rotation = 0f;
        m_rb.velocity = Vector2.zero;
        transform.position = start
            ? ScoreGateController.firstGate.transform.position
            : ScoreGateController.lastGate.transform.position;
        transform.rotation = m_startRotation;
        m_isDropAnchor = false;
        isInvincible = false;
        ScoreManager.i.Reset();
    }

    private void Update() {
        if (!FinishScreen.i.Visible()) {
            UpdateSailRotation();
            UpdateDropAnchor();
            UpdateWindForce();
            UpdateInvicibilityTimer();

            if (Input.GetKey(KeyCode.X) && isBoostAvailable) {
                print("x key was pressed");
                UseBoost();
            }


        } else {
            UpdateTryAgain();
        }
    }

    private void UpdateSailRotation() {
        float h = Input.GetAxisRaw("Horizontal");
        m_rb.AddTorque(-h * m_turnSpeed * Time.deltaTime);
        directionIndicator.transform.rotation = Quaternion.Euler(0f, 0f, m_rb.rotation);
    }

    private void UseBoost() {
        var playerDirection = Quaternion.Euler(0, 0, m_rb.rotation) * Vector2.up;
        m_rb.AddForce(playerDirection * m_speedBoost * Time.deltaTime);
        ScoreManager.i.Boost();
    }

    private void UpdateDropAnchor() {
        bool isDropAnchor = Input.GetButton("Submit");
        if (!isDropAnchor) {
            if (m_isDropAnchor) {
                // Stopped drop anchor.
                RestoreDrag("UpdateDropAnchor");
                m_isDropAnchor = false;
            }
            return;
        } else {
            if (!m_isDropAnchor) {
                // Started drop anchor.
                SetDrag("UpdateDropAnchor", m_dropAnchorFriction);
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
            sweetSpotIndicator.SetActive(false);
            isBoostAvailable = false;
            sweetSpotFactor = (m_windSweetSpotRotation - zRotation) / m_windSweetSpotRotation;
            scale = Mathf.Lerp(
                m_windSweetSpotSpeedMultiplierMin,
                m_windSweetSpotSpeedMultiplierMax,
                sweetSpotFactor);
            // Debug.LogFormat("sweet spot scale {0}", scale);

        } else if (zRotation < m_windBestRotation) {
            sweetSpotIndicator.SetActive(true);
            isBoostAvailable = true;
            float zRotationOffset = zRotation - m_windSweetSpotRotation;
            float zRotationOffsetInto = m_windBestRotation - m_windSweetSpotRotation;
            scale = Mathf.Lerp(
                m_windSweetSpotSpeedMultiplierMin,
                1f,
                1f - ((zRotationOffsetInto - zRotationOffset) / zRotationOffsetInto));
            // Debug.LogFormat("best spot scale {0}", scale);
        } else if (zRotation < m_windZeroRotation) {
            sweetSpotIndicator.SetActive(false);
            isBoostAvailable = false;

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
            SetDrag("shallows", m_rb.drag + m_shallowsFriction);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.name.Equals("shallows")) {
            RestoreDrag("shallows");
        }
    }

    private void SetDrag(string key, float val) {
        m_dragMods[key] = val;
        foreach (var kv in m_dragMods) {
            if (m_rb.drag < kv.Value) {
                m_rb.drag = kv.Value;
            }
        }
    }

    private void RestoreDrag(string key) {
        m_dragMods.Remove(key);
        if (m_dragMods.Count == 0) {
            m_rb.drag = m_startDrag;
            return;
        }
        foreach (var kv in m_dragMods) {
            if (m_rb.drag < kv.Value) {
                m_rb.drag = kv.Value;
            }
        }
    }
}
