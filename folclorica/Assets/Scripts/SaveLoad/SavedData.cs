using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System;

[System.Serializable]
public class SavedData{

	public static SavedData GAME_DATA = null;			//current data being saved

	private int _currentSaveIndex;						//index of this data relative to username and level

	private Dictionary <string, LevelData> _savedGames;	//"list" of saved games

	private SavedData(){

		_currentSaveIndex = -1;
		_savedGames = new Dictionary<string, LevelData> ();

	}

	/// <summary>
	/// Tries to load previous saved data into the game. If it cant, creates a new save file.
	/// </summary>
	private static void LoadSavedData () {

		Debug.Log("Carregando dados do savegame...");

		// If there is a SavedData file, loads it
		if (File.Exists(Application.persistentDataPath + "/savedGames.fd")) {

			Debug.Log("Dados existentes...");

			BinaryFormatter bf = new BinaryFormatter(); // Used to deserialize the SavedData file
			FileStream file = File.Open(Application.persistentDataPath + "/savedGames.fd", FileMode.Open);

			GAME_DATA = (SavedData)bf.Deserialize (file); // Sets the current SavedData to the saved SavedData

			Debug.Log("Found: " + file.ToString());
			file.Close ();
			
			// Else creates a new one and save it
		} else {
			Debug.Log("Dados nao existentes...");

			GAME_DATA = new SavedData();

			GAME_DATA._currentSaveIndex++;

			BinaryFormatter bf = new BinaryFormatter(); // Used to serialize the SavedData to the file
			FileStream file = File.Create (Application.persistentDataPath + "/savedGames.fd"); // FD stands for Folclorica Data
			bf.Serialize(file, GAME_DATA); // Serialize the current SavedData into the new file
			
			Debug.Log("Creating: " + file.ToString());
			file.Close();
		}
	}

	private static void UpdateSavedDataFile() {

		Debug.Log("Atualizando dados do savegame...");

		BinaryFormatter bf = new BinaryFormatter();
		FileStream dataFile = File.Create(Application.persistentDataPath + "/savedGames.fd"); // FD stands for Folclorica Data
		bf.Serialize(dataFile, SavedData.GetCurrentGameData());
		dataFile.Close();
	}

	public int CurrentSaveIndex(){

		return GAME_DATA._currentSaveIndex;
	}

	public static SavedData GetCurrentGameData () {
		// If current isn't initialized yet, loads it
		if (GAME_DATA == null) {
			LoadSavedData();
		}
		
		return GAME_DATA;
	}

	/// <summary>
	/// Gets a list of saved games.
	/// </summary>
	/// <returns>The games list.</returns>
	/*public List<LevelData> GetData () {
		if (_savedGames.Count <= 0)
			return new List<LevelData> ();
		else
			return _savedGames.Values.ToList();
	}*/

	public LevelData GetSavedData(string savename){

		try{

			if(GAME_DATA._savedGames.ContainsKey(savename)){
				
				LevelData data = GAME_DATA._savedGames[savename];
				
				return data;
			}

		}catch(Exception ex){

			Debug.Log("Formato de dados incompativel...");
		}

		return null;

		//return GAME_DATA._savedGames.ContainsKey(savename)? GAME_DATA._savedGames[savename] : null;
	}

	public void UpdateData(string savename, int lastCheckpoint, LevelData levelData) {

		if(GAME_DATA._savedGames.ContainsKey(savename))
			GAME_DATA._savedGames.Remove(savename);

		levelData.time = DateTime.Now;
		levelData.lastCheckpointIndex = lastCheckpoint;
		levelData.isFadedOut = FadeInOut.IS_FADED_OUT;

		GAME_DATA._savedGames.Add(savename, levelData);

		UpdateSavedDataFile();
	
	}

	public string[] SavedKeys(){

		return GAME_DATA._savedGames.Keys.ToArray();
	}

	public bool NoData(){
		return GAME_DATA._savedGames.Count == 0;
	}
}