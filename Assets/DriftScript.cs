using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftScript : MonoBehaviour
{

    private Rigidbody2D m_rb;
    [SerializeField] private float m_driftSpeed = 0.5f;
    [SerializeField] private Vector2 m_driftDirection = new Vector2(-1, 0);

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDrift();
    }

    private void UpdateDrift()
    {
        var levelMultiplier = ScoreManager.i.currentMultiplier;
        // Force from movement.
        float scale = m_driftSpeed;
        




      


        Vector2 force = m_driftDirection * scale * Time.deltaTime * levelMultiplier;

        m_rb.AddForce(force);


    }
}
