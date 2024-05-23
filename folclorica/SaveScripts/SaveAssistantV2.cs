using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveAssistantV2 {

	//-- LEVEL SAVED DATA
	private SavedData savedData;

	public SaveAssistantV2(){

		savedData = SavedData.GetCurrent();
	}
	/*private string savename;
	private GameObject target;

	/// <summary>
	/// Initializes a new instance of the <see cref="SaveAssistant"/> class.
	/// </summary>
	/// <param name="objectToSave">Object to save.</param>
	/// <param name="assistantLabel">Label to identify the current <see cref="SaveAssistant"/>.</param>
	public SaveAssistant(GameObject objectToSave, string assistantLabel) {
		savename = GetSavename(objectToSave, assistantLabel);
		target = objectToSave;
	}

	/// <summary>
	/// Return an unique savename based on label and position.
	/// </summary>
	/// <returns>The savename.</returns>
	/// <param name="objectToSave">Object use as reference.</param>
	/// <param name="label">Label.</param>
	private static string GetSavename (GameObject objectToSave, string label) {
		return objectToSave.name + "." + label
					 + "." + Application.loadedLevelName
					 + "." + objectToSave.transform.position.x
					 + "." + objectToSave.transform.position.y
					 + "." + objectToSave.transform.position.z;
	}
	
	/// <summary>
	/// Loads the value if it's saved, otherwise returns the default value.
	/// </summary>
	/// <returns>The saved value or default value.</returns>
	/// <param name="variableLabel">The label used to identify the varible.</param>
	/// <param name="defaultValue">Default value if it's not saved yet.</param>
	/// <typeparam name="T">The type of the variable.</typeparam>
	public T LoadValue<T> (string variableLabel, T defaultValue) {
		// If the object hasn't been saved, then return the default value
		if (Game.GetCurrent().ObjectSaved(savename) == false)
			return defaultValue;

		Hashtable values = Game.GetCurrent().GetSavedValues(savename);
		if (values.Contains(variableLabel) == false)
			return defaultValue;

		return (T)values[variableLabel];
	}

	/// <summary>
	/// Sets the value of the given variable to be saved the next time the game is saved.
	/// </summary>
	/// <param name="variableLabel">A label to identify the variable being saved.</param>
	/// <param name="value">The value to be saved.</param>
	/// <typeparam name="T">The type of the variable.</typeparam>
	public void SetToSave<T> (string variableLabel, T value) {
		Hashtable values = Game.GetCurrent().GetSavedValues(savename);
		values[variableLabel] = value;
		Game.GetCurrent().SetSavedValues(savename, values);
	}

	/// <summary>
	/// Loads the position of the object, if it's not saved yet keeps it at the same position.
	/// </summary>
	public void LoadPosition() {
		if (Game.GetCurrent().ObjectSaved(savename) == false)
			return;

		float x, y, z;
		Hashtable values = Game.GetCurrent().GetSavedValues (savename);
		if (values.Contains("transform.position.x")) {
			x = (float)values["transform.position.x"];
		} else {
			x = target.transform.position.x;
		}
		if (values.Contains("transform.position.y")) {
			y = (float)values["transform.position.y"];
		} else {
			y = target.transform.position.y;
		}
		if (values.Contains("transform.position.z")) {
			z = (float)values["transform.position.z"];
		} else {
			z = target.transform.position.z;
		}
		target.transform.position = new Vector3(x,y,z);
	}

	/// <summary>
	/// Sets the position to be saved next time the game is saved.
	/// </summary>
	public void SetToSavePosition() {
		Hashtable values = Game.GetCurrent().GetSavedValues(savename);
		values["transform.position.x"] = target.transform.position.x;
		values["transform.position.y"] = target.transform.position.y;
		values["transform.position.z"] = target.transform.position.z;
		Game.GetCurrent().SetSavedValues(savename, values);
	}*/
}
