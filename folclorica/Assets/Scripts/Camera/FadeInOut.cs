using UnityEngine;
using System;

public class FadeInOut : MonoBehaviour
{    
	// ---------------------------------------- 
	// 	PUBLIC FIELDS
	// ----------------------------------------
	
	// Alpha start value
	public float startAlpha = 1;
	
	// Texture used for fading
	public Texture2D fadeTexture;
	
	// Default time a fade takes in seconds
	public float fadeDuration = 2;
	
	// Depth of the gui element
	public int guiDepth = -1000;
	
	// Fade into scene at start
	public bool fadeIntoScene = true;
	
	// ---------------------------------------- 
	// 	PRIVATE FIELDS
	// ----------------------------------------
	
	// Current alpha of the texture
	[HideInInspector]
	public float currentAlpha;// {get; private set;}
	
	// Current duration of the fade
	private float currentDuration;
	
	// Direction of the fade
	private int fadeDirection = -1;
	
	// Fade alpha to
	private float targetAlpha = 0;
	
	// Alpha difference
	private float alphaDifference = 0;
	
	// Style for background tiling
	private GUIStyle backgroundStyle = new GUIStyle();
	private Texture2D dummyTex;
	
	// Color object for alpha setting
	private Color alphaColor = new Color();

	public static Color SKY_COLOR;
	public static Color CAMERA_BACKGROUND;

	public static bool IS_FADED_OUT = false;

	// ---------------------------------------- 
	// 	FADE METHODS
	// ----------------------------------------
	
	public void FadeIn(float duration, float to){
		//Debug.Log("Fading in");

		//Check to see if currentAlpha is set to 1.  It will need to be 1 to fade properly
		if (currentAlpha != 1){

			Debug.Log("Reset alpha from " + currentAlpha + " to 1");
			currentAlpha = 1;	
		}
		// Set fade duration
		currentDuration = duration;
		// Set target alpha
		targetAlpha = to;
		// Difference
		alphaDifference = Mathf.Clamp01(currentAlpha - targetAlpha);
		// Set direction to Fade in
		fadeDirection = -1;
	}
	
	public void FadeIn(){
		FadeIn(fadeDuration, 0);
	}
	
	public void FadeIn(float duration){
		FadeIn(duration, 0);
	}
	
	public void FadeOut(float duration, float to){
		//Check to see if currentAlpha is set to 0.  It will need to be 0 to fade properly
		if (currentAlpha != 0){
			currentAlpha = 0;	
		}
		// Set fade duration
		currentDuration = duration;
		// Set target alpha
		targetAlpha = to;
		// Difference
		alphaDifference = Mathf.Clamp01(targetAlpha - currentAlpha);
		// Set direction to fade out
		fadeDirection = 1;

		//Debug.Log("Fading out");
	}
	
	public void FadeOut(){
		FadeOut(fadeDuration, 1);
	}    
	
	public void FadeOut(float duration)	{
		FadeOut(duration, 1);
	}
	
	// ---------------------------------------- 
	// 	STATIC FADING FOR MAIN CAMERA
	// ----------------------------------------
	
	public static void FadeInMain(float duration, float to) {
		GetInstance().FadeIn(duration, to);
	}
	
	public static void FadeInMain()	{
		GetInstance().FadeIn();
	}
	
	public static void FadeInMain(float duration)
	{
		GetInstance().FadeIn(duration);
	}
	
	public static void FadeOutMain(float duration, float to)
	{
		GetInstance().FadeOut(duration, to);
	}
	
	public static void FadeOutMain()
	{
		GetInstance().FadeOut();
	}
	
	public static void FadeOutMain(float duration)
	{
		GetInstance().FadeOut(duration);
	}
	
	// Get script fom Camera
	public static FadeInOut GetInstance()
	{
		// Get Script
		FadeInOut fader = (FadeInOut)Camera.main.GetComponent("FadeInOut");
		// Check if script exists
		if (fader == null) 
		{
			Debug.LogError("No FadeInOut attached to the main camera.");
		}    
		return fader;
	}
	
	// ---------------------------------------- 
	// 	SCENE FADEIN
	// ----------------------------------------
	
	public void Start()
	{
		//Debug.Log("Starting FadeInOut");

		dummyTex = new Texture2D(1,1);
		dummyTex.SetPixel(0,0,Color.clear);
		backgroundStyle.normal.background = fadeTexture;
		currentAlpha = startAlpha;

		try{

			if (fadeIntoScene && !Pauser.instance.forcedSuspension)
			{
				FadeIn();
			}
		}catch(UnityException ex){
			Debug.Log(ex.ToString() + " " + ex.StackTrace);
		}catch(Exception ex){
			Debug.Log(ex.ToString() + " " + ex.StackTrace);
		}
	}
	
	// ---------------------------------------- 
	// 	FADING METHOD
	// ----------------------------------------
	
	public void OnGUI (){   

		// Fade alpha if active
		if ((fadeDirection == -1 && currentAlpha > targetAlpha) ||
		    (fadeDirection == 1 && currentAlpha < targetAlpha))
		{
		/*	if(currentAlpha >= 0.95f)
				currentAlpha = 1;
			else{*/
				// Advance fade by fraction of full fade time
				currentAlpha += (fadeDirection * alphaDifference) * (Time.deltaTime / currentDuration);
				// Clamp to 0-1
				currentAlpha = Mathf.Clamp01(currentAlpha);
			//}
		}
		
		// Draw only if not transculent
		if (currentAlpha > 0){

			// Draw texture at depth
			alphaColor.a = currentAlpha;
			GUI.color = alphaColor;
			GUI.depth = guiDepth;
			GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), dummyTex, backgroundStyle);

		//	IS_FADED_OUT = true;
		}//else

		IS_FADED_OUT = Camera.main.backgroundColor == Color.black;
	}

	public void ClearFader(){

		Debug.Log("Clear Fader...");
		alphaColor.a = 0f;
		GUI.color = alphaColor;
		GUI.depth = guiDepth;
		GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), dummyTex, backgroundStyle);
	}
}