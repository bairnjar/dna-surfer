using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNAStrandManager : MonoBehaviour {
    public static DNAStrandManager i;

    [SerializeField] private GameObject m_DNAPrefab;

    private GameObject m_lastSpawned;

    public void SpawnNext() {

    }

    private void Awake() {
        Singleton.Awake(this, ref i);
    }
}
