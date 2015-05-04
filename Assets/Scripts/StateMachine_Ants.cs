using UnityEngine;
using System.Collections;

/* 

Ants State Machine

*/

public class StateMachine_Ants : MonoBehaviour {
	
	enum States {
		Init,				// Initialise some of the enemy variables
		Follow_Leader,		// The ant is following the leader
		Seek_Player,
	}
	
	States current_state = States.Init;
	
	public Transform Player_ref;	// A reference to the player's transform to access his position anytime
	
	GameObject original_target;		// The enemies swap between targets : leader / player. So we need to copy
									// the original one at the beginning
									
	float min_distance = 60f;		// The ants will detect the player at this distance
	float escape_height = 40f;		// If the player is above this height the ants will ignore him

	// Update is called once per frame
	void Update () {
		// Process the different states
		switch(current_state)
		{
		case States.Init:
			Initialize();
			break;			
		case States.Follow_Leader:
			//Debug.Log("I am following the leader!");
			Following();
			break;
		case States.Seek_Player:
			//Debug.Log("I am following the player!");
			Seeking_Player();
			break;
		}
	}
	
	void Initialize()
	{
		// Make a copy of the original target in case we need to follow the leader again
		original_target = this.gameObject.GetComponent<SteeringBehaviours>().target;
		
		current_state = States.Follow_Leader;
	}
	
	void Following()
	{
		// While following the leader ... Are we close to the main character?
		
		Vector3 distance_to_player = this.transform.position - Player_ref.position;
		
		//Debug.Log("Distance : " + distance_to_player.magnitude);
		
		if(distance_to_player.magnitude < min_distance)
		{
			// Set the new target in the main script so that the ant follows the player instead from now on
			this.gameObject.GetComponent<SteeringBehaviours>().target = GameObject.Find("Player");
			current_state = States.Seek_Player;
		}
	}
	
	void Seeking_Player()
	{
		// If player has escaped because he climbed a building the ants will go back to the original state
		// and follow the leader again
		
		if(Player_ref.transform.position.y > escape_height)
		{
			// Follow the original Ant leader again
			this.gameObject.GetComponent<SteeringBehaviours>().target = original_target;
			current_state = States.Follow_Leader;
		}
	}
}
