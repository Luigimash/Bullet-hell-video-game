using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public Rigidbody rbody;
	public PlayerCombat combat;
	[SerializeField] private float horizontalVelocity = 10f;
	[SerializeField] private float verticalVelocity=10f;
	[SerializeField] private float fireRate=5f;		//shots/second 


	bool shootBuffer=false;	
	float horizontalMovement=0f;
	float verticalMovement = 0f;
	float nextFire=0f;
	
    // Update is called once per frame
    void Update()
    {
		horizontalMovement = 0f;
		verticalMovement = 0f;
        horizontalMovement = Input.GetAxisRaw("Horizontal") * horizontalVelocity; 
		//left is -1 right is 1
		verticalMovement = Input.GetAxisRaw("Vertical") * verticalVelocity;
		//idk which is which
		
		if (Input.GetButtonDown("Fire1"))
		{
			shootBuffer=true;
		}
		if (Input.GetButtonUp("Fire1"))
		{
			shootBuffer=false;
		}
    }

	
	void FixedUpdate()
	{
		if (shootBuffer && (Time.time >= nextFire))
		{
			combat.Attack(true);
			nextFire = Time.time + 1/fireRate;
		}

		Vector3 targetVelocity = new Vector3 (horizontalMovement, 0.0f, verticalMovement);
		rbody.velocity = targetVelocity;
	}
	
}
