using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBehavior : MonoBehaviour
{
	public Rigidbody rb;
	
	private float startTime=0f;
	private float rW=0f; //random width
	private float rL = 0f; //random length 
	private float rT=0f; //random time
	private float vertexZ=0f;
	private float parabolicConstZ=0f;
	private float xSlope=0f;
	private GameObject[] objs;
	private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
		startTime=0f;
        rW = Random.Range(35f, 110f);
		rL = Random.Range(35f, 50f);
		rT = Random.Range(2f, 5f); //amount of time in arc
		
		if (transform.position.x > 0)
		{ //if positive x value 
			vertexZ = transform.position.z - rL;
			//equation for Z
			parabolicConstZ = (transform.position.z - vertexZ) / (Mathf.Pow(rT/2f,2f));
			//slope of X translation 
			xSlope = -rW / rT;
		}
		else
		{
			vertexZ = transform.position.z - rL;
			//Equation for Z
			parabolicConstZ = (transform.position.z - vertexZ) / (Mathf.Pow(rT/2f,2f));
			//slope of X translation
			xSlope = rW / rT;
		}
		
		objs = GameObject.FindGameObjectsWithTag("Player");
		player = objs[0];
		
		
    }

    // Update is called once per frame
    void FixedUpdate()
    { //y=a(x-h)^2 + k, k vertical displacement, -h horizontal displacement, a scalar, (h,k) is vertex
		float xLoc=0f;
		float zLoc=0f;
		startTime += Time.deltaTime;
		
		xLoc = xSlope * Time.deltaTime;
		zLoc = parabolicConstZ * (Mathf.Pow(startTime - (rT/2), 2f)) + vertexZ;

		
		Vector3 newLocation = new Vector3(transform.position.x + xLoc, 10f, zLoc);
		transform.position = newLocation;
		transform.LookAt(player.transform.position);
    }
	
	private void OnTriggerExit(Collider other)
	{
		if (other.name == "World Border")
		{
			Destroy(gameObject);
		}
		
	}
}
