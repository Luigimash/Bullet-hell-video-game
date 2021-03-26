using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class bossAI : MonoBehaviour
{
	public GameObject enemy;
	public GameObject sphereBullet;
	public GameObject healthBullet;
	public GameObject shellBullet;
	public enemyAI enemyAIScript;
	[SerializeField] public float spawnRate = 0.3f; // enemies per second 
	[SerializeField] public float movespeed=60f;
	[SerializeField] public float timeBetweenOscillation=5f;
	
	private IEnumerator coroutine;
	
	private bool invuln = false;
	private bool spawnAnimation = false;
	public float bossHP = 1100f;
	
	private Slider slider;
	public GameObject bossHealthBar;
	private float maxHP;
	private GameObject healthBar;
	
	private GameObject[] objs;
	private GameObject player;
	private GameObject enemyObj;
	private float nextSpawn = 0f; 
	private float parabolicConstSpawn = 0f;
	
	//private float parabolicConstOscillate=0f;
	private float switchSide=0f;
	private float oscillateDist=40f;
	private bool startSide;
	private float xTranslation=0f;
	private float lastTrans=0f;
	
	private float startTime=0f;
	private float spawnZDist=40f;
	private float nextAttack;
	private bool inAttack=false;
	private float attackSelection=0f;
	
	private AudioManager audio;
	
	float locX=0f;
	float locY=0f;
	float locZ=0f;
	
	void Start()
	{
		nextAttack = Time.time + 4f;
		spawnAnimation = true;
		locX = transform.position.x;
		locY = transform.position.y;
		locZ = transform.position.z;
		nextSpawn = Time.time + 3f; 
		startTime=0f;
		attackSelection=0f;

		audio = FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
		
		
		parabolicConstSpawn = spawnZDist / (Mathf.Pow(2.5f, 2));
		//parabolicConstOscillate = -oscillateDist / (Mathf.Pow(timeBetweenOscillation,2f));
		
		objs = GameObject.FindGameObjectsWithTag("Enemy");
		enemyObj = objs[0];
		
		objs = GameObject.FindGameObjectsWithTag("Player");
		player = objs[0];
		
		enemyAIScript = enemyObj.GetComponent<enemyAI>();
		startSide = (Random.value >0.5f);
		
		invuln = true;
		
		
		healthBar = Instantiate(bossHealthBar, GameObject.Find("Canvas").GetComponent<Transform>());
		slider = healthBar.GetComponent<Slider>();
		maxHP = bossHP;
	}
	
    // Update is called once per frame
    void Update()
    { 
        if (Time.time >= nextSpawn)
		{ //transfer this to a reference in enemyAI so that it doesnt bug out, rotation of 
		 //the boss is different than enemyAI
			enemyAIScript.spawnEnemy();
			nextSpawn = Time.time + (3f/spawnRate);
		}
    }
	
	void FixedUpdate()
	{
		if (spawnAnimation)
		{ //opening spawn animation 
			float zLoc=0f;
			startTime += Time.deltaTime;
			zLoc = (parabolicConstSpawn * (Mathf.Pow(startTime - (2.5f), 2f))) - spawnZDist; //parabolic function
			Vector3 newLocation = new Vector3(transform.position.x, 10f, locZ + zLoc);
			transform.position = newLocation;
			
			//animate the healthbar fillup
			bossHP = (startTime / 2.5f) * maxHP;
			displayHealth();
			
			if (startTime >= 2.5f)
			{ //if the spawn animation is finished 
				switchSide = Time.time + timeBetweenOscillation;
				startTime =0f; //reset for idle timer 
				spawnAnimation = false; 
				invuln = false;
			}
		}
		else if (nextAttack <= Time.time || inAttack)
		{
			switch(attackSelection)
			{
				case 0: 
					spawnRate = 0.7f;
					attackSelection = Random.Range(1,4);
					Debug.Log(attackSelection);
					//attackSelection = 3f;
					break;
				case 1: //sweeping bullets barrage
					coroutine = BulletBarrage();
					StartCoroutine(coroutine);
					nextAttack = Time.time + 20f;
					attackSelection=0;
					break;
				case 2: //"machine gun" barrage 
					coroutine = machineGunBarrage();
					StartCoroutine(coroutine);
					nextAttack=Time.time+20f; 
					attackSelection=0f;
					break;
				case 3: //shoot big "grenade/bomb" that explodes behind the spaceship
					coroutine = explodingShells();
					StartCoroutine(coroutine);
					nextAttack=Time.time+25f; 
					attackSelection=0f;
					break;
				default:
					Debug.Log("Error on attackSelection");
					break;
			}
		}
		else
		{ //"idle animation" , oscillate side to side; going left is false, going right is true 
			xTranslation = oscillateDist * Mathf.Sin((2*Mathf.PI)/(timeBetweenOscillation) * startTime);
			Vector3 newLocation = new Vector3(transform.position.x + xTranslation - lastTrans, 10f, transform.position.z);
			transform.position = newLocation;
			
			startTime += Time.deltaTime;
			lastTrans = xTranslation;
		}
	}
	
	public void takeDamage(float damage)
	{
		if (!invuln)
		{
			bossHP -= damage;
		}
		
		if (bossHP <= 0)
		{ //if boss hp is below 0 (its dead)
			enemyAIScript.bossKilled();
			player.GetComponent<PlayerCombat>().addScore(1000f);
			player.GetComponent<PlayerCombat>().healDamage(5f);
			audio.Play("BossDeathExplosion");
			Destroy (healthBar);
			Destroy(gameObject);
		}
		
		displayHealth();
	}
	
	IEnumerator explodingShells()
	{ //summon large spheres that float past the player, and explode upon taking enough dmg or hitting world border, releasing spray of bullets 
		float timeBetweenShots=2.5f;
		float numShells = 7f;
		float shellSpeed=100f;
		spawnRate = 1f;
		
		for (int k=0; k<numShells; k++)
		{
			float rX = Random.Range(-105, 105);
			
			Vector3 spawnPoint = new Vector3 (rX, 10f, 100f); 
			Vector3 shotAngle = new Vector3 (0f,0f,-1f);
			
			GameObject shellClone = Instantiate (shellBullet, spawnPoint, transform.rotation);
			
			shellClone.GetComponent<Rigidbody>().AddForce(shotAngle*shellSpeed);
			
			yield return new WaitForSeconds(timeBetweenShots);
		}
	}
	
	IEnumerator machineGunBarrage()
	{ //shoot 3 waves of machine gun bullets in a wide spread in the general direction of the player 
		float numWaves=3f;
		float bullSpeed=150f;
		float numBullets;
		for (int k=0; k< numWaves; k++)
		{
			numBullets=20f;
			
			float timeBetweenShots = 0.15f;
			
			for (int i=0; i < numBullets; i++)
			{ 
				float pX=player.transform.position.x;
				float pZ=player.transform.position.z;
				float eX=transform.position.x;
				float eZ=transform.position.z;
				float rX = Random.Range(-70f,50f); //right gun
				float rZ = Random.Range(-10f,10f);
				float rXa = Random.Range(-70f, 50f); //left gun
				float rZa = Random.Range(-10f,10f);
				float rHealth = Random.Range(0f,100f);
				float chanceOfHealth = 2f;
				
				Vector3 spawnPoint = new Vector3 (eX+20f, 15f, eZ-18f); //angle/location for right gun
				spawnPoint = spawnPoint + (transform.forward*5f);
				float xComponent = pX - eX+rX;
				float zComponent = pZ - eZ+rZ;
				Vector3 shotAngle = new Vector3 (xComponent, 0f, zComponent);
				shotAngle = Vector3.Normalize(shotAngle);
				
				Vector3 spawnPointA = new Vector3 (eX-20f, 15f, eZ-18f); //angle/location for left gun 
				spawnPointA = spawnPointA + (transform.forward*5f);
				float xComponentA = pX - eX+rXa;
				float zComponentA = pZ - eZ+rZa;
				Vector3 shotAngleA = new Vector3 (xComponentA, 0f, zComponentA);
				shotAngleA = Vector3.Normalize(shotAngleA);
				
				if (rHealth <= chanceOfHealth)
				{ //shoot health sometimes 
					GameObject healthClone = Instantiate (healthBullet, spawnPoint, transform.rotation*Quaternion.Euler(90,0,0));
					healthClone.transform.localScale = new Vector3 (1f,2.5f,1f);
					healthClone.GetComponent<Rigidbody>().AddForce(shotAngle*bullSpeed);
				}
				else
				{
					GameObject bulletClone = Instantiate (sphereBullet, spawnPoint, transform.rotation);
					bulletClone.GetComponent<Rigidbody>().AddForce(shotAngle*bullSpeed);
					
					GameObject bulletClone_a = Instantiate(sphereBullet, spawnPointA, transform.rotation);
					bulletClone_a.GetComponent<Rigidbody>().AddForce(shotAngleA*bullSpeed);
					audio.Play("MachineGunFire");
				}
				yield return new WaitForSeconds(timeBetweenShots);
			}
			
			yield return new WaitForSeconds(2f);
		}
	}
	
	IEnumerator BulletBarrage()
	{ //send 3 waves of bullet waves, then one big explosion of bullets 
		float numWaves=3f;
		float bullSpeed=150f;
		float numBullets;
		for (int k=0; k< numWaves; k++)
		{
			numBullets=10f;
			float angleAttack=75f;
			
			float timeBetweenShots = 0.09f;
			
			for (int i=0; i < numBullets; i++)
			{ //barrage 1
			  //shoot from right gun 
				float trigAngle = Mathf.Deg2Rad*((i)*((2*angleAttack)/numBullets)+45);
				Vector3 tempBulletDir = new Vector3 (Mathf.Cos(trigAngle), 0f, -Mathf.Sin(trigAngle));
				
				Vector3 spawnLoc = transform.position + new Vector3 (25f,0f,-20f);
				GameObject sphereBulletClone = Instantiate (sphereBullet, spawnLoc, transform.rotation);
				
				Vector3 bulletDirection = Vector3.Normalize(tempBulletDir);
				sphereBulletClone.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(bulletDirection) * bullSpeed);
			
			  //shoot from left guns	
				spawnLoc = transform.position + new Vector3 (-25f,0f,-20f);
				GameObject sphereBulletClone_a = Instantiate (sphereBullet, spawnLoc, transform.rotation);
				
				sphereBulletClone_a.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(bulletDirection) * bullSpeed);

				audio.Play("MachineGunFire");
				yield return new WaitForSeconds(timeBetweenShots);
			}
			
			for (int i=0; i < numBullets; i++)
			{ //barrage 2
			  //shoot from right gun 
				float trigAngle = Mathf.Deg2Rad*((i)*((2*angleAttack)/numBullets)+45);
				Vector3 tempBulletDir = new Vector3 (-Mathf.Cos(trigAngle), 0f, -Mathf.Sin(trigAngle));
				
				Vector3 spawnLoc = transform.position + new Vector3 (25f,0f,-20f);
				GameObject sphereBulletClone = Instantiate (sphereBullet, spawnLoc, transform.rotation);
				
				Vector3 bulletDirection = Vector3.Normalize(tempBulletDir);
				sphereBulletClone.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(bulletDirection) * bullSpeed);
			
			  //shoot from left guns	
				spawnLoc = transform.position + new Vector3 (-25f,0f,-20f);
				GameObject sphereBulletClone_a = Instantiate (sphereBullet, spawnLoc, transform.rotation);
				
				sphereBulletClone_a.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(bulletDirection) * bullSpeed);
				
				audio.Play("MachineGunFire");
				yield return new WaitForSeconds(timeBetweenShots);
			}
			yield return new WaitForSeconds (0.3f);
		}
		
		numBullets = 10f;
		float conicAngle = 160f;
		float numBursts=2f;
		
		for (int k=0; k<numBursts; k++)
		{
			for (int i=0; i<numBullets; i++) 
			{ 
				float trigAngle = Mathf.Deg2Rad*((i)*(conicAngle/numBullets)+10f);
				Vector3 tempBulletDir = new Vector3 (-Mathf.Cos(trigAngle), 0f, -Mathf.Sin(trigAngle));

				Vector3 spawnLoc = transform.position;
				GameObject sphereBulletClone = Instantiate (sphereBullet, spawnLoc, transform.rotation);
					
				Vector3 bulletDirection = Vector3.Normalize(tempBulletDir);
				sphereBulletClone.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(bulletDirection) * bullSpeed);
			}
			yield return new WaitForSeconds(0.4f);
		}
		yield return new WaitForSeconds(1.5f);
		shootHealthBullet();
	}
	
	void shootHealthBullet()
	{
		float pX=player.transform.position.x;
		float pZ=player.transform.position.z;
		float eX=transform.position.x;
		float eZ=transform.position.z;
		float bulletSpeed = 60f;
		float rX = Random.Range(-20f,20f);
		float rZ = Random.Range(-20f,20f);
		
		Vector3 spawnPoint = new Vector3 (eX, 15f, eZ);
		spawnPoint = spawnPoint + (transform.forward*5f);
		float xComponent = pX - eX+rX;
		float zComponent = pZ - eZ+rZ;
		Vector3 shotAngle = new Vector3 (xComponent, 0f, zComponent);
		shotAngle = Vector3.Normalize(shotAngle);
		
		GameObject healthClone = Instantiate (healthBullet, spawnPoint, transform.rotation*Quaternion.Euler(90,0,0));
		healthClone.transform.localScale = new Vector3 (1f,2.5f,1f);
		healthClone.GetComponent<Rigidbody>().AddForce(shotAngle*bulletSpeed);
	}
	
	void displayHealth()
	{
		slider.value=bossHP;
	}
}
