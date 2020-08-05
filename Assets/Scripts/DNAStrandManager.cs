using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using ES3Internal;
//using ES3Types;

public class DNAStrandManager : MonoBehaviour {
    public static DNAStrandManager i;

    public int currentLevel = 0;
    private int currentCheckpoint = 0;
    public bool currentSafe = false;
    private int currentDNA = 0;
    private int previousLevel = 0;
    private int previousDNA = 0;
    public float currentSpeedMultiplier = 1f;
    public float currentRubberBandReduction = 1f;
    public float currentChaseWaveSpeedMultiplier = 1f;

    [SerializeField] private List<LevelInfo> levels;

    //[SerializeField] public LevelNames levelNames;

    public Dictionary<string, GameObject> levelDictionary =new Dictionary<string, GameObject>();

    public List<string> levelKeys;
    public List<GameObject> levelValues;



    //[SerializeField] private List<GameObject> m_rightEntryDNAPrefab;
    //[SerializeField] private List<GameObject> m_bothEntryDNAPrefab;
    private PATHTYPE lastExit;
    [SerializeField] private float m_spawnOffset;

    [SerializeField] private GameObject m_lastSpawned;
    private GameObject currentSegment;
    [SerializeField] private List<GameObject> m_spawned = new List<GameObject>();
    [SerializeField] private GameObject m_finalZone;


    private void Start()
    {
        //PlayerPrefs.SetInt("Checkpoint", 8);
        if (PlayerPrefs.GetInt("Checkpoint") != 0)
        {
            int loadingLevel = PlayerPrefs.GetInt("Checkpoint");

            currentCheckpoint = loadingLevel;
            currentLevel = loadingLevel;
            currentDNA = 0;
            previousLevel = loadingLevel;
            previousDNA = 0;
            currentSpeedMultiplier = levels[loadingLevel].levelSpeedMultipliers[0];
            // HUD.i.SetRightClickText(currentSpeedMultiplier);
            currentChaseWaveSpeedMultiplier = levels[loadingLevel].chaseWaveSpeedMultipliers[0];
            currentRubberBandReduction = levels[loadingLevel].levelRubberBandReduction[0];
            currentSafe = levels[loadingLevel].safe;
        }

        else
        {
            currentCheckpoint = 0;
            currentLevel = 0;
            currentDNA = 0;
            previousLevel = 0;
            previousDNA = 0;
            currentSpeedMultiplier = levels[0].levelSpeedMultipliers[0];
            // HUD.i.SetRightClickText(currentSpeedMultiplier);
            currentChaseWaveSpeedMultiplier = levels[0].chaseWaveSpeedMultipliers[0];
            currentRubberBandReduction = levels[0].levelRubberBandReduction[0];
            currentSafe = false;
        }

        //populateDictionary();
        


    }

    public void StartDNAStrand()
    {
        Debug.Log("dnastrandstart");
        currentCheckpoint = 0;
        currentLevel = 0;
        currentDNA = 0;
        previousLevel = 0;
        previousDNA = 0;
        currentSpeedMultiplier = levels[0].levelSpeedMultipliers[0];
        // HUD.i.SetRightClickText(currentSpeedMultiplier);
        currentChaseWaveSpeedMultiplier = levels[0].chaseWaveSpeedMultipliers[0];
        currentRubberBandReduction = levels[0].levelRubberBandReduction[0];
        currentSafe = false;
        int saveTest = 3;
        //ES3.Save("allLevels", levels, "levelTest3.es3");

    }

