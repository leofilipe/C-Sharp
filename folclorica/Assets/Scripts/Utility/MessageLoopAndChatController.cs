using UnityEngine;
using System.Collections;

public class MessageLoopAndChatController : MonoBehaviour {

	public Lock previousLock;

	public MonoBehaviour[] disableScripts;				//array of scripts that must be disabled once the chat starts.

	Lock _nextLock;

	UICharChat _uiChat;

	bool hasDisabledScripts = false;


	// Use this for initialization
	void Start () {
	
		_nextLock = GetComponent<Lock>();
		_uiChat = GetComponent<UICharChat>();

	}
	
	// Update is called once per frame
	void Update () {
	
		if(previousLock != null && previousLock.unlocked && !hasDisabledScripts){

			foreach(MonoBehaviour script in disableScripts){

				Collider2D col = script.gameObject.GetComponent<Collider2D>();

				if(col != null)
					col.enabled = false;
				else
					script.enabled = false;//StartCoroutine(DisableAfterInterval(script, 0.5f));
			}

			hasDisabledScripts = true;

			if(_uiChat != null)
				_uiChat.gameObject.GetComponent<Collider2D>().enabled = true;
		}

		if(_uiChat != null && _nextLock != null)
			if(_uiChat.isDone && !_nextLock.unlocked)
				_nextLock.unlocked = true;
	}

	/*IEnumerator DisableAfterInterval(MonoBehaviour script, float interval){

		yield return new WaitForSeconds(interval);

		script.enabled = false;
	}*/
}
