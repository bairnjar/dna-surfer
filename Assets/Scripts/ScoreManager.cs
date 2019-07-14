using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager i;

    [Header("Score that always increases")]
    [SerializeField] private float m_autoScorePeriod = 1f;
    [SerializeField] private int m_autoScoreValue = 1;
    [Header("Score that increases while boosting")]
    [SerializeField] private float m_boostScorePeriod = 0.1f;
    [SerializeField] private int m_boostScoreValue = 1;
    [SerializeField] private float m_boostTimerGracePeriod = 1f;
    [Header("Score that increases when collecting coins")]
    [SerializeField] private int m_coinScoreValue = 10;
    [Header("Score that increases when passing score gates")]
    [SerializeField] private int m_scoreGateValue = 100;
    [Header("Other")]
    [SerializeField] private Text m_scoreText;

    private int m_score;
    private float m_autoScoreTimer = 0f;
    private float m_boostScoreTimer = 0f;
    private float m_boostingTimer = 0f;

    public void Boost() {
        m_boostingTimer = m_boostTimerGracePeriod;
    }

    public void CollectCoin() {
        m_score += m_coinScoreValue;
        UpdateScoreText();
    }

    public void PassScoreGate() {
        m_score += m_scoreGateValue;
        UpdateScoreText();
    }

    public void Reset() {
        m_score = 0;
        UpdateScoreText();
    }

    private void Awake() {
        Singleton.Awake(this, ref i);
    }

    private void Start() {
        Reset();
    }

    private void Update() {
        int score = m_score;
        if (!FinishScreen.i.Visible()) {
            UpdatePeriodicScore(m_autoScorePeriod, m_autoScoreValue, ref m_autoScoreTimer);
        }
        if (m_boostingTimer > 0) {
            UpdatePeriodicScore(m_boostScorePeriod, m_boostScoreValue, ref m_boostScoreTimer);
            m_boostingTimer = Mathf.Max(m_boostScoreTimer - Time.deltaTime, 0f);
        }
        if (score != m_score) {
            UpdateScoreText();
        }
    }

    private void UpdatePeriodicScore(float period, int value, ref float timer) {
        timer += Time.deltaTime;
        while (timer >= period) {
            m_score += value;
            timer -= period;
        }
    }

    private void UpdateScoreText() {
        string format = "{0}";
        if (m_score >= 1000000) {
            format = "{0,12:0,000,000}";
        } else if (m_score >= 1000) {
            format = "{0,12:0,000}";
        }
        m_scoreText.text = string.Format(format, m_score);
    }
}
