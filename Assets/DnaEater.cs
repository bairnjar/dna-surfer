using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Btkalman.Unity.Util;


public class DnaEater : MonoBehaviour
{
    public static DnaEater i;
    public static float FollowDistance = 30f;
    private Rigidbody2D m_rb;
    private Vector2 startPosition;
    [SerializeField] private float m_accelleration = 1;
    private float start_accelleration = 1;
    [SerializeField] private float m_hillAccelleration = 1f;
    [SerializeField] private float m_rubberBandAttenuationPerLevel = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        Singleton.Awake(this, ref i);
        m_rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        start_accelleration = m_accelleration;

    }
    // Update is called once per frame
    private void Update()
    {
        m_accelleration = m_accelleration * 1.0000001f;

        UpdateAccelleration();


    }

    private void UpdateAccelleration()
    {
        var levelMultiplier = DNAStrandManager.i.currentChaseWaveSpeedMultiplier;
        // Force from movement.
        float scale = m_accelleration;



        var playerDirection = Vector2.up;
        Vector2 force = playerDirection * scale * Time.deltaTime * levelMultiplier * 0.7f;
        // Constant forward force.
        force += Vector2.up * m_hillAccelleration * Time.deltaTime * levelMultiplier;
        // Rubber band to center of track.


        var playerPos = m_rb.position;

        force += new Vector2(0, 0) * Time.deltaTime *
            Mathf.Pow(m_rubberBandAttenuationPerLevel, ScoreManager.i.currentLevel);
        m_rb.AddForce(force);
    }

    public void Reset()
    {
        m_accelleration = start_accelleration;
        transform.position = startPosition;
    }

    public void CatchUp()
    {
        if (transform.position.y < PlayerController.players[0].transform.position.y - FollowDistance)
        {
            transform.position = new Vector2(PlayerController.players[0].transform.position.x, PlayerController.players[0].transform.position.y - FollowDistance);
        }
    }
}