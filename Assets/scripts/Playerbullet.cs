using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerbullet : MonoBehaviour
{
   	[SerializeField] private float bulletDamage=1f;
	public enemyCombat script;
	public bossAI bossScript;
	public shellBullet shellScript;
    private void OnTriggerEnter (Collider other)
	{
		if (other.name == "minienemy (prefab)(Clone)")
		{
			GameObject enemy = other.gameObject;
			script = enemy.GetComponent<enemyCombat>();
			script.takeDamage(bulletDamage);
			Destroy(gameObject);
		}
		
		if (other.name == "bigboss prefab(Clone)")
		{
			GameObject bossEnemy = other.gameObject;
			bossScript = bossEnemy.GetComponent<bossAI>();
			bossScript.takeDamage(bulletDamage);
			Destroy(gameObject);
		}
		
		if (other.name == "sphereBullet(Clone)")
		{
			GameObject shellBull = other.gameObject;
			shellScript = shellBull.GetComponent<shellBullet>();
			shellScript.takeDamage(bulletDamage);
			Destroy (gameObject);
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		if (other.name == "World Border")
		{
			Destroy(gameObject);
		}
		
		
	}
}
