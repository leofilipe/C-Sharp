using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIScore : MonoBehaviour {

	public Text label {get; private set;}

	// Use this for initialization
	void Start () {
	
		label = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		string score = LevelControl.instance.GetScore().ToString();

		string[] text = label.text.Split(' ');

		text[1] = text[1].Substring(0, text[1].Length - score.Length);

		label.text = text[0] + " " + text[1] + score;

	}
}
