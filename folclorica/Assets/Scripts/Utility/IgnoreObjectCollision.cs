using UnityEngine;
using System.Collections;

public class IgnoreObjectCollision : MonoBehaviour {

	public Collider2D target;
	public bool ignoreNow = true;


	private Collider2D _selfCollider;
	
	// Use this for initialization
	void Start () {

		_selfCollider = GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
	
		Physics2D.IgnoreCollision(_selfCollider, target, ignoreNow);
	}
}
