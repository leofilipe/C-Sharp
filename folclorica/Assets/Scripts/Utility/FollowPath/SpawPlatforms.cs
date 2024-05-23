using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawPlatforms : MonoBehaviour {

	public GameObject[] platformPool;

	public float platformsBaseDistanceY;

	public PathDefinition path;

	List<GameObject> platforms;

	// Use this for initialization
	void Start () {

		//start a variable to measure the last platform 
		//current distance to the last position of the path
		float destinyDistanceY;

		//instatiate a list to keep track of the created platforms
		platforms = new List<GameObject>();

		//for each platform of the pool of available platforms
		foreach(GameObject platform in platformPool){

			//set its position as the starting position of the path
			platform.transform.position = path.Points[0].position;

			//deactivate it
			platform.SetActive(false);

			//deactivate its path
			platform.GetComponent<QuickCyclePath>().enabled = false;
		}

		do{

			//choose a random platform from the pool
			int random = Mathf.RoundToInt(Random.Range(0, platformPool.Length));
			GameObject platform = Instantiate (platformPool[random]);

			//if there are platforms on the list
			if(platforms.Count > 0){

				//first, find the desired y position for this platform, this value will serve as reference
				//to find a reference position between the first platform and this one.

				//As such. use the previous plaform on the list as reference for 
				//the desired for this platform y
				Transform previousPlatform = platforms[platforms.Count - 1].transform;
				
				BoxCollider2D collider = platform.GetComponent<BoxCollider2D>();
				
				//calculate the distance from the two platforms and set it as the keep distance of the previous one
				float y = platformsBaseDistanceY + collider.offset.x;				
				previousPlatform.GetComponent<QuickCyclePath>().yKeepDistance = y;
				//calculate the desired y position. As we go from an up position to a down one, we decrease y.
				y = previousPlatform.position.y - y;			

				//second, find a position in the path vector located bellow or right at this desired y
				int index;
				//note that, as the last position is the starting one again,you can ignore it
				for(index = 1; index < path.Points.Length - 1; index++){
					//as soon as you find a platform, break the loop
					if(path.Points[index].position.y <= y)
						break;
				}

				//OBS.PLACE THIS ON A METHOD AS ALL OF IT NEEDS TO BE DONE AGAIN TO SET THE VALUES FROM THE LAST PLATFORM TO THE FIRST
				//calculating x
				//third, find the hypotenuse from the last point until the reference point
				Vector2 node1 = new Vector2(path.Points[index - 1].position.x, path.Points[index - 1].position.y);
				Vector2 node2 = new Vector2(path.Points[index].position.x, path.Points[index].position.y);

				float catY = node1.y - node2.y;
				float catX = node1.x - node2.x;

				float hyp = Mathf.Sqrt(catY * catY + catX * catX);

				//fourth, find the new hypotenuse proportionally to y
				//calculate the desired y cateto as the variation of the y value
				float deltaY = node1.y - y;

				//find the hypotenuse propotional to this value;
				float newHyp = hyp * deltaY / catY;

				//find the x value that matches this hypotenuse and y values
				float x = Mathf.Sqrt(newHyp * newHyp - deltaY * deltaY);

				//use the found value as difference from the starting one to the target x
				x = node2.x < node1.x? node1.x - x: node1.x + x;

				//set the platform position.
				platform.transform.position = new Vector3(x, y, platform.transform.position.z);

				//set the previous platform to keep a minimun distance to this platform
				previousPlatform.GetComponent<QuickCyclePath>().watchDistanceTo = platform.GetComponent<QuickCyclePath>();

			}

			//set the platform as active and add it to the list
			platform.SetActive(true);
			platforms.Add(platform);

			//check the difference in distance from this platform to the last one
			destinyDistanceY = platform.transform.position.y - path.Points[path.Points.Length - 2].transform.position.y;

		}
		//repeat until the y distance from this platform's position
		//to the last position is greater or equal to the last position
		while(destinyDistanceY >= platformsBaseDistanceY);

		//fifth, set the keep distance from the last platform to the first one
		GameObject firstPlatform = platforms[0];
		GameObject lastPlatform = platforms[platforms.Count - 1];
		
		//calculate the distance from the two platforms and set it as the keep distance of the previous one
		float yKeepDistance = platformsBaseDistanceY + lastPlatform.GetComponent<BoxCollider2D>().offset.x;				
		lastPlatform.GetComponent<BoxCollider2D>().GetComponent<QuickCyclePath>().yKeepDistance = yKeepDistance;

		//set the last platform to keep a minimun distance to this platform
		lastPlatform.GetComponent<QuickCyclePath>().watchDistanceTo = firstPlatform.GetComponent<QuickCyclePath>();

		foreach(GameObject platform in platforms){
			platform.GetComponent<QuickCyclePath>().enabled = true;
		}
	}

}
