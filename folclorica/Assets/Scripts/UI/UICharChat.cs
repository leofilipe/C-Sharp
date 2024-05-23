using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


/// <summary>
/// User interface char chat. Used to display a conversation between the player and a NPC
/// </summary>
public class UICharChat : MonoBehaviour {

	public Collider2D triggeredBy;						//collider of the Game Object who triggers the tip
	
	public GameObject chatScreen;						//the chat screen

	public Text chatScreenText;							//the text of the chat screen

	public Button chatButton;							//The "next" button

	public Text chatButtonText;							//The "next" button text
		
	public Text[] msgText;								//the message pool for the text of the chat

	public AudioSource buttonClickAudio;				//buttonClick audio

	public bool chatDisabledOnStart = true;				//flag to enable the chat start as soon as the game begins or not

	public float waitBeforeLetPlayerMove = 0f;			//how long it should wait before let the player move again

	public bool isDone {get; private set;}				//flag to indicate that the chat has been concluded at least one time

	protected bool _chatting = false;						//flag to start displaying the chat

	protected List<Transform> _siblings;					//single list for all screens of this type

	protected PlatformerCharacter2D _playerControl;		//if it is triggered by the player, get the its controllers to stop its movements

	protected Platformer2DUserControl _playerAuxControl; 	//if it is triggered by the player, get the its secondary controllers to stop its movements

	protected Collider2D _selfCollider;					//this game object's own collider. Used to avoid operations if its disabled.

	protected int _chatIndex = 0;							//keeps track of the current index of the chat messages

	protected bool _isPlayer = false;						//checks if the trigger is the player or not
	
	// Use this for initialization
	protected void Start () {

		//reference for this game object's own collider
		_selfCollider = GetComponent<Collider2D>();

		//check if the chat should be possible from the very begining of the game
		if(chatDisabledOnStart)
			_selfCollider.enabled = false;

		//if there is no list, make one
		if(_siblings == null)
			//start a list for the simblings dialog panels
			_siblings = new List<Transform>();
		
		//if the list is empty populate it
		if(_siblings.Count == 0){
			
			//OBS: Up till now, there was no other way that worked to get 
			//the children of a canvas
			
			//get the parent component of the dialog screen
			Transform parent = chatScreen.transform.parent;
			
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

		//tries to retrieve player's controller from the target triggering collider
		_playerControl = triggeredBy.gameObject.GetComponent<PlatformerCharacter2D>();
		_playerAuxControl = triggeredBy.gameObject.GetComponent<Platformer2DUserControl>();

		//if it was possible, then its the player
		_isPlayer = _playerControl != null && _playerAuxControl != null;

		//reset the chat done flag
		isDone = false;
	}

	/// <summary>
	/// Updates the displaying text to the next line
	/// </summary>
	/// <returns>The line.</returns>
	protected IEnumerator NextLine() {

		//increment the line index
		_chatIndex++;

		EnableButton();

		//if there is an audio to playwhen the button is clicked, play it
		if(buttonClickAudio != null)
			buttonClickAudio.PlayOneShot(buttonClickAudio.clip);

		//if it is lower than the numer of lines
		if(_chatIndex < msgText.Length){

			//set the text to its next line
			chatScreenText.text = msgText[_chatIndex].text;
			chatScreenText.font = msgText[_chatIndex].font;
			chatScreenText.fontSize = msgText[_chatIndex].fontSize;
			chatScreenText.resizeTextForBestFit = msgText[_chatIndex].resizeTextForBestFit;

			//set the propoer button name according to being or not 
			//the last line of text
			if(_chatIndex < msgText.Length - 1)
				chatButtonText.text = "Próximo";
			else
				chatButtonText.text = "Ok";
		}
		//if the index has reached the number of lines
		else{

			//flag the conversation as over
			_chatting = false;

			//wait a moment before disable the window
			yield return new WaitForSeconds (0.2f);
			
			chatScreen.SetActive(false);

			//flag the chat as done
			isDone = true;

			//wait a moment before enable player movements
			yield return new WaitForSeconds (waitBeforeLetPlayerMove);

			//deactivate the trigger to enable player movement.
			GetComponent<Collider2D>().enabled = false;

			//if it was triggered by the player, then enable his movements
			if(_isPlayer){

				_playerControl.enabled = true;
				
				_playerAuxControl.enabled = true;
			}
		}
	}

	protected void PerformChat(Collider2D col){
		//if the collider belongs to the one who triggers the tip
		if(col == triggeredBy && !_chatting){

			//set the propoer button name according to being or not 
			//the last line of text
			if(_chatIndex < msgText.Length - 1)
				chatButtonText.text = "Próximo";
			else
				chatButtonText.text = "Ok";

			//set the showing flag to true
			_chatting = true;
			
			//remove all previous listeners from the button to avoid inconsistencies
			chatButton.onClick.RemoveAllListeners();
			
			//add the next line coroutine to the button
			chatButton.onClick.AddListener(() => StartCoroutine(NextLine()));
			
			//if the windown is disabled
			//OBS: do this check to avoid the text updating up after the last line of dialog is shown
			if(!chatScreen.activeSelf){
				
				//if there are still active siblings, deactivate them
				foreach(Transform sibling in _siblings){
					sibling.gameObject.SetActive(false);
				}
				//as the coroutine only applies when clicking the button, you must manually
				//set the window starting text
				chatScreenText.text = msgText[0].text;
				chatScreenText.font = msgText[0].font;
				chatScreenText.fontSize = msgText[0].fontSize;
				chatScreenText.resizeTextForBestFit = msgText[0].resizeTextForBestFit;

				EnableButton();

				//display the window
				chatScreen.SetActive(true);
			}
			
			//if it was triggered by the player, then stop his movements
			if(_isPlayer){
				_playerControl.enabled = false;
				
				_playerAuxControl.enabled = false;
				
				_playerControl.Move(0, false, false);
				_playerAuxControl.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				
			}
		}
	}
	/// <summary>
	/// Raises the trigger stay2d event.
	/// </summary>
	/// <param name="col">Col.</param>
	protected void OnTriggerStay2D(Collider2D col){

		PerformChat(col);
	}

	protected void OnTriggerExit2D(Collider2D col){

		//if the collider belongs to the one who triggers the tip
		if(col == triggeredBy){
			
			//set the showing flag to false
			_chatting = false;
		}
	}

	public virtual void PerformAction(){

		Debug.Log("Acao 1");
	}

	private void EnableButton(){
		DisableButtonTimer btTimer = GetComponent<DisableButtonTimer>();
		
		if(btTimer != null){
			btTimer.ResetTimer(_chatIndex);
		}
	}
}
