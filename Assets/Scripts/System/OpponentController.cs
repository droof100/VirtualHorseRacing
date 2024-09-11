//OpponentControl.cs handles AI input
using UnityEngine;
using UnityEngine.AI;
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

	Rigidbody  m_Rigidbody;
 

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

// 	void OnTriggerEnter(Collider other) {

// 		if(other.name == "Side_Fence"){
// 			return;
// 		}

// 	 	MeshRenderer m = GetComponent<MeshRenderer>();
// 		UnityEngine.Vector3 vec = m.bounds.size;

// 		//	other.transform.Translate()

// 	  	if(GameSharedMemory.currentWP > 10){
// 	 		float oldx = other.transform.position.x;

// 		// 	other.transform.Translate(-3.0f,0,0);
// 		print($"***** GameObject {other.name} as moved to {other.transform.position.x} and old x was {oldx}");}
//   }


	// void OnControllerColliderHit(ControllerColliderHit hit) {
  
 
    //   print($"**** GameObject {hit.gameObject.name} Was Hit!!!!");
	  	
	// }



void Start(){		
		
	Transform horseLOD =	 player.Find("Horse_Mobile/horse/horse_body");
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

	 m_Rigidbody = GetComponent<Rigidbody>();
		
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


		//m_Rigidbody.MovePosition(transform.position + transform.forward * (speed * Time.deltaTime));

	}
 


}
