using UnityEngine;
using System.Collections;

public class FenceTouch : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter(Collider coll)
	{
		print ("Fence Enter");

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnCollisionEnter(Collision coll)
	{
		print ("Fence Detected");
	}
}
