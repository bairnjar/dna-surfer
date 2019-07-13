using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour {
    public static CollectibleManager i;

    [SerializeField] private Collectible m_collectiblePrefab;
    [SerializeField] private float m_spawnPeriod = 1f;
    [SerializeField] private float m_spawnRadius = 1f;

    private float m_lastSpawnTime;
   
   
    public void Spawn(Vector2 position) {
        var collectible = GameObject.Instantiate(m_collectiblePrefab, transform);
        int randomNumber =Random.Range(0, 3);
        Debug.Log("Randomnumber == " + randomNumber);
        if (randomNumber == 1)
        {
            Debug.Log("spawnhealthup" + randomNumber);
            collectible.collectibleType =COLLECTIBLETYPE.HEALTHUP;
            collectible.GetComponentInChildren<SpriteRenderer>().color = Color.cyan;
        }else if(randomNumber == 2)
        {
            Debug.Log("spawnhealthdowngit " + randomNumber);
            collectible.collectibleType = COLLECTIBLETYPE.HEALTHDOWN;
            collectible.GetComponentInChildren<SpriteRenderer>().color = Color.black;
        }
        collectible.transform.position = position;
    }

   
    private void Awake() {
        Singleton.Awake(this, ref i);
    }

    private void Start() {
        m_lastSpawnTime = Time.time;
    }

    private void Update() {
        UpdateSpawn();
    }

    private void UpdateSpawn() {
        float time = Time.time;
        if (time - m_lastSpawnTime < m_spawnPeriod) {
            return;
        }
        m_lastSpawnTime = time;
        var player = PlayerController.i;
        var rot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        Spawn((rot * Vector2.up) * m_spawnRadius + player.transform.position);
    }
}
