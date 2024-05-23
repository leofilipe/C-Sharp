using UnityEngine;
using System.Collections;

/// <summary>
/// Stop moving platforms once the conditions are met
/// </summary>
public class StopMovingPlatforms : MonoBehaviour {

	public Lock[] locks;					//locks that keep the platforms moving. Once unlocked they mus be stopped.
											//This class MUST NOT perform operationS on its values othar than read them.

	public FollowPath[] movingPlatforms;	//platforms to stop

	private Collider2D _collider;			//collider for this game object. Used for displaying tips

	private bool _haveStoped = false;		//flags if the platforms have been stopped or if they keep moving

	void Start(){

		//retrieve the collider for this game object
		_collider = GetComponent<Collider2D>();

		//assure that it is a trigger
		_collider.isTrigger = true;

		//disable it
		_collider.enabled = false;
	}
	// Update is called once per frame
	void Update () {
	
		//if the platforms have not been stopped
		if(!_haveStoped){
		
			//check if the locks were removed
			bool unlock = CheckLocks();

			//if they were, stop the platforms
			if(unlock){
				foreach(FollowPath path in movingPlatforms)
					path.moveNow = false;

				//reactivate the collider
				_collider.enabled = true;

				//flag that the platforms have been stopped.
				_haveStoped = true;
			}
		}

	}

	/// <summary>
	/// Checks every lock of the Lock array. If all are flaged as unlock then the zipline can be used
	/// </summary>
	/// <returns><c>true</c>, if lock was checked, <c>false</c> otherwise.</returns>
	bool CheckLocks(){
		
		//start a flag variable with true
		bool unlock = true;
		
		//for each lock on the array, loop while the flag is set as true
		for(int i = 0; i < locks.Length && unlock; i++){
			//set the flag value as itsef and the value of unlock.
			//the first false ocurrence will turn it to false
			unlock = unlock && locks[i].unlocked;
			
		}
		
		return unlock;//locks.Length > 0? unlock : false;
	}
}
