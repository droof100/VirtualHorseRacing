//Statistics.cs keeps track of the racer's rank, lap, race times, race state, saving best times, split times , wrong way detecion etc.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Statistics : MonoBehaviour
{

    //Int
    public int rank;//current rank
    public int lap; //current lap
    public int checkpoint; //current checkpoint(Checkpoint Race)

    //Strings
    public string currentLapTime; //current lap time string displayed by RaceUI.cs
    public string prevLapTime; //Previous lap time string displayed by RaceUI.cs
    public string totalRaceTime; //Total lap time string displayed by RaceUI.cs

    //Floats
    private float lapTimeCounter; // keeps track of our current Lap time counter
    private float totalTimeCounter; //keeps track of our total race time
    private float currentBestTime; //keeps track of the current session best time in TimeTrial
    private float dotProduct; //used for wrong way detection
    private float registerDistance = 10.0f; //distance to register a passed node
    private float reviveTimer;
	private float wrongwayTimer; //delay timer
	
    //Hidden Vars
    [HideInInspector]
    public Transform lastPassedNode; //last node to passed - used when respawning.
    [HideInInspector]
    public Transform target; //progress tracker target
    [HideInInspector]
    public int currentNodeNumber; //next node index in the "path" list
    [HideInInspector]
    public List<Transform> path = new List<Transform>();
    [HideInInspector]
    public List<bool> passednodes = new List<bool>();
    [HideInInspector]
    public List<Transform> checkpoints = new List<Transform>();
    [HideInInspector]
    public List<bool> passedcheckpoints = new List<bool>();
    [HideInInspector]
    public bool finishedRace;
    [HideInInspector]
    public bool knockedOut;
    [HideInInspector]
    public bool goingWrongway;
    [HideInInspector]
    public bool passedAllNodes;
    [HideInInspector]
    public float speedRecord;//speed trap top speed

    void OnEnable()
    {
        if (!RaceManager.instance)
        {
            enabled = false;
            return;
        }
        else {
            //FindPath(); 
            Initialize();
        }
    }

    void Initialize()
    {
        lap = 1;
  
    }


    

    

    void Update()
    {
        GetPath(); 
        CalculateAngleDifference();
     
    }


    void GetPath()
    {
        int n = currentNodeNumber;

        Transform node = path[n] as Transform;
        Vector3 nodeVector = target.InverseTransformPoint(node.position);

        //register that we have passed this node
        if (nodeVector.magnitude <= registerDistance)
        {
            currentNodeNumber++;
            passednodes[n] = true;

            //set our last passed node
            if (n != 0)
                lastPassedNode = path[n - 1];
            else
                lastPassedNode = path[path.Count - 1];
        }

        //Check if all nodes have been passed
        foreach (bool pass in passednodes)
        {
            if (pass == true)
            {
                passedAllNodes = true;
            }
            else {
                passedAllNodes = false;
            }
        }

        //Reset the currentNodeNumber after passing all the nodes
        if (currentNodeNumber >= path.Count)
        {
            currentNodeNumber = 0;
        }
    }


    // Race time calculations
    
    //Called on new lap
    
 

//    void RegisterCheckpoint(Checkpoint.CheckpointType type, float timeAdd)
//    {
//        if (finishedRace || knockedOut)
//            return;
//
//        switch (type)
//        {
//
//            case Checkpoint.CheckpointType.Speedtrap:
//                if (RaceManager.instance._raceType != RaceManager.RaceType.SpeedTrap)
//                    return;
//
//                //add to the racers total speed
//                float speed = 0;
//
//                if (GetComponent<Car_Controller>())
//                    speed = GetComponent<Car_Controller>().currentSpeed;
//
//                if (GetComponent<Motorbike_Controller>())
//                    speed = GetComponent<Motorbike_Controller>().currentSpeed;
//
//                //if (GetComponent<Boat_Controller>())
//                    //speed = GetComponent<Boat_Controller>().currentSpeed;
//
//                speedRecord += speed;
//
//                //play a sound and show info
//                if (gameObject.tag == "Player")
//                {
//                    SoundManager.instance.PlaySound("checkpoint", true);
//                    if (RaceManager.instance.showRaceInfoMessages)
//                        RaceUI.instance.ShowRaceInfo("+ " + speed + " mph", 1.0f);
//                }
//
//                break;
//
//            case Checkpoint.CheckpointType.TimeCheckpoint:
//                //add our chekpoint
//                checkpoint++;
//
//                //add to the timer
//                lapTimeCounter += timeAdd;
//
//                //play a sound and show info
//                if (gameObject.tag == "Player")
//                {
//                    SoundManager.instance.PlaySound("checkpoint", true);
//                    if (RaceManager.instance.showRaceInfoMessages)
//                        RaceUI.instance.ShowRaceInfo("+ " + RaceManager.instance.FormatTime(timeAdd), 1.0f);
//                }
//                break;
//        }
//    }


   // Check for wrong way
    void CalculateAngleDifference()
    {
        float nodeAngle = target.transform.eulerAngles.y;
        float transformAngle = transform.eulerAngles.y;
        float angleDifference = nodeAngle - transformAngle;


        //Set wrong way to true after a dealy of 1.0 seconds
        goingWrongway = (wrongwayTimer >= 1.0f);

        if (Mathf.Abs(angleDifference) <= 230f && Mathf.Abs(angleDifference) >= 120)
        {
            //Add/reset the timer
            if (GetComponent<Rigidbody>().velocity.magnitude >= 5.0f)
            {
                wrongwayTimer += Time.deltaTime;
            }
            else
            {
                wrongwayTimer = 0.0f;
            }
        }
        else
        {
            wrongwayTimer = 0.0f;
        }
    }

    

//    void OnTriggerEnter(Collider other)
//    {
//
//        //Finish line
//        if (other.tag == "FinishLine" && passedAllNodes)
//        {
//            NewLap();
//        }
//
//        //Checkpoint
//        if (other.GetComponent<Checkpoint>())
//        {
//            for (int i = 0; i < checkpoints.Count; i++)
//            {
//                if (checkpoints[i] == other.transform && !passedcheckpoints[i])
//                {
//                    passedcheckpoints[i] = true;
//                    RegisterCheckpoint(checkpoints[i].GetComponent<Checkpoint>().checkpointType, checkpoints[i].GetComponent<Checkpoint>().timeToAdd);
//                }
//            }
//        }
 //   }
}
