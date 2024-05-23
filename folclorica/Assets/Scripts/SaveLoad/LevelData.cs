using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

/// <summary>
/// Level data. Aggregates all information that must be saved for a game level.
/// </summary>
[System.Serializable]
public class LevelData {
	//<username>.<id_fase>.<id_save>.fd
	public DateTime time;					//last update time of the level data
	public int lastCheckpointIndex = -1;			//index of the last check point triggered to save the data of a game level
	public bool isFadedOut;					//flag indicating whether or not

	//private string _username;				//player's username
	//private string _password;				//player's password
	//private string _currentSaveID;			//player's current save id <to be used for different saves of a same 
											//game level for a same user>

	private string _levelName;				//name of the unity scene for a game level.
	private string _levelID;				//level number, saved as a string

	//private bool _activeEffects;			//flag that indicates weather or not the above effects are active.
	//MIGHT BE UNNECESSARY as the LevelControl script will take care of that based only on the value of isFadedOut


	private PlayerData _playerData;									//level's player data to be saved
	private Dictionary<string, MobData> _mobsData;					//level's mob data to be saved. Objects' names are used as key to retrieve their values
	private Dictionary<string, ColletibleData> _colletibleData;		//level's colletible data to be saved. Objects' names are used as key to retrieve their values
	private Dictionary<string, PuzzleData> _puzzlesData;				//level's puzzle data to be saved. Objects' names are used as key to retrieve their values
	private Dictionary<string, bool> _unlockedBiosData;				//list of character bios unlocked by the player. Objects' names are used as key to retrieve their values

	private Dictionary<string, EventData> _eventsData;		

	private Dictionary<string, BgAudio> _bgAudios;

	private bool _isLevelFinished = false;

	[HideInInspector]
	public float levelTime = 0f;

	[HideInInspector]
	public int levelBonus = 0;			//bonus will not be accounted for level gameplay, being calculated and displayed only at its conclusion. 
											//Hence, a variable sufices in place of an array

	private static LevelData _instance;		//single instance for this class' object

	[System.Serializable]
	struct PlayerData{						//player's data
		
		public int healthPoints;			//player's current number of health points
		public int maxHealthPoints;			//player's current max number of health points
		public int[] score;					//player's score at each leavel

		public string username;				//player's username
		public string password;				//player's password

		public Position position;			//player's positon
	}

	[System.Serializable]
	struct MobData{							//mobs' data. Mob's name is not saved due to redundance. It is the key to retrieve its values

		public int healthPoints;			//mob' current number of health points
		public Position position;			//mob' positon
	}

	[System.Serializable]
	struct ColletibleData{					//colletible's data. Colletibes' name are not saved due to redundance. It is the key to retrieve their values
		public bool enabled;				//flag indicating if the item was already collected or not.
		public bool unlocked;				//flag indicanting that this colletible unlocks an item. Just a few colletibles have it
		public bool isSpecial;
	}
	
	[System.Serializable]
	struct PuzzleData{						//puzzle's data. Puzzles' name are not saved due to redundance. It is the key to retrieve their values

		public string sortingLayerName;		//current sorting layer of the puzzle object
		
		public int sortingLayerOrder;		//current sorting layer order of the puzzle object
		
		public bool enabled;				//flag indicating if the puzzle was already collected or not.
		public bool hasChildren;			//flag indicating if the puzzle object has children attatched to it.
		
		public Position position;			//puzzle's positon

		public Position eulerRotation;		//puzzle's rotation in euler angles

		public bool[] locks;
	}

	[System.Serializable]
	struct EventData{

		public string[] behaviours;
		public bool[] bhvEabled;

		public bool[] locksEnabled;

		public bool[] colsEnabled;

		//public Collider2D[] colliders;

		public bool activeSelf;


	}

	[System.Serializable]
	struct BgAudio{
		public bool playOnAwake;
		public bool loop;
	}

	[System.Serializable]
	struct Position{						//position. A tree axis serializable vector.

		public float x;						//x axis position
		public float y;						//y axis position
		public float z;						//z axis position

