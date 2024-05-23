using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UICharChatSeries : UICharChat {

	public List<Transform> Conversations;
	public List<Lock> locks;

	public Lock chatDoneLock;

	Lock currentLock;
	Collider2D thisCollider;

	new protected void Start () {

		base.Start();

		thisCollider = GetComponent<Collider2D>();

		int i = 0;

		while(i < locks.Count){

			if(locks[i].unlocked){
				locks.RemoveAt(i);
				Conversations.RemoveAt(i);
			}else{
				//Debug.Log("Trancada: " + locks[i].gameObject + " " + locks[i].unlocked);
				i++;
			}
		}

		currentLock = locks[0];

	}

	new protected void OnTriggerStay2D(Collider2D col){

		if(currentLock != null )
			if(col == triggeredBy && !_chatting && currentLock.unlocked){

				_chatIndex = 0;
				chatButtonText.text = "Próximo";

				msgText = Conversations[0].GetComponentsInChildren<Text>();

				locks.RemoveAt(0);
				Conversations.RemoveAt(0);

				currentLock = locks.Count > 0? locks[0] : null;

				PerformChat(col);
			}

	}

	void Update(){

		if(thisCollider.enabled == false && locks.Count > 0)
			thisCollider.enabled = true;

		if(chatDoneLock != null && !_chatting)
			if(!chatDoneLock.unlocked && locks.Count <= 0)
				chatDoneLock.unlocked = true;

		if(currentLock != null && currentLock != locks[0]){

		}

	}

	public override void PerformAction(){

		Console.Write("....");
	}

}
