using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Game
{
	public class GameManager : MonoBehaviour
	{
		public static E_GameState State => instance.gameState;
		public enum E_GameState
		{
			Pregame,
			Play,
			End
		}

		public static Action<E_GameState> OnGameStateChange;
		public static int ActivePlayers => instance.activePlayers;

		[SerializeField] private Player[] players;
		[SerializeField] private float pregameTimer;
		[SerializeField] private float gameTimer;

		[Header("HUD")]
		[SerializeField] private TMP_Text topText;

		[Header("Sounds")]
		[SerializeField] private AudioClip snd_letsGo;
		[SerializeField] private AudioClip snd_playerJoin;

		[Header("Scores")]
		[SerializeField] private ScorePopup popupPrefab;
		[SerializeField] private Transform popupCanvas;

		private static GameManager instance;

		private E_GameState gameState;
		private float timeRemaining;
		private int activePlayers;

		private void Awake()
		{
			if(instance != null) { Destroy(instance.gameObject); }
			instance = this;
		}

		private void Start()
		{
			foreach(Player player in players)
			{
				player.gameObject.SetActive(false);
			}

			topText.text = "Press <size=+10><color=green>Button1</color></size> to join!";
			timeRemaining = pregameTimer;
		}

		private void Update()
		{
			switch(gameState)
			{
				case E_GameState.Pregame:
					UpdatePregame();
					break;
				case E_GameState.Play:
					UpdatePlay();
					break;
				case E_GameState.End:
					UpdateEnd();
					break;
			}
		}

		private void UpdatePregame()
		{
			// Player join/leave
			for (int i = 0;i < 4; i++)
			{
				if (Input.GetButtonDown($"P{i+1}Button1"))
				{
					if (!players[i].gameObject.activeSelf)
					{
						players[i].gameObject.SetActive(true);
						activePlayers++;
						AudioManager.Play(snd_playerJoin, pitch: 0.9f + (i*0.1f));
					}
					else
					{
						timeRemaining -= 0.5f;
					}
				}
				if (Input.GetButtonDown($"P{i + 1}Button2"))
				{
					if (players[i].gameObject.activeSelf)
					{
						players[i].gameObject.SetActive(false);
						activePlayers--;
						if(activePlayers < 1)
						{
							timeRemaining = pregameTimer;
						}
					}
				}
			}

			if(activePlayers >= 1)
			{
				timeRemaining -= Time.deltaTime;
				topText.text = $"<size=+20>{Mathf.Ceil( timeRemaining)}</size>\nPress <size=+10><color=green>Button1</color></size> to join!";
			}
			else
			{
				topText.text = $"Press <size=+10><color=green>Button1</color></size> to join!";
			}

			//Start game
			if(timeRemaining <= 0)
			{
				gameState = E_GameState.Play;
				OnGameStateChange?.Invoke(gameState);
				timeRemaining = gameTimer;
				AudioManager.Play(snd_letsGo);
			}
		}

		private void UpdatePlay()
		{
			timeRemaining -= Time.deltaTime;
			topText.text = $"<size=+25>{Mathf.Ceil(timeRemaining)}</size>";
		}

		private void UpdateEnd()
		{

		}

		public static void ShowPoints(Vector3 pos, int score)
		{
			ScorePopup bub = Instantiate(instance.popupPrefab, pos, Quaternion.identity);
			bub.transform.SetParent(instance.popupCanvas, true);
			bub.Popup(score);
		}
	}
}