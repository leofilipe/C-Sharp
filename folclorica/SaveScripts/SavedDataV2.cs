using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SavedDataV2 {

	private static SavedData current = null;

	// Struct for the data of the saved games
	/*[System.Serializable]
	public struct gameData {
		//public string username;
		//public string password;
		public int id;
		public string levelName;
		public DateTime time;
	};

	[System.Serializable]
	public struct PlayerData{

	}*/

	public struct GameData{

	}

	private int currentId;	// The ID for the current saved game
							//LEONARDO - use in conjunction to the username to allow for multiple saves of same user
	private Dictionary<int, LevelData> savedGames;	// The list of saved games

	private SavedData() {
		currentId = -1;
		savedGames = new Dictionary<int, LevelData> ();
	}

	/// <summary>
	/// Returns the current instance of <see cref="SavedData"/>.
	/// </summary>
	/// <returns>The current instance of <see cref="SavedData"/>.</returns>
	public static SavedData GetCurrent () {
		// If current isn't initialized yet, loads it
		if (current == null) {
			LoadSavedData();
		}

		return current;
	}

	/// <summary>
	/// Loads the file containing information about all saved games, if it doesn't exist creates a new file.
	/// </summary>
	private static void LoadSavedData () {

		// If there is a SavedData file, loads it
		if (File.Exists(Application.persistentDataPath + "/savedGames.fd")) {
			BinaryFormatter bf = new BinaryFormatter(); // Used to deserialize the SavedData file
			FileStream file = File.Open(Application.persistentDataPath + "/savedGames.fd", FileMode.Open);
			current = (SavedData)bf.Deserialize (file); // Sets the current SavedData to the saved SavedData

			Debug.Log("Found: " + file.ToString());
			file.Close ();
			
		// Else creates a new one and save it
		} else {
			current = new SavedData(); // Creates a new SavedData to be the current
			BinaryFormatter bf = new BinaryFormatter(); // Used to serialize the SavedData to the file
			FileStream file = File.Create (Application.persistentDataPath + "/savedGames.fd"); // FD stands for Folclorica Data
			bf.Serialize(file, current); // Serialize the current SavedData into the new file

			Debug.Log("Creating: " + file.ToString());
			file.Close();
		}
	}

	/// <summary>
	/// Create an id for the new game to be saved.
	/// </summary>
	/// <returns>The id.</returns>
	public int NewData() {
		return ++currentId;
	}

	/// <summary>
	/// Update the time of the saved game and it's level name.
	/// </summary>
	public void UpdateData() {
		/*gameData currentData;
		int id;
		id = LevelData.GetCurrent().GetId();

		currentData.id = id;
		currentData.levelName = LevelData.GetCurrent().GetLevelName();
		currentData.time = DateTime.Now;

		if (savedGames.ContainsKey(id))
			savedGames.Remove(id);
		savedGames.Add(id, currentData);

		UpdateSavedDataFile();*/
	}

	private static void UpdateSavedDataFile() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream dataFile = File.Create(Application.persistentDataPath + "/savedGames.fd"); // FD stands for Folclorica Data
		bf.Serialize(dataFile, SavedData.GetCurrent());
		dataFile.Close();
	}

	/// <summary>
	/// Deletes the saved game.
	/// </summary>
	/// <param name="id">Identifier.</param>
	public void DeleteSavedGame(int id) {
		File.Delete(Application.persistentDataPath + "/" + id + ".fd");
		if (savedGames.ContainsKey (id)) {
			savedGames.Remove (id);
		}
		UpdateSavedDataFile();
	}
	
	/**
	 * DEBUG ONLY: WON'T BE IN THE FINAL VERSION
	 */
	public void CleanSavedData () {
		/*List<gameData> data = GetData();
		foreach (gameData game in data) {
			File.Delete(Application.persistentDataPath + "/" + game.id + ".fd");
			Debug.Log("File removed: " + Application.persistentDataPath + "/" + game.id + ".fd");
		}
		File.Delete(Application.persistentDataPath + "/savedGames.fd");
		Debug.Log("File removed: " + Application.persistentDataPath + "/savedGames.fd");*/
	}

	/// <summary>
	/// Gets a list of saved games.
	/// </summary>
	/// <returns>The games list.</returns>
	public List<gameData> GetData () {
		if (savedGames.Count <= 0)
			return new List<gameData> ();
		else
			return savedGames.Values.ToList ();
	}

}