using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Btkalman.Unity.Util;


public class BlueCoinActivator : MonoBehaviour
{

    public Collectible blueCoin;

    public bool allCoinsGot = false;
    public int coinLength = 0;

    // Start is called before the first frame update
    void Start()
    {
        
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
        coinLength = coins.Length;


        if (coins.Length <= 0)
        {
            allCoinsGot = true;
            blueCoin.gameObject.SetActive(true);
        }

    }

}
