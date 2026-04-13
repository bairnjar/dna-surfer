using Btkalman.Unity.Util;
using Btkalman.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour {
    public static int numPlayers = 0;
    public static List<PlayerController> players = new List<PlayerController>();

    [HideInInspector] public int currentHealth { get; private set; }
    [HideInInspector] public int currentLives { get; private set; }
    [HideInInspector] public int playerNumber { get; private set; }
    [HideInInspector] public string playerName { get; private set; }
    public bool playerWon;


    [Header("Level Settings")]
    [SerializeField] private float m_turnSpeed = 1f;
    [SerializeField] private int m_shallowsFriction = 4;
    [SerializeField] private float m_accelleration = 1;
    [SerializeField] private float m_hillAccelleration = 1f;
    [SerializeField] private int startingLives = 3;

    [Header("Boost Death")]
    [SerializeField] private bool emptyBoostDie = true;
    [SerializeField] private float m_neutralBoostCostPerSecond = 0.01f;
    [SerializeField] private float m_boostDistressLevel = 3f;
    [SerializeField] private float m_emptySlowDown = 2f;


    [Header("Boost")]
    [SerializeField] private float m_coinBoostValue = 1f;
    [SerializeField] private float m_boostTime = 5f;
    [SerializeField] private float m_speedBoostCostPerSecond = 1f;
    [SerializeField] private float m_maxSpeedBoost = 10f;
    [SerializeField] private float m_boostAccelleration = 1.5f;
    [SerializeField] private float m_boostMagnetism = 3f;

    [Header("Blue Energy")]
    [SerializeField] private float m_coinBlueValue = 1f;
    [SerializeField] private float m_blueCostPerSecond = 0.02f;
    [SerializeField] private float m_maxBlueValue = 10f;
    [SerializeField] private float m_availableBlueValue = 0f;

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

    private int rightClickCount = 0;
    private int leftClickCount = 0;

    private Rigidbody2D m_rb;
    private bool m_uiReset = false;

    private bool m_fuelDistress = false;
    private float m_screenClickRotation = 0f;
    private bool leftClicked = false;
    private bool rightClicked = false;
    private bool useAllBoost = false;
    private bool m_isDistress;
    private float m_distressFlashTimer;
    private Vector3 m_startPosition;
    private Quaternion m_startRotation;
    private float m_startDrag;
    private float m_availableBoost;
    private bool m_isBoosting;
    private bool m_isVaccined;
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
            case COLLECTIBLETYPE.BLUE:
                //ScoreManager.i.CollectCoin(playerNumber);
                SetAvailableBlue(m_availableBlueValue + m_coinBlueValue);
                break;
            case COLLECTIBLETYPE.BOOST:
                pressUseAllBoost();
                break;
        }
    }

    public void exitLevel()
    {
        playerWon = true;
        if (!FinishScreen.i.GetComponent<Canvas>().isActiveAndEnabled)
        {
            FinishScreen.i.Win();
        }
        return;
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
                    if (ScoreManager.i.player0Alive) {
                        HUD.i.ActivateWipeoutText();
                    }
                    ScoreManager.i.player0Alive = false;
                    //  var cinema = GameObject.FindObjectOfType<Cinemachine.CinemachineTargetGroup>();

                    //  cinema.RemoveMember(transform);
                    //  cinema.RemoveMember(m_mirror.transform);
                    return;
                } else if (playerNumber == 1 && ScoreManager.i.player0Alive) {

                    if (ScoreManager.i.player1Alive) {
                        HUD.i.ActivateWipeoutText();
                    }

                    ScoreManager.i.player1Alive = false;
                    //  var cinema = GameObject.FindObjectOfType<Cinemachine.CinemachineTargetGroup>();

                    //  cinema.RemoveMember(transform);
                    //  cinema.RemoveMember(m_mirror.transform);
                } else {

                    if (!FinishScreen.i.GetComponent<Canvas>().isActiveAndEnabled) {
                        currentLives--;
                        Debug.Log("______");
                        Debug.Log("CurrentLives = " + currentLives);
                        Debug.Log("______");
                        HUD.i.setLives(currentLives);
                        if (currentLives > 0)
                        {
                            FinishScreen.i.Continue();
                        }
                        else
                        {
                            FinishScreen.i.Lose();
                        }


                    }
                    return;
                }
            }
        }
        HUD.i.SetHealth(currentHealth, playerNumber);
    }

    private void Awake() {
        m_rb = GetComponent<Rigidbody2D>();
        playerNumber = 0;
        playerName = NameGenerator.Name();
        players.Add(this);
    }

    private void Start() {
        DNAStrandManager.i.populateDictionary();
        DNAStrandManager.i.StartDNAStrand();
        m_mirror = GameObject.Instantiate(new GameObject(), transform);
        m_startRotation = Quaternion.Euler(0, 0, playerNumber == 0 ? 30f : -30f);
        m_isVaccined = false;
        m_startDrag = m_rb.linearDamping;
        currentLives = startingLives;
        HUD.i.setDistanceText(DNAStrandManager.i.distanceToCheckpoint);
        HUD.i.setLives(currentLives);
        if (playerNumber == 1) {
            HUD.i.SetIsMultiplayer(true);
        }
        HUD.i.SetName(playerName, playerNumber);
        Reset(true);

        // Testing - Reese
        if (PlayerPrefs.GetFloat("turnSpeed") == 0)
            Debug.LogError("turnSpeed PlayerPref value not set in main menu.", this);
        else
        {
            m_turnSpeed = PlayerPrefs.GetFloat("turnSpeed");
            Debug.Log("TurnSpeed: " + m_turnSpeed, this);
        }
    }

    private void Reset(bool start = false) {
        // var beforeCinemachineReset = CinemachineReset.BeforeReset(0);

        useAllBoost = false;
        m_fuelDistress = false;
        m_isVaccined = false;
        currentHealth = 1;
        DnaEater.i.Reset();
        leftClicked = false;
        rightClicked = false;
        //  var cinema = GameObject.FindObjectOfType<Cinemachine.CinemachineTargetGroup>();
        //  cinema.AddMember(transform, 1, 10);
        //  cinema.AddMember(m_mirror.transform, 1, 10);

        if (playerNumber == 0) {
            ScoreManager.i.player0Alive = true;
        } else if (playerNumber == 1) {
            ScoreManager.i.player1Alive = true;
        }

        ScoreManager.i.resetCurrentLevel();
        transform.position = ScoreGateController.StartPosition(playerNumber, start);
        transform.rotation = m_startRotation;

        //use moveto instead of 
        m_rb.MovePosition(ScoreGateController.StartPosition(playerNumber, start));
        m_rb.MoveRotation(m_startRotation);
        m_rb.angularVelocity = 0;
        m_rb.linearVelocity = Vector2.zero;
        m_distressFlashTimer = 0;
        m_isLose = false;
        SetAvailableBoost(m_maxSpeedBoost);
        SetAvailableBlue(0);
        m_sprite.color = Color.white;
        m_isDistress = false;
        ScoreManager.i.Reset();
        m_uiReset = false;
        //beforeCinemachineReset.Reset();

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
            if (currentLives <= 0)
            {
                currentLives = startingLives;
                HUD.i.setLives(currentLives);
            }
            return;
        }

        UpdateSailRotation();
        UpdateBoost();
        UpdateAccelleration();
        UpdateMirror();
        UpdateInDistress();
    }

    public void setRotation(float h)
    {
        m_screenClickRotation = h;
    }

    public void pressLeftButton()
    {
        leftClicked = true;
        leftClickCount++;
        // HUD.i.SetLeftClickText(leftClickCount);
    }

    public void pressUseAllBoost()
    {
        useAllBoost = true;
        StartCoroutine(BoostForSeconds());
    }

    public void releaseLeftButton()
    {
        leftClicked = false;
    }

    public void pressRightButton()
    {
        rightClicked = true;
        rightClickCount++;
        //HUD.i.SetRightClickText(rightClickCount);
    }

    public void releaseRightButton()
    {
        rightClicked = false;
    }


    private void UpdateInDistress() {
        if (!m_isDistress) {
            // CinemachineController.i.cameraShake = 0;

            return;
        }
        //CinemachineController.i.cameraShake = 1;

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

        if (m_availableBoost <= 0)
        {

            useAllBoost = false;

            if (emptyBoostDie)
            {
                SetHealth(0);
            }

        }
        else
        {
            if (DNAStrandManager.i.currentSafe)
            {
                SetAvailableBoost(m_availableBoost + m_neutralBoostCostPerSecond);
            }
            else
            {
                SetAvailableBoost(m_availableBoost - m_neutralBoostCostPerSecond);
            }

        }

        if (m_availableBoost <= m_boostDistressLevel)
        {
            m_fuelDistress = true;
            InDistress();
        }
        else if (m_fuelDistress)
        {
            m_fuelDistress = false;
            EndDistress();
        }

        if (m_isVaccined)
        {
            SetAvailableBlue(m_availableBlueValue - m_blueCostPerSecond);
        }


        if ((InputButton("Boost") || useAllBoost) && m_availableBoost > 0) {
            ScoreManager.i.Boost();
            // SetAvailableBoost(m_availableBoost - m_speedBoostCostPerSecond * Time.deltaTime);
            m_isBoosting = true;

            m_backParticles.emissionRate = 20f;
            //m_backParticles.startColor = Color.yellow;
            //CinemachineController.i.cameraShake = 1;
            Debug.Log("dooper1");
            // DNAStrandManager.i.ActivateWalls(true);
        } else if (!m_isVaccined) {
            m_isBoosting = false;
            m_backParticles.emissionRate = 5f;
            //m_backParticles.startColor = Color.white;
            //CinemachineController.i.cameraShake = 0;
            //DNAStrandManager.i.ActivateWalls(false);
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

    private void SetAvailableBlue(float blue)
    {
        m_availableBlueValue = Mathf.Clamp(blue, 0, m_maxBlueValue);
        if (m_isVaccined && blue <= 0)
        {
            m_isVaccined = false;
            // DNAStrandManager.i.ActivateWalls(false);
        }
        if (m_availableBlueValue / m_maxBlueValue == 1)
        {
            currentLives++;
            HUD.i.setLives(currentLives);
            m_availableBlueValue = 0f;
        }
        HUD.i.SetBlue(m_availableBlueValue / m_maxBlueValue, playerNumber);
    }

    private void UpdateSailRotation() {

        float h = InputAxisRaw("Horizontal");
        if (leftClicked && rightClicked) {
            h = 0;
        }
        else if (leftClicked)
        {
            h = -1;
        } else if (rightClicked)
        {
            h = 1;
        }
        else
        {
            h = InputAxisRaw("Horizontal");
        }
        // h = 0;
        // h = 
        //        Debug.Log("horizontalvalue =" + h);
        m_rb.AddTorque(-h * m_turnSpeed * Time.deltaTime);
    }

    private void UpdateAccelleration() {
        var levelMultiplier = DNAStrandManager.i.currentSpeedMultiplier;
        // Force from movement.
        float scale = m_accelleration;
        float curBoostMagnetism;




        if (m_isBoosting) {
            scale *= m_boostAccelleration;
            curBoostMagnetism = m_boostMagnetism;

        }
        else
        {
            curBoostMagnetism = 1;
        }

        var playerDirection = Quaternion.Euler(0, 0, m_rb.rotation) * Vector2.up;

        Vector2 force = playerDirection * scale * Time.deltaTime * levelMultiplier;




        force += Vector2.up * m_hillAccelleration * Time.deltaTime * levelMultiplier;


        // Rubber band to center of track.
        Collider2D closestGuide = null;
        Vector2 closestPoint = Vector2.zero;

        var playerPos = m_rb.position;
        //Debug.Log("PlayerPos without Offset is: " + m_rb.position.x + ", " + playerPos.y);
        //Debug.Log("PlayerPos with Offset is: " + playerPos.x + ", " + playerPos.y);
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
        force += new Vector2(forceVector.x, 0) * m_rubberBandForce * Time.deltaTime * curBoostMagnetism *
            Mathf.Pow(m_rubberBandAttenuationPerLevel, DNAStrandManager.i.currentRubberBandReduction);


        m_rb.AddForce(force);
    }



    public void UpdateTryAgain() {
        if (Input.GetButtonDown("Submit") || m_uiReset == true) {
            FinishScreen.i.TryAgain();
            DNAStrandManager.i.Reset();
        }
    }

    public void UITryAgain()
    {
        if (playerWon)
        {
            string name = SceneManager.GetActiveScene().name;
            m_isVaccined = false;
            SceneManager.UnloadSceneAsync(name);

            SceneManager.LoadScene("Instructions1P2", LoadSceneMode.Single);
        }
        else
        {
            m_uiReset = true;
        }
    }

    public void LoadMenu()
    {
        m_isVaccined = false;
        SceneManager.LoadScene("MenuScreen", LoadSceneMode.Single);
    }

    public void turnOnVaccine()
    {
        Debug.Log("dooper2");
        //DNAStrandManager.i.ActivateWalls(true);
        m_isVaccined = true;
    }

    public bool getVaccine()
    {
        return m_isVaccined;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.name.Equals("shallows")) {
            SetDrag("shallows", m_rb.linearDamping + m_shallowsFriction);
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
            if (m_rb.linearDamping < kv.Value) {
                m_rb.linearDamping = kv.Value;
            }
        }
    }

    private void RestoreDrag(string key) {
        m_dragMods.Remove(key);
        if (m_dragMods.Count == 0) {
            m_rb.linearDamping = m_startDrag;
            return;
        }
        foreach (var kv in m_dragMods) {
            if (m_rb.linearDamping < kv.Value) {
                m_rb.linearDamping = kv.Value;
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

    private IEnumerator BoostForSeconds()
    {
        yield return new WaitForSeconds(m_boostTime);
        useAllBoost = false;
    }
    
    private void OnDestroy()
    {
        players.Clear();
        Destroy(gameObject);
    }
}