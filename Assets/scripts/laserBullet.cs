using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserBullet : MonoBehaviour
{
   	[SerializeField] public float bulletDamage=1f;
	public PlayerCombat script;
    private void OnTriggerEnter (Collider other)
	{
		if (other.name == "Spaceship")
		{	
			GameObject player = other.gameObject;
			script = player.GetComponent<PlayerCombat>();
			script.takeDamage(bulletDamage);
			
			Destroy(gameObject);
		}
	}
	
	private void OnTriggerExit (Collider other)
	{
		if (other.name == "World Border")
		{
			Destroy(gameObject);
		}
		
	}
}
