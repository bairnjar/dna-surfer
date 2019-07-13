
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum COLLECTIBLETYPE { HEALTHUP, HEALTHDOWN, NONE }




public class Collectible : MonoBehaviour {



    public COLLECTIBLETYPE collectibleType;

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.tag == "Player") {
            if (collectibleType == COLLECTIBLETYPE.HEALTHUP) {

            }
            var player = PlayerController.i;
            player.Collect(this);
            // Debug.Log("DestroyPRefab");
            GameObject.Destroy(gameObject);
        }
    }
}
