using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI NbRespawnUI;
    public bool TimeRunning = false;
    public Vector3 LastCheckpoint;
    public GameObject Player;
    public Transform StartPoint;
    public string PlayerTag;
    public int NbRespawn = 0;
    public string LevelName;
    public int CountDownTime = 3;

	[Header("Game State")]
	public GameState CurrentPlayerState;

    [Header("UI")]
	public GameObject TimerUI;
    public GameObject CountdownUI;
    public TextMeshProUGUI CountdownText;
    public GameObject EndScreenUI;
    public TextMeshProUGUI ScoreText;

    public static GameManager Instance;

    float _time = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        LastCheckpoint = StartPoint.position;

        NbRespawnUI.text = "Respawn Count : \n" + NbRespawn.ToString();

        CurrentPlayerState = GameState.COUNTDOWN;
    }

    private void Start()
    {
        TimerUI.SetActive(true);
        EndScreenUI.SetActive(false);
        CountdownUI.SetActive(true);

        Player.transform.position = StartPoint.position;
		Player.SetActive(false);
		StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        while(CountDownTime > 0)
        {
            CountdownText.text = CountDownTime.ToString();

			yield return new WaitForSecondsRealtime(1f);

			CountDownTime--;
		}

        Player.SetActive(true);
        CountdownText.text = "GO !";

        CurrentPlayerState = GameState.INGAME;
        TimeRunning = true;

        yield return new WaitForSeconds(1f);

        CountdownUI.SetActive(false);    
    }

    private void Update()
    {
        if (TimeRunning)
        {
            _time += Time.deltaTime;

            Timer.text = FormatTime(_time);
        }

        else if(!TimeRunning && CurrentPlayerState == GameState.INGAME)
        {
            CurrentPlayerState = GameState.ENDGAME;

            Player.SetActive(false);
            TimerUI.SetActive(false);

            Debug.Log("Bravo ! Tu as gagné en " + FormatTime(_time));

            string highScoreKey = LevelName + "_" + "HighScore";

            if (PlayerPrefs.HasKey(highScoreKey))
            {
                if(_time < PlayerPrefs.GetFloat(highScoreKey))
                {
                    PlayerPrefs.SetFloat(highScoreKey, _time);
                    PlayerPrefs.Save();
                }
            }
            else
            {
                if(_time < PlayerPrefs.GetFloat(highScoreKey))
                {
                    PlayerPrefs.SetFloat(highScoreKey, _time);
                    PlayerPrefs.Save();
                }
            }

			EndScreenUI.SetActive(true);

            ScoreText.text = "your actual score \n " + FormatTime(_time) + 
                "\n \n your best score \n" + FormatTime(PlayerPrefs.GetFloat(highScoreKey));
		}
    }

    public void UpdateNbRespawns(bool reload)
    {
        if (reload) NbRespawn = 0;
        else NbRespawn++;
        NbRespawnUI.text = "Respawn Count : \n" + NbRespawn.ToString();
    }

    public void ResetTimer()
    {
        _time = 0f;
        Timer.text = FormatTime(0f);
    }

    public void AddTimer(float time)
    {
        _time += time;
        Timer.text = FormatTime(_time);
    }

    string FormatTime(float time)
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
