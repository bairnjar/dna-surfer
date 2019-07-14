using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public static int numPlayers = 0;

    [HideInInspector] public int currentHealth { get; private set; }
    [HideInInspector] public int playerNumber { get; private set; }


    [SerializeField] private float m_turnSpeed = 1f;
    [SerializeField] private float m_dropAnchorSpeed = 1f;
    [SerializeField] private float m_dropAnchorFriction = 1f;
    [SerializeField] private int m_maxHealth = 5;
    [SerializeField] private int m_shallowsFriction = 4;
    [SerializeField] private float m_accelleration = 1;
    [SerializeField] private float m_hillAccelleration = 1f;
    [SerializeField] private float m_invincibilityTime = 2.0f;
    [Header("Boost")]
    [SerializeField] private float m_coinBoostValue = 1f;
    [SerializeField] private float m_speedBoostCostPerSecond = 1f;
    [SerializeField] private float m_maxSpeedBoost = 10f;
    [SerializeField] private float m_boostAccelleration = 1.5f;

    [SerializeField] private bool immediate_reset = false;

    private Rigidbody2D m_rb;
    private SpriteRenderer m_spriteRenderer;

    private bool isInvincible;
    private bool isDistress;

    private float invincibleTimer;
    private bool m_isDropAnchor;
    private Vector3 m_startPosition;
    private Quaternion m_startRotation;
    private float m_startDrag;
    private float m_availableBoost;
    private bool m_isBoosting;
    private bool m_isLose = false;
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
                ScoreManager.i.CollectCoin(playerNumber);
                SetAvailableBoost(m_availableBoost + m_coinBoostValue);
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
            if (immediate_reset) {
                Reset();
                return;
            } else {
                FinishScreen.i.Lose();
                return;
            }
        }
        HUD.i.SetHealth(currentHealth, playerNumber);
    }

    private void Awake() {
        m_rb = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerNumber = numPlayers++;
    }

    private void Start() {
        var cinema = GameObject.FindObjectOfType<Cinemachine.CinemachineTargetGroup>();
        cinema.AddMember(transform, 1, 5);
        if (playerNumber == 1) {
            HUD.i.SetIsMultiplayer(true);
        }
        m_startRotation = Quaternion.Euler(0, 0, playerNumber == 0 ? -30f : 30f);
        m_startDrag = m_rb.drag;
        Reset(true);
    }

    private void Reset(bool start = false) {
        currentHealth = m_maxHealth;
        ScoreManager.i.resetCurrentLevel();
        m_rb.rotation = m_startRotation.z;
        m_rb.angularVelocity = 0;
        m_rb.velocity = Vector2.zero;
        transform.position = ScoreGateController.StartPosition(playerNumber, start);
        m_rb.position = transform.position;
        transform.rotation = m_startRotation;
        m_isDropAnchor = false;
        isInvincible = false;
        isDistress = false;
        m_isLose = false;
        SetAvailableBoost(0);
        ScoreManager.i.Reset();
    }

    private void Update() {
        bool lose = FinishScreen.i.Visible();
        if (lose) {
            // Lost.
            m_isLose = lose;
            UpdateTryAgain();
            return;
        }

        if (m_isLose) {
            m_isLose = lose;
            Reset();
            return;
        }

        UpdateSailRotation();
        // UpdateDropAnchor();
        UpdateInvicibilityTimer();
        UpdateBoost();
        UpdateAccelleration();
    }

    private void UpdateBoost() {
        if (InputButton("Boost") && m_availableBoost > 0) {
            ScoreManager.i.Boost();
            SetAvailableBoost(m_availableBoost - m_speedBoostCostPerSecond * Time.deltaTime);
            m_isBoosting = true;
            CinemachineController.i.cameraShake = 1;
        } else {
            m_isBoosting = false;
            if (!isDistress) {
                CinemachineController.i.cameraShake = 0;
            }
        }
    }

    public void InDistress() {
        isDistress = true;
        m_spriteRenderer.color = Color.red;
        CinemachineController.i.cameraShake = 2;
    }

    public void EndDistress() {
        isDistress = false;
        m_spriteRenderer.color = Color.white;
        CinemachineController.i.cameraShake = 0;
    }

    private void SetAvailableBoost(float boost) {
        m_availableBoost = Mathf.Clamp(boost, 0, m_maxSpeedBoost);
        HUD.i.SetBoost(m_availableBoost / m_maxSpeedBoost, playerNumber);
    }

    private void UpdateSailRotation() {
        float h = InputAxisRaw("Horizontal");
        m_rb.AddTorque(-h * m_turnSpeed * Time.deltaTime);
    }

    // private void UpdateDropAnchor() {
    //     bool isDropAnchor = Input.GetButton("Submit" + playerNumber);
    //     if (!isDropAnchor) {
    //         if (m_isDropAnchor) {
    //             // Stopped drop anchor.
    //             RestoreDrag("UpdateDropAnchor");
    //             m_isDropAnchor = false;
    //         }
    //         return;
    //     } else {
    //         if (!m_isDropAnchor) {
    //             // Started drop anchor.
    //             SetDrag("UpdateDropAnchor", m_dropAnchorFriction);
    //             m_isDropAnchor = true;
    //         }
    //     }

    //     // Drop anchor in the direction of the turn.
    //     float h = Input.GetAxisRaw("Horizontal" + playerNumber);
    //     m_rb.AddTorque(-h * m_dropAnchorSpeed * Time.deltaTime);
    // }

    private void UpdateInvicibilityTimer() {
        if (isInvincible) {
            m_spriteRenderer.color = Color.red;
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0) {
                isInvincible = false;
                m_spriteRenderer.color = Color.white;
            }
        }
    }

    private void UpdateAccelleration() {
        float scale = m_accelleration;
        if (m_isBoosting) {
            scale *= m_boostAccelleration;
        }
        var playerDirection = Quaternion.Euler(0, 0, m_rb.rotation) * Vector2.up;
        var force = playerDirection * scale * Time.deltaTime;
        force += Vector3.up * m_hillAccelleration * Time.deltaTime;
        m_rb.AddForce(force * ScoreManager.i.currentMultiplier);
    }

    private void UpdateTryAgain() {
        if (Input.GetButtonDown("Submit")) {
            FinishScreen.i.TryAgain();
            DNAStrandManager.i.Reset();
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

    private bool InputButton(string id) {
        if (playerNumber == 0) {
            return Input.GetButton(id);
        }
        return Input.GetButton(id + playerNumber);
    }

    private float InputAxisRaw(string id) {
        if (playerNumber == 0) {
            return Input.GetAxisRaw(id);
        }
        return Input.GetAxisRaw(id + playerNumber);
    }
}