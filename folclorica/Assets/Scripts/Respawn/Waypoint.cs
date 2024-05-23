using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D coll) {

		if (coll.gameObject.tag == "Player" && coll is BoxCollider2D) {

			Vector3 backTo = new Vector3(transform.position.x, transform.position.y + transform.GetComponent<BoxCollider2D>().bounds.size.y, 
			                             coll.gameObject.transform.position.z);

			coll.gameObject.GetComponent<Respawn>().backLocation = backTo;
		}
	}
}
