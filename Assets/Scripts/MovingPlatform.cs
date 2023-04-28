using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MovingPlatform : MonoBehaviour
{
    public List<Transform> Nodes = new List<Transform>();
	public float PlatformSpeed;

	[SerializeField] private int _nextNode;
	private float _timer;

	private void Start()
	{
		_nextNode = 0;
	}

	private void Update()
	{
		_timer += Time.deltaTime * PlatformSpeed;
		if (Mathf.Abs(transform.position.x - Nodes[_nextNode].position.x) < 2f &&
			Mathf.Abs(transform.position.y - Nodes[_nextNode].position.y) < 2f &&
			Mathf.Abs(transform.position.z - Nodes[_nextNode].position.z) < 2f)
		{
			_nextNode = _nextNode != (Nodes.Count - 1) ? _nextNode + 1 : 0;
			_timer = 0;
		}
	}

	private void FixedUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, Nodes[_nextNode].position, _timer);
	}
}
