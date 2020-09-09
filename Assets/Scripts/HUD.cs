using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    public static HUD i;

    [System.Serializable]
    public struct Player {
        public Text scoreText;
        public Image boostImage;
        public Image boostImageBackground;
        public Text nameText;
    }

 

    public Text distanceToCheckpoint;

    public List<Image> lives;
    public Image blueImage;
    public Image blueImageBackground;
    public GameObject vaccineButton;

   // public Text rightbuttClickOuter;
   // public Text leftbuttClickInner;
    [SerializeField] private Player m_singlePlayer;
    [SerializeField] private Player[] m_multiPlayer;

    [SerializeField] private Text nextLevelText;
    private bool nextLevelTextActive;
    private float nextLevelTextTime = 2.0f;
    private float nextLevelTextTimer;

    [SerializeField] private GameObject wipeoutText;
    private bool wipeoutTextActive;
    private float wipeoutTextTime = 2.0f;
    private float wipeoutTextTimer;

    private bool m_multiplayerHasBeenSet = false;
    private bool m_isMultiplayer = false;
    private Dictionary<int, string> m_names = new Dictionary<int, string>();

    public void SetIsMultiplayer(bool isMulti) {
        m_multiplayerHasBeenSet = true;
        m_isMultiplayer = isMulti;
        SetEnabled(0, m_singlePlayer, !isMulti);
        int i = 0;
        foreach (var player in m_multiPlayer) {
            SetEnabled(i++, player, isMulti);
        }
    }

    public void setDistanceText(int i)
    {
        distanceToCheckpoint.text = "NEXT CHECKPOINT IN " + i;
    }


    public void setLives(int i)
    {

        foreach(Image life in lives)
        {
            life.gameObject.SetActive(false);
        }

        int j = 0;
        foreach (Image life in lives)
        {
            if (j< i)
            {
                life.gameObject.SetActive(true);
            }
            j++;

        }
    }

    private void SetEnabled(int playerNumber, Player player, bool active) {
        player.scoreText.enabled = active;
        player.boostImage.enabled = active;
        player.boostImageBackground.enabled = active;
        player.nameText.enabled = active;
        if (m_names.ContainsKey(playerNumber)) {
            player.nameText.text = m_names[playerNumber];
        }
    }

    public void SetHealth(int health, int playerNumber) {
        // GetPlayer(playerNumber).healthText.text = "Health: " + health;
    }

    public void SetName(string name, int playerNumber) {
        GetPlayer(playerNumber).nameText.text = name;
        m_names[playerNumber] = name;
    }

    //public void SetRightClickText(float clickNumber)
    //{
    //    rightbuttClickOuter.text = "-" + clickNumber;
    //}
    //public void SetLeftClickText(int clickNumber)
   // {
    //    leftbuttClickInner.text = "-" + clickNumber;
   // }

    public void SetScore(int score, int playerNumber) {
        string format = "{0}";
        if (score >= 1000000) {
            format = "{0,12:0,000,000}";
        } else if (score >= 1000) {
            format = "{0,12:0,000}";
        }
        GetPlayer(playerNumber).scoreText.text = string.Format(format, score);
    }

    public void SetBoost(float pct, int playerNumber) {
        GetPlayer(playerNumber).boostImage.fillAmount = Mathf.Clamp(pct, 0f, 1f);
    }

    public void SetBlue(float pct, int playerNumber)
    {
        if (pct >= 1f)
        {
            vaccineButton.SetActive(true);
        }
        else
        {
            vaccineButton.SetActive(false);
        }
        blueImage.fillAmount = Mathf.Clamp(pct, 0f, 1f);
    }

    private Player GetPlayer(int playerNumber) {
        return m_isMultiplayer ? m_multiPlayer[playerNumber] : m_singlePlayer;
    }

    private void Awake() {
        Singleton.Awake(this, ref i);
    }

    private void Start() {
        if (!m_multiplayerHasBeenSet) {
            SetIsMultiplayer(false);
        }
        nextLevelTextTimer = nextLevelTextTime;
    }

    public void Update() {
        if (nextLevelTextActive == true) {
            nextLevelTextTimer -= Time.deltaTime;
            Vector3 grow = new Vector3(2f - nextLevelTextTimer, 2f - nextLevelTextTimer, 2f - nextLevelTextTimer);
            nextLevelText.transform.localScale = grow;
            //nextLevelText.transform.localScale.x = nextLevelText.transform.localScale.x=* 1.1;
            if (nextLevelTextTimer < 0) {
                nextLevelTextActive = false;
                nextLevelText.gameObject.SetActive(false);
                //nextLevelText.localScale.x
                Vector3 one = new Vector3(1, 1, 1);
                nextLevelText.transform.localScale = one;
            }
        }
        if (wipeoutTextActive == true) {
            wipeoutTextTimer -= Time.deltaTime;
            Vector3 grow = new Vector3(2f - wipeoutTextTimer, 2f - wipeoutTextTimer, 2f - wipeoutTextTimer);
            wipeoutText.transform.localScale = grow;
            //nextLevelText.transform.localScale.x = nextLevelText.transform.localScale.x=* 1.1;
            if (wipeoutTextTimer < 0) {
                wipeoutTextActive = false;
                wipeoutText.SetActive(false);
                //nextLevelText.localScale.x
                Vector3 one = new Vector3(1, 1, 1);
                wipeoutText.transform.localScale = one;
            }
        }

    }

    public void ActivateNextLevelText(string levelName) {
        Debug.Log("ACTIVATE " + levelName);
        nextLevelTextActive = true;
        nextLevelTextTimer = nextLevelTextTime;
        nextLevelText.text = levelName;
        nextLevelText.gameObject.SetActive(true);
        


    }

    public void ActivateWipeoutText() {
        wipeoutTextActive = true;
        wipeoutTextTimer = wipeoutTextTime;
        wipeoutText.SetActive(true);


    }
}
