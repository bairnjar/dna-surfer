using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.tag == "Player") {
            var player = PlayerController.i;
            player.Collect(this);
            GameObject.Destroy(this);
        }
    }
}
