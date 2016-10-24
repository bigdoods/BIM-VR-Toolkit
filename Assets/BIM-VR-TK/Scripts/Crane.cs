using UnityEngine;
using System.Collections;

public class Management : MonoBehaviour {

	public bool clockOrNot = true;
	public int speed = 10; //speed of crane

	void Update(){
		rotator (speed);
	}
		
	void rotator (int speed){
			if (clockOrNot){
				transform.Rotate (Vector3.down, speed * Time.deltaTime);
			} else if (!clockOrNot){
					transform.Rotate (Vector3.up, speed * Time.deltaTime);
				}	
	}
}
