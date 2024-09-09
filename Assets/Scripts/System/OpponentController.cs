//OpponentControl.cs handles AI input
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
using System;


public class OpponentController : MonoBehaviour
{
	public float speed = 8f;                          // The nav mesh agent's speed when patrolling.
	public float chaseSpeed =  2f;                           // The nav mesh agent's speed when chasing.
	public float chaseWaitTime = 5f;                        // The amount of time to wait when the last sighting is reached.
	public float patrolWaitTime = 1f;                       // The amount of time to wait when the patrol way point is reached.
	public Transform[] waypoints;                     // An array of transforms for the patrol route.


	//private EnemySight enemySight;                          // Reference to the EnemySight script.
	private NavMeshAgent nav;                               // Reference to the nav mesh agent.
	private Transform player;                               // Reference to the player's transform.
	//private PlayerHealth playerHealth;                      // Reference to the PlayerHealth script.
	//private LastPlayerSighting lastPlayerSighting;          // Reference to the last global sighting of the player.
	private float chaseTimer;                               // A timer for the chaseWaitTime.
	private float patrolTimer;                              // A timer for the patrolWaitTime.
	private int currentWP = 1;                              // A counter for the way point array.
	private Animator anim;

	int rotSpeed = 1;

	public int programNumber;
	private Boolean speedHandled = false;

	public Texture2D m_MainTexture;
	public Texture2D m_Normal;

	GameObject tracker;

	float lookAhead = 10.0f;
 

	void Awake ()
	{

	
		// Setting up the references.
		//enemySight = GetComponent<EnemySight>();
		nav = GetComponent<NavMeshAgent>();
		//programNumber = PlayerPrefs.GetInt("FieldRunners", 0);

		// if(programNumber < 1){
		// 	return;
		// }
		String playerString = $"Opponent_{programNumber}";
		//player = GameObject.FindGameObjectWithTag(playerString).transform;
		player = transform;
		PlayerPrefs.SetInt("FieldRunners", programNumber - 1);
		//playerHealth = player.GetComponent<PlayerHealth>();
		//lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();




	}

	void OnCollisionEnter(){
		speed = 0;
	}

void Start(){		
		
	Transform horseLOD =	 player.Find("Horse_Mobile/horse/horse_body");
	Renderer renderer = horseLOD.GetComponent<Renderer>();

	print($"Current program number is {this.programNumber}");

   if(0 < programNumber && programNumber <= 6 ){

	string filename = $"Assets/HorseJockey/Textures/Horse_body_0{programNumber}.tga";
	var rawData = System.IO.File.ReadAllBytes(filename);
	//Texture2D tex = new Texture2D(2048, 2048); // Create an empty Texture; size doesn't matter (she said)
	Texture2D tex = Resources.Load<Texture2D>($"horse_body_0{programNumber}");
	//tex.LoadImage(rawData);

	//  string bumpFileName = $"Assets/HorseJockey/Textures/Horse_body_01.tga";
	// var rawBump = System.IO.File.ReadAllBytes(bumpFileName);
	// Texture2D bumpTex = new Texture2D(2, 2); // Create an empty Texture; size doesn't matter (she said)
	// tex.LoadImage(rawData);

	       // Create a new MaterialPropertyBlock
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

            // Set a random color in the MaterialPropertyBlock
            propertyBlock.SetTexture("_MainTex", tex);

 	renderer.material.EnableKeyword ("_NORMALMAP");
    renderer.material.EnableKeyword ("_METALLICGLOSSMAP");
	renderer.material.mainTexture = tex;
		
	}

	tracker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
	DestroyImmediate(tracker.GetComponent<Collider>());
 	tracker.GetComponent<MeshRenderer>().enabled = false;
	var myPos = this.transform.position ;
	myPos.z -= 10f;
	tracker.transform.position = myPos;
	tracker.transform.rotation = this.transform.rotation;

	foreach (Animator anim in transform.GetComponentsInChildren<Animator> ()) {

			anim.SetBool ("IsRunning", true);
		}

 }

