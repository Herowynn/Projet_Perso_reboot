using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishFlag : MonoBehaviour
{
    private string _playerTag;

    private void Start()
    {
        _playerTag = GameManager.Instance.PlayerTag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == _playerTag)
        {
            GameManager.Instance.TimeRunning = false;

            Debug.Log("Tu as gagné !  Bravo !");
        }
    }
}
