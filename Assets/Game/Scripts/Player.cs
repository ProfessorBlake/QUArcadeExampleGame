using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
	public class Player : MonoBehaviour
	{
		public Transform RotateTransform => rotateTransform;
		public Transform HarpoonTransform => harpoonTranform;

		[SerializeField] private int playerID = 1;       // 1 - 4 used to match InputManager keys
		private string inputPrefix = "P1";  // "P1" for example

		[Header("Movement")]
		[SerializeField] private float moveSpeed;
		[SerializeField] private float rotSpeed;
		[SerializeField] private Transform rotateTransform;
		[SerializeField] private Vector4 swimBounds;
		private Vector3 moveDir;

		[Header("Collisions")]
		[SerializeField] private float injureTimeReset;
		[SerializeField] private float collisionRadius;
		private float injureTime;
		private Collider2D[] hits = new Collider2D[2];

		[Header("Harpoon")]
		[SerializeField] private Transform harpoonTranform;
		[SerializeField] private float harpoonFlightTime = 0.5f;
		[SerializeField] private float harpoonSpeed = 10f;
		[SerializeField] private int harpoonState = 0; // idle, move forward, retract
		private float harpoonStopTime;
		private Vector3 harpoonStartPos;
		private Item hookedItem;

		[Header("Boost")]
		[SerializeField] private float boostCooldown = 5f;
		[SerializeField] private float boostDuration = 1f;
		[SerializeField] private float boostStrength = 2f;
		[SerializeField] private SpriteRenderer boostReadySprite;
		private float boostAvailableTime;
		private float boostEndTime;

		[Header("Effects")]
		[SerializeField] private ParticleSystem ps_bubbles;
		[SerializeField] private ParticleSystem ps_damage;
		[SerializeField] private ParticleSystem ps_score;
		[SerializeField] private float shakeTakeDamage;
		[SerializeField] private float shakeCatchItem;
		[SerializeField] private float shakeGetPoints;

		[Header("Score")]
		[SerializeField] private PlayerScore playerScore;
		[SerializeField] private SpriteRenderer playerSprRend;
		[SerializeField] private GameObject scoreContainer;
		private int score;

		[Header("Sounds")]
		[SerializeField] private AudioClip snd_fireHook;
		[SerializeField] private AudioClip snd_hookHit;
		[SerializeField] private AudioClip snd_boost;
		[SerializeField] private AudioClip snd_hurt;
		[SerializeField] private AudioClip snd_returnCatch;
		[SerializeField] private AudioClip snd_goodCatch;
		[SerializeField] private AudioClip snd_greatCatch;

		private void OnEnable()
		{
			scoreContainer.SetActive(true);
		}

		private void OnDisable()
		{
			scoreContainer.SetActive(false);
		}

		private void Start()
		{
			inputPrefix = "P" + playerID;
			harpoonState = 0;
			harpoonStartPos = harpoonTranform.localPosition;
			boostReadySprite.color = Color.black;
			boostAvailableTime = Time.time + boostCooldown;
			playerScore.Init(playerID, playerSprRend.color) ;
		}

		private void Update()
		{
			GetInput();
			if(GameManager.State == GameManager.E_GameState.Play)
			{
				Harpoon();
				Boost();
				CheckCollect();
				CheckCollisions();
			}
			MovePlayer();
		}

		private void GetInput()
		{
			// Movement
			moveDir = new Vector3(
				Input.GetAxisRaw(inputPrefix + "Horizontal"),
				Input.GetAxisRaw(inputPrefix + "Vertical")
				);

			if (GameManager.State == GameManager.E_GameState.Play)
			{
				//Harpoon
				if (Input.GetButtonDown(inputPrefix + "Button1"))
				{
					if (harpoonState == 0 && injureTime <= 0f && hookedItem == null)
					{
						harpoonState = 1;
						harpoonStopTime = Time.time + harpoonFlightTime;
						ParticleSystem.EmitParams parms = new ParticleSystem.EmitParams();
						parms.position = harpoonTranform.position;
						ps_bubbles.Emit(parms, 4);
						AudioManager.Play(snd_fireHook, pitch: Random.Range(0.9f, 1.1f));
					}
				}

				// Boost
				if (Input.GetButtonDown(inputPrefix + "Button2"))
				{
					if (Time.time >= boostEndTime && Time.time >= boostAvailableTime) // Use boost
					{
						boostEndTime = Time.time + boostDuration;
						boostAvailableTime = Time.time + boostDuration + boostCooldown;
						boostReadySprite.color = Color.green;
						AudioManager.Play(snd_boost, pitch: Random.Range(0.9f, 1.1f));
					}
				}
			}
		}

		private void Harpoon()
		{
			if (harpoonState == 1) // Flying forward
			{
				if (Time.time < harpoonStopTime)
				{

					if (Random.value > 0.99f)
					{
						ParticleSystem.EmitParams parms = new ParticleSystem.EmitParams();
						parms.position = harpoonTranform.position;
						ps_bubbles.Emit(parms, 1);
					}
					harpoonTranform.position += rotateTransform.right * harpoonSpeed * Time.deltaTime;

					Collider2D[] hits = Physics2D.OverlapCircleAll(harpoonTranform.position, 0.1f);
					if(hits.Length > 0)
					{
						foreach(Collider2D hit in hits)
						{
							Item item = hit.GetComponent<Item>();
							if(item && injureTime <= 0f && !hookedItem)	// Hook item
							{
								if(item.CanCatch)
									hookedItem = item;
								item.OnHook(this);
								CameraManager.AddShake(shakeCatchItem);
								AudioManager.Play(snd_hookHit, pitch: Random.Range(0.9f, 1.1f));
								return;
							}
						}
					}
				}
				else
				{
					harpoonState = 2;
					harpoonStopTime = Time.time + harpoonFlightTime * 0.75f;
				}
			}
			else if (harpoonState == 2) // retract
			{
				if (Time.time < harpoonStopTime)
				{
					harpoonTranform.position += -rotateTransform.right * harpoonSpeed * 1.75f * Time.deltaTime;
				}
				else
				{
					harpoonTranform.localPosition = harpoonStartPos;
					harpoonState = 0; // reset
				}
			}
		}

		private void Boost()
		{
			if (Time.time > boostEndTime) // Not currently boosting
			{
				if (Time.time < boostAvailableTime) boostReadySprite.color = Color.black;           // Boost not available
				else if (Time.time >= boostAvailableTime) boostReadySprite.color = Color.white; // Boost ready
			}
			else
			{
				if (Random.value > 0.99f)
					ps_bubbles.Emit(1);
			}
		}

		private void CheckCollect()
		{
			if (hookedItem && injureTime <= 0)
			{
				if(transform.position.y >= swimBounds.y) // Collect
				{
					AudioManager.Play(snd_returnCatch, pitch: Random.Range(0.9f, 1.1f));
					if(hookedItem.Value >= 25) AudioManager.Play(snd_greatCatch);
					else if (hookedItem.Value >= 10) AudioManager.Play(snd_goodCatch);
					
					GameManager.ShowPoints(transform.position, hookedItem.Value);
					score += hookedItem.Value;
					playerScore.SetScore(score);
					hookedItem.Caught();
					hookedItem = null;
					boostAvailableTime = Time.time;
					ps_score.Emit(50);
					ps_bubbles.Emit(50);
					CameraManager.AddShake(shakeGetPoints);
				}
			}
		}

		private void CheckCollisions()
		{
			if (injureTime > 0)
			{
				boostReadySprite.color = Color.red;
				injureTime -= Time.deltaTime;
				return;
			}

			int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, collisionRadius, hits);
			if(hitCount > 0)
			{
				for(int i  = 0; i < hitCount; i++)
				{
					Item item = hits[i].GetComponent<Item>();
					if (item && item.CauseDamage && !item.Hooked)
					{ 
						if(hookedItem != item) // Don't get hurt by hooked item
						{
							TakeDamage();
							return;
						}
					}
				}
			}
		}

		private void TakeDamage()
		{
			injureTime = injureTimeReset;
			ps_damage.Emit(1);
			if (hookedItem)
			{
				hookedItem.OnHook(null); // Drop
				hookedItem = null;
			}
			if(harpoonState != 0)
			{
				harpoonStopTime = Time.time;
			}
			CameraManager.AddShake(shakeTakeDamage);
			AudioManager.Play(snd_hurt, pitch: Random.Range(0.9f, 1.1f));
		}

		private void MovePlayer()
		{
			if (injureTime > 0)
			{
				rotateTransform.Rotate(transform.forward, rotSpeed * 80f * Time.deltaTime);
				transform.position += Vector3.up * 1.25f * Time.deltaTime;
			}
			else
			{

				if (harpoonState == 0 && moveDir != Vector3.zero)
				{
					// Rotate
					float angletotarget = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
					rotateTransform.rotation = Quaternion.Lerp(rotateTransform.rotation,
						Quaternion.AngleAxis(angletotarget, Vector3.forward),               // Turn toward desired direction
						rotSpeed * Time.deltaTime);

					// Movement
					float boost = (Time.time < boostEndTime ? boostStrength : 1f);  // Use boosted speed
					float speedChangeEffect = 1f;
					if (Time.time < boostEndTime) // Boosted players move more steadily
					{
						speedChangeEffect = (Mathf.Abs(Mathf.Clamp(Mathf.Sin(Time.time * 7f), 0.9f, 1f)));
					}
					else
					{
						speedChangeEffect = (Mathf.Abs(Mathf.Clamp(Mathf.Sin(Time.time * 5f), 0.8f, 1f)));
					}
					if(hookedItem != null)
					{
						speedChangeEffect *= hookedItem.SlowPlayerMulti;
					}

					transform.position += rotateTransform.right * moveSpeed * boost * speedChangeEffect * Time.deltaTime;
				}
			}



			// spawnBounds
			if (transform.position.x < swimBounds.x) { transform.position = new Vector3(swimBounds.x, transform.position.y); }
			if (transform.position.x > swimBounds.z) { transform.position = new Vector3(swimBounds.z, transform.position.y); }
			if (transform.position.y > swimBounds.y) { transform.position = new Vector3(transform.position.x, swimBounds.y); }
			if (transform.position.y < swimBounds.w) { transform.position = new Vector3(transform.position.x, swimBounds.w); }
		}

		public void SetID(int id)
		{
			Debug.Log($"Player {id} added!");
			playerID = id;
			inputPrefix = $"P{id}";
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(new Vector3(swimBounds.x, swimBounds.w), new Vector3(swimBounds.x, swimBounds.y));
			Gizmos.DrawLine(new Vector3(swimBounds.x, swimBounds.y), new Vector3(swimBounds.z, swimBounds.y));
			Gizmos.DrawLine(new Vector3(swimBounds.z, swimBounds.y), new Vector3(swimBounds.z, swimBounds.w));
			Gizmos.DrawLine(new Vector3(swimBounds.z, swimBounds.w), new Vector3(swimBounds.x, swimBounds.w));

			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(transform.position, collisionRadius);
		}
	}
}