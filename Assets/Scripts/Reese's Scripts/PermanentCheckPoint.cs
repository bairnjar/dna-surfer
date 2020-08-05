using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentCheckPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.GetComponent<PlayerController>() != null)
        {
            Debug.Log("SETTING NEW CHECKPOINT");
            PlayerPrefs.SetInt("Checkpoint", DNAStrandManager.i.currentLevel);
        }
    }
}
