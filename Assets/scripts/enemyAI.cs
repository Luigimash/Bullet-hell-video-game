using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
	public GameObject enemy;
	public GameObject bossObject;
	[SerializeField] public float spawnRate = 2f; // enemies per second 
	[SerializeField] public float movespeed=40f;
	[SerializeField] public float bossScoreSpawnThreshold=50f;
	public bool boss=false; 
	public PlayerCombat playerScript;
	
	private float nextSpawn = 0f; 
	float locX=0f;
	float locY=0f;
	float locZ=0f;
	
	void Start()
	{
		locX = transform.position.x;
		locY = transform.position.y;
		locZ = transform.position.z;
	}
	
    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextSpawn && !boss)
		{
			
			if (playerScript.scoreVal >= bossScoreSpawnThreshold)
			{
				Debug.Log("spawning boss");
				Vector3 eulerRotation = new Vector3 (90f,-90f,180f);
				Vector3 cloneDir = new Vector3 (0f,0f,-1f); 
				Vector3 cloneSpawn = new Vector3(locX, 10f, locZ+100f); //50f is "normal" distance 
				GameObject enemyBoss = Instantiate (bossObject, cloneSpawn, transform.rotation);
				
				enemyBoss.transform.localScale = 5*enemyBoss.transform.localScale;
				enemyBoss.transform.eulerAngles = eulerRotation;
				//enemyClone.GetComponent<Rigidbody>().AddForce(cloneDir * movespeed);
				
				boss=true;
			}
			else
			{
				spawnEnemy();
				nextSpawn = Time.time + (1f/spawnRate);
			}
		}
	
    }
	
	public void bossKilled()
	{
		bossScoreSpawnThreshold = playerScript.scoreVal + 1300f;
		
		boss = false;
	}
	
	public void spawnEnemy()
	{
			float r = Random.Range(-120f,120f);
			
			Vector3 eulerRotation = new Vector3 (0f,180f,0f);
			Vector3 cloneDir = new Vector3 (0f,0f,-1f); 
			Vector3 cloneSpawn = new Vector3(locX + (r), 10f, locZ+76f);
			GameObject enemyClone = Instantiate (enemy, cloneSpawn, transform.rotation);
			enemyClone.transform.eulerAngles = eulerRotation;
			enemyClone.GetComponent<Rigidbody>().AddForce(cloneDir * movespeed);
			
	}
}
