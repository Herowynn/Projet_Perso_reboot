using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Inputs")]
    public KeyCode RespawnKey = KeyCode.Tab;
    public KeyCode RestartKey = KeyCode.R;

    private void Update()
    {
        if (Input.GetKeyDown(RespawnKey))
        {
            transform.SetPositionAndRotation(GameManager.Instance.LastCheckpoint, Quaternion.identity);

            GameManager.Instance.AddTimer(2);

            Debug.Log("You respawn at the checkpoint");

            GameManager.Instance.UpdateNbRespawns(false);
        }

        if(Input.GetKeyDown(RestartKey))
        {
            transform.SetPositionAndRotation(GameManager.Instance.StartPoint.position, Quaternion.identity);

            GameManager.Instance.UpdateNbRespawns(true);
            GameManager.Instance.ResetTimer();

            Debug.Log("You restart the run");

            GameManager.Instance.TimeRunning = true;
        }
    }
}
