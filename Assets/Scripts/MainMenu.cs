using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject Menu;
    public GameObject LevelSelection;

    public void OnClickStart()
    {
        Menu.SetActive(false);
        LevelSelection.SetActive(true);
    }

    public void LaunchGame()
    {
        SceneManager.LoadScene(1);
    }
}