    public void ScoreIncrement()
    {
        previousDNA++;
        int i = levels[previousLevel].levelThresholds.IndexOf(previousDNA);
        if (i > 0)
        {
            float oldChaseWaveMultiplier = currentChaseWaveSpeedMultiplier;
            currentSpeedMultiplier = levels[previousLevel].levelSpeedMultipliers[i];
            //HUD.i.SetRightClickText(currentSpeedMultiplier);

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

            currentSafe = levels[previousLevel].safe;
            if (currentSafe)
            {
                currentCheckpoint = previousLevel;
            }
            HUD.i.ActivateNextLevelText(levels[previousLevel].Name);
            currentSpeedMultiplier = levels[previousLevel].levelSpeedMultipliers[previousDNA];
            //HUD.i.SetRightClickText(currentSpeedMultiplier);
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
            Debug.Log("incrementLEvel");
            currentLevel++;
            currentDNA = 0;



        }

        if (currentLevel >= levels.Count)
        {
            PlayerController.players[0].turnOnVaccine();
        }
        Debug.Log("SPAWNING...");
        Debug.Log("CurrentDNA = " + currentDNA);
        Debug.Log("CurrentLevel = " + currentLevel);



        bool spawnRight = true;
        bool spawnBoost = false;
        
        if (m_lastSpawned != null) {
            position = m_lastSpawned.transform.position + Vectors.Y(m_spawnOffset);
            if (m_lastSpawned.GetComponent<DnaInfo>().exitType == PATHTYPE.RIGHT)
            {
                spawnRight = true;
            }
            else if (m_lastSpawned.GetComponent<DnaInfo>().exitType == PATHTYPE.BOTH)
            {
                spawnRight = false;
            }

            if ((currentDNA + 1) % levels[currentLevel].boostFrequency == 0)
            {
                spawnBoost = true;
            }
        }

        GameObject instantiateDNA = new GameObject();
        if (PlayerController.players[0].getVaccine())
        {
            var dna = GameObject.Instantiate(m_finalZone, transform);
            dna.transform.position = position;
            m_lastSpawned = dna;
            m_spawned.Add(dna);
        }
        
        else if (currentDNA == 0 && spawnRight)
        {
            Random random = new Random();
            int ran = Random.Range(0, levels[currentLevel].firstTileRightEntryDNAPrefab.Count);
            Debug.Log("CurrentLevel = " + currentLevel);
            Debug.Log("TileName = " + levels[currentLevel].firstTileRightEntryDNAPrefab[ran]);
            Debug.Log("DictonaryLength= " + levelDictionary.Count);
            GameObject instantiateDNA2 = levelDictionary[levels[currentLevel].firstTileRightEntryDNAPrefab[ran]];
            //levelDictionary.TryGetValue(levels[currentLevel].firstTileRightEntryDNAPrefab[ran], out instantiateDNA);
            var dna = GameObject.Instantiate(instantiateDNA2, transform);
            dna.transform.position = position;
            m_lastSpawned = dna;
            m_spawned.Add(dna);
        }
        else if (currentDNA == 0 && !spawnRight)
        {
            Random random = new Random();
            int ran = Random.Range(0, levels[currentLevel].firstTileBothEntryDNAPrefab.Count);
            levelDictionary.TryGetValue(levels[currentLevel].firstTileBothEntryDNAPrefab[ran], out instantiateDNA);

            var dna = GameObject.Instantiate(instantiateDNA, transform);
            dna.transform.position = position;
            m_lastSpawned = dna;
            m_spawned.Add(dna);
        }
        

        else if ((currentDNA == levels[currentLevel].levelLength - 1) && spawnRight)
        {
            Random random = new Random();
            int ran = Random.Range(0, levels[currentLevel].finalTileRightEntryDNAPrefab.Count);
            
            levelDictionary.TryGetValue(levels[currentLevel].finalTileRightEntryDNAPrefab[ran], out instantiateDNA);
            var dna = GameObject.Instantiate(instantiateDNA, transform);
            dna.transform.position = position;
            m_lastSpawned = dna;
            m_spawned.Add(dna);
        }
        else if ((currentDNA == levels[currentLevel].levelLength - 1) && !spawnRight)
        {
            Random random = new Random();
            int ran = Random.Range(0, levels[currentLevel].finalTileBothEntryDNAPrefab.Count);
            levelDictionary.TryGetValue(levels[currentLevel].finalTileBothEntryDNAPrefab[ran], out instantiateDNA);

            var dna = GameObject.Instantiate(instantiateDNA, transform);
            dna.transform.position = position;
            m_lastSpawned = dna;
            m_spawned.Add(dna);
        }
        else if (spawnRight && !spawnBoost)
        {
            Random random = new Random();
            int ran = Random.Range(0, levels[currentLevel].rightEntryDNAPrefab.Count);
            levelDictionary.TryGetValue(levels[currentLevel].rightEntryDNAPrefab[ran], out instantiateDNA);
            var dna = GameObject.Instantiate(instantiateDNA, transform);
            dna.transform.position = position;
            m_lastSpawned = dna;
            m_spawned.Add(dna);
        } else if (spawnRight && spawnBoost)
        {
            Random random = new Random();
            // int ran = Random.Range(0, levels[currentLevel].m_boostRightEntryDNAPrefab.Count);
            // var dna = GameObject.Instantiate(levels[currentLevel].m_boostRightEntryDNAPrefab[ran], transform);
            // dna.transform.position = position;
            // m_lastSpawned = dna;
            // m_spawned.Add(dna);
        }
        else if (!spawnRight && !spawnBoost)
        {
            Random random = new Random();
            int ran = Random.Range(0, levels[currentLevel].bothEntryDNAPrefab.Count);
            levelDictionary.TryGetValue(levels[currentLevel].bothEntryDNAPrefab[ran], out instantiateDNA);

            var dna = GameObject.Instantiate(instantiateDNA, transform);
            dna.transform.position = position;
            m_lastSpawned = dna;
            m_spawned.Add(dna);
        }
        else
        {
            Random random = new Random();
            // int ran = Random.Range(0, levels[currentLevel].m_boostBothEntryDNAPrefab.Count);
            //var dna = GameObject.Instantiate(levels[currentLevel].m_boostBothEntryDNAPrefab[ran], transform);
            // dna.transform.position = position;
            //. m_lastSpawned = dna;
            // m_spawned.Add(dna);
        }
        currentDNA++;

        if (levels[currentLevel].Walls)
        {
            ActivateWalls(true);

        }
        else
        {
            ActivateWalls(false);
        }

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
        if (active) {
            Debug.Log("WallStatus= TRUE");
        }
        else
        {
            Debug.Log("WallStatus= FALSE");
        }

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

        //if has lives
        if (PlayerController.players[0].currentLives > 0)
        {
            currentLevel = currentCheckpoint;
        }
        else
        {
            Debug.Log("Reset LIVES andcheckpoint");
            currentCheckpoint = 0;
            currentLevel = 0;
        }



        //if no lives
        //currentLevel = 0;

        if (currentLevel != 0)
        {
            currentSafe = true;
        }
        else
        {
            currentSafe = false;
        }

        currentDNA = 0;
        //currentSafe = false;
        previousLevel = currentLevel;
        previousDNA = 0;
        currentSpeedMultiplier = levels[0].levelSpeedMultipliers[0];
        // HUD.i.SetRightClickText(currentSpeedMultiplier);
        currentChaseWaveSpeedMultiplier = levels[0].chaseWaveSpeedMultipliers[0];
        currentRubberBandReduction = levels[0].levelRubberBandReduction[0];
        m_spawned.Clear();
        m_lastSpawned = null;



    }

