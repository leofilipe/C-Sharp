sing UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class Game {
	
	private static Game current = null;

	private int id;
	private int lifes;
	private int points;
	private string levelName;

	private Dictionary<string, Hashtable> saveables;

	private Game (int gameId, string firstLevelName, int lifes) {
		this.id = gameId;
		this.lifes = lifes;
		this.points = 0;
		this.levelName = firstLevelName;

		this.saveables = new Dictionary<string, Hashtable>();
	}

	/// <summary>
	/// Returns the current instance of <see cref="Game"/>.
	/// </summary>
	/// <returns>The current instance of <see cref="Game"/>.</returns>
	public static Game GetCurrent() {
		// If current isn't initialized, initializes as a new game with 3 lifes.
		if (current == null) {
			current = new Game (SavedData.GetCurrent().NewData(), Application.loadedLevelName, 3);
		}

		return current;
	}
	
	public static void SetCurrentGame(Game game) {
		current = game;
	}

	/// <summary>
	/// Creates a new <see cref="Game"/> object and it's file.
	/// </summary>
	/// <param name="firstLevelName">First level name.</param>
	/// <param name="lifeAmount">Life amount.</param>
	public static void NewGame(string firstLevelName, int lifeAmount) {
		current = new Game (SavedData.GetCurrent().NewData(), firstLevelName, lifeAmount);
		Save();
	}

	/// <summary>
	/// Saves the current <see cref="Game"/>.
	/// </summary>
	public static void Save() {
		// First update the SavedData to the current time and date
		SavedData.GetCurrent().UpdateData();
		
		// Then save the current game
		Debug.Log("Saving " + Game.GetCurrent().GetId() + ".fd");
		BinaryFormatter bf = new BinaryFormatter(); // To serialize the Game instance
		FileStream gameFile = File.Create(Application.persistentDataPath + "/" + Game.GetCurrent().GetId() + ".fd"); // Creates a files named with the id of the current Game (fd stands for folclorica data)
		bf.Serialize(gameFile, Game.GetCurrent()); // Serialize the current Game into the file
		gameFile.Close();
	}

	/// <summary>
	/// Load the specified id into current <see cref="Game"/>.
	/// </summary>
	/// <param name="id">Game id.</param>
	public static void Load (int id) {
		if (File.Exists (Application.persistentDataPath + "/" + id + ".fd")) {
			Debug.Log ("Loading " + id + ".fd");
			BinaryFormatter bf = new BinaryFormatter();
			FileStream gameFile = File.Open (Application.persistentDataPath + "/" + id + ".fd", FileMode.Open);
			current = (Game)bf.Deserialize(gameFile);
			gameFile.Close();
		} else 
			Debug.LogError("No such file: " + id + ".fd");
	}

	public int GetId() {
		return id;
	}

	public int GetLifes() {
		return lifes;
	}

	public void RemoveOneLife() {
		this.lifes--;
	}
	
	public void IncrementOneLife() {
		this.lifes++;
	}

	public int GetPoint() {
		return points;
	}

	public void AddPoints(int quantity) {
		this.points += Mathf.Abs(quantity);
	}

	public string GetLevelName() {
		return levelName;
	}

	public void LoadScene() {
		Application.LoadLevel(this.levelName);
	}

	/// <summary>
	/// Returns the list of saved components of the given object.
	/// </summary>
	/// <returns>The saved components.</returns>
	/// <param name="savename">The object label.</param>
	public Hashtable GetSavedValues(string savename) {
		if (saveables.ContainsKey(savename)) {
			return this.saveables[savename];
		} else {
			return new Hashtable(6, 0.8F);
		}
	}

	public void SetSavedValues(string savename, Hashtable values) {
		saveables[savename] = values;
	}

	/// <summary>
	/// Return true if the object was saved, otherwise false.
	/// </summary>
	/// <returns><c>true</c>, if objected was saved, <c>false</c> otherwise.</returns>
	/// <param name="savename">The object label.</param>
	public bool ObjectSaved(string savename) {
		if (this.saveables.ContainsKey(savename))
			return true;
		else
			return false;
	}
}