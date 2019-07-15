using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinishScreen : MonoBehaviour
{
    public static FinishScreen i;

    [SerializeField] private GameObject m_winText;
    [SerializeField] private GameObject m_loseText;
    [SerializeField] private GameObject score1Name;
    [SerializeField] private GameObject score1Number;
    [SerializeField] private GameObject score2Name;
    [SerializeField] private GameObject score2Number;
    [SerializeField] private GameObject score3Name;
    [SerializeField] private GameObject score3Number;
    private Canvas m_canvas;

    public bool Visible()
    {
        return m_canvas.enabled;
    }

    public void TryAgain()
    {
        m_canvas.enabled = false;
    }

    public void Win()
    {
        m_canvas.enabled = true;
        m_winText.SetActive(true);
        m_loseText.SetActive(false);
    }

    public void Lose()
    {
        m_canvas.enabled = true;
        bool player1Winner = false;
        float player1Score = ScoreManager.i.GetScore(0);
        float player2Score = ScoreManager.i.GetScore(1);
        Debug.Log("PLAYER 1 SCORE: " + player1Score);
        Debug.Log("PLAYER 2 SCORE: " + player2Score);
        if (player1Score > player2Score)
        {
            player1Winner = true;
            Debug.Log("Player 1 is number 1");

            StorePlayer1Score(player1Score);
            StorePlayer2Score(player2Score);
        }
        else
        {
            Debug.Log("Player 2 is number 1");
            StorePlayer2Score(player2Score);
            StorePlayer1Score(player1Score);
        }

        DisplayScores();
        m_winText.SetActive(false);
        m_loseText.SetActive(true);

    }

    private void Awake()
    {
        Singleton.Awake(this, ref i);
        m_canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        m_canvas.enabled = false;
    }

    public void StorePlayer1Score(float player1Score)
    {
        if (player1Score > PlayerPrefs.GetFloat("1F"))
        {
           


            PlayerPrefs.SetFloat("3F", PlayerPrefs.GetFloat("2F"));
            PlayerPrefs.SetString("3S", PlayerPrefs.GetString("2S"));
            PlayerPrefs.SetFloat("2F", PlayerPrefs.GetFloat("1F"));
            PlayerPrefs.SetString("2S", PlayerPrefs.GetString("1S"));

            Debug.Log("writePlayer1 score to 1");
            PlayerPrefs.SetFloat("1F", player1Score);
            PlayerPrefs.SetString("1S", "Player 1....");
            Debug.Log("Access 1 is: " + PlayerPrefs.GetFloat("1F"));

        }
        else if (player1Score > PlayerPrefs.GetFloat("2F"))
        {
            PlayerPrefs.SetFloat("3F", PlayerPrefs.GetFloat("2F"));
            PlayerPrefs.SetString("3S", PlayerPrefs.GetString("2S"));
            Debug.Log("writePlayer1 score to 2");
            PlayerPrefs.SetFloat("2F", player1Score);
            PlayerPrefs.SetString("2S", "Player 1....");

        }
        else if (player1Score > PlayerPrefs.GetFloat("3F"))
        {
            Debug.Log("writePlayer1 score to 3");
            PlayerPrefs.SetFloat("3F", player1Score);
            PlayerPrefs.SetString("3S", "Player 1....");
        }
        else
        {
            Debug.Log("don't write player 1 score");
        }
    }

    public void StorePlayer2Score(float player2Score)
    {
        if (player2Score > PlayerPrefs.GetFloat("1F"))
        {

            
            PlayerPrefs.SetFloat("3F", PlayerPrefs.GetFloat("2F"));
            PlayerPrefs.SetString("3S", PlayerPrefs.GetString("2S"));
            PlayerPrefs.SetFloat("2F", PlayerPrefs.GetFloat("1F"));
            PlayerPrefs.SetString("2S", PlayerPrefs.GetString("1S"));

            Debug.Log("writePlayer2 score to 1");

            PlayerPrefs.SetFloat("1F", player2Score);
            PlayerPrefs.SetString("1S", "Player 2....");

        }
        else if (player2Score > PlayerPrefs.GetFloat("2F"))
        {
            PlayerPrefs.SetFloat("3F", PlayerPrefs.GetFloat("2F"));
            PlayerPrefs.SetString("3S", PlayerPrefs.GetString("2S"));

            Debug.Log("writePlayer2 score to 2");
            PlayerPrefs.SetFloat("2F", player2Score);
            PlayerPrefs.SetString("2S", "Player 2....");

        }
        else if (player2Score > PlayerPrefs.GetFloat("3F"))
        {

            Debug.Log("writePlayer2 score to 3");
            PlayerPrefs.SetFloat("3F", player2Score);
            PlayerPrefs.SetString("3S", "Player 1....");
        }
        else
        {
            Debug.Log("don't write Player2 score");
        }
    }


    public void DisplayScores()
    {


        score1Name.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("1S");
        score1Number.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetFloat("1F").ToString();
        score2Name.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("2S");
        score2Number.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetFloat("2F").ToString();
        score3Name.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("3S");
        score3Number.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetFloat("3F").ToString();

    }
}
