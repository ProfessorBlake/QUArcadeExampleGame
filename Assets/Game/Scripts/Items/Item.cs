using UnityEngine;

namespace Game
{
	/// <summary>
	/// Item base calss.
	/// Items can be harpooned by players, and returned to surface for points
	/// </summary>
	public abstract class Item : MonoBehaviour
	{
		public bool CauseDamage => causeDamage;
		public bool CanCatch => canCatch;
		public float SlowPlayerMulti => slowPlayerMulti;
		public int Value => value;
		public bool Hooked => hooked;

		[SerializeField] protected Vector2Int valueRange;
		[SerializeField] protected Vector2 sizeRange;
		[SerializeField] protected Vector2 slowPlayerMultiRange;
		[SerializeField] protected Color[] colorOptions;
		[SerializeField] protected bool causeDamage;
		[SerializeField] protected bool canCatch;

		protected int value;
		protected float size;
		protected float slowPlayerMulti; // Player move * this speed when carrying
		protected Color color;
		protected bool hooked;      // True while hooked to a players harpoon
		protected ItemManager itemManager;
		protected Player hookedByPlayer;

		/// <summary>
		/// Override Spawn to change behavior on spawining items
		/// </summary>
		public virtual void Spawn(ItemManager manager)
		{
			itemManager = manager;
			value = Random.Range(valueRange.x, valueRange.y);
			size = Random.Range(sizeRange.x, sizeRange.y);
			transform.localScale = Vector3.one * size;
			slowPlayerMulti = Random.Range(slowPlayerMultiRange.x, slowPlayerMultiRange.y);
			if(colorOptions.Length > 0) color = colorOptions[Random.Range(0,colorOptions.Length)];
			
		}

		/// <summary>
		/// Called when a players harpoon hits the item
		/// </summary>
		public virtual void OnHook(Player player)
		{
			if(canCatch)
			{
				hooked = player != null ? true : false;
				hookedByPlayer = player;
			}
		}

		public virtual void Caught()
		{
			itemManager.ItemCaught(this);
		}

		protected bool OffScreen()
		{
			if (transform.position.x < itemManager.SpawnBounds.x ||
				transform.position.x > itemManager.SpawnBounds.z)
			{
				return true;
			}
			return false;
		}
	}
}