﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//Author: Mengyu Chen, 2019
//For questions: mengyuchenmat@gmail.com
public class NaviTutorialInstructionsManager : MonoBehaviour
{
    [Header("Reference Setup")]
    [SerializeField] TextMeshPro InstructionText;
    [SerializeField] GameObject InstructionObject;
    [SerializeField] GameObject Target;
    [SerializeField] GameObject UserAvatar;

    [Header("Multiple Position Setup")]
    [SerializeField] GameObject[] TargetPositions;

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
            
            if (phase > 7)
            {

                naviManager.CompleteMaze(true, "Learning Phase");
                StartCoroutine(Activation(5));
                return;
            }
            if (phase != 0 && phase % 2 == 0)
            {
                InstructionObject.SetActive(false);
                mapManager.RevealMap(8);
                logManager.WriteCustomInfo("Learning Tutorial: user starts reading map for 10 seconds");
                phase++;
                StartCoroutine(Activation(9));
            }
            else
            {

                InstructionObject.SetActive(true);
                phase++;
                InstructionText.text = GuidingTexts[textInstrCounter];
                textInstrCounter++;

                logManager.WriteCustomInfo("Learning Tutorial: user selects target " + (phase / 2 - 1) + " position");
                if (targetPosCounter != 3)
                {
                    Target.transform.position = TargetPositions[targetPosCounter].transform.position;
                    UserAvatar.transform.rotation = UserAvatarRotations[targetPosCounter].transform.rotation;
                }

                if (phase >= 2)
                {
                    targetPosCounter++;
                }

                StartCoroutine(Activation(5));
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
