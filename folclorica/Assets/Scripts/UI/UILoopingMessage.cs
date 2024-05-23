using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILoopingMessage : MonoBehaviour {

	public Collider2D triggeredBy;				//collider of the Game Object who triggers the tip
	
	public GameObject dialogScreen;				//the tip screen
	
	public Text dialogScreenText;				//the text of the tip screen

	public float onScreenDuration = 2;			//how long the message must be shown on screen
	
	private float _duration = 0;				//controller for the elapsed time

	public float wait = 10;						//how long must wait to show the message again

	public Text[] msgText;						//the message pool for the text of the chat

	public bool run {get; private set;}			//flag to indicate that the chat has been concluded at least one time
	
	private List<Transform> _siblings;			//single list for all screens of this type

	private int _chatIndex = -1;				//keeps track of the current index of the chat messages

	private Collider2D _selfCollider;			//this game object's own collider. Used for checking purposeses	

	// Use this for initialization
	void Start () {
	
		//_duration = onScreenDuration/msgText.Length;

		//if there is no list, make one
		if(_siblings == null)
			//start a list for the simblings dialog panels
			_siblings = new List<Transform>();
		
		//if the list is empty populate it
		if(_siblings.Count == 0){
			
			//OBS: Up till now, there was no other way that worked to get 
			//the children of a canvas
			
			//get the parent component of the dialog screen
			Transform parent = dialogScreen.transform.parent;
			
			//populate it
			for(int i = 0; i < parent.childCount; i++){
				
				//get the child i
				Transform sibling = parent.GetChild(i);
				
				//if it is a tip panel
				if(sibling.name.Contains("Dialog")){
					
					//deactivate it
					sibling.gameObject.SetActive(false);
					
					//add to the list
					_siblings.Add(sibling);
				}
			}
		}
	
		//try to get this game object's collider
		_selfCollider = GetComponent<Collider2D>();

		//set the running flag as false
		run = false;
	}
	
	// Update is called once per frame
	void Update () {

		bool shouldStop = false;

		if(_selfCollider != null)
			shouldStop = !_selfCollider.enabled;

		//if it is running but should stop
		if(run && shouldStop){
			//reset the controlling parameters
			run = false;
			_duration = 0;

			//Debug.Log("Collier desligado identificado...");
		}

		//if the screen is not and should not be running and the displaying
		//text belongs to this game object, secure the screen is not displaying
		//and exit
		if(!run && _duration <=0){

			if(dialogScreen.activeSelf && MatchMsg(dialogScreenText.text))
				dialogScreen.SetActive(false);

			return;
		}

		//decrease the duration
		_duration -= Time.deltaTime;
		
		//if its running and duration is equal or lower than zero
		if(run && _duration  <=0){

			//incremente the msg line index
			_chatIndex++;

			//if it has reached the max number of lines
			if(_chatIndex >= msgText.Length){

				//reset the _chatIndex and _duration values and exit
				_chatIndex = -1;
				_duration = wait;

				dialogScreen.SetActive(false);
				return;
			}

			//set the displaying text
			dialogScreenText.text = msgText[_chatIndex].text;
			dialogScreenText.font = msgText[_chatIndex].font;
			dialogScreenText.fontSize = msgText[_chatIndex].fontSize;
			dialogScreenText.resizeTextForBestFit = msgText[_chatIndex].resizeTextForBestFit;

			//show the screen
			if(!dialogScreen.activeSelf){		
				dialogScreen.SetActive(true);
			}
			//reset the duration
			_duration = onScreenDuration / msgText.Length;
		}
	}

	/// <summary>
	/// Checks if a given string exists within the array of Text objects
	/// </summary>
	/// <returns><c>true</c>, if message was matched, <c>false</c> otherwise.</returns>
	/// <param name="msg">Message.</param>
	private bool MatchMsg(string msg){

		bool isMatch = false;

		for(int i = 0; i < msgText.Length && !isMatch; i++){

			string text = msgText[i].text;

			isMatch = isMatch || msg == text;
		}

		return isMatch;
	}


	/// <summary>
	/// Raises the trigger stay2d event.
	/// </summary>
	/// <param name="col">Col.</param>
	void OnTriggerStay2D(Collider2D col){
		
		//if the collider belongs to the one who triggers the tip
		if(col == triggeredBy && !run){
			
			//set the showing flag to true
			run = true;
			
			dialogScreen.SetActive(true);
		}
	}

	/// <summary>
	/// Raises the trigger exit2d event.
	/// </summary>
	/// <param name="col">Col.</param>
	void OnTriggerExit2D(Collider2D col){
		
		//if the collider belongs to the one who triggers the tip
		if(col == triggeredBy){
			
			//set the showing flag to false
			run = false;
		}
	}
}
