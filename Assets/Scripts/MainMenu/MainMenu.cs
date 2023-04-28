using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Menus")]
    public GameObject Menu;
    public GameObject LevelSelection;
    public GameObject OptionsMenu;
    public TextMeshProUGUI HighScore;

    private string _levelSelected;

	private void Awake()
	{
		if(!PlayerPrefs.HasKey("HorizontalSentivity") && !PlayerPrefs.HasKey("VerticalSensitivity"))
        {
            PlayerPrefs.SetFloat("HorizontalSensitivity", .2f);
            PlayerPrefs.SetFloat("VerticalSensitivity", .2f);
        }
	}

	public void OnClickStart()
    {
        Menu.SetActive(false);
        LevelSelection.SetActive(true);
    }

    public void OnClickOptions()
    {
        Menu.SetActive(false);
        OptionsMenu.SetActive(true);
    }

    public void LevelSelectionButton(string levelName)
    {
        _levelSelected = levelName;
		string highScoreKey = levelName + "_" + "HighScore";

        if(PlayerPrefs.HasKey(highScoreKey))
		    HighScore.text = $"best time for {levelName} : \n {FormatTime(PlayerPrefs.GetFloat(highScoreKey))}";

        else
			HighScore.text = $"best time for {levelName} : \n 00:00:00";

	}

	public void LaunchGame()
    {
        SceneManager.LoadScene(_levelSelected);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

	private string FormatTime(float time)
	{
		int intTime = (int)time;
		int minutes = intTime / 60;
		int seconds = intTime % 60;
		float miliseconds = time * 1000;
		miliseconds = (miliseconds % 1000);
		string timeText = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, miliseconds);

		return timeText;
	}
}
