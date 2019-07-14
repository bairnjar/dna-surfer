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
    [SerializeField] private float m_accelleration = 1;
    [SerializeField] private float m_invincibilityTime = 2.0f;
    [SerializeField] private float m_speedBoost = 1000f;
    [SerializeField] private GameObject healthText;

    [SerializeField] private GameObject coinBar;
    [SerializeField] private int maxCoinBar;

    private Rigidbody2D m_rb;
    private SpriteRenderer m_spriteRenderer;

    private bool isInvincible;
    private float invincibleTimer;
    private bool m_isDropAnchor;
    private Vector3 m_startPosition;
    private Quaternion m_startRotation;
    private float m_startDrag;
    private int currentCoinBar;
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
                addToCoinBar(10);
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
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
        addToCoinBar(-1000);
        m_rb.rotation = 0f;
        m_rb.velocity = Vector2.zero;
        transform.position = start
            ? ScoreGateController.firstGate.transform.position
            : ScoreGateController.lastGate.transform.position;
        if (!start) {
            DNAStrandManager.i.SpawnNext();
        }
        transform.rotation = m_startRotation;
        m_isDropAnchor = false;
        isInvincible = false;
        ScoreManager.i.Reset();
    }

    private void addToCoinBar(int amount) {
        if (amount + currentCoinBar > maxCoinBar) {
            currentCoinBar = maxCoinBar;
        } else if (amount + currentCoinBar < 0) {
            currentCoinBar = 0;
        } else {
            currentCoinBar += amount;
        }
        Debug.Log("currentCoinBar = " + currentCoinBar);
        Debug.Log("maxCoinBar = " + maxCoinBar);
        float ccb = (float)currentCoinBar;
        float mcb = (float)maxCoinBar;

        var percent = ccb / mcb;
        var percent2 = Mathf.Lerp(0, 1, percent);
        Debug.Log("filamount= percent2 " + percent2);
        coinBar.GetComponent<Image>().fillAmount = percent2;


    }

    private void Update() {
        if (!FinishScreen.i.Visible()) {
            UpdateSailRotation();
            UpdateDropAnchor();
            UpdateAccelleration();
            UpdateInvicibilityTimer();
            UpdateBoost();
        } else {
            UpdateTryAgain();
        }
    }

    private void UpdateBoost() {
        if (Input.GetKey(KeyCode.X)) {
            UseBoost();
        } else {
            CinemachineController.i.cameraShake = 0;
        }
    }

    private void UpdateSailRotation() {
        float h = Input.GetAxisRaw("Horizontal");
        m_rb.AddTorque(-h * m_turnSpeed * Time.deltaTime);
    }

    private void UseBoost() {
        if (currentCoinBar > 0) {
            CinemachineController.i.cameraShake = 1;

            var playerDirection = Quaternion.Euler(0, 0, m_rb.rotation) * Vector2.up;
            m_rb.AddForce(playerDirection * m_speedBoost * Time.deltaTime);
            ScoreManager.i.Boost();
            addToCoinBar(-1);
        }

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
            m_spriteRenderer.color = Color.red;
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0) {
                isInvincible = false;
                m_spriteRenderer.color = Color.white;
            }
        }
    }

    private void UpdateAccelleration() {
        var playerDirection = Quaternion.Euler(0, 0, m_rb.rotation) * Vector2.up;
        m_rb.AddForce(playerDirection * m_accelleration * Time.deltaTime);
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
