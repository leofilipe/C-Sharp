using UnityEngine;
using System.Collections;

public class TriggerGameEvent : MonoBehaviour {

	public GameObject triggeredBy;

	public GameObject target;

	bool loop = true;

	public bool doOnce = false;
	public bool triggerOn = true;

	void Awake(){

		target.SetActive(false);
	}

	void OnTriggerEnter2D(Collider2D col){

		if(col.gameObject == triggeredBy && loop){
			target.SetActive(triggerOn);

			loop = !doOnce;

			Debug.Log("Disparou");
		}
	}

}
