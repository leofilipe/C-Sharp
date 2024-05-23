using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Controls most of the parameters within a game level scope. 
/// Performs behaviors that must affect the whole game level
/// </summary>
public class LevelControl : MonoBehaviour {

	public GameObject HUD;				//game object holder for the game's HUD 
	public GameObject GameOverScreen;	
	//GAME EFFECTS --------------------------
	public GameObject[] effects;			//game object holder for the effects of the game level.
										//particle systems, no-terrain background/foreground images, etc.

	private bool _activeEffects = true; 	//flag to indicate weather or not the above effects are active


	//SAVE OBJECTS --------------------------
	//game objeccts that  will be passed down to the
	//SaveAssistant class

	public GameObject player;					//the player game object
	public PlatformerCharacter2D playerControls;
	public Platformer2DUserControl playerAuxControls;

	public UIHealthMeter healthMeter;

	public GameObject[] mobs;					//the savable monsters

	public GameObject[] colletibles;			//the colletibles

	public List<GameObject> specialColletibles;	//these are the colletibles that are 
												//accounted for on the HUD as the total 
												//itens for collecting on a level

	public GameObject[] puzzles;				//assorted puzzles

	public GameObject[] events;					//assorted events

	public UIUnlockable[] UiUnlockables;			//assorted array of game object (usually buttons) 
												//originally locked on the user's game interface

	public AudioSource[] bgAudios;				//background audios

	public string levelName;					//level name in PT-BR

	public string levelID;						//level number within the game

	public int defaultHealthPoints = 6;

	public int defaultMaxHealthPoints = 6;

	public int extraLifeMinScore = 1000;

	public AudioSource extraLifeAudio;

	public int levelMaxScoreBonus = 0;

	public GameObject msgPanel;

	[HideInInspector]
	public string playerDefaultSortingLayer;	//player's sorting layer 
	[HideInInspector]
	public int playerDefaultSortingOrder;		//and its priority order
	[HideInInspector]
	public int lastCheckpointIndex;				//index of the last activated checkpoint

	[HideInInspector]
	public GameObject lastCheckpoint;			//last activated checkpoint

	[HideInInspector]
	public bool playerDead = false;				//whether or not the player is dead
	[HideInInspector]
	public float levelTime = 0f;

	[HideInInspector]
	public bool isLevelFinished = false;

	private bool levelLoaded = false;			//failsafe to instantiate the level 
												//controllers a single time during 
												//update if everything else fails.

	private SpriteRenderer _playerRenderer;

	private static LevelControl _instance;

	private Animator _anim;

	public Color PlayerStartingRendererColor {get; private set;}

	public static LevelControl instance {
		get{
			if(_instance == null){

				_instance = GameObject.FindObjectOfType<LevelControl>();
				DontDestroyOnLoad(_instance);
			}

			return _instance;
		}
	}//the one and only accessible variable representing this level

	/// <summary>
	/// Awake this instance. Its is called before the Start method. 
	/// Hence, the first attempt to load a prevous game must be done here.
	/// </summary>
	void Awake(){

		//get the player's renderer
		_playerRenderer = player.GetComponentInChildren<SpriteRenderer>();

		PlayerStartingRendererColor = _playerRenderer.color;

		StartLevel();
	}