 void ProgressTracker(){

	if(Vector3.Distance(tracker.transform.position, this.transform.position) > lookAhead)
		return;


	// if(programNumber == 6 && GameSharedMemory.currentWP < 4){
	// 	  print($"********Distance to {GameSharedMemory.currentWP} for Opponent_6 is {Vector3.Distance(tracker.transform.position, waypoints[GameSharedMemory.currentWP].transform.position)}");

	// 	  print($"********Distance to {GameSharedMemory.currentWP} for {GameSharedMemory.currentLeader.name} is {Vector3.Distance(GameSharedMemory.currentLeader.transform.position, waypoints[GameSharedMemory.currentWP].transform.position)}");
	// }

	if(Vector3.Distance(tracker.transform.position, waypoints[currentWP].transform.position) < 3){
		currentWP++;

		if (currentWP != GameSharedMemory.currentWP && currentWP > GameSharedMemory.currentWP){
                print($"*****New WayPoint is {currentWP}");
			GameSharedMemory.currentWP = currentWP;
               // print($"*****Updating Shared WayPoint to {currentWP}");
		}
	}

	if(currentWP >= waypoints.Length)
		currentWP = 0;


	tracker.transform.LookAt(waypoints[currentWP].transform);
	tracker.transform.Translate(0,0,speed * Time.deltaTime);
 }


	void FixedUpdate ()
	{
		if(!GameSharedMemory.playGame){
			return;
		}

		ProgressTracker();

		float pathOffset = -.5f;

		if(currentWP > 8){
			pathOffset = .9f;
		}

	
		Vector3 newLookAt =  new Vector3(tracker.transform.position.x + (programNumber * pathOffset), tracker.transform.position.y, tracker.transform.position.z);


		Quaternion lookAtWP = Quaternion.LookRotation(newLookAt - this.transform.position);

		this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookAtWP,  rotSpeed * Time.deltaTime);

		this.transform.Translate(0, 0, speed * Time.deltaTime);


		
	}


	void Shooting ()
	{
		// Stop the enemy where it is.
		nav.Stop();
	}


	void Chasing ()
	{
		// Create a vector from the enemy to the last sighting of the player.
		//Vector3 sightingDeltaPos = enemySight.personalLastSighting - transform.position;

		// If the the last personal sighting of the player is not close...
		//if(sightingDeltaPos.sqrMagnitude > 4f)
			// ... set the destination for the NavMeshAgent to the last personal sighting of the player.
			//nav.destination = enemySight.personalLastSighting;

		// Set the appropriate speed for the NavMeshAgent.
		nav.speed = chaseSpeed;

		// If near the last personal sighting...
		if(nav.remainingDistance < nav.stoppingDistance)
		{
			// ... increment the timer.
			chaseTimer += Time.deltaTime;

			// If the timer exceeds the wait time...
			if(chaseTimer >= chaseWaitTime)
			{
				// ... reset last global sighting, the last personal sighting and the timer.
				 
				chaseTimer = 0f;
			}
		}
		else
			// If not near the last sighting personal sighting of the player, reset the timer.
			chaseTimer = 0f;
	}





	void Patrolling ()
	{
		if (currentWP != PlayerPrefs.GetInt("LatestWayPoint", -1)){
			PlayerPrefs.SetInt("LatestWayPoint", currentWP);
		}

		// if (PlayerPrefs.GetInt("LatestWayPoint", -1) == 2 && player.tag == "Opponent_4" && !speedHandled) {
		// 		FindObjectOfType<CinemachineVirtualCamera>().Follow = player;
		// 		FindObjectOfType<CinemachineVirtualCamera>().LookAt = player;

		// 		//nav.speed = nav.speed + 20;
		// 		speedHandled = true;
		// 	} else if (!speedHandled){
		// 		nav.speed = patrolSpeed;
		// 	}

		//nav.speed = speed * 2;

		float dist = Vector3.Distance (waypoints [currentWP].position, nav.transform.position);


		if( dist < 6.0f ){

				 
				// ... increment the wayPointIndex.
				if(currentWP == waypoints.Length - 1)
					currentWP = 0;
				else {

					Vector3 p = Vector3.MoveTowards(transform.position, waypoints[currentWP].position, nav.speed * Time.deltaTime);
            		 GetComponent<Rigidbody>().MovePosition(p);

					currentWP++;

				}

		}



		// if(nav.destination != patrolWayPoints[wayPointIndex].transform.position){
		 //	nav.destination = waypoints[currentWP].transform.position;
		   // player.Translate(0,0, speed * Time.deltaTime);
		 //	player.LookAt(nav.destination);

		// }

	
			
	}
}
