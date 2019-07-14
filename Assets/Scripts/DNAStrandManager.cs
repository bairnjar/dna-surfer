using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNAStrandManager : MonoBehaviour {
    public static DNAStrandManager i;

    [SerializeField] private GameObject m_DNAPrefab;
    [SerializeField] private float m_spawnOffset;

    private GameObject m_lastSpawned;

    public void SpawnNext() {
        var position = Vector2.zero;
        if (m_lastSpawned != null) {
            position = m_lastSpawned.transform.position + Vectors.Y(m_spawnOffset);
        }
        var dna = GameObject.Instantiate(m_DNAPrefab, transform);
        dna.transform.position = position;
        m_lastSpawned = dna;
    }

    private void Awake() {
        Singleton.Awake(this, ref i);
    }

    private void Start() {
        SpawnNext();
        SpawnNext();
    }
}
