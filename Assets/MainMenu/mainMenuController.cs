using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void optionsMenu()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    public void Back()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void Level1()
    {
        SceneManager.LoadScene(3, LoadSceneMode.Single);
    }

    public void Level2()
    {
        SceneManager.LoadScene(4, LoadSceneMode.Single);
    }

    public void Level3()
    {
        SceneManager.LoadScene(5, LoadSceneMode.Single);
    }

    public void Level4()
    {
        SceneManager.LoadScene(6, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
