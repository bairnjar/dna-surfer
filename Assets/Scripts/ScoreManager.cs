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

    private int currentLevel = 0;
    public float currentMultiplier = 1;
    [SerializeField] private int[] levelScoreThresholds = { 500, 1000, 2000, 3000 };
    [SerializeField] private float[] levelSpeedMultipliers = { 1f, 1.2f, 1.5f, 2f };


    private Dictionary<int, float> m_scores = new Dictionary<int, float>();
    private float m_autoScoreTimer = 0f;
    private float m_boostScoreTimer = 0f;
    private float m_boostingTimer = 0f;

    public void Boost() {
        m_boostingTimer = m_boostTimerGracePeriod;
    }

    public void CollectCoin(int playerNumber) {
        m_scores[playerNumber] = GetScore(playerNumber) + m_coinScoreValue;
        UpdateScoreText(playerNumber);
    }

    public void PassScoreGate(int playerNumber) {
        m_scores[playerNumber] = GetScore(playerNumber) + m_scoreGateValue;
        UpdateScoreText(playerNumber);
    }

    public void Reset() {
        m_scores.Clear();
        for (int i = 0; i < PlayerController.numPlayers; i++) {
            UpdateScoreText(i);
        }
    }

    private void Awake() {
        Singleton.Awake(this, ref i);
    }

    private void Start() {
        Reset();
    }

    private void Update() {
        for (int playerNumber = 0; playerNumber < PlayerController.numPlayers; playerNumber++) {
            float score = GetScore(playerNumber);
            if (!FinishScreen.i.Visible()) {
                UpdatePeriodicScore(playerNumber, m_autoScorePeriod, m_autoScoreValue, ref m_autoScoreTimer);
            }
            if (m_boostingTimer > 0) {
                UpdatePeriodicScore(playerNumber, m_boostScorePeriod, m_boostScoreValue, ref m_boostScoreTimer);
                m_boostingTimer = Mathf.Max(m_boostScoreTimer - Time.deltaTime, 0f);
            }
            if (score != GetScore(playerNumber)) {
                UpdateScoreText(playerNumber);
            }
        }
    }

    private void UpdatePeriodicScore(int playerNumber, float period, int value, ref float timer) {
        timer += Time.deltaTime;
        while (timer >= period) {
            m_scores[playerNumber] = GetScore(playerNumber) + value;
            timer -= period;

            checkCompleteLevel(playerNumber);
        }
    }

    private void UpdateScoreText(int playerNumber) {
        HUD.i.SetScore(Mathf.RoundToInt(GetScore(playerNumber)), playerNumber);
    }

    private float GetScore(int playerNumber) {
        float score = 0;
        if (m_scores.ContainsKey(playerNumber)) {
            score = m_scores[playerNumber];
        } else {
            m_scores[playerNumber] = score;
        }
        return score;
    }

    public void resetCurrentLevel()
    {
        currentLevel = 0;
        currentMultiplier = levelSpeedMultipliers[currentLevel];
    }


    private void checkCompleteLevel(int playerNumber)
    {
        if (m_scores[playerNumber] > levelScoreThresholds[currentLevel])
        {
            currentLevel++;
            currentMultiplier = levelSpeedMultipliers[currentLevel];
        }
    }
}
