using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
    public float speed = 5f;

	void Start () {
	
	}
	
	void Update () {
        Vector3 pos = transform.position;
        pos.x += Time.deltaTime * speed;
        if (pos.x > 10)
        {
            pos.x = -10;
        }
        transform.position = pos;
	}
}
