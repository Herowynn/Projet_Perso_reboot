using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI NbRespawnUI;
    public bool TimeRunning = true;
    public Vector3 LastCheckpoint;
    public GameObject Player;
    public Transform StartPoint;
    public string PlayerTag;
    public int NbRespawn = 0;

    public static GameManager Instance;

    float _time = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        LastCheckpoint = StartPoint.position;

        NbRespawnUI.text = "Respawn Count : \n" + NbRespawn.ToString();
    }

    private void Update()
    {
        if (TimeRunning)
        {
            _time += Time.deltaTime;

            Timer.text = FormatTime(_time);
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
