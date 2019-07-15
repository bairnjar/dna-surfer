using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreGateController : MonoBehaviour {
    public static ScoreGateController firstGate;

    private bool m_didPass = false;

    public static Vector2 StartPosition(int playerNumber, bool start) {
        if (!firstGate) {
            DNAStrandManager.i.SpawnNext();
            DNAStrandManager.i.SpawnNext();
        }
        var startObject = GameObjects.GetComponentWithTagInChildren<Transform>(
            firstGate.gameObject, "SpawnP" + playerNumber);
        return startObject.position;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        var player = collider.gameObject.GetComponent<PlayerController>();
        if (player && !m_didPass) {
            m_didPass = true;
            ScoreManager.i.PassScoreGate(player.playerNumber);
            DNAStrandManager.i.SpawnNext();
        }
    }

    private void Awake() {
        if (!firstGate) {
            firstGate = this;
        }
    }
}
