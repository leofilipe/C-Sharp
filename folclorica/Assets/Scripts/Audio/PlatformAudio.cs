using UnityEngine;
using System.Collections;

public class PlatformAudio : MonoBehaviour {

	public FollowPath targetPlatform;
	public AudioSource movementAudio;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(targetPlatform.enabled && targetPlatform.moveNow && !movementAudio.isPlaying){
			movementAudio.loop = true;
			movementAudio.Play();
		}else if(!targetPlatform.moveNow && movementAudio.isPlaying){
			movementAudio.Stop();
		}
	}
}
