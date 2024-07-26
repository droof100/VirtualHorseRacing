using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {


	public float speed = 4f;
	public GameObject horse;
	public GameObject jockey;

	Vector3 movement;
	Animator anim;
	Rigidbody playerRigidBody;
	float camRayLength = 100f;
	int floorMask;


	Vector2 fp, lp;
	float dragDistance;
	float timeCounter;

	private float distance = 10;

	public float rotspeed = 20;//45

//	void OnTriggerEnter(Collider coll)
//	{
//		print ("Fence Detected");
//
//		transform.Rotate (0, (transform.position.x + -10)  * rotspeed * Time.deltaTime, 0);
//
//
//
//	}
//
	void OnCollisionEnter(Collision collision) 
	{
		print ("Fence Detected");
	}

	void Start()
	{
		floorMask = LayerMask.GetMask ("Floor");

		if (horse != null) {
			
			//anim = 	transform.Find ("horse").GetComponent<Animation>();

			//horse.GetComponent<Animator> (); //GetComponent<Animator> ();




		}
		playerRigidBody = GetComponent<Rigidbody> (); 



	}

	private enum TouchDir
	{
		Up,
		Down,
		Right,
		Left,
		None
	}

	void FixedUpdate()
	{
		float h = 0;
		float v = 0;
		bool hitLeft = false;
		bool hitRight = false;

		Transform upperSpine;

		upperSpine = transform.GetChild(2).GetChild(0).Find(@"HorsePelvis/HorseSpine1/HorseSpine2/HorseSpine3/HorseSpine4/HorseSpine5/HorseSpine6/HorseRibcage/HorseNeck1");

		if (Application.platform == RuntimePlatform.Android) {



			v = 1;

			if (TouchInput()   == TouchDir.Up) {
				timeCounter = 2;

			} else  
			{
				if (timeCounter > 0) {
					timeCounter -= Time.deltaTime;
				} else {
					timeCounter = 0;
					v = 0;
				}
			}
			
		} else {
			h = Input.GetAxis ("Horizontal");
		    v = Input.GetAxis ("Vertical");
			hitLeft = Input.GetKeyDown (KeyCode.LeftShift);
			
		}

		 Move (h, v);
		//Turning ();
		Animating (h, v);
		Hit (hitLeft, false);

	}

	void Hit(bool left,bool right)
	{ 

		//anim.SetBool ("IsRunning", walking);

		foreach (Animator anim in transform.GetComponentsInChildren<Animator> ()) {

			anim.SetBool ("LeftHit", left);
		}
	}

	void Move(float h,float v)
	{

		Quaternion localRotation = transform.rotation;
		
		movement.Set (h, 0f, v);

		//movement = movement.normalized * speed * Time.deltaTime;


		//playerRigidBody.MovePosition (transform.position + Camera.main.transform.forward * distance * Time.deltaTime);

		//transform.position = transform.position + Camera.main.transform.forward * distance * Time.deltaTime;

		if (h != 0f || v != 0f) {

			playerRigidBody.MovePosition ((transform.position + movement) + (this.transform.forward * distance * Time.deltaTime));

			//transform.position += this.transform.forward * distance * Time.deltaTime * speed;
		}

		if (Application.platform == RuntimePlatform.Android) {
			localRotation.x = Input.acceleration.x;
			localRotation.y = Input.acceleration.y;

			if (localRotation.x > .2)
			{
				transform.Rotate (0, (rotspeed * Time.deltaTime), 0);
			}
			else if (localRotation.x < -.2)
			{
				transform.Rotate(0,(-rotspeed * Time.deltaTime), 0);
			}
			
		} else {
			transform.Rotate (0, Input.GetAxis ("Horizontal") * rotspeed * Time.deltaTime, 0);
		}
		 
	}

	TouchDir TouchInput()
	{

		if (Input.touchCount == 1) // user is touching the screen with a single touch
		{
			Touch touch = Input.GetTouch(0); // get the touch
			if (touch.phase == TouchPhase.Began) //check for the first touch
			{
				fp = touch.position;
				lp = touch.position;
			}
			else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
			{
				lp = touch.position;
			}
			else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
			{
				lp = touch.position;  //last touch position. Ommitted if you use list

				//Check if drag distance is greater than 20% of the screen height
				if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
				{//It's a drag
					//check if the drag is vertical or horizontal
					if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
					{   //If the horizontal movement is greater than the vertical movement...
						if ((lp.x > fp.x))  //If the movement was to the right)
						{   //Right swipe
							Debug.Log("Right Swipe");
						}
						else
						{   //Left swipe
							Debug.Log("Left Swipe");
						}
					}
					else
					{   //the vertical movement is greater than the horizontal movement
						if (lp.y > fp.y)  //If the movement was up
						{   //Up swipe
							return TouchDir.Up;
						}
						else
						{   //Down swipe
							Debug.Log("Down Swipe");
						}
					}
				}
				else
				{   //It's a tap as the drag distance is less than 20% of the screen height
					Debug.Log("Tap");
				}
			}
		}

		return TouchDir.None;
	}
	void Turning()
	{
//		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
//			
//		RaycastHit floorhit;
//
//		if (Physics.Raycast (camRay, out floorhit, camRayLength, floorMask)) {
//
//			Vector3 playerToMouse = floorhit.point - transform.position;
//
//			playerToMouse.y = 0f;
//
//
//			Quaternion newRotation = Quaternion.LookRotation (playerToMouse);
//			playerRigidBody.MoveRotation (newRotation);
//		}


	}

	void Animating(float h,float v)
	{
		bool walking = h != 0f || v != 0f;
 
		//anim.SetBool ("IsRunning", walking);
 
 
			foreach (Animator anim in transform.GetComponentsInChildren<Animator> ()) {

				anim.SetBool ("IsRunning", walking);
			}
 
	}



}
