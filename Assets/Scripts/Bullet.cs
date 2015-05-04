using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	float speed = 100f;
	float lifetime;
	public Vector3 direction;
	public ParticleSystem ps_ref;

	// Use this for initialization
	void Start () {
		// Get the players forward vector
		direction = - GameObject.FindGameObjectWithTag("Player").transform.forward;
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position += direction * speed * Time.deltaTime;
		
		lifetime += Time.deltaTime;
		
		if(lifetime > 1f)  // Last for one second
		{
			Destroy(this.gameObject);
		}
	}
	
	void OnCollisionEnter(Collision collision) {
	
		// Did we hit an enemy?
		if(collision.collider.CompareTag("Boid") || collision.collider.CompareTag("Leader"))
		{
			// Yes, then destroy it.
			// But before ...
			// Instantiate a particle system where the bullet was to simulate a nice ant explosion
			Instantiate(ps_ref, transform.position, Quaternion.identity);
		
			// Dont hurt the player anymore. In case the enemy we are about to destroy is the
			// one that was colliding with the player (little hack :) )
		
			GameObject.FindGameObjectWithTag("Player").GetComponent<Main>().hurt = false;
			// Same for the girl
			GameObject.FindGameObjectWithTag("Girl").GetComponent<Girl_StateMachine>().hurt = false;
			
			Destroy(collision.gameObject);
			Destroy(this.gameObject);
		}	
	}
}
