using UnityEngine;
using System.Collections;



public class ColliderTest : MonoBehaviour{

	public bool WestfiledCollider;
	public GameObject Drone;

	void Awake (){
		Drone.SetActive (true);
	}
		
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player") 
		{
			Debug.Log ("i SMELL A DRONE");
			if (WestfiledCollider == true) 
			{
				Drone.SetActive (true);
			} 
			else if (WestfiledCollider == false)
			{
				Drone.SetActive (false);
			}

		}

	}

	void OnTriggerExit (Collider other)
	{
		if (other.tag == "Player")
		{
			if (WestfiledCollider == true)
			{
				Drone.SetActive (false);
			}
			else if (WestfiledCollider == false)
			{
				Drone.SetActive (true);
			}
		}
	}
}
