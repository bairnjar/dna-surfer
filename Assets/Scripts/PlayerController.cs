using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public static PlayerController i;

    public int currentHealth;

    [SerializeField] private float m_turnSpeed = 1f;
    [SerializeField] private float m_dropAnchorSpeed = 1f;
    [SerializeField] private float m_dropAnchorFriction = 1f;
    [SerializeField] private int m_maxHealth = 5;
    [SerializeField] private int m_shallowsFriction = 4;
    private float m_previousDrag = 2;

    private Rigidbody2D m_rb;

    public GameObject healthText;
    public GameObject directionIndicator;

    private float timeInvincible = 2.0f;
    private bool isInvincible;
    private float invincibleTimer;
    private bool m_isDropAnchor;
    private float m_frictionBeforeDropAnchor;

    public void Collect(Collectible collectible) {
        if (collectible.collectibleType == COLLECTIBLETYPE.HEALTHUP) {
            ChangeHealth(1);
        } else if (collectible.collectibleType == COLLECTIBLETYPE.HEALTHDOWN) {
            ChangeHealth(-1);
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        ChangeHealth(-1);
    }

    void ChangeHealth(int amount) {
        if (!FinishScreen.i.Visible()) {
            return;
        }
        if (amount < 0) {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
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
        //Reset();
    }

    private void Reset() {
        currentHealth = m_maxHealth;
        m_rb.rotation = 0f;
        m_rb.velocity = Vector2.zero;
        transform.position = Vector2.zero;
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
                Debug.Log("raise anchor");
                // Stopped drop anchor.
                m_rb.drag = m_frictionBeforeDropAnchor;
                m_isDropAnchor = false;
            }
            return;
        } else {
            if (!m_isDropAnchor) {
                // Started drop anchor.
                Debug.Log("drop anchor");
                m_frictionBeforeDropAnchor = m_rb.drag;
                m_rb.drag = m_dropAnchorFriction;
                m_isDropAnchor = true;
            }
        }

        // Drop anchor in the direction of the turn.
        float h = Input.GetAxisRaw("Horizontal");
        Debug.Log(h);
        m_rb.AddTorque(-h * m_dropAnchorSpeed * Time.deltaTime);
    }

    private void UpdateInvicibilityTimer() {
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
            Reset();
            FinishScreen.i.Hide();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        //Debug.Log("You're in: "+collision.name);
        if (collision.name.Equals("shallows"))
        {
            m_previousDrag = m_rb.drag;
            float newDrag = m_rb.drag + m_shallowsFriction;
            m_rb.drag = newDrag;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        //Debug.Log("You're in: "+collision.name);
        if (collision.name.Equals("shallows"))
        {

            m_rb.drag = m_previousDrag;
                
        }
    }

}
