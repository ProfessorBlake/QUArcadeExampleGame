using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class AudioManager : MonoBehaviour
	{
		[SerializeField] private AudioSource aSource;

		private static AudioManager instance;

		private void Awake()
		{
			if(instance != null) { Destroy(instance.gameObject); }
			instance = this;
		}

		public static void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
		{
			instance.aSource.pitch = pitch;
			instance.aSource.PlayOneShot(clip, volume);
		}
	}
}