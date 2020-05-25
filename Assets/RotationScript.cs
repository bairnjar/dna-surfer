using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    private Rigidbody2D m_rb;
    [SerializeField] private float m_turnSpeed = 0.5f;


    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
    
    }

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateConstantRotation();
    }

    private void UpdateConstantRotation()
    {
            float h = -1;
        // h = 0;
        // h = 
        //        Debug.Log("horizontalvalue =" + h);
        m_rb.AddTorque(-h * m_turnSpeed * Time.deltaTime);
    }
}
