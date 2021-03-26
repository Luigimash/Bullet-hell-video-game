using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PlayerCombat : MonoBehaviour
{
	public GameObject bullet;
	[SerializeField] private float bulletSpeed=250f;
	private float invulnTime=1f;
	private float invulnPeriod=0f; 
	public float health=5f;
	
	//variables for health bar
	public Slider slider;
	public Gradient gradient;
	public Image fill;
	
	public AudioSource source;
	private AudioManager audio;
	
	public Text scoreTxt;
	public GameObject scoreStore;
	public Renderer rend; 
	public float scoreVal=0f;
	
	private IEnumerator coroutine;

	void Start()
	{
		rend = GetComponent<Renderer>();
		scoreVal=0f;
		addScore(0f);
		audio = FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
	}


	void Update()
	{
         Vector3 mousePos = Input.mousePosition;
		 Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);

		mousePos.x = mousePos.x - objectPos.x;
		mousePos.y = mousePos.y - objectPos.y;
		
		float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
		
		if (-angle+90 >= 20)
		{
			angle = 70;
		}
		else if (-angle + 90 <= -20)
		{
			angle = 110;
		}
         transform.rotation = Quaternion.Euler(new Vector3(0, -angle+90,0));
	}
	
	public void Attack (bool shoot)
	{
		if (shoot)
		{
			//source.PlayOneShot(source.clip);
			audio.Play("SpaceshipGun");
			
			//Vector3 bulletDirection = new Vector3 (0,0,1);
			Vector3 bulletDirection = transform.forward;
			//Vector3 bulletSpawn_a = new Vector3 (transform.position.x+4.3f, 10, transform.position.z + 1.5f);
			//Vector3 bulletSpawn_b = new Vector3 (transform.position.x-4.3f, 10, transform.position.z + 1.5f);
			Vector3 relativeBulletSpawn_a = new Vector3 (2f, 0f, 0.2f);
			Vector3 relativeBulletSpawn_b = new Vector3 (-2f, 0f, 0.2f);
			
			Vector3 bulletSpawn_a = transform.TransformPoint(relativeBulletSpawn_a);
			Vector3 bulletSpawn_b = transform.TransformPoint(relativeBulletSpawn_b);
			
			GameObject bulletClone_a = Instantiate (bullet, bulletSpawn_a, transform.rotation) as GameObject;
			GameObject bulletClone_b = Instantiate (bullet, bulletSpawn_b, transform.rotation) as GameObject;
			
			bulletClone_b.transform.localScale = new Vector3(5f,5f,5f);
			bulletClone_b.GetComponent<Rigidbody>().AddForce(bulletDirection*bulletSpeed);
			
			bulletClone_a.transform.localScale = new Vector3(5f,5f,5f);
			bulletClone_a.GetComponent<Rigidbody>().AddForce(bulletDirection*bulletSpeed);
		}
	}
	
	public void takeDamage(float damage)
	{
		if (Time.time >= invulnPeriod)
		{
			health -= damage;
			displayHealth();
			invulnPeriod = Time.time + invulnTime;
			coroutine = InvincibilityFlicker();
			audio.Play("SpaceshipHit");
			StartCoroutine (coroutine);
		}
		
		if (health == 0)
		{
			scoreStore.GetComponent<ScoreStore>().scoreValue=scoreVal; //keep the point score in a non-destroyed game object 
			SceneManager.LoadScene(2, LoadSceneMode.Additive);
		}
		//Debug.Log("player health = " + health);
	}
	
	public void healDamage(float heal)
	{
		health += heal;
		if (health > 5f)
		{
			health = 5f;
		}
		displayHealth();
		//Debug.Log("player health = " + health);
	} 
	
	public void displayHealth()
	{
		slider.value = health;
		
		fill.color = gradient.Evaluate(slider.normalizedValue);
	}
	
	public void addScore(float addition)
	{
		scoreVal += addition;
		int temp = (int)scoreVal;
		scoreTxt.text = temp.ToString("D7");
	}
	
	IEnumerator InvincibilityFlicker()
	{
		for (int k =0; k<5; k++)
		{
			rend.enabled = false; 
			
			yield return new WaitForSeconds(0.05f);
			
			rend.enabled = true; 
			yield return new WaitForSeconds(0.15f);
		}
	}
}
