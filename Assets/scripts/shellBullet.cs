using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shellBullet : MonoBehaviour
{
	public GameObject sphereBullet;
	public GameObject healthBullet;
	private GameObject player;
	public float health = 3f;
	public float numBullets = 40f;
	public float bullSpeedBase=110f;
	public float healthBulletSpeed = 100f;
	private AudioManager audio;
	
	void Start()
	{
		player = GameObject.Find("Spaceship");
		audio = FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
	}

	public void takeDamage(float damage)
	{
		health -= damage;
		if (health <= 0)
		{
			explode(true);
			Destroy(gameObject);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.name == "World Border")
		{
			explode(false);
			Destroy(gameObject);
		}
	}
	
	void explode(bool playerDestroy)
	{
		audio.Play("ShellBulletExplosion");
		for (float i=0f; i<numBullets; i++)
		{
			float rSpeed = Random.Range(0f, 30f) + bullSpeedBase;
			float rAngle = Random.Range(0, 2*Mathf.PI); //random angle from 0 to 360 deg 
			
			Vector3 shotAngle = new Vector3 (Mathf.Sin(rAngle), 0f, Mathf.Cos(rAngle));
			GameObject explodeBullet = Instantiate(sphereBullet, transform.position, transform.rotation);
			explodeBullet.GetComponent<Rigidbody>().AddForce(shotAngle*rSpeed);
		}
		
		if (playerDestroy)
		{
			GameObject healthBulletClone = Instantiate(healthBullet, transform.position, transform.rotation);
			Vector3 directionOfPlayer = new Vector3 (player.transform.position.x-transform.position.x, 0f, player.transform.position.z -transform.position.z); 
			healthBulletClone.transform.LookAt(player.transform.position);
			healthBulletClone.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(directionOfPlayer)*healthBulletSpeed);
		}
	}
}
