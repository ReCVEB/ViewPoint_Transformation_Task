﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//Author: Mengyu Chen, 2019
//For questions: mengyuchenmat@gmail.com
public class NaviTutorialInstructionsManager : MonoBehaviour
{
    public Transform PlayerTransform;
    [Header("Reference Setup")]
    [SerializeField] TextMeshPro InstructionText;
    [SerializeField] GameObject InstructionObject;
    [SerializeField] GameObject Target;
    [SerializeField] GameObject UserAvatar;
    [SerializeField] GameObject VisibleTarget;
    public GameObject guidingParticles;

    [Header("Multiple Position Setup")]
    [SerializeField] GameObject[] TargetPositions;
    
    [Header("Multiple Visible Position Setup")]
    [SerializeField] GameObject[] VisibleTargetPositions;

    [Header("Perspective Shift Setup")]
    [SerializeField] GameObject[] UserAvatarRotations;

    


    [Header("Instruction Texts")]
    [TextArea][SerializeField] string[] GuidingTexts;

    int phase = 0;
    int textInstrCounter = 0;
    int targetPosCounter = 0;
    bool activated = false;
    NaviManager naviManager;
    TrackPlayer logManager;
    bool moveOn=false;
    InteractionManager interactionManager;
    MapManager mapManager;
    void Start()
    {
        logManager = TrackPlayer.instance;
        interactionManager = InteractionManager.instance;
        mapManager = MapManager.instance;
        naviManager = NaviManager.instance;
        interactionManager.PinchClicked += PinchClickDetected;

        //init target position
        Target.transform.position = TargetPositions[phase].transform.position;
        UserAvatar.transform.rotation = UserAvatarRotations[phase].transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void PinchClickDetected(bool state)
    {
        if (!state)
        {
            return;
        }

        if (!activated)
        {
            VisibleTarget.transform.position = VisibleTargetPositions[2].transform.position;

            // Learning is over
            if (phase > 11)
            {
                naviManager.CompleteMaze(true, "Learning Phase");
                StartCoroutine(Activation(5));
                return;
            }

            // show the map every other phase besides the first two phases, which will be instruction text
            else if (phase != 0 && phase % 2 == 0)
            {
                if((logManager.PlayerTransform.position.x < guidingParticles.transform.position.x + 0.6 &&
                        logManager.PlayerTransform.position.x > guidingParticles.transform.position.x - 0.6) &&
                        (logManager.PlayerTransform.position.z < guidingParticles.transform.position.z + 0.6 &&
                        logManager.PlayerTransform.position.z > guidingParticles.transform.position.z - 0.6)){
                    InstructionObject.SetActive(false);
                    guidingParticles.SetActive(false);
                    // only reveal map for 2 seconds for last 2 practice trials 
                    if (phase > 6)
                    {
                        mapManager.RevealMap(2);
                    }
                    else
                    {
                        mapManager.RevealMap(4);
                    }

                    logManager.WriteCustomInfo("Learning Tutorial: user starts reading map for 4 seconds");

                    // the first two practice trials will have the targets be visible
                    if (phase == 2)
                    {
                        VisibleTarget.transform.position = VisibleTargetPositions[0].transform.position;
                    }

                    else if (phase == 4)
                    {
                        VisibleTarget.transform.position = VisibleTargetPositions[1].transform.position;

                    }
                    phase++;
                    StartCoroutine(Activation(1));
                }
            }
            else
            {
                guidingParticles.SetActive(true);
                // practice phase 1
                if (phase == 3 && !moveOn)
                {
                    //make sure the participant cannot move on unless they are at the target 
                    if ((logManager.PlayerTransform.position.x < VisibleTargetPositions[0].transform.position.x + 0.4 &&
                         logManager.PlayerTransform.position.x > VisibleTargetPositions[0].transform.position.x - 0.4) &&
                        (logManager.PlayerTransform.position.z < VisibleTargetPositions[0].transform.position.z + 0.4 &&
                         logManager.PlayerTransform.position.z > VisibleTargetPositions[0].transform.position.z - 0.4))
                    {
                        moveOn = true;
                    }

                    VisibleTarget.transform.position = VisibleTargetPositions[0].transform.position;
                }
                
                // practice phase 2
                else if (phase == 5 && !moveOn)
                {
                    if ((logManager.PlayerTransform.position.x < VisibleTargetPositions[1].transform.position.x + 0.4 &&
                         logManager.PlayerTransform.position.x > VisibleTargetPositions[1].transform.position.x - 0.4) &&
                        (logManager.PlayerTransform.position.z < VisibleTargetPositions[1].transform.position.z + 0.4 &&
                         logManager.PlayerTransform.position.z > VisibleTargetPositions[1].transform.position.z - 0.4 ))
                    {
                        moveOn = true;
                    }

                    VisibleTarget.transform.position = VisibleTargetPositions[1].transform.position;
                }
                else
                {
                    moveOn = true;
                }

                if (moveOn)
                {
                    VisibleTarget.transform.position = VisibleTargetPositions[2].transform.position;
                    InstructionObject.SetActive(true);
                    InstructionText.text = GuidingTexts[textInstrCounter];
                    

                    logManager.WriteCustomInfo(
                        "Learning Tutorial: user selects target " + (phase / 2 - 1) + " position");
                    if (targetPosCounter != 5)
                    {
                        Target.transform.position = TargetPositions[targetPosCounter].transform.position;
                        UserAvatar.transform.rotation = UserAvatarRotations[targetPosCounter].transform.rotation;
                    }

                    // make sure participant is on the footprints
                    // if (((logManager.PlayerTransform.position.x < guidingParticles.transform.position.x + 0.6 &&
                    //     logManager.PlayerTransform.position.x > guidingParticles.transform.position.x - 0.6) &&
                    //     (logManager.PlayerTransform.position.z < guidingParticles.transform.position.z + 0.6 &&
                    //     logManager.PlayerTransform.position.z > guidingParticles.transform.position.z - 0.6)) || phase < 2){
                    textInstrCounter++;
                    phase++;
                    moveOn = false;

                    if (phase >= 2)
                    {
                        targetPosCounter++;
                    }
                    StartCoroutine(Activation(1));
                    //}
                }
            }
        }

    }
    IEnumerator Activation(float seconds)
    {
        activated = true;
        yield return new WaitForSeconds(seconds);
        activated = false;
        
    }
}