		public Position(float h, float v, float d){
			x = h;
			y = v;
			z = d;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="LevelData"/> class.
	/// </summary>
	/// <param name="dataSaveName">Data save name.</param>
	private LevelData(string dataSaveName){


		try{
			//data save must be made of 3 parts separated by a dot character
			string[] info = dataSaveName.Split('.');

			//first is the player's username
			//_username = info[0];
			//second is the current level id (it's level number)
			_levelID = info[1];
			//third is the current save index for this player at this level
			//_currentSaveID = info[2];

			//retrieve the user's password from the game control instancce
			//_password = GameControl.instance.password;

		}
		//if anything wrogs happens
		catch(Exception ex){
			//show it on debug log (console)
			Debug.Log(ex.GetType() + " " + ex.StackTrace);
		}
	}

	/// <summary>
	/// Prepares the level to save.
	/// </summary>
	private void PrepareLevelToSave(){

		try{

			//retrieve the level name from the current loaded scene
			_levelName = Application.loadedLevelName;

			//check if the level is faded out or not and store the value
			isFadedOut = FadeInOut.IS_FADED_OUT; 

			levelTime = LevelControl.instance.levelTime;

			_bgAudios = new Dictionary<string, BgAudio>();

			_isLevelFinished = LevelControl.instance.isLevelFinished;

			foreach(AudioSource audio in LevelControl.instance.bgAudios){

				BgAudio data = new BgAudio();
				data.playOnAwake = audio.playOnAwake;
				data.loop = audio.loop;

				_bgAudios.Add(audio.name, data);
			}

			//check if the level's background efffects are active or not
			//_activeEffects = LevelControl.instance.activeEffects;

			//prepare the different level data to be saved
			SetPlayerToSave(LevelControl.instance.player);
			SetMobsToSave(LevelControl.instance.mobs);
			SetColletiblesToSave(LevelControl.instance.colletibles);
			SetPuzzlesToSave(LevelControl.instance.puzzles);
			SetEventsToSave(LevelControl.instance.events);
			SetUnlockablesToSave(LevelControl.instance.UiUnlockables);


			//store the current time
			time = DateTime.Now;

			Debug.Log("Level concluido (pre-bonus): " + _isLevelFinished);

			if(_isLevelFinished){
				int collected_itens = TotalColletedItens();
				int multiplier = LevelControl.instance.levelMaxScoreBonus;

				//the bonus is the number of collected itens times the multiplier divided by the time took in the game level.
				//That way explore the game and collect its itens is as much important as the time it takes to do it.
				levelBonus = Mathf.RoundToInt(collected_itens * multiplier / (int)levelTime);

				Debug.Log("Level bonus: " + levelBonus + "("+collected_itens+" * "+ multiplier + "/ "+ levelTime + ")");
			}
			//levelBonus = LevelControl.instance.LevelMaxScoreBonus;
		}
		//if anything wrogs happens
		catch(Exception ex){
			//show it on debug log (console)
			Debug.Log(ex.ToString() + " " + ex.StackTrace);
		}
	}

	/// <summary>
	/// Froms the save to level. Restore level control variables from save game to the level.
	/// Currently, only isFadedOut is in need of being accounted for.
	/// </summary>
	public void FromSaveToLevel(){
		
		//set the level as faded ou or not depending on the stored value for it.
		FadeInOut.IS_FADED_OUT = isFadedOut;
		
		//if the level is faded out
		if(isFadedOut){
			//set the sky and camera background colors to black
			RenderSettings.ambientSkyColor = Color.black;
			Camera.main.backgroundColor = Color.black;
		}

		
		foreach(AudioSource audio in LevelControl.instance.bgAudios){

			BgAudio data = _bgAudios[audio.name];

			audio.playOnAwake = data.playOnAwake;
			audio.loop = data.loop;

			if(audio.playOnAwake){
				audio.Play();
			}else{
				audio.Stop();
			}
		}
	}

	/// <summary>
	/// Retrieves and stores the player's data within the _playerData object.
	/// </summary>
	/// <param name="player">the player game object.</param>
	private void SetPlayerToSave(GameObject player){

		//instantiate the player data object
		_playerData = new PlayerData();

		//retrieve and store the player's username and password from GameControl
		_playerData.username = GameControl.instance.username;
		_playerData.password = GameControl.instance.password;

		//retrieve and store the player's health points
		_playerData.healthPoints = GameControl.instance.currentHealthPoints;
		_playerData.maxHealthPoints = GameControl.instance.maxHealthPoints;

		//retrieve and store the player's score and position
		if(_playerData.score == null){
			_playerData.score = new int[GameControl.instance.score.Length];
		}

		_playerData.score[int.Parse(_levelID) - 1] = LevelControl.instance.GetScore();
		_playerData.position = new Position(player.transform.position.x, player.transform.position.y, player.transform.position.z);

		Debug.Log("Salvando pontuacao: " + _playerData.score[int.Parse(_levelID) - 1] + " ..." );
	}

	/// <summary>
	/// Replaces player data by the saved data.
	/// 
	/// ---WARNING!!!---
	/// username and password SHOULD ONLY BE SET THROUGH THE LOGIN SCREEN. IT SHOULD BOT BE DONE HERE.
	/// </summary>
	/// <param name="targetObject">the target game object.</param>
	public void FromSaveToPlayer(GameObject targetObject){
		
		//retrieve and set player's health and score for this level
		GameControl.instance.currentHealthPoints = _playerData.healthPoints;
		GameControl.instance.maxHealthPoints = _playerData.maxHealthPoints;
		GameControl.instance.score[int.Parse(_levelID) - 1] = _playerData.score[int.Parse(_levelID) - 1];

		//Debug.Log("HP: " + GameControl.instance.currentHealthPoints + " MaxHP: " + GameControl.instance.maxHealthPoints + " Score: " + GameControl.instance.score[int.Parse(_levelID) - 1]);

		//set the player's position according to the saved one.
		targetObject.transform.position = new Vector3(_playerData.position.x, _playerData.position.y, _playerData.position.z);
		
		//set the camera's position to just a little above the player
		Camera.main.transform.position = new Vector3(_playerData.position.x, _playerData.position.y + 2f, 
		                                             Camera.main.transform.position.z);
	}

	/// <summary>
	/// Retrieves and stores the mob's data within the _mobsData dictonary object.
	/// Mobs' names are used as keys to the dictionary, hence, they must be unique
	/// </summary>
	/// <param name="mobs">the mobs game objects.</param>
	private void SetMobsToSave(GameObject[] mobs){

		//instantiate the mobs' dictionary data object
		_mobsData = new Dictionary<string, MobData>();

		//for each mob game object
		foreach(GameObject mob in mobs){

			//instantiate a new mob data
			MobData aMobData = new MobData();

			//retrieve and store its current health points
			aMobData.healthPoints = mob.GetComponentInChildren<MobHitPoints>().hitPoints;

			//if its hit points are equal or smaller than zero, 
			//then retrieve and store its current position
			if(aMobData.healthPoints <= 0)
				aMobData.position = new Position(mob.transform.position.x, mob.transform.position.y, mob.transform.position.z);

			//add this mob's data to the dictionary according to its name
			_mobsData.Add(mob.name, aMobData);
		}
	}

	/// <summary>
	/// Retrieves the mobs saved data and set them on the level's mobs.
	/// </summary>
	/// <param name="targetObjects">Target game objects.</param>
	public void FromSaveToMobs(GameObject[] targetObjects){
		
		//for each mob game object
		foreach(GameObject mob in targetObjects){
			
			//retrieve the mob data from the dictionary by its name
			MobData aMobData = _mobsData[mob.name];

			//set its health poitns to the stored value
			mob.GetComponentInChildren<MobHitPoints>().hitPoints = aMobData.healthPoints;

			//if the hit points are equal or lower than zero, then set its position to the same as the stored value
			if(aMobData.healthPoints <= 0)
				mob.transform.position = new Vector3(aMobData.position.x, aMobData.position.y, aMobData.position.z);
		}
	}

	/// <summary>
	/// Retrieves and stores the colletibles's data within the _colletibleData dictonary object.
	/// Colletibles' names are used as keys to the dictionary, hence, they must be unique
	/// </summary>
	/// <param name="colletibles">the colletibles game objects.</param>
	private void SetColletiblesToSave(GameObject[] colletibles){

		//instantiate the colletibles' dictionary data object
		_colletibleData = new Dictionary<string, ColletibleData>();

		//for each colletible game object
		foreach(GameObject colletible in colletibles){

			//instantiate a new colletible data
			ColletibleData aColletible = new ColletibleData();

			//If the item was colleted, then its collider is the first value to become inactive while
			//its activeSelf is the last one (so to properly fade particles). Hence, the colletible data
			//enable value is set as the same as the game object's collider. 
			aColletible.enabled = colletible.GetComponent<Collider2D>().enabled;

			aColletible.isSpecial = colletible.GetComponent<Colletible>().isSpecial;

			//Check if this colletible unlocks a menu. If it this, store its value. If not, set it to false
			if(colletible.GetComponent<UnlockableContent>())
				aColletible.unlocked = colletible.GetComponent<UnlockableContent>().unlocked;
			else
				aColletible.unlocked = false;
			//add this colletible's data to the dictionary according to its name
			_colletibleData.Add(colletible.name, aColletible);
		}
	}

	/// <summary>
	/// Retrieves the colletibles saved data and set them on the level's colletibles.
	/// </summary>
	/// <param name="targetObjects">Target game objects.</param>
	public void FromSaveToColletibles(GameObject[] targetObjects){

		//for each colletible game object
		foreach(GameObject colletible in targetObjects){

			//retrieve the colletible data from the dictionary by its name
			ColletibleData aColletibleData = _colletibleData[colletible.name];

			//if this data unlocked an item, set its value on the game object
			if(aColletibleData.unlocked)
				colletible.GetComponent<UnlockableContent>().unlocked = true;

			//set the colletible game obejct active state as the same as the stored value
			//do the same for the collider as its value is the one retrieved for saving
			colletible.SetActive(aColletibleData.enabled);
			colletible.GetComponent<Collider2D>().enabled = aColletibleData.enabled;
		}
	}

	/// <summary>
	/// Retrieves and stores the puzzles's data within the _puzzlesData dictonary object.
	/// Puzzles' names are used as keys to the dictionary, hence, they must be unique
	/// </summary>
	/// <param name="puzzles">the puzzles game objects.</param>
	private void SetPuzzlesToSave(GameObject[] puzzles){
		
		//instantiate the puzzles' dictionary data object
		_puzzlesData = new Dictionary<string, PuzzleData>();

		//for each puzzle game object
		foreach(GameObject puzzle in puzzles){

			//instantiate a new puzzle data
			PuzzleData aPuzzleData = new PuzzleData();
			
			//retrieve and store its position and rotation
			aPuzzleData.position = new Position(puzzle.transform.position.x, puzzle.transform.position.y, puzzle.transform.position.z);
			aPuzzleData.eulerRotation = new Position(puzzle.transform.rotation.eulerAngles.x, puzzle.transform.rotation.eulerAngles.y, puzzle.transform.rotation.eulerAngles.z);

			//retrieve and store its active (enable) state
			aPuzzleData.enabled = puzzle.activeSelf;
			//check and store if it has children game objects
			aPuzzleData.hasChildren = puzzle.transform.childCount > 0;
			//if it does not have
			if(!aPuzzleData.hasChildren){

				//retreive and store it's sprite renderer layer and order
				SpriteRenderer sprite = puzzle.GetComponent<SpriteRenderer>();				
				aPuzzleData.sortingLayerName = sprite.sortingLayerName;
				aPuzzleData.sortingLayerOrder = sprite.sortingOrder;
			}

			Lock[] locks = puzzle.GetComponentsInChildren<Lock>();

			if(locks != null && locks.Length > 0){

				aPuzzleData.locks = new bool[locks.Length];

				for(int i = 0; i < locks.Length; i++)
					aPuzzleData.locks[i] = locks[i].unlocked;

			}


			//add this puzzle's data to the dictionary according to its name
			_puzzlesData.Add(puzzle.name, aPuzzleData);
		}
	}
	
	/// <summary>
	/// Retrieves the puzzles saved data and set them on the level's puzzles.
	/// </summary>
	/// <param name="targetObjects">Target game objects.</param>
	public void FromSaveToPuzzles(GameObject[] targetPuzzles){

		//for each puzzle game object
		foreach(GameObject puzzle in targetPuzzles){

			//retrieve the puzzle data from the dictionary by its name
			PuzzleData aPuzzleData = _puzzlesData[puzzle.name];

			//retrieve and set the puzzle's position and rotation
			puzzle.transform.position = new Vector3(aPuzzleData.position.x, aPuzzleData.position.y, aPuzzleData.position.z);
			puzzle.transform.rotation = Quaternion.Euler(aPuzzleData.eulerRotation.x, aPuzzleData.eulerRotation.y, aPuzzleData.eulerRotation.z);

			//set the puzzle game obejct active state as the same as the stored value
			puzzle.SetActive(aPuzzleData.enabled);

			//if this puzzle does not have children
			if(!aPuzzleData.hasChildren){

				//retreive and the puzzle game obejct sprite renderer layer and 
				//order from the saved data
				SpriteRenderer sprite = puzzle.GetComponent<SpriteRenderer>();
				sprite.sortingLayerName = aPuzzleData.sortingLayerName;
				sprite.sortingOrder = aPuzzleData.sortingLayerOrder;

			}

			Lock[] locks = puzzle.GetComponentsInChildren<Lock>();
			
			if(locks != null && locks.Length > 0){
				
				int lenght = Mathf.Min(aPuzzleData.locks.Length, locks.Length);
				
				for(int i = 0; i < lenght; i++)
					locks[i].unlocked = aPuzzleData.locks[i];
				
			}
		}

	}

	/// <summary>
	/// Retrieves and stores the puzzles's data within the _puzzlesData dictonary object.
	/// Puzzles' names are used as keys to the dictionary, hence, they must be unique
	/// </summary>
	/// <param name="puzzles">the puzzles game objects.</param>
	private void SetEventsToSave(GameObject[] events){
		
		//instantiate the puzzles' dictionary data object
		_eventsData = new Dictionary<string, EventData>();

		
		//for each puzzle game object
		foreach(GameObject obj in events){
			
			//instantiate a new event data
			EventData data = new EventData();

			//check is it is activate and store it
			data.activeSelf = obj.activeSelf;

			//get all MonoBehaviour from this object
			MonoBehaviour[] behaviours = obj.GetComponentsInChildren<MonoBehaviour>();
			data.behaviours = new string[behaviours.Length];
			data.bhvEabled = new bool[behaviours.Length];

			//for each of them
			for (int i = 0; i < behaviours.Length; i++){

				MonoBehaviour behaviour = behaviours[i];

				//store their type names and if they're enabled or not
				data.behaviours[i] = behaviour.GetType().Name;
				data.bhvEabled[i] = behaviour.enabled;
			}

			//get all Collider2D from this object
			Collider2D[] colliders = obj.GetComponentsInChildren<Collider2D>();
			data.colsEnabled = new bool[colliders.Length];

			//Debug.Log(obj.name + " colisores: " + colliders.Length);

			//for each of them
			for (int i = 0; i < colliders.Length; i++){

				//store them
				data.colsEnabled[i] = colliders[i].enabled;
				//Debug.Log("colisor: " + i + data.colsEnabled[i]);
			}


			//get all Lock from this object
			Lock[] locks = obj.GetComponentsInChildren<Lock>();
			data.locksEnabled = new bool[locks.Length];

			//Debug.Log(obj.name + " locks: " + locks.Length);

			for (int i = 0; i < locks.Length; i++){

				data.locksEnabled[i] = locks[i].unlocked;

				//Debug.Log(locks[i].gameObject.name + " Lock: " + i + " ligado: " + 
				//          locks[i].enabled + " destrancado: " + locks[i].unlocked);
			}

			//add the event to the dictionary according to its name
			_eventsData.Add(obj.name, data);
		}
	}

	public void FromSaveToEvents(GameObject[] targetEvents){
		
		//for each puzzle game object
		foreach(GameObject obj in targetEvents){
			
			//retrieve the puzzle data from the dictionary by its name
			EventData data = _eventsData[obj.name];

			obj.SetActive(data.activeSelf);

			//get all MonoBehaviour from this object
			MonoBehaviour[] behaviours = obj.GetComponentsInChildren<MonoBehaviour>();


			//use a temp dictionary to diminish risk of wrong attribution
			for (int i = 0; i < data.behaviours.Length; i++){

				if(data.behaviours[i] == behaviours[i].GetType().Name)
					behaviours[i].enabled = data.bhvEabled[i];
			}

			//get all Collider2D from this object
			Collider2D[] colliders = obj.GetComponentsInChildren<Collider2D>();

			//use a temp dictionary to diminish risk of wrong attribution
			for (int i = 0; i < data.colsEnabled.Length; i++){
				colliders[i].enabled = data.colsEnabled[i];

				//Debug.Log(obj.name + " recuperando colisor: " + i + colliders[i].enabled);
			}

			//get all Lock from this object
			Lock[] locks = obj.GetComponentsInChildren<Lock>();
			
			for (int i = 0; i < data.locksEnabled.Length; i++){

				locks[i].unlocked = data.locksEnabled[i];

				//Debug.Log(obj.name + " recuperando lock: " + i + " ligado: " + 
				//          locks[i].enabled + " destrancado: " + locks[i].unlocked);
			}
		}
		
	}



	public void PrintData(){

		Debug.Log("Pontuacao armazenada: " + _playerData.score[0]);
	}

	public void ReadyToReaload(){

		GameControl.instance.currentHealthPoints = _playerData.healthPoints;
		GameControl.instance.maxHealthPoints = _playerData.maxHealthPoints;
	}

	private void SetUnlockablesToSave(UIUnlockable[] targetObjects){

		_unlockedBiosData = new Dictionary<string, bool>();

		foreach(UIUnlockable unlockable in targetObjects){
			_unlockedBiosData.Add(unlockable.gameObject.name, unlockable.unlocked);
		}
	}

	public void FromSaveToUnlockable(UIUnlockable[] targetObjects){

		foreach(UIUnlockable unlockable in targetObjects){

			unlockable.Unlock(_unlockedBiosData[unlockable.name]);
		}
	}

	/*public void ConcludeLevel(){

		Debug.Log("Concluido level!!");

		_isLevelFinished = true;
	}*/

	/*public void LevelIsOnProgress(){
		_isLevelFinished = false;
	}
	*/
	public bool IsLevelFinished(){
		return _isLevelFinished;
	}

	public bool ValidPlayerData(string username, string password){
		
		return _playerData.username == username && _playerData.password == password;
	}
	
	public string LevelName(){
		return _levelName.ToString();
	}

	public string LevelID(){
		return _levelID;
	}

	public int LevelScore(int levelNum){

		//this can never happen, structs are no nullable. 
		//Check values inside of it to check for initialization
		//if(_playerData == null)
		//	return 0;

		if(_playerData.score == null)
			return 0;

		try{

			return _playerData.score[levelNum];

		}catch(Exception ex){
			return 0;
		}
	}

	public int MaxColletible(){
		int total = 0;
		
		foreach(ColletibleData aColletible in _colletibleData.Values){
			
			//if the item is disabled, then it was collected
			if(aColletible.isSpecial)
				total++;
		}
		
		return total;	
	}

	public int TotalColletedItens(){

		int total = 0;

		foreach(ColletibleData aColletible in _colletibleData.Values){

			//if the item is disabled, then it was collected
			if(!aColletible.enabled && aColletible.isSpecial)
				total++;
		}

		return total;	
	}

	public static LevelData GetCurrent(string savename){

		_instance = SavedData.GAME_DATA.GetSavedData(savename);

		if(_instance != null){

			_instance.PrepareLevelToSave();

			Debug.Log("Criando nova instancia");

		}else{
			_instance = new LevelData(savename);
			_instance.PrepareLevelToSave();

			Debug.Log("Reinicializando instancia");
		}

		return _instance;
	}


}
