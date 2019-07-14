using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreGateController : MonoBehaviour {
    public static ScoreGateController firstGate;
    public static ScoreGateController lastGate;

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Player") {
            PlayerController.i.IncrScore();
            DNAStrandManager.i.SpawnNext();
        }
    }

    private void Awake() {
        if (!firstGate) {
            firstGate = this;
        }
        lastGate = this;
    }
}
