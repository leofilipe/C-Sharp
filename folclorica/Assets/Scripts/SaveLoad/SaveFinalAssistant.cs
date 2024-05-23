using UnityEngine;
using System.Collections;

/// <summary>
/// Save final assistant. 
/// Differs from SaveAssistant for setting levelFinished as true once the targeted collider has entered the trigger
/// </summary>
public class SaveFinalAssistant : SaveAssistant {

	protected override IEnumerator OnTriggerStay2D (Collider2D other) {

		//uses the same restrictions of overrided method just to avoid multiple calls
		if (canSave && other == triggeredBy && GameControl.instance.currentHealthPoints > 0) {

			//flag the level as finished
			LevelControl.instance.isLevelFinished = true;

			//executes the overrided method and wait for its conclusion
			yield return StartCoroutine(base.OnTriggerStay2D(other));
		}
	}
}
