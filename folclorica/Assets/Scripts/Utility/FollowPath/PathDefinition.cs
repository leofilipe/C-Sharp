using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class PathDefinition : MonoBehaviour {

	public Transform[] Points;
	public bool reversePath = true;

	public IEnumerator<Transform> GetPathEnumerator(){

		if(Points == null || Points.Length < 1)
			yield break;

		int direction = 1;
		int index = 0;
		
		while(true){
			
			yield return Points[index];
			
			if(Points.Length == 1)
				continue;
			
			if(index <= 0){
				direction = 1;
			}else if (index >= Points.Length - 1){

				if(reversePath)
					direction = -1;
				else{
					direction = 1;
					index = 0;
				}
			}
			
			index = index + direction;

		}
	}

	public void OnDrawGizmos(){

		if(Points == null || Points.Length < 2)
			return;

		var points = Points.Where(t => t != null).ToList();

		if(points.Count < 2)
			return;
		
		for(int i = 1; i < points.Count; i++){
			Gizmos.DrawLine(points[i - 1].position, points[i].position);
		}

	}
}
