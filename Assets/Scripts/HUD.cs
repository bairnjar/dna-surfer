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
    }

    [SerializeField] private Player m_singlePlayer;
    [SerializeField] private Player[] m_multiPlayer;

    [SerializeField] private GameObject nextLevelText;
    private bool nextLevelTextActive;
    private float nextLevelTextTime = 2.0f;
    private float nextLevelTextTimer;

    private bool m_multiplayerHasBeenSet = false;
    private bool m_isMultiplayer = false;

    public void SetIsMultiplayer(bool isMulti) {
        m_multiplayerHasBeenSet = true;
        m_isMultiplayer = isMulti;
        SetEnabled(m_singlePlayer, !isMulti);
        foreach (var player in m_multiPlayer) {
            SetEnabled(player, isMulti);
        }
    }

    private void SetEnabled(Player player, bool active) {
        player.scoreText.enabled = active;
        player.boostImage.enabled = active;
        player.boostImageBackground.enabled = active;
    }

    public void SetHealth(int health, int playerNumber) {
        // GetPlayer(playerNumber).healthText.text = "Health: " + health;
    }

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

    public void Update()
    {
        if (nextLevelTextActive == true)
        {
            nextLevelTextTimer -= Time.deltaTime;
            Vector3 grow = new Vector3(2f - nextLevelTextTimer, 2f - nextLevelTextTimer, 2f - nextLevelTextTimer);
            nextLevelText.transform.localScale = grow; 
            //nextLevelText.transform.localScale.x = nextLevelText.transform.localScale.x=* 1.1;
                    if (nextLevelTextTimer < 0) {
                        nextLevelTextActive = false;
                        nextLevelText.SetActive(false);
                //nextLevelText.localScale.x
                Vector3 one = new Vector3(1, 1, 1);
                nextLevelText.transform.localScale = one;
                    }
        }

    }

    public void ActivateNextLevelText()
    {
        nextLevelTextActive = true;
        nextLevelTextTimer = nextLevelTextTime;
        nextLevelText.SetActive(true);


    }
}
