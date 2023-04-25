using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    public Ghost Ghost;

    private float _timeValue;
    private int _index1;
    private int _index2;

	private void Awake()
	{
		_timeValue = 0;
	}

	private void Update()
	{
		_timeValue += Time.unscaledDeltaTime;

		if(Ghost.IsReplaying)
		{
			GetIndex();
			SetTransform();
		}
	}

	private void GetIndex()
	{
		for (int i = 0; i < Ghost.TimeStamps.Count; i++)
		{
			if (Ghost.TimeStamps[i] == _timeValue)
			{
				_index1 = i;
				_index2 = i;
				return;
			}
			else if (Ghost.TimeStamps[i] < _timeValue && _timeValue < Ghost.TimeStamps[i + 1])
			{
				_index1 = i;
				_index2 = i + 1;
				return;
			}
		}

		_index1 = Ghost.TimeStamps.Count - 1;
		_index2 = Ghost.TimeStamps.Count - 1;
	}

	private void SetTransform()
	{
		if(_index1 == _index2)
		{
			transform.position = Ghost.Positions[_index1];
			transform.eulerAngles = Ghost.Rotations[_index1];
		}
		else
		{
			float interpolationFactor = (_timeValue - Ghost.TimeStamps[_index1]) / (Ghost.TimeStamps[_index2] - Ghost.TimeStamps[_index1]);

			transform.position = Vector3.Lerp(Ghost.Positions[_index1], Ghost.Positions[_index2], interpolationFactor);
			transform.eulerAngles = Vector3.Lerp(Ghost.Rotations[_index1], Ghost.Rotations[_index2], interpolationFactor);
		}
	}
}
