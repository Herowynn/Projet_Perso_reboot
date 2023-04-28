using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
    public Ghost GhostRecord;
	public Ghost GhostReplay;

    private float _timer;
    private float _timeValue;

	private void Awake()
	{
		if(GhostRecord.IsRecording)
		{
			GhostRecord.ResetData();
			_timeValue = 0;
			_timer = 0;
		}
	}

	private void Update()
	{
		_timer += Time.unscaledDeltaTime;
		_timeValue += Time.unscaledDeltaTime;

		if(GhostRecord.IsRecording & _timer >= 1 / GhostRecord.RecordFrequency)
		{
			GhostRecord.TimeStamps.Add( _timeValue );
			GhostRecord.Positions.Add(transform.position );
			GhostRecord.Rotations.Add(transform.eulerAngles);

			_timer = 0;
		}
	}
}
