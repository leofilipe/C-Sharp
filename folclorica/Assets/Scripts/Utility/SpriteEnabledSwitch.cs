using UnityEngine;
using System.Collections;

/// <summary>
/// Used mainly to Enable or Disable an sprite and its colliders when they pass through a specific region.
/// </summary>
public class SpriteEnabledSwitch : MonoBehaviour {
	
	public bool enableFirst = true;			//if the action is to enable the sprite and their colliders
	public bool ignoreOtherLayers = true;
	public string targetLayer = "Climber";

	private int _targetLayer;


	void Start(){

		_targetLayer = LayerMask.NameToLayer(targetLayer);
	}
	/// <summary>
	/// Raises the trigger enter2 d event.
	/// </summary>
	/// <param name="col">Col.</param>
	void OnTriggerEnter2D(Collider2D other){

		if(ignoreOtherLayers && other.gameObject.layer != _targetLayer){
		//if(ignoreOtherLayers && other.gameObject.layer != gameObject.layer){
			return;
		}
		//if the action is to enable, it must be performed during the enter event
		if(enableFirst){

			//Retrieve the object's sprite renderer and its collider
			SpriteRenderer renderer = other.gameObject.GetComponent<SpriteRenderer>();
			Collider2D collider = other.GetComponent<Collider2D>();

			//enable both of them
			renderer.enabled = true;
			collider.enabled = true;

			//Debug.Log("Changed: " + other.gameObject.name);
		}

	}

	/// <summary>
	/// Raises the trigger exit2 d event.
	/// </summary>
	/// <param name="col">Col.</param>
	void OnTriggerExit2D(Collider2D other){

		if(ignoreOtherLayers && other.gameObject.layer != _targetLayer){
		//if(ignoreOtherLayers && other.gameObject.layer != gameObject.layer){
			return;
		}

		//if the action is to disable, it must be performed during the exit event
		if(!enableFirst){

			//if it is the player, finish and do nothing
			if(other.gameObject.tag == "Player")
				return;

			//Retrieve the object's sprite renderer and its collider
			SpriteRenderer renderer = other.gameObject.GetComponent<SpriteRenderer>();
			Collider2D collider = other.GetComponent<Collider2D>();

			//disable both of them
			renderer.enabled = false;
			collider.enabled = false;

		}
	}	
}
