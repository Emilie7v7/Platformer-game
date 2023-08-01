using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    Practice bunny;
    Practice bird;

    // Start is called before the first frame update
    void Start()
    {
        bunny = new Practice(1,1, "Bunny");
        bird = new Practice (1,1, "Bird");  
    }
}
