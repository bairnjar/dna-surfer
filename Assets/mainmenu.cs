using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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

    public void loadOnePlayerGame()
    {
        string name = SceneManager.GetActiveScene().name;

        SceneManager.UnloadSceneAsync(name);

        SceneManager.LoadScene("ben-dna1p", LoadSceneMode.Single);

    }

    public void loadTwoPlayerGame()
    {
        string name = SceneManager.GetActiveScene().name;

        SceneManager.UnloadSceneAsync(name);

        SceneManager.LoadScene("ben-dna", LoadSceneMode.Single);
    }
}
