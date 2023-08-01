using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectObject : MonoBehaviour
{

    private bool isPickedUp = false;

    private Object thisObject;
    
    private void Awake()
    {
        thisObject = GetComponent<Object>();
    }


    //Picking up the cherries, "destroying them" and adding to the counter
    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.CompareTag("Player") && !isPickedUp)
        {
            isPickedUp = true;
            PlayerPrefs.SetInt(thisObject.ID, PlayerPrefs.GetInt(thisObject.ID) + 1);
            Destroy(gameObject);
        }

    }
}