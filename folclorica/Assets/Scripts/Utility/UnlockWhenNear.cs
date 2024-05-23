using UnityEngine;
using System.Collections;

public class UnlockWhenNear : MonoBehaviour {

	public Lock Lock;

	public Collider2D unlockedBy;

	// Use this for initialization
	void Start () {
	
		if(Lock == null)
			Lock = GetComponent<Lock>();

		//Debug.Log("Unlock When Near: " + gameObject.name);
	}
	
	void OnTriggerStay2D(Collider2D col){

		if(!Lock.unlocked && col == unlockedBy){
			Lock.unlocked = true;

			//Debug.Log("Unlocked...");
		}
	}
}
