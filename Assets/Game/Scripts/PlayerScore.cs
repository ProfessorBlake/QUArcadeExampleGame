using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game
{
	public class PlayerScore : MonoBehaviour
	{
		[SerializeField] private TMP_Text text;

		private int playerId;
		private Color color;
		private int score;
		private float tmpScore;
		private float delay;
		private float rot;
		private Vector2 offset;
		private float scale = 1f;

		private void OnEnable()
		{
			
		}

		private void OnDisable()
		{
			
		}

		public void Init(int id, Color col)
		{
			playerId = id;
			color = col;
			text.color = Color.Lerp( col, Color.white, 0.5f);
			text.text = $"<size=-15>Player {playerId}</size>\n0";
		}

		public void SetScore(int newScore)
		{
			score = newScore;
		}

		private void Update()
		{
			if(GameManager.State == GameManager.E_GameState.Play)
			{
				delay -= Time.deltaTime;
				if (delay <= 0)
				{
					if (score + 0f > tmpScore)
					{
						tmpScore += Mathf.Sign(score - tmpScore);
						delay = Mathf.Clamp(1 / (score - tmpScore), 0.001f, 0.25f);
						text.text = $"<size=-15>Player {playerId}</size>\n" + Mathf.Ceil(tmpScore);
						rot = Random.Range(-10f, 10f);
						offset = new Vector2(Random.Range(-20f, 20f), Random.Range(-20f, 20f));
						scale = Random.Range(1.1f, 1.2f);
						text.color = Color.white;
					}
				}

				rot = Mathf.Lerp(rot, 0, 50 * Time.deltaTime);
				offset = Vector2.Lerp(offset, Vector2.zero, 50 * Time.deltaTime);
				scale = Mathf.Lerp(scale, 1f, 10 * Time.deltaTime);

				text.rectTransform.localPosition = offset;
				text.rectTransform.eulerAngles = new Vector3(0, 0, rot);
				text.rectTransform.localScale = Vector3.one * scale;
				text.color = Color.Lerp(text.color, color, 10f * Time.deltaTime);
			}
		}
	}
}