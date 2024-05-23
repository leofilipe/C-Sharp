using UnityEngine;
using System.Collections;

public class TurnScriptOff : MonoBehaviour {

	public bool runOnStart = false;
	public MonoBehaviour actionScript;


	void Start(){

		if(!runOnStart)
			this.enabled = false;
	}

	// Update is called once per frame
	void Update () {
	
		if(actionScript.enabled)
			actionScript.enabled = false;
	}
}
