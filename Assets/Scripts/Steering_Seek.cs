using UnityEngine;
using System.Collections;


// My own very first class implementing Steering behaviours

public class Steering : MonoBehaviour {

	float speed = 30f;
	public Transform target;
	Vector3 desired_vel, current_vel;
	Vector3 steering;
	
	// Update is called once per frame
	void Update () {
		Seek();
	}
	
	void Seek()
	{
		// Get the first vector
		desired_vel = - transform.position + target.position;
		desired_vel.Normalize();
		desired_vel *= speed;
		current_vel = transform.forward * 30f;	// Use forward vector as velocity
		steering = desired_vel - current_vel;	// This is the vector that connects the last two
		steering /= 2f;
		
		Debug.DrawRay(transform.position, current_vel);
		Debug.DrawRay(transform.position, desired_vel);
		
		transform.position += (current_vel + steering) * Time.deltaTime;
		
		// The girl is always looking at the boy.
		transform.LookAt(target.position);
	}
}
