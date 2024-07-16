using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
	public class ItemManager : MonoBehaviour
	{
		public Vector4 SpawnBounds => spawnBounds;

		[Header("Spawn")]
		[SerializeField] private List<SpawnableItem> spawnableItems = new List<SpawnableItem>();
		private float totalSpawnWeight;

		[Header("Other")]
		[SerializeField] private Vector4 spawnBounds;
		[SerializeField] private Vector2 spawnDelayRange;
		private float spawnDelay;

		private List<Item> spawnedItems = new List<Item>();

		private void Start()
		{
			totalSpawnWeight = 0f;
			for (int i = 0; i < spawnableItems.Count; i++)
			{
				totalSpawnWeight += spawnableItems[i].Weight;
			}

			spawnDelay = Random.Range(spawnDelayRange.x, spawnDelayRange.y);
			GameManager.OnGameStateChange += HandleGameStateChange;
		}

		private void OnDisable()
		{
			GameManager.OnGameStateChange -= HandleGameStateChange;
		}


		private void Update()
		{
			spawnDelay -= Time.deltaTime;
			if(spawnDelay <= 0f)
			{
				SpawnItem();
			}
		}

		/// <summary>
		/// Spawns items based on weight
		/// modified from: https://gamedev.stackexchange.com/questions/119623/set-a-chance-to-spawn-for-each-gameobject-in-an-array
		/// </summary>
		private void SpawnItem()
		{
			float randWeight = Random.value * totalSpawnWeight;
			int chosenIndex = 0;
			float cumulativeWeight = spawnableItems[0].Weight;

			if (GameManager.State == GameManager.E_GameState.Pregame) //Spawn fish in pregame only
			{
				chosenIndex = 0;
			}
			else
			{
				while (randWeight > cumulativeWeight && chosenIndex < spawnableItems.Count - 1)
				{
					chosenIndex++;
					cumulativeWeight += spawnableItems[chosenIndex].Weight;
				}
			}			

			// Spawn the chosen item.
			Item newItem = Instantiate(spawnableItems[chosenIndex].ItemPrefab, new Vector3(1000,1000,0), Quaternion.identity);
			newItem.transform.SetParent(transform);
			spawnedItems.Add(newItem);
			newItem.Spawn(this);

			spawnDelay = Random.Range(spawnDelayRange.x, spawnDelayRange.y) * (1f - GameManager.ActivePlayers * 0.15f);
		}

		public void ItemCaught(Item item)
		{
			spawnedItems.Remove(item);
			Destroy(item.gameObject);
		}


		private void HandleGameStateChange(GameManager.E_GameState state)
		{
			if(state == GameManager.E_GameState.Play)
			{
				foreach (Fish fish in spawnedItems)
				{
					fish.SpeedOff();
				}
				spawnDelay = 2f;
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(new Vector3(spawnBounds.x, spawnBounds.w), new Vector3(spawnBounds.x, spawnBounds.y));
			Gizmos.DrawLine(new Vector3(spawnBounds.x, spawnBounds.y), new Vector3(spawnBounds.z, spawnBounds.y));
			Gizmos.DrawLine(new Vector3(spawnBounds.z, spawnBounds.y), new Vector3(spawnBounds.z, spawnBounds.w));
			Gizmos.DrawLine(new Vector3(spawnBounds.z, spawnBounds.w), new Vector3(spawnBounds.x, spawnBounds.w));
		}
	}
}
