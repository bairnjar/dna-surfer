using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusGrow : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private float growthRate;
    [SerializeField] private float difference = 0.15f;
    [SerializeField] private float timeToMax = 6f;

    private void Start()
    {
        minScale = transform.localScale.x;
        maxScale = minScale + difference;
        growthRate = difference / timeToMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x < maxScale)
            transform.localScale += new Vector3(growthRate * Time.deltaTime, growthRate * Time.deltaTime);
    }
}
