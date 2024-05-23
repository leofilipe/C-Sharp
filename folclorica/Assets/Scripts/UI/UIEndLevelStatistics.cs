using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UIEndLevelStatistics : MonoBehaviour {

	//retrieve the saved data
	public SavedData savedData;
	public LevelData finishedLevelData;

	public Text txtUsername;
	public Text txtHealthPoints;
	public Text txtColletibles;
	public Text txtScore;
	public Text txtElapsedTime;
	public Text txtBonus;
	public Text txtTotal;
	// Use this for initialization
	void Start () {
	
		savedData = SavedData.GetCurrentGameData();
		string key = GameControl.instance.LastFinishedLevelKey;

		finishedLevelData = savedData.GetSavedData(key);

		txtUsername.text = GameControl.instance.username;
		txtHealthPoints.text = GameControl.instance.currentHealthPoints / 2 + " de " + GameControl.instance.maxHealthPoints / 2;
		txtColletibles.text = finishedLevelData.TotalColletedItens() + " de " + finishedLevelData.MaxColletible();// + " de " + GameControl.instance.CurrentLevel.colletibles.Length;

		int score = finishedLevelData.LevelScore(int.Parse(finishedLevelData.LevelID()) - 1);
		int bonus = finishedLevelData.levelBonus;

		txtScore.text = score.ToString();

		txtElapsedTime.text = ToTime(finishedLevelData.levelTime);

		txtBonus.text = bonus.ToString();
		txtTotal.text = (score + bonus).ToString();


		//GameControl.instance.score[int.Parse(levelID) - 1];
		//txtScore.text = 

		//Debug.Log(LevelControl.instance.levelID);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	string ToTime(float sec){

		string time = "";

		TimeSpan ts = TimeSpan.FromSeconds(sec);

		//time += ts.TotalHours

		int d = ts.Days;
		int h = ts.Hours;
		int m = ts.Minutes;
		int s = ts.Seconds;

		if(d > 0){
			time += d < 10? "0" + d : d.ToString();
			time += "d";
		}

		time += h < 10? "0" + h : h.ToString();
		time += "h";

		time += m < 10? "0" + m : m.ToString();
		time += "m";

		time += s < 10? "0" + s : s.ToString();
		time += "s";


		return time;
	}
}
