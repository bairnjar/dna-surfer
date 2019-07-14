using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    public static HUD i;

    [SerializeField] private Text healthText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Image boostImage;

    public void SetHealth(int health) {
        healthText.text = "Health: " + health;
    }

    public void SetScore(int score) {
        string format = "{0}";
        if (score >= 1000000) {
            format = "{0,12:0,000,000}";
        } else if (score >= 1000) {
            format = "{0,12:0,000}";
        }
        scoreText.text = string.Format(format, score);
    }

    public void SetBoost(float pct) {
        boostImage.fillAmount = Mathf.Clamp(pct, 0f, 1f);
    }

    private void Awake() {
        Singleton.Awake(this, ref i);
    }
}
