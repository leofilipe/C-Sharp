using UnityEngine;
using System.Collections;

public class ObjectDamage : MonoBehaviour {

	public int damage = 2;
	public Collider2D playerCollider;
	public bool forceFeetCollider = true;

	void Start(){
		if(forceFeetCollider){
			Collider2D[] cols =  playerCollider.gameObject.GetComponents<Collider2D>();
			
			foreach(Collider2D col in cols)
				if(col is CircleCollider2D)
					playerCollider = col;
		}
	}
	void OnCollisionEnter2D(Collision2D other){
		
		if(GameControl.instance.currentHealthPoints <= 0)
			return;

		if(other.collider == playerCollider){
			other.gameObject.GetComponent<PlatformerCharacter2D>().TakeObjDamage(damage, transform);
		}
		
	}
}
