﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Mengyu Chen, 2019; Carol He, 2021
//For questions: mengyuchenmat@gmail.com; carol.hcxy@gmail.com
public class ArrowCollisionCheck : MonoBehaviour
{
    
    NaviManager naviManager;
    ArrowManager arrowManager;
    FadeManager fadeManager;
    InteractionManager interactionManager;
    TrackPlayer logManager;
    
    //public Transform PlayerTransform;

    private bool controllerConfirm = false;
    private bool controllerTouching = false;
    private bool pinchClicked = false;
    private bool facingRightDir = false;
    void Start(){
        naviManager = NaviManager.instance;
        arrowManager = ArrowManager.instance;
        fadeManager = FadeManager.instance;
        logManager = TrackPlayer.instance;

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
        Quaternion playerRot = logManager.PlayerTransform.rotation;

        //Make sure player is facing the right direction
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
            Debug.Log("facing+pinch+touching");
            transform.position = new Vector3(0, 100, 0);
            fadeManager.FadeIn();
            // determines how much time needed to wait after clicking trigger and seeing the map
            // Debug.Log("trigger0:loadlevel()");
            StartCoroutine(Trigger(0.5f));
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
                //Debug.Log("trigger1:loadlevel()");
                StartCoroutine(Trigger(0.5f));
            }
        } else
        {
            if (collider.tag == "GameController")
            {
                controllerTouching = true;
                //Debug.Log("controllerTouching");
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
                //Debug.Log("controllerTouchingFalse");
            }
        }
    }
    private IEnumerator Trigger(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime / 3.0f);
        //Debug.Log("trigger1");
        fadeManager.FadeOut();
        yield return new WaitForSecondsRealtime(waitTime);
        naviManager.LoadLevel();
        arrowManager.Reset();
        arrowManager.TriggerFootPrint();
    }
    private void PinchClickDetected(bool state)
    {
        pinchClicked = state;
        //Debug.Log("pinch"+state);
    }
}
