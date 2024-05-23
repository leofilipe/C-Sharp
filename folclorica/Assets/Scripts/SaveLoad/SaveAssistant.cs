using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;

public class SaveAssistant : MonoBehaviour{

	private string _savename = null;

	private SavedData _savedData;
	private LevelData _levelData;

	private string _currentSaveID;

	private float duration = 0f;


	private static bool _justLoadedLevel = false;
	
	private static bool _internetLoad = false;

	protected bool canSave = true;

	public float waitToNextSave = 180f;

	public Collider2D triggeredBy;

	public bool isSaving {get; private set;}

	public LevelData LoadSaveFromInternet(){
		_internetLoad = true;

		//TODO internet load requires another means to aquire data, needs to be redone at the end
		_savedData = SavedData.GetCurrentGameData(); 
		_savename = GetSaveName();
		_levelData = _savedData.GetSavedData(_savename);

		return _levelData;
	}

	public LevelData LoadSaveFile(){

		_internetLoad = false;

		_savedData = SavedData.GetCurrentGameData();
		_savename = GetSaveName();

		_levelData = _savedData.GetSavedData(_savename);

		return _levelData;
	}

	/*public void RestartLevel(){
		LevelControl.instance.Restart();
	}*/

	public bool IsLevelSavedAsFinished(){

		if(_levelData != null){
			
			//se os dados fornecidos batem com os do save
			if(_levelData.ValidPlayerData(GameControl.instance.username, GameControl.instance.password)){
				return _levelData.IsLevelFinished();
			}
		}

		return false;
	}
	public void LoadLevel(){

		//se encontrou dados para a fase, carregueos.
		if(_levelData != null){
			
			//se os dados fornecidos batem com os do save
			if(_levelData.ValidPlayerData(GameControl.instance.username, GameControl.instance.password)){

				_levelData.FromSaveToLevel();
				_levelData.FromSaveToPlayer(LevelControl.instance.player);
				_levelData.FromSaveToMobs(LevelControl.instance.mobs);
				_levelData.FromSaveToColletibles(LevelControl.instance.colletibles);
				_levelData.FromSaveToPuzzles(LevelControl.instance.puzzles);
				_levelData.FromSaveToEvents(LevelControl.instance.events);
				_levelData.FromSaveToUnlockable(LevelControl.instance.UiUnlockables);
				
				LevelControl.instance.lastCheckpointIndex = _levelData.lastCheckpointIndex;
				LevelControl.instance.levelTime = _levelData.levelTime;
				

				
				_justLoadedLevel = !_internetLoad;

				Debug.Log("Succeded loading data for: " + _savename + " from checkpoint: " + _levelData.lastCheckpointIndex + "Just loaded: " + _justLoadedLevel);
				return;
				
			}
		}
	}

	public void LoadLevel(GameObject MsgPanel){
		if(_levelData != null){
			
			//se os dados fornecidos batem com os do save
			if(_levelData.ValidPlayerData(GameControl.instance.username, GameControl.instance.password)){

				if(!_levelData.IsLevelFinished()){
					LoadLevel();
					return;
				}

				if (!GameControl.instance.RestartLevel){

					WarningMessage(MsgPanel);

					//definir fader como transparente
					//definir background como negro
					//verificar as possibilidades com o oclusion mask da camera... (provavelmente o mais simples)
				}

			}
		}
	}

	public string LevelName(){
		return _levelData.LevelName();
	}

	public string LevelID(){
		return _levelData.LevelID();
	}

	protected virtual IEnumerator OnTriggerStay2D (Collider2D other) {
		
		if(_justLoadedLevel){
			
			_justLoadedLevel = false;
			
			canSave = false;
			duration = waitToNextSave;
		}
		
		//check if there is any life before saving
		if (canSave && other == triggeredBy && GameControl.instance.currentHealthPoints > 0) {
			
			if(other.gameObject.GetComponent<PlatformerCharacter2D>().grounded){
				
				isSaving = true;
				
				SaveGame();
				
				canSave = false;
				duration = waitToNextSave;

				if(!LevelControl.instance.isLevelFinished)
					yield return new WaitForSeconds(2f);
				
				Debug.Log("Saved...");
				
				isSaving = false;
			}
		}
	}

	private void SaveGame(){
		
		try{
			
			string[] split = gameObject.name.Split('_');
			LevelControl.instance.lastCheckpointIndex = int.Parse(split[1]);
			LevelControl.instance.lastCheckpoint = gameObject;
			
		}catch(Exception ex){
			Debug.Log(ex.StackTrace);
			LevelControl.instance.lastCheckpointIndex = 0;
			LevelControl.instance.lastCheckpoint = null;
		}
		//por ser um save, nao importa se ha dados salvos anteriormente ou nao. Os dados devem ser coletados
		//da instancia atual do jogo e atualizados no arquivo.
		_savedData = SavedData.GetCurrentGameData();
		_savename = GetSaveName();
		_levelData = LevelData.GetCurrent(_savename);

		if(LevelControl.instance.isLevelFinished){
			GameControl.instance.LastFinishedLevelKey = _savename;
		}

		//update verifica se ja ha um jogo salvo para esta versao. Caso nao haja, insere um novo. Caso haja, deleta e insere o novo.
		_savedData.UpdateData(_savename, LevelControl.instance.lastCheckpointIndex, _levelData);
		
	}

	private string GetSaveName(){

		string[] split = Application.loadedLevelName.Split(' ');

		string name = GameControl.instance.username + ".";
		name += (split[1] == "00"? "01" : split[1]) + ".";
		name+= SavedData.GAME_DATA.CurrentSaveIndex();

		return name;
	}

	private void WarningMessage(GameObject MsgPanel){

		FadeInOut.GetInstance().ClearFader();

		Camera.main.cullingMask = 1 << LayerMask.NameToLayer("UIMessage");

		MsgPanel.SetActive(true);
		Pauser.instance.ForceGameSuspension(true);
		Debug.Log("Nao reiniciado... --> " + GameControl.instance.RestartLevel);
	}
	
	private void Update(){

		if(LevelControl.instance == null)
			return;

		if(LevelControl.instance.playerDead)
			duration = 0f;

		if(duration > 0){
			duration -= Time.deltaTime;
		}else
			canSave = true;
	}
}