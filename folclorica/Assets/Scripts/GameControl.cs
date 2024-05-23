using UnityEngine;
using System.Collections;

/// <summary>
/// Game control. Intermediates acces to players hit points, 
/// user account and level scores.
/// </summary>
public class GameControl : MonoBehaviour {

	//the one and only variable representing the game general controls
	private static GameControl _instance;

	//variable used for accesing and editting data of the game general controls
	//it ensures that there is no more than one of a class instance at a time
	public static GameControl instance {
		get{
			if(_instance == null){
				
				_instance = GameObject.FindObjectOfType<GameControl>();
				DontDestroyOnLoad(_instance);
			}
			
			return _instance;
		}
	}

	//-- SAVE GAME ------------------
	public string username; 				//this value is set during the game login
	public string password; 				//this value is set during the game login

	//HEALTH POINTS
	public int maxHealthPoints = 2;			//player's current max number of health points. It starts the same  
											//as the user's current health points and might increase over time
	public int currentHealthPoints = 2;		//player's current humber of health points

	public int[] score;						//player's score, it is kept for each level


	[HideInInspector]
	public  SaveAssistant saveAssistant;	//assistant for saving game data

	[HideInInspector]
	public bool RestartLevel = false;
	[HideInInspector]
	public string LastFinishedLevelKey;

	//public LevelControl CurrentLevel;

	public int deafaultGameMaxBonus = 3000;

	public readonly Color PlayerHurtColor = new Color(.98f, .36f, .36f); //(249/255, 91/255, 91/255); //colors are expressed as RGB values from 0 to 1;

	public static int DEFAULT_BONUS {get; private set;}

	// Use this for initialization
	void Awake () {

		//if there is no game instance
		if(_instance == null){

			//instantiate the score array
			score = new int[5];

			//set the default game bonus
			DEFAULT_BONUS = deafaultGameMaxBonus;

			//if either the max health points or the current health points is an odd number,
			//make it even
			if(maxHealthPoints%2 == 1)
				maxHealthPoints++;
			
			if(currentHealthPoints%2 == 1)
				currentHealthPoints++;

			//retrieve the save assistant component from this game object
			saveAssistant = gameObject.GetComponent<SaveAssistant>();

			//if there is no pause menu attatched to this game object Pauser class 
			//(as for example in the login screen), then disable the class to avoid erros.
			if(gameObject.GetComponentInChildren<Pauser>().pauseMenu == null)
				gameObject.GetComponentInChildren<Pauser>().enabled = false; //transform.gameObject.SetActive(false);

			//set the singleton instance as this one
			_instance = this;

			//set to not destroy it on future loads
			DontDestroyOnLoad(this);

		}
		//if there is an instance of the object and it is not this one
		else if (_instance != this){

			//destroy this instance of the object
			Destroy(this.gameObject);
		}
	}

	//void Start(){
		/*Debug.Log("Starting...");
		if(Pauser.instance != null)
			if(Pauser.instance.isPaused)
				Pauser.instance.PauseGame();*/
	//}
}
