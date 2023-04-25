using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Ghost : ScriptableObject
{
    public bool IsRecording;
    public bool IsReplaying;
    public float RecordFrequency;

    public List<float> TimeStamps;
    public List<Vector3> Positions;
    public List<Vector3> Rotations;

    public void ResetData()
    {
        TimeStamps.Clear();
        Positions.Clear();
        Rotations.Clear();
    }
}
