using UnityEngine;
using System.Collections;

public class TriggerConditionalImmunity : MonoBehaviour {

	public Collider2D immunityTarget;

	public Collider2D[] protectAgainst;

	//private Coll
	private bool _onTrigger = false;

	

	// Update is called once per frame
	void Update () {

		if(_onTrigger){

			bool crouch = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S); 
			//Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

			if(crouch)
				SetImmunity(true);
			else
				SetImmunity(false);
		}
	}

	void SetImmunity(bool isImmune){

		Collider2D[] colliders = immunityTarget.gameObject.GetComponents<Collider2D>();
		
		foreach(Collider2D immunity in colliders){
			foreach(Collider2D against in protectAgainst){
				
				Physics2D.IgnoreCollision(immunity, against, isImmune);
			}
		}
	}

	void OnTriggerStay2D(Collider2D col){

		if(col == immunityTarget)
			_onTrigger = true;
	}

	void OnTriggerExit2D(Collider2D col){
		
		if(col == immunityTarget){
			_onTrigger = false;

			SetImmunity(false);
		}
	}

}
