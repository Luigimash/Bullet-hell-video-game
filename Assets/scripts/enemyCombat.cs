using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyCombat : MonoBehaviour
{
	public GameObject bullet;
	public GameObject healthBullet;
	[SerializeField] public float fireRate = 0.5f;
	[SerializeField] public float bulletSpeed=10f;
	[SerializeField] public float health = 5f;
	[SerializeField] public float chanceOfHealth=10f;
	
	private GameObject[] objs;
	private GameObject player;
	private float nextShot=1f;
	
	private AudioManager audio;
	
    // Start is called before the first frame update
    void Start()
    {
		audio = FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
		nextShot = Time.time + 1f;
        objs = GameObject.FindGameObjectsWithTag("Player");
		player = objs[0];
    }

    // Update is called once per frame
    void Update()
    {
        float pX=player.transform.position.x;
		float pZ=player.transform.position.z;
		float eX=transform.position.x;
		float eZ=transform.position.z;
		float rX = Random.Range(-20f,20f);
		float rZ = Random.Range(-20f,20f);
		float rHealth = Random.Range(0f,100f);
		
		if (Time.time >= nextShot)
		{
			Vector3 spawnPoint = new Vector3 (eX, 10f, eZ);
			spawnPoint = spawnPoint + (transform.forward*5f);
			float xComponent = pX - eX+rX;
			float zComponent = pZ - eZ+rZ;
			Vector3 shotAngle = new Vector3 (xComponent, 0f, zComponent);
			shotAngle = Vector3.Normalize(shotAngle);

			if (rHealth <= chanceOfHealth)
			{
				GameObject healthClone = Instantiate (healthBullet, spawnPoint, transform.rotation);
				healthClone.transform.localScale = new Vector3 (1f,2.5f,1f);
				healthClone.GetComponent<Rigidbody>().AddForce(shotAngle*bulletSpeed);
			}
			else
			{
				GameObject bulletClone = Instantiate (bullet, spawnPoint, transform.rotation);
				bulletClone.transform.localScale = new Vector3 (3f,3f,3f);
				bulletClone.GetComponent<Rigidbody>().AddForce(shotAngle*bulletSpeed);
			}
			
			nextShot = Time.time + (1/fireRate);
		}
		
		if (health <=0f)
		{
			audio.Play("MiniEnemyDeath");
			
			player.GetComponent<PlayerCombat>().addScore(10f);
			
			Destroy(gameObject);
		}
		
    }
	
	public void takeDamage (float damage)
	{
		health -= damage;
	}
}
