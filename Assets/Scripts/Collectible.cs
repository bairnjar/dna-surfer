
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum COLLECTIBLETYPE { HEALTHUP, HEALTHDOWN, COIN }

public class Collectible : MonoBehaviour {
    public COLLECTIBLETYPE collectibleType;

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Player") {
            collider.gameObject.GetComponent<PlayerController>().Collect(this);
            GameObject.Destroy(this.gameObject);
        }
    }
}
