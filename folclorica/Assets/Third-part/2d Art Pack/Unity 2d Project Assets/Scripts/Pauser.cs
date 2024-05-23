using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System;
using beffio.OneMinuteGUI;

public class Pauser : MonoBehaviour {

	//indicates if the game is paused or not
	[HideInInspector]
	public bool isPaused = false;		

	//[HideInInspector]
	public bool forcedSuspension {get; private set;}

	public bool forcePauseOnBios = false;

	public MenuManagerLevel menu;

	//private GameObject lastPanel;
	//the one and only variable representing the game general controls
	private static Pauser _instance;	

	//variable used for accesing and editting data of the game general controls
	//it ensures that there is no more than one of a class instance at a time
	public static Pauser instance{
		get{
			if(_instance == null){
				_instance = GameObject.FindObjectOfType<Pauser>();

				if(_instance != null)
					DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}

	//array of game object (usually buttons) originally locked on the user's game interface
	public UIUnlockable[] uiUnlockables;

	public AudioSource[] muteAudios;

	//game object of the pause menu
	public GameObject pauseMenu;
	
	// Awake this instance.
	void Awake(){
		if(_instance == null){
			_instance = this;
			DontDestroyOnLoad(this);
		}else{
			if(this != _instance){

				if(_instance.pauseMenu == null){
					_instance.pauseMenu = pauseMenu;
					_instance.uiUnlockables = uiUnlockables;
					_instance.muteAudios = muteAudios;
					_instance.enabled = true;//transform.gameObject.SetActive(true);
				}

				Destroy(this.gameObject);
			}
		}

		//when restarting from pause menu, sometimes the game will remain paused.
		//this, forces the game to resume when restarting
		if(instance.isPaused && !instance.forcedSuspension)
			PauseGame();
	}

	// Update is called once per frame
	protected void Update () {

		//if player is alive
		if(GameControl.instance.currentHealthPoints > 0){
			//if the P or Esc key are pressed on the keyboard
			if(Input.GetKeyUp(KeyCode.P) || Input.GetKeyUp(KeyCode.Escape)){
				//pause the game
				PauseGame();
			}

			//if the game is paused
			if(isPaused)
				Time.timeScale = 0;	//set the time scale to zero in order to suspend all game processes
			//otherwise
			else
				Time.timeScale = 1;	//set the time scale to one in order to retake all game processes
		}

	}

	public void ForceGameSuspension(bool suspend){

		//invert the value of the paused game flag
		instance.isPaused = suspend;
		//if the game is paused
		if(isPaused)
			Time.timeScale = 0;	//set the time scale to zero in order to suspend all game processes
		//otherwise
		else
			Time.timeScale = 1;	//set the time scale to one in order to retake all game processes

		instance.forcedSuspension = suspend;

		Debug.Log("Jogo parado (suspenso): " + instance.isPaused);
	}

	/// <summary>
	/// Pauses the game.
	/// </summary>
	public void PauseGame(){

		//invert the value of the paused game flag
		instance.isPaused = !instance.isPaused;

		//if the game is paused
		if(instance.isPaused){

			//retrieve the game panels
			//Transform[] panels = instance.pauseMenu.getchi;

			//for each of them
			foreach(Transform panel in instance.pauseMenu.transform){
				//try to update it
				UpdatePanelData(panel as RectTransform);
			}
		}
		//else
		else{

			//try to play the button's audio when closing the pause menu
			//(it is not always working)
			AudioSource audio = GetComponent<AudioSource>();

			//set its parameters
			audio.volume = 1;
			audio.priority = 128;

			//play it
			audio.PlayOneShot(audio.clip);

			/*if(lastPanel != null)
				lastPanel.SetActive(false);*/

			/*foreach(Transform panel in instance.pauseMenu.transform){
				if(panel.name == "panelInfo"){
					panel.gameObject.SetActive(false);
					//lastPanel = panel.gameObject;
					//Debug.Log("panelInfo desativado...");
				}else if(panel.name == "panelControls"){
					panel.gameObject.SetActive(false);
					//lastPanel = panel.gameObject;
					//Debug.Log("panelControls desativado...");
				}else if(panel.name == "panelChars"){
					panel.gameObject.SetActive(false);
					//lastPanel = panel.gameObject;
					//Debug.Log("panelChars desativado...");
				}
			}*/
		}

		Debug.Log("Jogo parado (pausado): " + instance.isPaused);

		//set the activation state of the pause menu as the same
		//of the paused game flag
		instance.pauseMenu.SetActive(isPaused);

		foreach(AudioSource audio in muteAudios){
			//audio.mute = isPaused;

			if(isPaused){
				if(audio.isPlaying)
					audio.Pause();
			}else{

				if(!audio.isPlaying && audio.time != 0)
					audio.UnPause();
			}
		}

	}

	/// <summary>
	/// Updates the data of a panel on the pause menu. 
	/// Should not be used for the character bios panel, as the
	/// buttons unlockable feature requires special procediments
	/// to be taken for each button instead of the panel as a whole.
	/// </summary>
	/// <param name="panel">the target panel.</param>
	private void UpdatePanelData(RectTransform panel){

		//if its the panel info ("Resmo" tab)
		if(panel.name == "panelInfo"){

			//update the data of the panel's fields
			panel.GetComponent<UITabInfo>().UpdateFields();
		}

		if(forcePauseOnBios){

			if(panel.name == "panelInfo"){
				panel.gameObject.SetActive(false);
				//lastPanel = panel.gameObject;
				//Debug.Log("panelInfo desativado...");
			}else if(panel.name == "panelControls"){
				panel.gameObject.SetActive(false);
				//lastPanel = panel.gameObject;
				//Debug.Log("panelControls desativado...");
			}else if(panel.name == "panelChars"){
				panel.gameObject.SetActive(false);
				//lastPanel = panel.gameObject;
				//Debug.Log("panelChars desativado...");
			}else if(panel.name == "panelCharsDesc"){

				int r = UnityEngine.Random.Range(0, (uiUnlockables.Count() - 1));

				while(!uiUnlockables[r].unlocked)
					r = UnityEngine.Random.Range(0, uiUnlockables.Count() - 1);

				menu.SetCallerLockedButton(uiUnlockables[r].gameObject);
				menu.GoToLockedMenu(panel.gameObject);
					//GoToLockedMenu
				//biosToOpen = uiUnlockables[r].gameObject.name;

				panel.gameObject.SetActive(true);
			}

		}
	}
	
}
