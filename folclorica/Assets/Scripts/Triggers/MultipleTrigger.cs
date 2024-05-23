using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultipleTrigger : MonoBehaviour {


	public Collider2D[] targets;
	public MonoBehaviour runScript;

	//bool[] activeTarget;

	Dictionary<Collider2D, bool> _activeColliders;
	// Use this for initialization
	void Start () {
	
		_activeColliders = new Dictionary<Collider2D, bool>();

		foreach(Collider2D target in targets)
			_activeColliders.Add(target, false);
	}
	
	// Update is called once per frame
	void Update () {
	
		bool activate = true;

		if(_activeColliders == null)
			return;

		if(_activeColliders.Values == null)
			return;

		foreach(bool value in _activeColliders.Values){
			activate = activate && value;
		}

		if(activate != runScript.enabled){

			if(!activate && runScript is UITipDialog){

				if(((UITipDialog)runScript).duration <= 0)
					runScript.enabled = activate;
			}else
				runScript.enabled = activate;
		}


	}

	void OnTriggerEnter2D(Collider2D other){

		if(_activeColliders == null)
			return;

		if(_activeColliders.ContainsKey(other)){
			_activeColliders[other] = true;
		}
	}

	void OnTriggerExit2D(Collider2D other){

		if(_activeColliders == null)
			return;

		if(_activeColliders.ContainsKey(other)){
			_activeColliders[other] = false;
		}
	}
}
