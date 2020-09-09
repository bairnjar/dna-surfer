using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusGenerator : MonoBehaviour
{
    public int numberOfViruses;
    [SerializeField] private float minYGapFromVirus;
    [SerializeField] private float distanceFromPath;
    [SerializeField] private GameObject[] virusPrefabs;
    [SerializeField] private bool spawnOnRightSide;
    public int virusesSpawned;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateVirus(Transform instantiateParent)
    {
        float rand = Random.Range(-3, 3);

        if (spawnOnRightSide)
        {
            // spawn virus on left side

            Instantiate(virusPrefabs[Random.Range(0, virusPrefabs.Length - 1)], new Vector3(distanceFromPath, rand), Quaternion.identity, instantiateParent);
        }
        else
        {
            // spawn virus on right side

            Instantiate(virusPrefabs[Random.Range(0, virusPrefabs.Length - 1)], new Vector3(-distanceFromPath, rand), Quaternion.identity, instantiateParent);
        }

        spawnOnRightSide = !spawnOnRightSide;
    }
}
