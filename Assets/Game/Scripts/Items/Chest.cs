using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class Chest : Item
	{
		const string ANI_OPEN = "chestOpen";

		[SerializeField] private float spawnHeight;
		[SerializeField] private float waterHeight;
		[SerializeField] private float groundHeight;
		[SerializeField] private float speed;
		[SerializeField] private float gravity;

		[Header("Effects")]
		[SerializeField] private ParticleSystem ps_bubbles;

		public override void Spawn(ItemManager manager)
		{
			base.Spawn(manager);
			transform.position = new Vector3(
				Random.Range(manager.SpawnBounds.x, manager.SpawnBounds.z),
				spawnHeight);
		}

		public override void OnHook(Player player)
		{
			if (transform.position.y <= groundHeight && !hooked)
			{
				GetComponent<Animator>().Play(ANI_OPEN);
				ps_bubbles.Emit(75);
				hooked = true;
			}
		}

		private void Update()
		{
			if( transform.position.y > waterHeight) // above water
			{
				speed += gravity * Time.deltaTime;
				transform.position += new Vector3(0, speed) * Time.deltaTime;
				if(transform.position.y <= waterHeight) // hit water
				{
					speed *= 0.10f;
					ps_bubbles.Emit(55);
				}
			}
			else if(transform.position.y > groundHeight)
			{
				speed += gravity * 0.5f * Time.deltaTime;
				transform.position += new Vector3(0, speed) * Time.deltaTime;
				if(Random.value > 0.95f)
					ps_bubbles.Emit(1);
				if(transform.position.y <= groundHeight) // hit grolund
				{
					ps_bubbles.Emit(100);
					causeDamage = false;
				}
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(new Vector3(-50, spawnHeight), new Vector3(50, spawnHeight));
			Gizmos.DrawLine(new Vector3(-50, waterHeight), new Vector3(50, waterHeight));
			Gizmos.DrawLine(new Vector3(-50, groundHeight), new Vector3(50, groundHeight));

		}
	}
}