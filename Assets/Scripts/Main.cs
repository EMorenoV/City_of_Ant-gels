/**************

Steering behaviours assignment
by Esteban Moreno Valdes
May 2015
v. 1.0

Game Name : CITY OF ANT-GELS

DESCRIPTION
-----------

This is a little action game inspired by the ZX Spectrum 80s classic Ant Attack. The goal is to find our lover 
that is lost somewhere in the city and go back to the starting point. That won't be an easy task because the place 
is inhabited by hovering ants and they will attack us if we get too close.
The girl can't walk too fast either and we will have to protect her on the way back by throwing
stones at the ants that want to eat us. If you don't want to fight you can always run away and
climb one of the buildings around. Once on the top the ants will follow their leader again.

I made the assets using Unity primitives in the editor. It's my own music I made using FL Studio.
And for sound effects I used the free online tool BFXR.

Controls
--------

WASD - Move
Space - Jump
Mouse pointer - Change player's direction
Mouse left button - Shoot
ESC - Restart the Level

Comments
--------

At the beginning I started implementing my own steering behaviours like SEEK (please see file Steering_Seek.CS).
Then I decided not to reinvent the wheel again and put more time into using steering behaviours in a creative way, 
implement state machines and a bit of jazz :)

I am using Path follow (for ant leader), seek, pursue and separation.

The ants have a state machine. The girl has her own state machine too and once the player gets closer she will go
into the SEEK state and follow the player with that particular steering behaviour.

**************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Main : MonoBehaviour {

	float speed = 75f;					// Boy's speed
	float jump_force = 50f;				// Jumping force applied
	public LayerMask layermask_floor;   // Mask needed to change player's direction when we move the mouse pointer over the floor only
	Camera camera_ref;					// Reference to the main camera game. The camera is the child of an empty gameobject to help us to emulate a sort of isometric perspective
	bool falling = false;				// The Player is colliding with the floor
	public bool hurt;					// The Player is coliding with an ant
	
	float life;							// Health
	
	public AudioClip sfx_jump, sfx_hurt;
	public GameObject bullet_ref;		// We need this to intantiate the bullet from the player's position

	// Use this for initialization
	void Start () {
		camera_ref = GameObject.Find("Camera").GetComponent<Camera>();
		life = 100;
		hurt = false;
		GameObject.Find("Boy_HP").GetComponent<Text>().text = life.ToString();  // Update the UI
		Time.timeScale = 1f;	// Restart this after game over. I set it to 0 when is game over.
	}
	
	// Update is called once per frame
	void Update () {
	
		// Move controls
		if(Input.GetKey(KeyCode.W))
		{
			transform.position += -this.transform.forward * Time.deltaTime * speed;
		}
		else if(Input.GetKey(KeyCode.S))
		{
			transform.position += this.transform.forward * Time.deltaTime * speed;
		}
		if(Input.GetKey(KeyCode.A))
		{
			transform.position += this.transform.right * Time.deltaTime * speed;
		}
		else if(Input.GetKey(KeyCode.D))
		{
			transform.position += -this.transform.right * Time.deltaTime * speed;
		}
		
		// Shoot
		if(Input.GetMouseButtonDown(0))
		{
			Instantiate(bullet_ref, this.transform.position, Quaternion.identity);
		}
		
		// Jump
		if(Input.GetKeyDown(KeyCode.Space) && !falling)
		{
			audio.PlayOneShot(sfx_jump);
			Jump();
			falling = true;   // We are in the air now!
		}
		
		// Restart level
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.LoadLevel(0);
		}
		
		// Change player's direction depending on where the mouse pointer is located.
		Look_at_Mouse();
		
		// If colliding with an ant reduce HP and play a wee sound
		if(hurt)
		{
			life -= Time.deltaTime * 75f;
			audio.PlayOneShot(sfx_hurt);
		}
		
		if(life < 0 || transform.position.y < -50)
		{
			Game_Over();
			hurt = false;   // Avoid unwanted secondary effects when is game over and the enemy is still attacking us
			Time.timeScale = 0;	// Game Over!!
		}
	
		// Don't forget to update the UI!
		GameObject.Find("Boy_HP").GetComponent<Text>().text = Mathf.RoundToInt(life).ToString();
	}
	
	// The next function change the rotation of the main character so that is
	// always looking at the mouse cursor
	
	void Look_at_Mouse()
	{
		Ray camera_ray = camera_ref.ScreenPointToRay(Input.mousePosition);
		
		RaycastHit hit_floor;
		
		// Throw a ray from the camera using the mouse position as the starting point.
		// Once it collides againt the floor get the vector between the player
		// and the hit. Then make the player look at this new position
		
		if(Physics.Raycast(camera_ray, out hit_floor, 1000f, layermask_floor))
		{
			//Debug.Log("Floor hit!");
			
			Vector3 mouse_player_vector = - hit_floor.point + transform.position;
			mouse_player_vector.y = 0;  // Be sure is flat, Just get X-Z
			
			Quaternion rotation = Quaternion.LookRotation(mouse_player_vector);
			this.GetComponent<Rigidbody>().MoveRotation(rotation); // Rotate the player
		}
	}
	
	void Jump()
	{
		// Add vertical force
		this.rigidbody.velocity = new Vector3(rigidbody.velocity.x, jump_force, this.rigidbody.velocity.z);	
	}
	
	void OnCollisionStay(Collision col_info)
	{
		//We are colliding with something
		falling = false; // Then don't fall anymore
	}
	
	void Game_Over()
	{
		// Update UI. Show Game Over message
		GameObject.Find("Game_Over").GetComponent<Text>().enabled = true;
	}
	
	void OnCollisionEnter(Collision collision) {
	
		// Are we colliding with an ant?
		if(collision.collider.CompareTag("Boid") || collision.collider.CompareTag("Leader"))
		{
			//Debug.Log("Ouch!!");
			hurt = true;	// Do some damage while its touching us
		}
		
		// Did we reach the GOAL area with the girl?
		if(collision.collider.CompareTag("Goal") && GameObject.FindGameObjectWithTag("Girl").GetComponent<Girl_StateMachine>().goal_reached())
		{
			// Yes! We did!
			Debug.Log("You win!!");
			// Display nice message
			GameObject.Find("You_Win").GetComponent<Text>().enabled = true;
			Time.timeScale = 0f;	// Stop everything
		}
		else if(collision.collider.CompareTag("Goal"))
		{
			// If no girl yet go and find her ...
			//Debug.Log("Find the girl first!");
		}
	}
	
	void OnCollisionExit(Collision collision)
	{
		if(collision.collider.CompareTag("Boid") || collision.collider.CompareTag("Leader"))
		{
			// Stop hurting me!
			hurt = false;
		}
	}
}