	void StartLevel(){

		//disable the pause menu for now
		if(Pauser.instance != null)
			Pauser.instance.enabled = true;

		GameOverScreen.SetActive(false);

		specialColletibles = new List<GameObject>();
		
		foreach(GameObject colletible in colletibles){
			if(colletible.GetComponent<Colletible>().isSpecial)
				specialColletibles.Add(colletible);
		}
		_anim = player.GetComponent<Animator>();
		 
		_anim.SetBool("Dead", false);
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Monsters"), false);
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Climber"), false);

		//activates the hud if it is deactivated (there is a know "issue" that if
		//it is deactivated it starts the game with GameControl's default values)
		HUD.SetActive(true);
		
		//set the default sorting layer and its priority. This is done before loading the
		//game data to ensure that the values from unity editor are always the default.
		playerDefaultSortingLayer = _playerRenderer.sortingLayerName;
		playerDefaultSortingOrder = _playerRenderer.sortingOrder;



		//make sure the game effects and HUD are active
		foreach(GameObject effect in effects)
			effect.SetActive(true);

		//if a level bonus was not set, them make it the default
		if(levelMaxScoreBonus == 0)
			levelMaxScoreBonus = GameControl.DEFAULT_BONUS;

		//stores default camera settings
		FadeInOut.SKY_COLOR = RenderSettings.ambientSkyColor;
		FadeInOut.CAMERA_BACKGROUND = Camera.main.backgroundColor;

		//DontDestroyOnLoad(this);
		//if there is no instance
		if(_instance == null)
			_instance = this;	//set it as this instance
		//if there is
		else if (_instance != null && _instance != this){

			//_restart = _instance._restart;	//flag the restart as the one of the previous instance

			int defaultHealthPoints = _instance.defaultHealthPoints;
			int defaultMaxHealthPoints = _instance.defaultMaxHealthPoints;

			Destroy(_instance); //destroy the previous instance

			_instance = this;	//set it as this instance
			_instance.defaultHealthPoints = defaultHealthPoints;
			_instance.defaultMaxHealthPoints = defaultMaxHealthPoints;
		}

		//GameControl.instance.CurrentLevel = _instance;
		//try to load the level
		LoadLevel(msgPanel);
	}

	// Update is called once per frame
	void Update () {

		//if for any reason it reaches the update and the level is not loaded yet
		if(!levelLoaded)
			LoadLevel(msgPanel); //try to load the level

		//WARNING: simple deactivation of the effects object causes 
		//a noticeable lag in the game. Thus the conditional bellow
		//was needed. As of now, deactivation of Fluvio objects 
		//in the game is the cause for this.
		//if it is fade out and the background particle effects are
		//active...
		if(FadeInOut.IS_FADED_OUT && _activeEffects){

			//...then turn the background effects off
			EnableBgEffects(false);

		}
		//if it is not fade out and the background particle effects 
		//are deactive...
		else if(!FadeInOut.IS_FADED_OUT && !_activeEffects){

			//...then turn the background effects on
			EnableBgEffects(true);

		}


		/*foreach(LayerMask i in ignoreCollisionOnDeath.){

		}*/
		if(GameControl.instance.currentHealthPoints <= 0 && !GameOverScreen.activeSelf){

			//disable player controls
			playerControls.enabled = false;
			playerAuxControls.enabled = false;
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Monsters"), true);
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Climber"), true);

			//se grounded
			//tornar kinematico
			//desligar colisores.

			Debug.Log("Player layer: " + player.layer);

			//start death animation


			//show game over screen
			GameOverScreen.SetActive(true);
		}

		if(GameOverScreen.activeSelf){

			if(/*playerControls.grounded && */!_anim.GetBool("Dead")){

				_playerRenderer.color = GameControl.instance.PlayerHurtColor;
				_anim.SetBool("Dead", true);
			}else if (_anim.GetBool("Dead") && _playerRenderer.color != GameControl.instance.PlayerHurtColor){
				_playerRenderer.color = GameControl.instance.PlayerHurtColor;
			}
		}

		if(GameControl.instance.LastFinishedLevelKey == null)
			levelTime += Time.deltaTime;

		int score = GetScore();

		//adds an extra life if min score has been reached
		if(score >= extraLifeMinScore && GameControl.instance.maxHealthPoints <= defaultMaxHealthPoints){
			AddHeart();
		}

		//adds a second (and last) extra life if 2x min score has been reached
		if(score >= 2 * extraLifeMinScore && GameControl.instance.maxHealthPoints <= defaultMaxHealthPoints + 2){
			AddHeart();
		}
	}

