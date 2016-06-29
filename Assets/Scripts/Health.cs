using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Health : NetworkBehaviour
{
	public const int maxHealth = 100;

	[SyncVar (hook = "OnChangeHealth")]
	public int currentHealth = maxHealth;
	public RectTransform healthBar;
	public bool destroyOnDeath;

	private NetworkStartPosition[] spawnPoints;

	public void Start () {
		if (isLocalPlayer) {
			spawnPoints = FindObjectsOfType<NetworkStartPosition> ();
		}
	}

	public void TakeDamage (int amount)
	{
		if (!isServer) {
			return;
		}
		currentHealth -= amount;
		if (currentHealth <= 0) {
			if (destroyOnDeath) {
				Destroy (gameObject);
				return;
			} else {
				currentHealth = maxHealth;
				RpcRespawn ();
			}

		}
		OnChangeHealth (currentHealth);
	}

	private void OnChangeHealth (int currentHealth)
	{
		healthBar.sizeDelta = new Vector2 (currentHealth, healthBar.sizeDelta.y);
	}

	[ClientRpc]
	private void RpcRespawn ()
	{
		if (isLocalPlayer) {
			// Set spawn point to origin as default
			Vector3 spawnPoint = Vector3.zero;

			// If there is spawn point array and array not empty, point spawn point at random
			if (spawnPoints != null && spawnPoints.Length > 0) {
				spawnPoint = spawnPoints [Random.Range (0, spawnPoints.Length)].transform.position;
			}
			transform.position = spawnPoint;
		}
	}
}
