using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(GameSharedMemory.currentLeader == null){
            return;
        }

        //Transform cameraTransform = GameObject.FindGameObjectWithTag("Opponent_4").transform;
         Transform cameraTransform = GameSharedMemory.currentLeader.transform;


         transform.LookAt(cameraTransform);

         Camera x = Camera.main;

        
        transform.position =  new Vector3(cameraTransform.position.x + 10,4,cameraTransform.position.z - 10);

        
    }
}
