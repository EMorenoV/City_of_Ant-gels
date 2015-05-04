using UnityEngine;
using System.Collections;


/*
Just a litle camera follow script with a bit of dampening
*/

public class CameraFollow : MonoBehaviour {
	
	public GameObject target;  // The target will be the player
	public float damping = 1;
	Vector3 offset;
	
	void Start() {
		offset = transform.position - target.transform.position;
	}
	
	void Update() {
		// In other words. Move the camera to the new calculated vector position smoothly thanks to Lerp
		Vector3 desiredPos = target.transform.position + offset;
		Vector3 position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * damping);
		transform.position = position;
		
		// Then keep the player in the center. Strictly speaking we loose the clasic isometric angles
		// but I like the effect (very subtle)
		transform.LookAt(target.transform.position);
	}
}
