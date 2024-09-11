using UnityEngine;
using System;
using System.Linq;
using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine.AI;


public static class GameSharedMemory {


    public static Boolean playGame;
    public static int currentWP = 1;

    public static String finishOrder;

    public static GameObject currentLeader = null;

    public static GameObject currentSecondRunner = null;



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


    private String win_runner = "";

    private string show_runner = "";



    public void DoThing()
    {
        ThingHappened?.Invoke();
    }

    void Start(){

        GameSharedMemory.currentLeader = runners[0];

    }


    void FixedUpdate(){

        if(!GameSharedMemory.playGame){
            return;
        }


         List<RunnerPositions> runnerPositions = new List<RunnerPositions>();

        if(win_runner.Length == 0 ){
            String[] wpRunners = GameSharedMemory.finishOrder.Split(",");
            win_runner = $"Opponent_{wpRunners[0]}";
            show_runner = $"Opponent_{wpRunners[1]}";
        }

        GameObject prevLeader = GameSharedMemory.currentLeader;

        if(lastWP != GameSharedMemory.currentWP){
            currentLeaderDist = null;
        }

       foreach(GameObject runner in runners){
             Vector3 dir = (runner.transform.position - waypoints[GameSharedMemory.currentWP].transform.position);

             runnerPositions.Add(new RunnerPositions{
                runner = runner,
                distance = dir.magnitude
             });

       }

       GameSharedMemory.currentLeader = runnerPositions.OrderBy(x=>x.distance).ElementAt(0).runner;

       GameSharedMemory.currentSecondRunner = runnerPositions.OrderBy(x=>x.distance).ElementAt(1).runner;


    if(GameSharedMemory.currentLeader.tag != prevLeader.tag){
            FindObjectOfType<CinemachineCamera>().Follow = GameSharedMemory.currentLeader.transform;
            FindObjectOfType<CinemachineCamera>().LookAt = GameSharedMemory.currentLeader.transform;
            prevLeader = GameSharedMemory.currentLeader;
            print($"*****Current Leader is {GameSharedMemory.currentLeader.tag}");
    }

    if(GameSharedMemory.currentWP >= 10 && GameSharedMemory.currentLeader.name != win_runner){
        OpponentController opponentController = runners.FirstOrDefault(x=>x.name ==  win_runner).GetComponent<OpponentController>();
        NavMeshAgent agent = runners.FirstOrDefault(x=>x.name ==  show_runner).GetComponent<NavMeshAgent>();

        if(opponentController != null){
            opponentController.speed = 19;
            agent.avoidancePriority = 1;
        }
    } else {
        OpponentController opponentController = runners.FirstOrDefault(x=>x.name == win_runner).GetComponent<OpponentController>();

        if(opponentController != null){
            opponentController.speed = 15;
        }
    }

    if(GameSharedMemory.currentWP >= 10 && GameSharedMemory.currentSecondRunner.name != show_runner && show_runner != GameSharedMemory.currentLeader.name){
        
        OpponentController opponentController = runners.FirstOrDefault(x=>x.name ==  show_runner).GetComponent<OpponentController>();
            NavMeshAgent agent = runners.FirstOrDefault(x=>x.name ==  show_runner).GetComponent<NavMeshAgent>();

        if(opponentController != null){
            opponentController.speed = 19;
            agent.avoidancePriority = 2;
        }
        
    } else {
        OpponentController opponentController = runners.FirstOrDefault(x=>x.name == show_runner).GetComponent<OpponentController>();

        if(opponentController != null){
            opponentController.speed = 15;
        }
    }

    
}

       
   
}