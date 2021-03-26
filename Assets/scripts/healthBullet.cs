using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthBullet : MonoBehaviour
{
	[SerializeField] public float bulletHealth=3f;
	[SerializeField] public float bulletHeal=1f;
	[SerializeField] public float speedConversionRate = 0.2f;

	public PlayerCombat script;
	
	private bool alreadySlowed=false;
    private void OnTriggerEnter (Collider other)
	{
		if (other.name == "Spaceship")
		{	
			GameObject player = other.gameObject;
			script = player.GetComponent<PlayerCombat>();
			script.healDamage(bulletHeal);
			
			Destroy(gameObject);
		}
		
		if (other.name == "bullet (prefab)(Clone)")
		{
			bulletHealth -= 1f;
			if (bulletHealth <= 0 && !alreadySlowed)
			{
				SpawnHealthDrop();
				alreadySlowed = true;
			}
		}
		
	}
	
	private void OnTriggerExit (Collider other)
	{
		if (other.name == "World Border")
		{
			Destroy(gameObject);
		}
		
	}
	
	private void SpawnHealthDrop()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		Vector3 newVelocity = (speedConversionRate) * rb.velocity;
		rb.velocity = newVelocity;
	}
}
