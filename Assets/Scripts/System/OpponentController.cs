//OpponentControl.cs handles AI input
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
using System;
using Cinemachine;

public class OpponentController : MonoBehaviour
{
	public float patrolSpeed = 8f;                          // The nav mesh agent's speed when patrolling.
	public float chaseSpeed =  2f;                           // The nav mesh agent's speed when chasing.
	public float chaseWaitTime = 5f;                        // The amount of time to wait when the last sighting is reached.
	public float patrolWaitTime = 1f;                       // The amount of time to wait when the patrol way point is reached.
	public Transform[] patrolWayPoints;                     // An array of transforms for the patrol route.


	//private EnemySight enemySight;                          // Reference to the EnemySight script.
	private NavMeshAgent nav;                               // Reference to the nav mesh agent.
	private Transform player;                               // Reference to the player's transform.
	//private PlayerHealth playerHealth;                      // Reference to the PlayerHealth script.
	//private LastPlayerSighting lastPlayerSighting;          // Reference to the last global sighting of the player.
	private float chaseTimer;                               // A timer for the chaseWaitTime.
	private float patrolTimer;                              // A timer for the patrolWaitTime.
	private int wayPointIndex;                              // A counter for the way point array.
	private Animator anim;

	private int programNumber;
	private Boolean speedHandled = false;

	public Texture2D m_MainTexture;
	public Texture2D m_Normal;
 

	void Awake ()
	{

	
		// Setting up the references.
		//enemySight = GetComponent<EnemySight>();
		nav = GetComponent<NavMeshAgent>();
		programNumber = PlayerPrefs.GetInt("FieldRunners", 0);

		if(programNumber < 1){
			return;
		}
		String playerString = $"Opponent_{programNumber}";
		//player = GameObject.FindGameObjectWithTag(playerString).transform;
		player = transform;
		PlayerPrefs.SetInt("FieldRunners", programNumber - 1);
		print("Hello");
		//playerHealth = player.GetComponent<PlayerHealth>();
		//lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();




	}

	void Start(){		
		
	Transform horseLOD =	 player.transform.Find("Horse_Mobile/horse/horse_body");
	Renderer renderer = horseLOD.GetComponent<Renderer>();
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
	//renderer.SetPropertyBlock(propertyBlock);
	//renderer.material.SetTexture("_BumpMap", m_Normal);

	//renderer.materials[0].SetTexture("_MainTex", tex);

		



// 			horse.GetComponent<Renderer>().material.SetTexture("_Maintext", tex);
		}

 }


	void Update ()
	{
		// If the player is in sight and is alive...
//		if(enemySight.playerInSight && playerHealth.health > 0f)
//			// ... shoot.
//			Shooting();

		// If the player has been sighted and isn't dead...
		//else if(enemySight.personalLastSighting != lastPlayerSighting.resetPosition && playerHealth.health > 0f)
			// ... chase.
			//Chasing();

		// Otherwise...
		//else
			// ... patrol.
			Patrolling();


		foreach (Animator anim in transform.GetComponentsInChildren<Animator> ()) {

			anim.SetBool ("IsRunning", true);
		}
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
		if (wayPointIndex != PlayerPrefs.GetInt("LatestWayPoint", -1)){
			PlayerPrefs.SetInt("LatestWayPoint", wayPointIndex);
		}

		if (PlayerPrefs.GetInt("LatestWayPoint", -1) == 2 && player.tag == "Opponent_4" && !speedHandled) {
				FindObjectOfType<CinemachineVirtualCamera>().Follow = player;
				FindObjectOfType<CinemachineVirtualCamera>().LookAt = player;

				//nav.speed = nav.speed + 20;
				speedHandled = true;
			} else if (!speedHandled){
				nav.speed = patrolSpeed * 2.0f ;
			}

		float dist = Vector3.Distance (patrolWayPoints [wayPointIndex].position, nav.transform.position);


		if( dist < 6.0f )
			{
				 
				// ... increment the wayPointIndex.
				if(wayPointIndex == patrolWayPoints.Length - 1)
					wayPointIndex = 0;
				else
					wayPointIndex++;

			
		}

		//if(nav.destination != patrolWayPoints[wayPointIndex].transform.position){
			//nav.destination = patrolWayPoints[wayPointIndex].transform.position;
			 player.Translate(0,0, patrolSpeed * Time.deltaTime);
			//player.LookAt(nav.destination);

		//}


		
			
	}
}
