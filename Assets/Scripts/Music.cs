using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

	static GameObject music_ref;
	
	void Awake()
	{
		if(music_ref != null)
		{
			Destroy(this.gameObject);   // If the music was already in the scene delete the object
										// or we will have 2 tracks playing at the same time
		}
	}

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
		music_ref = this.gameObject;
	}
}
