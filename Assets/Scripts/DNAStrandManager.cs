using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNAStrandManager : MonoBehaviour {
    public static DNAStrandManager i;

    [SerializeField] private GameObject m_DNAPrefab;
    [SerializeField] private float m_spawnOffset;

    private GameObject m_lastSpawned;
    private List<GameObject> m_spawned = new List<GameObject>();

    public void SpawnNext() {
        var position = Vector2.zero;
        if (m_lastSpawned != null) {
            position = m_lastSpawned.transform.position + Vectors.Y(m_spawnOffset);
        }
        var dna = GameObject.Instantiate(m_DNAPrefab, transform);
        dna.transform.position = position;
        m_lastSpawned = dna;
        m_spawned.Add(dna);
    }

    public List<Collider2D> GetTrackGuides(int playerNumber) {
        List<Collider2D> guides = new List<Collider2D>();
        foreach (var spawn in m_spawned) {
            var trackGuides = GameObjects.GetObjectsWithTagInDescendents(
                    spawn, "TrackGuideP" + playerNumber);
            foreach (var trackGuide in trackGuides) {
                guides.Add(trackGuide.GetComponent<Collider2D>());
            }
        }
        return guides;
    }

    // public GameObject GetCurrentDNAStrand(Vector2 playerPosition) {
    //     if (!m_lastSpawned) {
    //         return null;
    //     }
    //     for (int i = 1; i < m_spawned.Count - 1; i++) {
    //         if (i * m_spawnOffset < playerPosition.y) {
    //             return m_spawned[i - 1];
    //         }
    //     }
    //     return m_spawned[m_spawned.Count - 1];
    // }

    public void Reset() {
        foreach (var dna in m_spawned) {
            GameObject.Destroy(dna);
        }
        m_spawned.Clear();
        m_lastSpawned = null;
    }

    private void Awake() {
        Singleton.Awake(this, ref i);
    }
}
