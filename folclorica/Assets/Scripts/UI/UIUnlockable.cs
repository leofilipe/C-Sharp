using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UIUnlockable : Lock {

	public Image img;
	public Text text;

	//public bool unlocked {get; private set;}

	public void Unlock(bool unlock){

		try{

			unlocked = unlock;
			
			img.enabled = !unlocked;
			text.enabled = unlocked;

		}catch(Exception ex){
			Debug.Log("Em " + gameObject.name + ": " + ex.ToString() + " " + ex.StackTrace);
		}
	}
}
