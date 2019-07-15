using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public static int numPlayers = 0;

    [HideInInspector] public int currentHealth { get; private set; }
    [HideInInspector] public int playerNumber { get; private set; }
    [HideInInspector] public string playerName { get; private set; }

    [SerializeField] private float m_turnSpeed = 1f;
    [SerializeField] private int m_shallowsFriction = 4;
    [SerializeField] private float m_accelleration = 1;
    [SerializeField] private float m_hillAccelleration = 1f;
    [Header("Boost")]
    [SerializeField] private float m_coinBoostValue = 1f;
    [SerializeField] private float m_speedBoostCostPerSecond = 1f;
    [SerializeField] private float m_maxSpeedBoost = 10f;
    [SerializeField] private float m_boostAccelleration = 1.5f;
    [Header("Rubber banding")]
    [SerializeField] private float m_rubberBandForce = 1f;
    [SerializeField] private float m_rubberBandAttenuationPerLevel = 0.5f;
    [Header("In distresss")]
    [SerializeField] private Color m_distressColorStart;
    [SerializeField] private Color m_distressColorEnd;
    [SerializeField] private float m_distressFlashPeriod;
    [Header("Components")]
    [SerializeField] private ParticleSystem m_backParticles;
    [SerializeField] private SpriteRenderer m_sprite;

    [SerializeField] private bool immediate_reset = false;

    private Rigidbody2D m_rb;

    private bool m_isDistress;
    private float m_distressFlashTimer;
    private Vector3 m_startPosition;
    private Quaternion m_startRotation;
    private float m_startDrag;
    private float m_availableBoost;
    private bool m_isBoosting;
    private bool m_isLose = false;
    private Dictionary<string, float> m_dragMods =
        new Dictionary<string, float>();
    private GameObject m_mirror;

    public void Collect(Collectible collectible) {
        switch (collectible.collectibleType) {
            case COLLECTIBLETYPE.COIN:
                ScoreManager.i.CollectCoin(playerNumber);
                SetAvailableBoost(m_availableBoost + m_coinBoostValue);
                break;
        }
    }

    public void SetHealth(int health) {
        ChangeHealth(health - currentHealth);
    }

    public void ChangeHealth(int amount) {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, 1);
        if (currentHealth == 0) {
            if (immediate_reset) {
                Reset();
                return;
            } else {
                if (playerNumber == 0 && ScoreManager.i.player1Alive) {
                    if (ScoreManager.i.player0Alive)
                    {
                        HUD.i.ActivateWipeoutText();
                    }
                    ScoreManager.i.player0Alive = false;
                    var cinema = GameObject.FindObjectOfType<Cinemachine.CinemachineTargetGroup>();
                    
                    cinema.RemoveMember(transform);
                    cinema.RemoveMember(m_mirror.transform);
                    return;
                } else if (playerNumber == 1 && ScoreManager.i.player0Alive) {

                    if (ScoreManager.i.player1Alive)
                    {
                        HUD.i.ActivateWipeoutText();
                    }

                    ScoreManager.i.player1Alive = false;
                    var cinema = GameObject.FindObjectOfType<Cinemachine.CinemachineTargetGroup>();
                    
                    cinema.RemoveMember(transform);
                    cinema.RemoveMember(m_mirror.transform);
                } else {
                    if (!FinishScreen.i.GetComponent<Canvas>().isActiveAndEnabled) {
                        FinishScreen.i.Lose();
                    }
                    return;
                }
            }
        }
        HUD.i.SetHealth(currentHealth, playerNumber);
    }

    private void Awake() {
        m_rb = GetComponent<Rigidbody2D>();
        playerNumber = numPlayers++;
        playerName = NameGenerator.Name();
    }

    private void Start() {
        m_mirror = GameObject.Instantiate(new GameObject(), transform);
        m_startRotation = Quaternion.Euler(0, 0, playerNumber == 0 ? -30f : 30f);
        m_startDrag = m_rb.drag;


        if (playerNumber == 1) {
            HUD.i.SetIsMultiplayer(true);
        }
        Reset(true);
    }

    private void Reset(bool start = false) {
        currentHealth = 1;
        var cinema = GameObject.FindObjectOfType<Cinemachine.CinemachineTargetGroup>();
        cinema.AddMember(transform, 1, 10);
        cinema.AddMember(m_mirror.transform, 1, 10);

        if (playerNumber == 0) {
            ScoreManager.i.player0Alive = true;
        } else if (playerNumber == 1) {
            ScoreManager.i.player1Alive = true;
        }

        ScoreManager.i.resetCurrentLevel();
        transform.position = ScoreGateController.StartPosition(playerNumber, start);
        transform.rotation = m_startRotation;
        m_rb.position = transform.position;
        m_rb.rotation = m_startRotation.z;
        m_rb.angularVelocity = 0;
        m_rb.velocity = Vector2.zero;
        m_distressFlashTimer = 0;
        m_isLose = false;
        SetAvailableBoost(0);
        m_sprite.color = Color.white;
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
        UpdateBoost();
        UpdateAccelleration();
        UpdateMirror();
        UpdateInDistress();
    }

    private void UpdateInDistress() {
        if (!m_isDistress) {
            return;
        }
        float lerp = m_distressFlashTimer / m_distressFlashPeriod;
        if (lerp > 1) {
            lerp = 2 - lerp;
        }
        m_sprite.color = Color.Lerp(m_distressColorEnd, m_distressColorEnd, lerp);
        m_distressFlashTimer += Time.deltaTime;
        if (m_distressFlashTimer >= 2 * m_distressFlashPeriod) {
            m_distressFlashTimer -= 2 * m_distressFlashPeriod;
        }
    }

    private void UpdateMirror() {
        m_mirror.transform.position = new Vector2(-transform.position.x, transform.position.y);
    }

    private void UpdateBoost() {
        if (InputButton("Boost") && m_availableBoost > 0) {
            ScoreManager.i.Boost();
            SetAvailableBoost(m_availableBoost - m_speedBoostCostPerSecond * Time.deltaTime);
            m_isBoosting = true;
            m_backParticles.startColor = Color.yellow;
            CinemachineController.i.cameraShake = 1;
        } else {
            m_isBoosting = false;
            m_backParticles.startColor = Color.white;
            CinemachineController.i.cameraShake = 0;
        }
    }

    public void InDistress() {
        m_isDistress = true;
        m_distressFlashTimer = 0;
    }

    public void EndDistress() {
        m_isDistress = false;
        m_sprite.color = Color.white;
        m_distressFlashTimer = 0;
    }

    private void SetAvailableBoost(float boost) {
        m_availableBoost = Mathf.Clamp(boost, 0, m_maxSpeedBoost);
        HUD.i.SetBoost(m_availableBoost / m_maxSpeedBoost, playerNumber);
    }

    private void UpdateSailRotation() {
        float h = InputAxisRaw("Horizontal");
        m_rb.AddTorque(-h * m_turnSpeed * Time.deltaTime);
    }

    private void UpdateAccelleration() {
        var levelMultiplier = ScoreManager.i.currentMultiplier;
        // Force from movement.
        float scale = m_accelleration;
        if (m_isBoosting) {
            scale *= m_boostAccelleration;
        }
        var playerDirection = Quaternion.Euler(0, 0, m_rb.rotation) * Vector2.up;
        Vector2 force = playerDirection * scale * Time.deltaTime * levelMultiplier;
        // Constant forward force.
        force += Vector2.up * m_hillAccelleration * Time.deltaTime * levelMultiplier;
        // Rubber band to center of track.
        Collider2D closestGuide = null;
        Vector2 closestPoint = Vector2.zero;
        var playerPos = m_rb.position;
        foreach (var guide in DNAStrandManager.i.GetTrackGuides(playerNumber)) {
            if (closestGuide == null) {
                closestGuide = guide;
                closestPoint = guide.ClosestPoint(playerPos);
            } else {
                var cp = guide.ClosestPoint(playerPos);
                if ((cp - playerPos).magnitude < (closestPoint - playerPos).magnitude) {
                    closestGuide = guide;
                    closestPoint = cp;
                }
            }
        }
        var forceVector = closestPoint - playerPos;
        force += new Vector2(forceVector.x, 0) * m_rubberBandForce * Time.deltaTime *
            Mathf.Pow(m_rubberBandAttenuationPerLevel, ScoreManager.i.currentLevel);
        m_rb.AddForce(force);
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