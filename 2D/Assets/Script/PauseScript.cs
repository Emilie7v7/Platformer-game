using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseGame;

public void Update()
{
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseGame.SetActive(!pauseGame.activeSelf);

        }
        
    }

}
