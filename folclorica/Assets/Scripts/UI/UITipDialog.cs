using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UITipDialog : MonoBehaviour {

	public Collider2D triggeredBy;				//collider of the Game Object who triggers the tip

	public GameObject dialogScreen;				//the tip screen

	public Text dialogScreenText;				//the text of the tip screen

	public Text msgText;						//the message for the text of the tip screen

	public float onScreenDuration = 2;			//how long the message must be shown on screen

	public float duration {get; private set;}	//controller for the elapsed time

	private bool _startTip = false;				//flag to start displaying the tip

	private static List<Transform> _siblings;	//single list for all screens of this type

	private static GameObject LastDialog;

	// Use this for initialization
	void Start () {

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

	}
	
	// Update is called once per frame
	void Update () {
	
		//if by chance a problem ocurred and siblings is null
		if(_siblings == null)
			Start ();

		//if should start showing the tip
		if(_startTip){

			//set the flag to false
			_startTip = false;

			//restart the duration
			duration = onScreenDuration;

			//deactivate all tips upon new start
			foreach(Transform sibling in _siblings){
				sibling.gameObject.SetActive(false);
			}

			//reset the tip text
			dialogScreenText.text = msgText.text;
			dialogScreenText.font = msgText.font;
			dialogScreenText.fontSize = msgText.fontSize;
			dialogScreenText.resizeTextForBestFit = msgText.resizeTextForBestFit;

			//show the tip
			dialogScreen.SetActive(true);

			LastDialog = dialogScreen;
		}

		//if the screen is activated and the text showing corresponds to this
		//object's text
		if(dialogScreen.activeSelf && dialogScreenText.text == msgText.text){

			//decrease the duration
			duration -= Time.deltaTime;

			//if its equal or lower than zero
			if(duration  <=0){
				dialogScreen.SetActive(false);
				dialogScreenText.text = "";
			}
		}
		//otherwise
		else {
			duration = 0;
		}

	}

	/// <summary>
	/// Raises the trigger stay2d event.
	/// </summary>
	/// <param name="col">Col.</param>
	void OnTriggerStay2D(Collider2D col){

		if(triggeredBy == null)
			return;

		//if the collider belongs to the one who triggers the tip
		//OBS: the null checking is to avoid the player triggering nearby
		//tips while on rope, due the incosistency ropes create.
		//TODO MAYBE IT IS BEST TO DEACTIVE PLAYER'S COLLIDERS WHILE ON ROPE. MUST CHECK
		if(col == triggeredBy && triggeredBy.transform.parent == null){

			//set the showing flag to true
			_startTip = true;
		}
	}

	/// <summary>
	/// Used for externally triggering the tip.
	/// </summary>
	public void TriggerTip(){
		_startTip = true;
	}

	public static void ResetDialogs(){

		if(_siblings == null)
			return;

		while(_siblings.Count > 0)
			_siblings.RemoveAt(0);
	}

	public static void HideLastTip(){

		if(LastDialog != null)
			LastDialog.SetActive(false);
	}
}
