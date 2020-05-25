using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNAStrandManager : MonoBehaviour {
    public static DNAStrandManager i;

    private int currentLevel = 0;
    private int currentDNA=0;
    private int previousLevel = 0;
    private int previousDNA = 0;
    public float currentSpeedMultiplier = 1f;
    public float currentRubberBandReduction = 1f;
    public float currentChaseWaveSpeedMultiplier = 1f;


    [SerializeField] private List<LevelInfo> levels;
    //[SerializeField] private List<GameObject> m_rightEntryDNAPrefab;
    //[SerializeField] private List<GameObject> m_bothEntryDNAPrefab;
    private PATHTYPE lastExit;
    [SerializeField] private float m_spawnOffset;

    private GameObject m_lastSpawned;
    private List<GameObject> m_spawned = new List<GameObject>();

    private void Start()
    {
        currentLevel = 0;
        currentDNA = 0;
        previousLevel = 0;
        previousDNA = 0;
        currentSpeedMultiplier = levels[0].levelSpeedMultipliers[0];
        HUD.i.SetRightClickText(currentSpeedMultiplier);
        currentChaseWaveSpeedMultiplier = levels[0].chaseWaveSpeedMultipliers[0];
        currentRubberBandReduction = levels[0].levelRubberBandReduction[0];

    }

    public void ScoreIncrement()
    {
        previousDNA++;
        int i = levels[previousLevel].levelThresholds.IndexOf(previousDNA);
        if (i > 0)
        {
            float oldChaseWaveMultiplier = currentChaseWaveSpeedMultiplier;
            currentSpeedMultiplier = levels[previousLevel].levelSpeedMultipliers[i];
            HUD.i.SetRightClickText(currentSpeedMultiplier);
            currentRubberBandReduction = levels[previousLevel].levelRubberBandReduction[i];
            currentChaseWaveSpeedMultiplier = levels[previousLevel].chaseWaveSpeedMultipliers[i];
            Debug.Log("Level Value = " + previousLevel);
            Debug.Log("DNA enter = " + previousDNA);
            Debug.Log("CurrentSpeedMultiplier =" + currentSpeedMultiplier);
            Debug.Log("currentrubberbandreduction =" + currentRubberBandReduction);
            if (currentChaseWaveSpeedMultiplier > oldChaseWaveMultiplier)
            {
                //catchupchasewave
                DnaEater.i.CatchUp();
            }
        }

        if (previousDNA >= levels[previousLevel].levelLength)
        {

            float oldChaseWaveMultiplier = currentChaseWaveSpeedMultiplier;
            previousDNA = 0;
            previousLevel++;
            currentSpeedMultiplier = levels[previousLevel].levelSpeedMultipliers[previousDNA];
            HUD.i.SetRightClickText(currentSpeedMultiplier);
            currentChaseWaveSpeedMultiplier = levels[previousLevel].chaseWaveSpeedMultipliers[previousDNA];
            Debug.Log("Level Value = " + previousLevel);
            Debug.Log("DNA enter = " + previousDNA);
            Debug.Log("CurrentSpeedMultiplier =" + currentSpeedMultiplier);
            Debug.Log("currentrubberbandreduction =" + currentRubberBandReduction);
            if (currentChaseWaveSpeedMultiplier > oldChaseWaveMultiplier)
            {
                //catchupchasewave
                DnaEater.i.CatchUp();
            }
        }
        
    }

    public void SpawnNext() {
        var position = Vector2.zero;
        if (currentDNA >= levels[currentLevel].levelLength)
        {
            currentLevel++;
            currentDNA = 0;
          
            

        }




       

        bool spawnRight = true;
        bool spawnBoost = false;
        if (m_lastSpawned != null) {
            position = m_lastSpawned.transform.position + Vectors.Y(m_spawnOffset);
            if (m_lastSpawned.GetComponent<DnaInfo>().exitType==PATHTYPE.RIGHT)
            {
                spawnRight = true;
            }
            else if (m_lastSpawned.GetComponent<DnaInfo>().exitType == PATHTYPE.BOTH)
            {
                spawnRight = false;
            }

            if ((currentDNA + 1) % levels[currentLevel].boostFrequency==0)
            {
                spawnBoost = true;
            }
        }

        if (spawnRight && !spawnBoost)
        {
            Random random = new Random();
            int ran = Random.Range(0, levels[currentLevel].m_rightEntryDNAPrefab.Count);
            var dna = GameObject.Instantiate(levels[currentLevel].m_rightEntryDNAPrefab[ran], transform);
            dna.transform.position = position;
            m_lastSpawned = dna;
            m_spawned.Add(dna);
        }else if(spawnRight && spawnBoost)
        {
            Random random = new Random();
            int ran = Random.Range(0, levels[currentLevel].m_boostRightEntryDNAPrefab.Count);
            var dna = GameObject.Instantiate(levels[currentLevel].m_boostRightEntryDNAPrefab[ran], transform);
            dna.transform.position = position;
            m_lastSpawned = dna;
            m_spawned.Add(dna);
        }
        else if(!spawnRight && !spawnBoost)
        {
            Random random = new Random();
            int ran = Random.Range(0, levels[currentLevel].m_bothEntryDNAPrefab.Count);
            var dna = GameObject.Instantiate(levels[currentLevel].m_bothEntryDNAPrefab[ran], transform);
            dna.transform.position = position;
            m_lastSpawned = dna;
            m_spawned.Add(dna);
        }
        else
        {
            Random random = new Random();
            int ran = Random.Range(0, levels[currentLevel].m_boostBothEntryDNAPrefab.Count);
            var dna = GameObject.Instantiate(levels[currentLevel].m_boostBothEntryDNAPrefab[ran], transform);
            dna.transform.position = position;
            m_lastSpawned = dna;
            m_spawned.Add(dna);
        }
        currentDNA++;
         
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

    public void ActivateWalls(bool active)
    {

        foreach (var spawn in m_spawned)
        {
            var walls = GameObjects.GetObjectsWithTagInDescendents(
                    spawn, "Wall");
            foreach (var wall in walls)
            {
                wall.SetActive(active);
            }
        }
       
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
        currentLevel = 0;
        currentDNA = 0;
        previousLevel = 0;
        previousDNA = 0;
        currentSpeedMultiplier = levels[0].levelSpeedMultipliers[0];
        HUD.i.SetRightClickText(currentSpeedMultiplier);
        currentChaseWaveSpeedMultiplier = levels[0].chaseWaveSpeedMultipliers[0];
        currentRubberBandReduction = levels[0].levelRubberBandReduction[0];
        m_spawned.Clear();
        m_lastSpawned = null;
    }

    private void Awake() {
        Singleton.Awake(this, ref i);
    }


    [System.Serializable]
    public class LevelInfo
    {
        [SerializeField] public List<GameObject> m_rightEntryDNAPrefab;
        [SerializeField] public List<GameObject> m_bothEntryDNAPrefab;
        [SerializeField] public List<GameObject> m_boostBothEntryDNAPrefab;
        [SerializeField] public List<GameObject> m_boostRightEntryDNAPrefab;
        [SerializeField] public int boostFrequency;
        [SerializeField] public int levelLength;
        [SerializeField] public List<int> levelThresholds;
        [SerializeField] public float[] levelSpeedMultipliers = { 1f, 1.2f, 1.5f, 2f };
        [SerializeField] public float[] levelRubberBandReduction= { 1f, 1.2f, 1.5f, 2f };
        [SerializeField] public float[] chaseWaveSpeedMultipliers = { 1f, 1.2f, 1.5f, 2f };
    }
}
