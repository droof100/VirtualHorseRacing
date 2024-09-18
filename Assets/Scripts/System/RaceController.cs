using UnityEngine;
using System;
using System.Linq;
using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;


public static class GameSharedMemory {


    public static Boolean playGame;
    public static int currentWP = 1;

    public static String finishOrder;

    public static GameObject currentLeader = null;

    public static GameObject currentSecondRunner = null;

    public static GameObject selectedWinner;

    public static GameObject selectedPlace;




}
public  class RaceController: MonoBehaviour
{
    private class RunnerPositions{
        public GameObject runner;
        public float distance;
        
    }

    public event Action ThingHappened;
    public GameObject[] runners;
    private Boolean leaderChanged = false;
    private float? currentLeaderDist;

    private int lastWP = -1;

    public GameObject[] waypoints;

    public Camera finishCamera;

    public Camera fpsCamera;

    public Camera raceCamera;

    public GameObject finisOrderMenu;


    private String win_runner = "";

    private string place_runner = "";

    private  List<RunnerPositions> runnerPositions = new List<RunnerPositions>();

    private Boolean? showFPS = null;



    public void DoThing()
    {
        ThingHappened?.Invoke();
    }

    void Start(){

        GameObject.Find("Canvas").SetActive(false);

        GameSharedMemory.currentLeader = runners[0];


        finishCamera.enabled = false;
        fpsCamera.enabled = false;

        raceCamera.enabled = true;
        showFPS = false;

    }

void Update(){

    if(GameSharedMemory.currentWP == 15 && (GameSharedMemory.currentLeader.transform.position - waypoints[GameSharedMemory.currentWP].transform.position).magnitude < 20.0 ){
        raceCamera.enabled = false;
        fpsCamera.enabled = false;
        FindObjectOfType<CinemachineCamera>().enabled = false;
        finishCamera.enabled = true;
    } else if (showFPS != null && fpsCamera.enabled != showFPS){
         raceCamera.enabled = !showFPS??false;
        fpsCamera.enabled = showFPS??false;
        FindObjectOfType<CinemachineCamera>().enabled = !showFPS??false;
    }
}

void Input(){

    if (showFPS == null){
        return;
    }

	 Touch touch = new Touch();

    if(touch.tapCount == 2){
        showFPS = !showFPS;
    }
}
void FixedUpdate(){

        if(!GameSharedMemory.playGame){
            return;
        }

        runnerPositions.Clear();


        if(win_runner.Length == 0 ){
            String[] wpRunners = GameSharedMemory.finishOrder.Split(",");
            win_runner = $"Opponent_{wpRunners[0]}";
            GameSharedMemory.selectedWinner = GameObject.Find(win_runner);
            place_runner = $"Opponent_{wpRunners[1]}";
            GameSharedMemory.selectedPlace = GameObject.Find(place_runner);
            if(wpRunners.Length == 3 && wpRunners[2] == "fps"){
                showFPS = true;
            }
        }

        GameObject prevLeader = GameSharedMemory.currentLeader;

        if(lastWP != GameSharedMemory.currentWP){
            currentLeaderDist = null;
        }

       foreach(GameObject runner in runners){
             Vector3 dir = (runner.transform.position - waypoints[GameSharedMemory.currentWP].transform.position);

            

             runnerPositions.Add(new RunnerPositions{
                runner = runner,
                distance = dir.magnitude}
                //distance = runner.transform.position.x - waypoints[GameSharedMemory.currentWP].transform.position.x}
             );

       }

       List<RunnerPositions> sortedRunners = runnerPositions.OrderBy(x=>x.distance).ToList();

       GameSharedMemory.currentLeader = sortedRunners.ElementAt(0).runner;

       GameSharedMemory.currentSecondRunner = sortedRunners.ElementAt(1).runner;

    if(runnerPositions[5] != null && runnerPositions[5].runner.transform.position.z > waypoints[15].transform.position.z){


		//UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        //GameObject.FindGameObjectsWithTag("FinishOrderMenu").FirstOrDefault().SetActive(true);


        finisOrderMenu.SetActive(true);


          TextMeshProUGUI wnrTextField = GameObject.Find("WinTextField").GetComponent<TextMeshProUGUI>();

          TextMeshProUGUI placeTextField = GameObject.Find("PlaceTextField").GetComponent<TextMeshProUGUI>();

            wnrTextField.text = win_runner.Replace("Opponent_", "");

            placeTextField.text = place_runner.Replace("Opponent_", "");
          

            //  if(txtField.name == "PlaceTextField"){
            //     txtField.text = show_runner;
            // }

        
    }


    if(GameSharedMemory.currentLeader.tag != prevLeader.tag){
            FindObjectOfType<CinemachineCamera>().Follow = GameSharedMemory.currentLeader.transform;
            FindObjectOfType<CinemachineCamera>().LookAt = GameSharedMemory.currentLeader.transform;
            prevLeader = GameSharedMemory.currentLeader;
            print($"*****Current Leader is {GameSharedMemory.currentLeader.tag}");
    }

    if(GameSharedMemory.currentWP >= 10 && (GameSharedMemory.currentLeader.name != win_runner || GameSharedMemory.currentSecondRunner.name != place_runner )){
        OpponentController opponentController = runners.FirstOrDefault(x=>x.name ==  win_runner).GetComponent<OpponentController>();
        NavMeshAgent agent = runners.FirstOrDefault(x=>x.name ==  win_runner).GetComponent<NavMeshAgent>();

        if(opponentController != null){
            opponentController.speed = 18;
            agent.avoidancePriority = 1;
        }
    } else if (win_runner == GameSharedMemory.currentLeader.name && (sortedRunners[1].distance-sortedRunners[0].distance > 1)) {
        OpponentController opponentController = runners.FirstOrDefault(x=>x.name == win_runner).GetComponent<OpponentController>();

        if(opponentController != null){
            opponentController.speed = 15;
        }
    }

    if(GameSharedMemory.currentWP >= 10 && GameSharedMemory.currentSecondRunner.name != place_runner && place_runner != GameSharedMemory.currentLeader.name){
        
        OpponentController opponentController = runners.FirstOrDefault(x=>x.name ==  place_runner).GetComponent<OpponentController>();
            NavMeshAgent agent = runners.FirstOrDefault(x=>x.name ==  place_runner).GetComponent<NavMeshAgent>();

        if(opponentController != null){
            opponentController.speed = 17;
            agent.avoidancePriority = 10;
        }
        
    } else if((GameSharedMemory.currentSecondRunner.name == place_runner && (sortedRunners[2].distance-sortedRunners[1].distance> 3)) || place_runner == GameSharedMemory.currentLeader.name) {
        OpponentController opponentController = runners.FirstOrDefault(x=>x.name == place_runner).GetComponent<OpponentController>();

        if(opponentController != null ){
            opponentController.speed = 15;
        }
    }

   

    
}

       
   
}