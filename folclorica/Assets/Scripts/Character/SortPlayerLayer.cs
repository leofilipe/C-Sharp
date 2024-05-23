using UnityEngine;
using System.Collections;

public class SortPlayerLayer : MonoBehaviour {

	public string targetSortingLayer;			//the target sorting layer for when the character steps on this object
	public int targetSortingOrder;				//the sorting order for the layer for when the character steps on this object

	/// <summary>
	/// Raises the collision enter2 d event.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnCollisionEnter2D(Collision2D coll){

		ToTargetSortingLayer(coll.collider, false);

	}

	/// <summary>
	/// Raises the collision stay2 d event.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnCollisionStay2D(Collision2D coll){

		ToTargetSortingLayer(coll.collider, false);
	}

	/// <summary>
	/// Raises the collision exit2 d event.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnCollisionExit2D(Collision2D coll){
		
		ToTargetSortingLayer(coll.collider, true);
	}

	/// <summary>
	/// Sets the player sorting layer to either its default layer or the alternate one
	/// </summary>
	/// <param name="coll">Coll.</param>
	/// <param name="toDefaultLayer">If set to <c>true</c> to default layer.</param>
	void ToTargetSortingLayer(Collider2D coll, bool toDefaultLayer){

		//if its not the player, finish and do nothing
		if(coll.gameObject.tag != "Player")
			return;

		//if it is not the player's CircleCollider2D finish and do nothing
		if(!(coll is CircleCollider2D))
			return;
		//get the player's SpriteRenderer
		SpriteRenderer playerRenderer = coll.gameObject.
			GetComponentInChildren<SpriteRenderer>();

		//if must swithc the player's sorting layer to its alternate ones
		if(!toDefaultLayer){

			//set the player sorting layer and its order to their alternate values
			playerRenderer.sortingLayerName = targetSortingLayer;
			playerRenderer.sortingOrder = targetSortingOrder;
			
		}else{ //otherwise

			//set the player sorting layer and its order to the default ones
			playerRenderer.sortingLayerName = LevelControl.instance.playerDefaultSortingLayer;
			playerRenderer.sortingOrder = LevelControl.instance.playerDefaultSortingOrder;
		}
	}
}
