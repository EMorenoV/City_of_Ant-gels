using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**************************************************

This is where the first SM is implemented. 
The other one is in StateMachine_Ants.cs
For extra info about the game please see Main.cs

***************************************************/

public class Girl_StateMachine : MonoBehaviour {
	
	enum States {
		Init,			// Initialise some of the girl's variables
		Wait,			// The girl is waiting for her lover :)
		Follow_Boy,		// Once the boy is in range follow him
	}
	
	States current_state = States.Init;
	
	public Transform Player_ref;	// A reference to the player's transform to access his position anytime
	public AudioClip sfx_hurt;
	
	float life;
	public bool hurt;
	bool goal_area;    // Did the girl reach the GOAL area?
	
	float distance = 60f;		// Minimum distance for the girl to follow the boy
	
	// Update is called once per frame
	void Update () {
		// Process the different states
		switch(current_state)
		{
		case States.Init:
			Initialize();
			break;			
		case States.Wait:
			Waiting();		// Stay in this state till the distance to the boy is less than the value set at the beginning
			break;
		case States.Follow_Boy:
			// While in this state the girl wont be invincible anymore
			// The collision detection is managed in OnCollisionEnter
			Follow();
			break;
		}
		
		// The girl can be hurt from any state
		
		if(hurt)
		{
			life -= Time.deltaTime * 50f;
			audio.PlayOneShot(sfx_hurt);
		}
		
		// Update the UI
		GameObject.Find("Girl_HP").GetComponent<Text>().text = Mathf.RoundToInt(life).ToString();
	}
	
	void Initialize()
	{
		// Place the girl in a random position
		this.transform.position = new Vector3(Random.Range(-480, 480), 7f, Random.Range(-112, 480));
		life = 100;	// Full health at the beginning
		hurt = false;
		GameObject.Find("Girl_HP").GetComponent<Text>().text = Mathf.RoundToInt(life).ToString();  // Update UI
		current_state = States.Wait;	// Then wait for the boy
		goal_area = false;    // At the beginning the girl is spawned away from the goal area
	}
	
	void Waiting()
	{
		// If the boy gets close enough then follow him
		
		Vector3 distance_to_player = this.transform.position - Player_ref.position;
		
		//Debug.Log("Distance : " + distance_to_player.magnitude);
		
		if(distance_to_player.magnitude < distance)
		{
			// Set the new target in the main script so that the girl follows the player instead from now on
			this.gameObject.GetComponent<SteeringBehaviours>().target = GameObject.Find("Player");
			// And enable Seek in the main class
			this.gameObject.GetComponent<SteeringBehaviours>().seekEnabled = true;
			
			// To make the make a bit more challenging, get any any leader
			// and change the target to the girl herself
			
			GameObject temp = GameObject.Find("Ant Leader");
			
			temp.GetComponent<SteeringBehaviours>().pathFollowEnabled = false;
			temp.GetComponent<SteeringBehaviours>().target = this.gameObject;
			temp.GetComponent<SteeringBehaviours>().seekEnabled = true;
			
			current_state = States.Follow_Boy;
		}
	
	}
	
	void Follow()
	{
		// Are we still alive?
		if(life < 0)
		{
			GameObject.Find("Game_Over").GetComponent<Text>().enabled = true;
			hurt = false;   // Avoid looping sounds after game over
			Time.timeScale = 0;	// Game Over!!
		}
	}

	void OnCollisionEnter(Collision collision) {
		if(current_state == States.Follow_Boy && collision.collider.CompareTag("Boid") || collision.collider.CompareTag("Leader"))
		{
			//Debug.Log("Girl says Ouch!!");
			hurt = true;
		}
		
		if(collision.collider.CompareTag("Goal"))
		{
			goal_area = true;		// The girl entered the goal area
		}
	}
	
	void OnCollisionExit(Collision collision)
	{
		if(current_state == States.Follow_Boy && collision.collider.CompareTag("Boid") || collision.collider.CompareTag("Leader"))
		{
			//Debug.Log("Ouch!!");
			hurt = false;
			//life--;
		}
		
		if(collision.collider.CompareTag("Goal"))
		{
			goal_area = false;		// The girl exited the goal area
		}
	}
	
	public bool goal_reached()
	{
		return goal_area;
	}
}
