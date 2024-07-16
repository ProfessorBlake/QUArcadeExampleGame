using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Game
{
	public class ScorePopup : MonoBehaviour
	{
		[SerializeField] private TMP_Text txt;
		[SerializeField] private float time;

		public void Popup(int score)
		{
			txt.text = $"+{score}";
			//transform.localScale = Vector3.one * 0.01f;
			transform.DOScale(Vector3.one, 2f).SetEase(Ease.OutBounce,1.4f);
		}

		private void Update()
		{
			transform.position += new Vector3(Random.Range(-1f, 1f), 1f) * Time.deltaTime;
			 time -= Time.deltaTime;
			if (time < 0)
			{
				Destroy(gameObject);
			}
		}
	}
}