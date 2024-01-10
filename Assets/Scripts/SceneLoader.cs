using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
   public void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void LoadTutorial()
    {
        TutorialManager.popUPIndex = 0;
        SceneManager.LoadScene("TutorialScene");

    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("MenuScene");

    }
    public void LoadLanguages()
    {
        SceneManager.LoadScene("LanguageScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
