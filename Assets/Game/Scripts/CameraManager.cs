using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class CameraManager : MonoBehaviour
	{
		[SerializeField] private float falloff;

		private static CameraManager instance;

		private float shake;
		private Vector3 startPos;

		private void Awake()
		{
			if(instance != null) { Destroy(instance.gameObject); }
			instance = this;
		}

		private void Start()
		{
			startPos = transform.position;
		}

		/// <summary>
		/// If new shake is more than current, replace
		/// </summary>
		/// <param name="power"></param>
		public static void AddShake(float power)
		{
			if(power > instance.shake)
			{
				instance.shake = power;
			}
		}

		private void Update()
		{
			if(shake > 0.01f)
			{
				shake = Mathf.Lerp(shake, 0f, falloff * Time.deltaTime);
			}
			transform.position = Vector3.Lerp(startPos, 
				startPos + new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f), startPos.z) * shake, 
				shake * Time.deltaTime);
		}
	}
}