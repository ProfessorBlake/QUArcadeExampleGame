using System.Data;
using UnityEngine;

namespace Game
{
	public class Fish : Item
	{
		[SerializeField] private Vector2 speedRange;
		[SerializeField] private float waveFreq;
		[SerializeField] private float waveAmp;
		[SerializeField] private SpriteRenderer[] baseColorSprites;
		[SerializeField] private SpriteRenderer[] darkColorSprites;
		[SerializeField] private Transform spriteTransform;
		[Tooltip("Value from 0-1 for how far down this fish should spawn.")]
		[SerializeField] [Range(0f,1f)] private float depthModifier;

		private float speed;    // Direction set in spawn
		private float startPosY;
		private float spawnTime;
		private Transform resetParent;
		private float resetRot;

		public override void Spawn(ItemManager manager)
		{
			base.Spawn(manager);
			
			transform.position = new Vector3(
				manager.SpawnBounds.x * Mathf.Sign(Random.value - 0.5f),
				Mathf.Lerp( Random.Range(manager.SpawnBounds.y, manager.SpawnBounds.w), manager.SpawnBounds.w, depthModifier));
			startPosY = transform.position.y;

			speed = Random.Range(speedRange.x, speedRange.y) * Mathf.Sign(-transform.position.x);

			//Sprite
			spriteTransform.eulerAngles = new Vector3(0, (speed > 0) ? 0f : 180, spriteTransform.eulerAngles.z);    // rotate on y axis
			resetRot = spriteTransform.eulerAngles.y;
			// Colors
			if (colorOptions.Length > 0)
			{
				foreach (SpriteRenderer sr in baseColorSprites)
				{
					sr.color = color;
				}
				foreach (SpriteRenderer sr in darkColorSprites)
				{
					sr.color = color * 0.8f;
				}
			}

			//Value
			value = value + Mathf.CeilToInt(size * 10);

			spawnTime = Time.time;

			resetParent = transform.parent;
		}

		private void Update()
		{
			if (!hooked)
			{

				transform.position = new Vector3(
					transform.position.x + speed * Time.deltaTime,
					startPosY + Mathf.Sin(spawnTime + Time.time * waveFreq) * waveAmp
					);
			}

			if (OffScreen())
			{
				Destroy(gameObject);
			}
		}

		public void SpeedOff()
		{
			speed *= 10;
			canCatch = false;
		}

		public override void OnHook(Player player)
		{
			base.OnHook(player);
			if (hooked)
			{
				transform.SetParent(player.HarpoonTransform);
			}
			else // drop
			{
				speed *= 1.5f;
				startPosY = transform.position.y;
				transform.SetParent(resetParent, true);
				transform.eulerAngles = Vector3.zero;
				spriteTransform.eulerAngles = new Vector3(0, resetRot, spriteTransform.eulerAngles.z);    // rotate on y axis
			}
		}

	}
}
