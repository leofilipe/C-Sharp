using UnityEngine;
using System.Collections;

public class AfterChatAction : MonoBehaviour {

	public UICharChat chat;
	public float waitAfter = 0f;
	public MonoBehaviour actionScript;
	public bool deactivateTargetScriptOnStart = true;
	public bool activateTargetScript = true;

	private bool initialState;

	// Use this for initialization
	void Awake () {
	
		if(deactivateTargetScriptOnStart)
			actionScript.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
		if(chat.isDone && actionScript.enabled != activateTargetScript){//!actionScript.enabled){

			if(waitAfter > 0){

				waitAfter -= Time.deltaTime;
				return;
			}

			actionScript.enabled = activateTargetScript;
		}
	}
}
