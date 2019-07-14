using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNACollider : MonoBehaviour {

    [SerializeField] private bool hardCollider = true;


    private void OnTriggerEnter2D(Collider2D collider) {

        if (collider.tag == "Player" && hardCollider) {
            PlayerController.i.SetHealth(0);
        }
        else if (collider.tag == "Player" && !hardCollider)
        {
            Debug.Log("Distress!");
            PlayerController.i.InDistress();
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {

        if (collider.tag == "Player" && !hardCollider)
        {
            Debug.Log("Distress!");
            PlayerController.i.InDistress();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {

        if (collider.tag == "Player" && !hardCollider)
        {
            PlayerController.i.EndDistress();

        }
    }
}
