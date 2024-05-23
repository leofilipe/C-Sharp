using UnityEngine;
using System.Collections;

/// <summary>
/// Face target. 
/// Forces a game object to face a single preestrablished target at all times.
/// OBS: transform.LookAt(target) works on the rotation, so when the target is above or 
/// beneath the game object, the game object will also incline up and down on the proper 
/// angle in order to face it
/// </summary>
public class FaceTarget : MonoBehaviour {

	public Transform target;			//target that must be faced

	public bool useLookAt = false;		//flags if should use look at or not. Check the OBS on class summary.

	private bool _facingRight;			//checks if the game object is facing right or left

	// Use this for initialization
	void Start () {
	
		//checks if the character starts facing left or right
		_facingRight = transform.localScale.x > 0;//target.position.x > transform.position.x;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		//if should not use look at
		if(!useLookAt){
			//get the actual direction the character must face
			bool facing = target.position.x > transform.position.x;
			
			//if the actual direction and the current one are different
			//then turn the game object by inverting the x scale
			if(facing != _facingRight){
				
				_facingRight = facing;					
				
				Vector3 scale = transform.localScale;					
				scale.x = -scale.x;
				
				transform.localScale = scale;
			}
		}
		//otherwise, use look at
		else
			transform.LookAt(target.position);
	}
}