	/// <summary>
	/// Tries to load the level.
	/// </summary>
	private void LoadLevel(GameObject msgPanel){

		try{

			//if level was not restarted
			if(!GameControl.instance.RestartLevel){
				//try to retrieve the save assistant from the game core
				SaveAssistant savedGame = GameControl.instance.saveAssistant;	

				//try to load the level
				//if(!savedGame.IsLevelSavedAsFinished())

				if(msgPanel == null)
					savedGame.LoadLevel();
				else
					savedGame.LoadLevel(msgPanel);
				//else{

				//}
			}
			//if it was
			else{
				ResetScore();
				levelTime = 0f;
			}

			GameControl.instance.RestartLevel = false;
			GameControl.instance.LastFinishedLevelKey = null;

			//flag the level as loaded
			levelLoaded = true;
		}
		//if anything wrogs happens
		catch(Exception ex){
			//show it on debug log (console)
			Debug.Log(ex.ToString() + " " + ex.StackTrace);
		}
	}

	/// <summary>
	/// Enables the background effects.
	/// </summary>
	/// <param name="isActive">If set to <c>true</c> effects are set to activated.</param>
	private void EnableBgEffects(bool isActive){

		//if there are no effects, return
		if(effects == null || effects.Length <= 0)
			return;

		//retrieve the particle effects and background 'effects' sprites
		//ParticleSystem[] particles = effects.GetComponentsInChildren<ParticleSystem>();
		//SpriteRenderer[] bgSprites = effects.GetComponentsInChildren<SpriteRenderer>();

		//set the emission of the particles to acitvated <if isActive is true>
		//or to deactivated <if isActive is false>
		//foreach(ParticleSystem p in particles){
		//	p.enableEmission = isActive;
		//}

		//set the background 'effects' sprites to visible <if isActive is true>
		//or to invisible <if isActive is false>
		//foreach(SpriteRenderer bgSprite in bgSprites){
		//	bgSprite.enabled = isActive;
		//}

		//retrieve the particle effects and background 'effects' sprites and set them
		//according to isActive
		foreach(GameObject effect in effects){

			try{

				//retrieve the children of each effect
				GameObject[] children = effect.GetComponentsInChildren<GameObject>();
				
				//for each child
				foreach (GameObject child in children){

					Debug.Log("(Des)Ligando Efeito de: " + child.name);
					ParticleSystem particle = child.GetComponent<ParticleSystem>();
					SpriteRenderer sprite = child.GetComponent<SpriteRenderer>();
					
					//if there is a ParticleSystem
					if(particle != null){
						
						//set the emission of the particles to acitvated <if isActive is true>
						//or to deactivated <if isActive is false>
						particle.enableEmission = isActive;
					}
					//if there is a SpriteRenderer
					if (sprite != null){
						
						//set the background 'effects' sprites to visible <if isActive is true>
						//or to invisible <if isActive is false>
						sprite.enabled = isActive;
					}
				}
			}catch(Exception ex){
				Debug.Log(effect.gameObject.name + " " + ex.StackTrace);
			}
		}

		//update the value of the activeEffects flag
		_activeEffects = isActive;
	}

	/// <summary>
	/// Adds an extra heart (+2 life).
	/// </summary>
	private void AddHeart(){

		extraLifeAudio.PlayOneShot(extraLifeAudio.clip);

		healthMeter.AddHeart();

		GameControl.instance.maxHealthPoints+=2;
		GameControl.instance.currentHealthPoints+=2;

		healthMeter.HeartUpdate(GameControl.instance.currentHealthPoints);
	}

	/// <summary>
	/// Adds score to the current game level.
	/// </summary>
	/// <param name="score">the amount of score to be added.</param>
	public void AddScore(int score){
		GameControl.instance.score[int.Parse(levelID) - 1] += score;
	}

	/// <summary>
	/// Gets the score of the current game level.
	/// </summary>
	/// <returns>The score.</returns>
	public int GetScore(){
		return GameControl.instance.score[int.Parse(levelID) - 1];
	}

	public void ResetScore(){
		GameControl.instance.score[int.Parse(levelID) - 1] = 0;
	}

	/*public void ConcludeLevel(){
		levelIsOver = true;
	}
	
	public bool IsLevelFinished(){
		return levelIsOver;
	}*/
	/*public void Restart(){
		_restart = true;
	}*/
}
