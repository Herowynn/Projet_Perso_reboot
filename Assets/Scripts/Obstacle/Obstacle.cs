using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
	public float Movespeed;

	private void Update()
	{
		transform.Translate(Vector3.back * Movespeed * Time.deltaTime);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("WallEnd") == true)
		{
			Destroy(gameObject);
		}
	}
}
