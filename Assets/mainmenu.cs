using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class mainmenu : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadOnePlayerGameIntro1()
    {
        string name = SceneManager.GetActiveScene().name;

        SceneManager.UnloadSceneAsync(name);

        SceneManager.LoadScene("Instructions1P1", LoadSceneMode.Single);

    }

    public void loadOnePlayerGameIntro2()
    {
        string name = SceneManager.GetActiveScene().name;

        SceneManager.UnloadSceneAsync(name);

        SceneManager.LoadScene("Instructions1P2", LoadSceneMode.Single);

    }

    public void loadOnePlayerGame()
    {
        string name = SceneManager.GetActiveScene().name;

        SceneManager.UnloadSceneAsync(name);

        SceneManager.LoadScene("reese-dna1p 1", LoadSceneMode.Single);

    }

    public void loadOnePlayerGameEasy()
    {
        string name = SceneManager.GetActiveScene().name;

        SceneManager.UnloadSceneAsync(name);

        SceneManager.LoadScene("ben-dna1p-easy", LoadSceneMode.Single);

    }

    public void loadOnePlayerGameHard()
    {
        string name = SceneManager.GetActiveScene().name;

        SceneManager.UnloadSceneAsync(name);

        SceneManager.LoadScene("ben-dna1p-hard", LoadSceneMode.Single);

    }

    public void loadTwoPlayerGameIntro1()
    {
        string name = SceneManager.GetActiveScene().name;

        SceneManager.UnloadSceneAsync(name);

        SceneManager.LoadScene("Instructions2P1", LoadSceneMode.Single);

    }

    public void loadTwoPlayerGameIntro2()
    {
        string name = SceneManager.GetActiveScene().name;

        SceneManager.UnloadSceneAsync(name);

        SceneManager.LoadScene("Instructions2P2", LoadSceneMode.Single);

    }

    public void loadTwoPlayerGame()
    {
        string name = SceneManager.GetActiveScene().name;

        SceneManager.UnloadSceneAsync(name);

        SceneManager.LoadScene("ben-dna", LoadSceneMode.Single);
    }

    public void ResetCheckPoint()
    {
        PlayerPrefs.SetInt("Checkpoint", 0);
    }
}