    public void populateDictionary()
    {
        Debug.Log("populatedictionary");
        levelDictionary = new Dictionary<string, GameObject>();
        int i = 0;
        foreach(string key in levelKeys)
        {
            levelDictionary.Add(levelKeys[i], levelValues[i]);
            i++;
        }
    }

    private void Awake() {
        Singleton.Awake(this, ref i);
    }


    [System.Serializable]
    public class LevelInfo
    {
        [SerializeField] public string Name;
        [SerializeField] public List<string> firstTileRightEntryDNAPrefab;
        [SerializeField] public List<string> firstTileBothEntryDNAPrefab;
        [SerializeField] public List<string> rightEntryDNAPrefab;
        [SerializeField] public List<string> bothEntryDNAPrefab;
        //[SerializeField] public List<GameObject> m_boostBothEntryDNAPrefab;
        //[SerializeField] public List<GameObject> m_boostRightEntryDNAPrefab;
        [SerializeField] public List<string> finalTileRightEntryDNAPrefab;
        [SerializeField] public List<string> finalTileBothEntryDNAPrefab;
        [SerializeField] public int boostFrequency;
        [SerializeField] public int levelLength;
        [SerializeField] public List<int> levelThresholds;
        [SerializeField] public float[] levelSpeedMultipliers = { 1f, 1.2f, 1.5f, 2f };
        [SerializeField] public float[] levelRubberBandReduction = { 1f, 1.2f, 1.5f, 2f };
        [SerializeField] public float[] chaseWaveSpeedMultipliers = { 1f, 1.2f, 1.5f, 2f };
        [SerializeField] public bool safe = false;
        [SerializeField] public bool Walls = false;
    }

    //[System.Serializable] public class LevelNames : SerializableDictionary<string, GameObject> { }

}