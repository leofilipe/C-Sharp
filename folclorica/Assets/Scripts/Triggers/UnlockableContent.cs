using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnlockableContent : MonoBehaviour {

	public GameObject unlockTargetUI;

	[HideInInspector]
	public bool unlocked = false;

	void OnTriggerEnter2D(Collider2D col){
		
		if(!(col is BoxCollider2D))
			return;

		if(col.tag == "Player"){
			unlocked = true;

			unlockTargetUI.GetComponent<UIUnlockable>().Unlock(true);
		}
	}
}
