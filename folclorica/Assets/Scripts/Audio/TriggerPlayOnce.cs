using UnityEngine;
using System.Collections;

public class TriggerPlayOnce : MonoBehaviour {

	public Collider2D triggeredBy;
	public AudioSource audio;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other){

		if(other == triggeredBy && !audio.isPlaying)
			audio.PlayOneShot(audio.clip);
	}


}
