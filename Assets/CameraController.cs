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
    void Update()
    {
        Transform cameraTransform = GameObject.FindGameObjectWithTag("Opponent_1").transform;

         transform.LookAt(cameraTransform);

         Camera x = Camera.main;

        
        transform.position =  new Vector3(cameraTransform.position.x + 10,4,cameraTransform.position.z - 10);

        


        
    }
}
