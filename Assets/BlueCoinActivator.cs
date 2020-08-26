using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Btkalman.Unity.Util;


public class BlueCoinActivator : MonoBehaviour
{

    public Collectible blueCoin;
    public List<Collectible> comboCoins;
    public bool allCoinsGot = false;
    public int coinToCombo = 5;
    public int coinLength = 0;

    // Start is called before the first frame update
    void Start()
    {
        comboCoins = new List<Collectible>(GetComponentsInChildren<Collectible>());
        Collectible tempCoin;
        for (int j = 0; j < comboCoins.Count - 1; j++)
        {
            for (int i = 0; i < comboCoins.Count - 1; i++)
            {
                if (comboCoins[i].GetComponent<Transform>().position.y > comboCoins[i + 1].GetComponent<Transform>().position.y)
                {
                    tempCoin = comboCoins[i + 1];
                    comboCoins[i + 1] = comboCoins[i];
                    comboCoins[i] = tempCoin;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (allCoinsGot == false)
        {
            CheckCoins();
        }
    }

    public void CheckCoins()
    {
        Collectible[] coins = GetComponentsInChildren<Collectible>();
        


        if (coins.Length <= 0)
        {
            allCoinsGot = true;
            if (blueCoin != null)
            {
                blueCoin.gameObject.SetActive(true);
            }
            
        }

        if (coinLength != coins.Length)
        {
            CheckCombo();
        }


        coinLength = coins.Length;
    }

    public void CheckCombo()
    {
        if (DNAStrandManager.i.currentSafe || DNAStrandManager.i.currentLevel < 2)
            return;

        int collectedCoins = 0;
        foreach(Collectible coin in comboCoins)
        {
            if (coin == null)
            {
                collectedCoins += 1;
            }
        }

        if (collectedCoins > 5)
        {
            if (collectedCoins == comboCoins.Count)
            {
                HUD.i.ActivateNextLevelText("Perfect");
            }

            else
            {
                HUD.i.ActivateNextLevelText(collectedCoins.ToString() + "x Combo");
            }
        }
    }

}
