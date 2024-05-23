using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class DisableButtonTimer : MonoBehaviour {

	public Button button;
	public float[] duration;

	private float _duration;
	// Use this for initialization
	void Start () {
	
		button.interactable = false;
		_duration = duration[0];
	}
	
	// Update is called once per frame
	void Update () {
	
		if(_duration > 0){
			button.interactable = false;
			_duration -= Time.deltaTime;
		}

		if(_duration <= 0 && !button.interactable){
			button.interactable = true;
			this.enabled = false;
		}
	}

	public void ResetTimer(int index){

		try{
			_duration = duration[index];
			Debug.Log(_duration + " index: " + index);
		}catch(Exception ex){
			Debug.Log(ex.StackTrace);
		}
		button.interactable = false;
		this.enabled = true;
	}
}
