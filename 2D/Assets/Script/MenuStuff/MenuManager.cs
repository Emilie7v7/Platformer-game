using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour

{

     public int gameStartScene;
     public int gameExitScene;
    
    
    //Quits the application
    public void QuitGame()
    {
        Application.Quit();
    }   
    //Starts new game
    public void StartGame()
    {
        SceneManager.LoadScene(gameStartScene);
    }
    //Exits to main menu
    public void ExitToMenu()
    {
        SceneManager.LoadScene(gameExitScene);
    }
    
    public void GameOver()
    {
       
    }
}
