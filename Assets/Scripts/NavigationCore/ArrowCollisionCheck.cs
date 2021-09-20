﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Mengyu Chen, 2019
//For questions: mengyuchenmat@gmail.com
public class ArrowCollisionCheck : MonoBehaviour
{
    
    NaviManager naviManager;
    ArrowManager arrowManager;
    FadeManager fadeManager;
    InteractionManager interactionManager;

    [Header("Debug Options")]
    [Tooltip("For debug purpose.")] 
    [SerializeField] bool debug = false;
    
    public Transform PlayerTransform;

    private bool controllerConfirm = false;
    private bool controllerTouching = false;
    private bool pinchClicked = false;
    private bool facingRightDir = false;
    void Start(){
        naviManager = NaviManager.instance;
        arrowManager = ArrowManager.instance;
        fadeManager = FadeManager.instance;

        interactionManager = InteractionManager.instance;
        controllerConfirm = interactionManager.ControllerConfirm;

        //event subscription from interaction manager
        if (controllerConfirm) interactionManager.PinchClicked += PinchClickDetected;
    }
    private void OnDestroy()
    {
        if (controllerConfirm) interactionManager.PinchClicked -= PinchClickDetected;
    }
    void Update(){
        if (controllerConfirm != interactionManager.ControllerConfirm)
        {
            controllerConfirm = interactionManager.ControllerConfirm;
        }
        Quaternion playerRot = PlayerTransform.rotation;
        
        //Debug.Log(PlayerTransform.rotation);
        if (playerRot.eulerAngles.y < 5 || playerRot.eulerAngles.y > 355)
        {
            facingRightDir = true;
        }
        else{
            facingRightDir = false;
        }
        if (controllerTouching && pinchClicked && facingRightDir)
        {
            // Debug.Log("arrow collision detected");
            transform.position = new Vector3(0, 100, 0);
            fadeManager.FadeIn();
            StartCoroutine(Trigger(2.0f));
        }

    }
    void OnTriggerEnter(Collider collider){
        if (!controllerConfirm)
        {
            if (collider.tag == "Player")
            {
                // Debug.Log("arrow collision detected");
                transform.position = new Vector3(0, 100, 0);
                fadeManager.FadeIn();
                StartCoroutine(Trigger(1.0f));
            }
        } else
        {
            if (collider.tag == "GameController")
            {
                controllerTouching = true;
            }
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (controllerConfirm)
        {
            if (collider.tag == "GameController")
            {
                controllerTouching = false;
            }
        }
    }
    private IEnumerator Trigger(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime / 3.0f);
        fadeManager.FadeOut();
        yield return new WaitForSecondsRealtime(waitTime);
		naviManager.LoadLevel();
        arrowManager.Reset();
        arrowManager.TriggerFootPrint();
    }
    private void PinchClickDetected(bool state)
    {
        pinchClicked = state;
    }
}
