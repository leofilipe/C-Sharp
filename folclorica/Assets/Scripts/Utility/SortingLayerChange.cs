using UnityEngine;
using System.Collections;

public class SortingLayerChange : MonoBehaviour {
	
	public string objectTag = "";

	public string targetLayer;

	public int targetLayerOrder;

	void Start(){

		//Debug.Log("SortingLayerChange: " + gameObject.name);

	}
	
	void OnTriggerExit2D(Collider2D other){

		SwithcLayer(other.gameObject);
	}

	void OnCollisionExit2D(Collision2D other){
		
		SwithcLayer(other.gameObject);
	}

	void SwithcLayer(GameObject other){

		//Debug.Log(gameObj.tag + " " + objectTag.ToLower());

		if(other.tag.ToLower() == objectTag.ToLower()){
			gameObject.GetComponent<SpriteRenderer>().sortingLayerName = targetLayer;
			gameObject.GetComponent<SpriteRenderer>().sortingOrder = targetLayerOrder;
		}
	}
}
