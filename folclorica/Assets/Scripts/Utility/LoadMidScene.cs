using UnityEngine;
using System.Collections;

public class LoadMidScene : MonoBehaviour {

	public Collider2D triggeredBy;		//Collider that triggers the action for this class
	public float wait = 2f;				//how long to wait after the events have been triggered
	public string levelName;			//name of the level to load next

	private bool load = false;			//flag if should start to load the next level

	// Update is called once per frame
	void FixedUpdate () {
	
		//if load
		if(load){

			//decrease the waiting time
			wait -= Time.deltaTime;

			//if it has reached at least zero
			if(wait <= 0){
				//load the next level and disable this class
				Debug.Log("Carregando proxima fase...");
				Application.LoadLevel(levelName);
				this.enabled = false;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other){

		//if the target has entered the trigger, flag load as true
		if(other == triggeredBy){
			load = true;
		}
	}
}
