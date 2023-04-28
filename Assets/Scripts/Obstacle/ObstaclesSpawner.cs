using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesSpawner : MonoBehaviour
{
	public GameObject ObstaclesPrefab;

	private void Start()
	{
		StartCoroutine(SpawnObstacle());
	}

	private IEnumerator SpawnObstacle()
	{
		yield return new WaitForSeconds(Random.Range(.5f, 3f));

		GameObject obstacle = Instantiate(ObstaclesPrefab);
		obstacle.GetComponent<Obstacle>().Movespeed = Random.Range(3.5f, 10f);
		obstacle.transform.position = new Vector3(Random.Range(0.5f, 10f), Random.Range(0f, 6f), transform.position.z);

		StartCoroutine(SpawnObstacle());
	}
}
