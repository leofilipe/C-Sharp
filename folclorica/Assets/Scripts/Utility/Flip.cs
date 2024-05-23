using UnityEngine;
using System.Collections;

public class Flip {

	static public bool HorizontalFlip(Transform transform, bool facingRight){
	
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;

		// Switch the way the player is labelled as facing.
		return !facingRight;
	}
}
