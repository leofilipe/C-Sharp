using UnityEngine;
using System.Collections;

public class SavingDialog : MonoBehaviour {
	
	public GameObject dialogScreen;			//the tip screen or game object

	public float onScreenDuration = 4.0f;	//how long the message must be shown on screen
	
	private float _duration;				//controller for the elapsed time

	private SaveAssistant saveGame;			//instance of SaveAssistant for this game object

	// Use this for initialization
	void Start () {
	
		//deactivate the saving tip
		dialogScreen.SetActive(false);

		//retrive the SaveAssistant instance of this game object
		saveGame = GetComponent<SaveAssistant>();

		//restart the duration
		_duration = onScreenDuration;
	}
	
	// Update is called once per frame
	void Update () {
	
		//if the game is saving
		if(saveGame.isSaving){
			//keep the saving tip activated
			dialogScreen.SetActive(true);
		}

		//if the saving tip is activated
		if(dialogScreen.activeSelf){

			//decrease the duration
			_duration -= Time.deltaTime;

			//if it has reached at least zero, deactivate the tip
			if(_duration <= 0)
				dialogScreen.SetActive(false);
		}
		//if the tip is not activated, restart the duration 
		//(this resolves a blinking bug)
		else
			_duration = onScreenDuration;
	}
}
