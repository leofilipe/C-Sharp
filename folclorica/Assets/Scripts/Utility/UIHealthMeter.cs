using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIHealthMeter : MonoBehaviour {
	
	public GameObject HeartContainer;	//game object for the heart containers

	private List<Image> _heartContainers;

	// Update is called once per frame
	void Awake () {

		if(_heartContainers == null)
			_heartContainers = new List<Image>();
			
	}

	void Start(){

		HeartUpdate(GameControl.instance.currentHealthPoints);

	}

	public void StartHeartTank(int startingHP, int maxHP){

		int numHearts = maxHP/2;

		for (int i = 0; i < numHearts; i++)
			AddHeart();

	}

	public void HeartUpdate(int currentHitPoints){   //Dano tem que ser 1 ou -1
	
		for(int i = 0; i < _heartContainers.Count; i++){

			Image innerHeart = _heartContainers[i].gameObject.transform.GetChild(0).gameObject.GetComponentInChildren<Image>();

			if(i > currentHitPoints/2 - 1){
				innerHeart.fillAmount = 0;
			}else
				innerHeart.fillAmount = 1;
		}

		if(currentHitPoints <= 0){
			
			Debug.Log("Ja esta morto: " + currentHitPoints);
			return;
		}

		if(currentHitPoints%2 != 0){

			int index = Mathf.FloorToInt(currentHitPoints/2);

			Debug.Log(index + " " + _heartContainers.Count);
			Image innerHeart = _heartContainers[index].gameObject.transform.GetChild(0).gameObject.GetComponentInChildren<Image>();
			innerHeart.fillAmount = 0.5f;
		}

	}

	public void AddHeart (){

		if(_heartContainers == null)
			_heartContainers = new List<Image>();

		HeartContainer.SetActive(false);

		GameObject heart = Instantiate (HeartContainer);
		Image heartImage = heart.GetComponent<Image>();

		heart.transform.SetParent(gameObject.transform,false);

		RectTransform heartRec = HeartContainer.GetComponent<RectTransform>(); 
		Vector3 heartPos = heartRec.anchoredPosition;

		if(_heartContainers.Count > 0){

			heartPos.x += heartRec.rect.width * _heartContainers.Count;
		}

		heartImage.rectTransform.anchoredPosition = heartPos;

		_heartContainers.Add(heartImage);


		heart.SetActive(true);
	}
}

