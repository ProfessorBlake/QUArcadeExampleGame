using UnityEngine;

namespace Game
{
	public class Water : MonoBehaviour
	{
		[SerializeField] private Transform[] waves;
		[SerializeField] private float spacing;
		[SerializeField] private float speed;
		[SerializeField] private float spawnBounds;

		private void Start()
		{
			Setup();
		}

		private void OnValidate()
		{
			Setup();
		}

		private void Setup()
		{
			for (int i = 0; i < waves.Length; i++)
			{
				waves[i].transform.position = new Vector3(-spawnBounds + (i * spacing) * Random.Range(0.95f,1.05f), waves[i].position.y, 0);
			}
		}

		private void Update()
		{
			for (int i = 0; i < waves.Length; i++)
			{
				waves[i].transform.position += new Vector3((Mathf.Sin(Time.time * speed) + 0.75f) * Time.deltaTime, 0, 0);
				if (waves[i].position.x > spawnBounds)
				{
					waves[i].position = new Vector3(-waves[i].position.x, waves[i].position.y);
				}
			}
		}
	}
}