using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNACollider : MonoBehaviour {
    public bool hardCollider = true;
    public bool levelExit = false;

    private void OnTriggerEnter2D(Collider2D collider) {
        var player = GetPlayer(collider);
        if (player) {
            if (hardCollider)
            {
                player.SetHealth(0);
            }
            else if (levelExit)
            {
                player.exitLevel();
            }
            else { 
                player.InDistress();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collider) {
        var player = GetPlayer(collider);
        if (player && !hardCollider) {
            player.InDistress();
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        var player = GetPlayer(collider);
        if (player && !hardCollider) {
            player.EndDistress();
        }
    }

    private PlayerController GetPlayer(Collider2D collider) {
        return collider.gameObject.GetComponent<PlayerController>();
    }
}
