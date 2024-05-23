using UnityEngine;
using System.Collections;

public class ShotDownInArc : MonoBehaviour {

	public Collider2D targetCollider;
	
	public GameObject leftTarget;
	public GameObject rightTarget;
	public float trajectoryHeight = 5;
	public float horizontalOffSet = 0.5f;
	public float speed = 1f;
	public bool isPlayer = true;

	Vector3 throwTarget;
	
	float minimumX;
	
	bool throwRight;
	bool shoot = false;

	bool firstImpact = true;

	
	// Update is called once per frame
	void Update () {
	
		if(!shoot)
			return;

		if(isPlayer){
			
			bool isGrounded = targetCollider.gameObject.GetComponent<PlatformerCharacter2D>().grounded;
			
			if(isGrounded && !firstImpact){
				shoot = false;
				firstImpact = true;
				return;
			}
		}


		float _speed = Time.deltaTime * speed;
		throwTarget.z = targetCollider.transform.position.z;

		// calculate straight-line lerp position:
		Vector3 currentPos = Vector3.Lerp(targetCollider.transform.position, throwTarget, _speed);
		
		// add a value to Y, using Sine to give a curved trajectory in the Y direction
		currentPos.y += -1 * (trajectoryHeight * Mathf.Sin(Mathf.Clamp01(_speed) * Mathf.PI)) /* +- horizontalOffSet*/;

		if(throwRight)
			currentPos.x += horizontalOffSet;
		else
			currentPos.x -= horizontalOffSet;
		
		// finally assign the computed position to our gameObject:
		targetCollider.transform.position = currentPos;
		
		var distanceSquared = currentPos.x - throwTarget.x;
		distanceSquared *= distanceSquared;
		
		shoot = distanceSquared > .1f;
	
	}

	public void OnCollisionEnter2D(Collision2D col){
		
		if(col.collider != targetCollider)
			return;
		
		if(isPlayer){

			if(GameControl.instance.currentHealthPoints <= 0)
				return;

			throwRight = targetCollider.transform.position.x >= transform.position.x;
		}
		
		//throwTarget = facingRight? leftTarget : rightTarget;

		Debug.Log("Right? " + throwRight);
		
		if(throwRight){

			throwTarget = rightTarget.transform.position;
			
			float minX = throwTarget.x - transform.position.x;
			
			float actualX = throwTarget.x - targetCollider.transform.position.x;
			
			if(actualX < minX)
				throwTarget.x += minX - actualX;			
		}else{

			throwTarget = leftTarget.transform.position;
			
			float minX = transform.position.x - throwTarget.x;
			
			float actualX = targetCollider.transform.position.x - throwTarget.x;
			
			if(actualX < minX)
				throwTarget.x += actualX - minX;

		}
		
		
		shoot = true;
		
		
	}
}
