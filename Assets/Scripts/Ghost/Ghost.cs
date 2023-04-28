using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Ghost : ScriptableObject
{
    #region Variables

    [Header("Parameters")]
	public bool IsRecording;
    public bool IsReplaying;
    public float RecordFrequency;

    [Header("Lists")]
    public List<float> TimeStamps;
    public List<Vector3> Positions;
    public List<Vector3> Rotations;

    #endregion

    public void ResetData()
    {
        TimeStamps.Clear();
        Positions.Clear();
        Rotations.Clear();
    }
}
