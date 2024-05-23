using UnityEngine;
using System.Collections;

public class ShootInArc : MonoBehaviour {

	public Collider2D targetCollider;

	public GameObject leftTarget;
	public GameObject rightTarget;
	public float trajectoryHeight = 5;
	public float speed = 1f;
	public bool isPlayer = true;
	public bool useHurtColor = true;

	Vector3 throwTarget;

	float minimumX;

	bool facingRight;
	bool shoot = false;

	SpriteRenderer playerRenderer;


	void Start(){

		if(isPlayer && !(targetCollider is CircleCollider2D)){
			
			Collider2D[] cols =  targetCollider.gameObject.GetComponents<Collider2D>();
			
			foreach(Collider2D col in cols)
				if(col is CircleCollider2D)
					targetCollider = col;

			playerRenderer = targetCollider.gameObject.GetComponent<SpriteRenderer>();

		}

	}

	void Update () {

		if(shoot){

			if(isPlayer){

				bool isGrounded = targetCollider.gameObject.GetComponent<PlatformerCharacter2D>().grounded;
				playerRenderer.color = GameControl.instance.PlayerHurtColor;

				//SpriteRenderer renderer = targetCollider.gameObject.GetComponent<SpriteRenderer>();

				if(isGrounded){
					shoot = false;
					//playerRenderer.color = startingRendererColor;
					return;
				}
			}


			float _speed = Time.deltaTime * speed;
			throwTarget.z = targetCollider.transform.position.z;

			// calculate straight-line lerp position:
			Vector3 currentPos = Vector3.Lerp(targetCollider.transform.position, throwTarget, _speed);
			
			// add a value to Y, using Sine to give a curved trajectory in the Y direction
			currentPos.y += trajectoryHeight * Mathf.Sin(Mathf.Clamp01(_speed) * Mathf.PI);
			
			// finally assign the computed position to our gameObject:
			targetCollider.transform.position = currentPos;

			var distanceSquared = currentPos.x - throwTarget.x;
			distanceSquared *= distanceSquared;

			shoot = distanceSquared > .1f;

			Debug.Log("Atual: " + currentPos);
			Debug.Log("Alvo: " + throwTarget);
		}else if (!shoot && isPlayer && GameControl.instance.currentHealthPoints > 0 &&
		          playerRenderer.color == GameControl.instance.PlayerHurtColor &&
		          playerRenderer.gameObject.GetComponent<Platformer2DUserControl>().enabled)
			playerRenderer.color = LevelControl.instance.PlayerStartingRendererColor;
	}
	
	
	public void OnCollisionEnter2D(Collision2D col){

		if(col.collider != targetCollider)
			return;

		if(isPlayer){

			if(GameControl.instance.currentHealthPoints <= 0)
				return;

			facingRight = targetCollider.gameObject.GetComponent<PlatformerCharacter2D>().facingRight;
		}

		//throwTarget = facingRight? leftTarget : rightTarget;

		if(facingRight){

			throwTarget = leftTarget.transform.position;

			float minX = transform.position.x - throwTarget.x;

			float actualX = targetCollider.transform.position.x - throwTarget.x;

			if(actualX < minX)
				throwTarget.x += actualX - minX;

		}else{

			throwTarget = rightTarget.transform.position;
			
			float minX = throwTarget.x - transform.position.x;
			
			float actualX = throwTarget.x - targetCollider.transform.position.x;
			
			if(actualX < minX)
				throwTarget.x += minX - actualX;
		}


		shoot = true;


	}

}
