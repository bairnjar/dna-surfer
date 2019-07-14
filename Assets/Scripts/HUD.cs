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
    }
}
