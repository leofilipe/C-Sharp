using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChaseNPC : FollowPath {

	public PlatformerCharacter2D character;
	public Platformer2DUserControl characterControl;
	public GameObject optionalMsgPanel;
	public Text optionalMsgPanelText;
	public Font font;
	public string[] textLines;
	public float lineInterval;

	//public List<Collider2D> deactivateColliders;



	private Vector3 destiny;
	private bool _onTarget = false;
	private Rigidbody2D _characterRB;
	private float _duration;
	private int lineIndex = 0;

	// Use this for initialization
	void Start () {
	
		base.Start();

		/*foreach(Collider2D col in deactivateColliders)
			col.enabled = false;*/

		//Debug.Log("Encantada: " + transform.position);

		//foreach( Transform p in Path.Points)
		//	Debug.Log(p.position);

		destiny = new Vector3();
		destiny.x = Path.Points[Path.Points.Length - 1].position.x;
		destiny.z = character.transform.position.z;

		character.enabled = false;
		characterControl.enabled = false;
		_characterRB = character.gameObject.GetComponent<Rigidbody2D>();
		_characterRB.velocity = Vector2.zero;

		optionalMsgPanelText.text = textLines[lineIndex];
		optionalMsgPanelText.font = font;
		optionalMsgPanelText.resizeTextForBestFit = true;

		optionalMsgPanel.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
	
		base.Update();

		//just to be sure, disable again.
		character.enabled = false;
		characterControl.enabled = false;
		//Debug.Log(transform.position);

		bool onDestiny = OnDestiny();

		if(onDestiny)
			moveNow = false;
		else
			_characterRB.velocity = Vector2.zero;

		if(onDestiny && !_onTarget){

			_duration += Time.deltaTime;

			if(_duration >=  lineInterval){

				_duration = 0;

				if(lineIndex < textLines.Length - 1){
					lineIndex++;
					optionalMsgPanelText.text = textLines[lineIndex];
				}
			}

			character.Move(1, false, false);
			destiny.y = character.transform.position.y;

			//the current distance between the object and its intended location
			var distanceSquared = (character.transform.position - destiny).sqrMagnitude;
			
			//the onTarget parameter indicates whether or not this distance is within the acceptable limit
			_onTarget = distanceSquared < MaxDistanceToGoal * MaxDistanceToGoal;
		}
	}

	/*IEnumerator TimedMsg(){

		if(optionalMsgPanel != null){

		}
	}*/
}
