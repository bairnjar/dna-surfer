using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNACollider : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Player") {
            PlayerController.i.SetHealth(0);
        }
    }
}
