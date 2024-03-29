﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
//Author: Mengyu Chen, 2019; Carol He, 2021
//For questions: mengyuchenmat@gmail.com; carol.hcxy@gmail.com
public enum NaviMode : int{tutorial, learning, testing, full};
public class NaviManager : MonoBehaviour
{
    [Header("Managers")]
    static public NaviManager instance;
    [SerializeField] LevelLauncher levelManager;
    [SerializeField] ArrowManager arrowManager;
    [SerializeField] TimeManager timeManager;
    [SerializeField] TrackPlayer logManager;
    [SerializeField] FadeManager fadeManager;
    [SerializeField] MapManager mapManager;

    [Header("Level Params")]
    [HideInInspector]public NaviMode CurrentMode;
    [HideInInspector]public int CurrentLevel;
    [HideInInspector] public bool LevelStartConfirmed = false;

    public int levelPassed = 0;
    public int trialNumber = 0;
    [Header("Random Level Loader")]
    private int levelcount = 50;
    private int testPhaseStartingIndex = 2;
    private int nextLevel;
    private bool missionComplete = false;

    private HashSet<int> candidates = new HashSet<int>();
    public TrialOrderInfo trialOrderInfo = new TrialOrderInfo();

    private bool isRestoring = false;

    [SerializeField] string TrialOrderInfoFilename = "trialOrderInfo.json";

    System.Random random = new System.Random();

    // [Header("Space Visibility")]
    // public bool SpatialObjectOff = true;
    void Awake()
    {
        if (instance == null){
            instance = this;
        } else if (instance != this){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start(){
        //find all other manager instances
        if (fadeManager == null) fadeManager = FadeManager.instance;
        if (levelManager == null) levelManager = LevelLauncher.instance;
        if (arrowManager == null) arrowManager = ArrowManager.instance;
        if (timeManager == null) timeManager = TimeManager.instance;
        if (logManager == null) logManager = TrackPlayer.instance;
        if (mapManager == null) mapManager = MapManager.instance;

        //the total level count depends on how many starting points
        levelcount = arrowManager.StartingPoints.Length;
        int[] TrialLevels = new int[levelcount-testPhaseStartingIndex];
        for (int i = 0; i<levelcount-testPhaseStartingIndex; i++)
        {
            TrialLevels[i] = i+1;
        }

        trialNumber = levelcount - testPhaseStartingIndex;
        GenerateRandomSequence(trialNumber);
        
    }

    // Update is called once per frame
    void Update()
    {       

    }
    public void ResetManagers(){
        levelManager.LevelCheck();
        timeManager.Reset();
        logManager.Reset();
        arrowManager.Reset();
    }
    //steps of proceeding to next level: choose level -> prepare level -> loadlevel
    public void ChooseLevel(){
        GetNextLevel(testPhaseStartingIndex);
        logManager.Reset();
    }

    public void PrepareLevel(){
        levelManager.LevelCheck(); //check level and unload existing level
        CurrentLevel = nextLevel; // 1 -> training scene
        LevelStartConfirmed = false; //prepared but not confirmed, need user active confirmation later
        //arrowManager.Reset();
        //targetManager.Reset();
        fadeManager.ResetFadeOut();
        if (missionComplete == false){
            if (CurrentLevel == 1){
                Debug.Log("Preparing tutorial level");
            } else if (CurrentLevel == 2){
                Debug.Log("Preparing learning level");
            } else {
                Debug.Log("Preparing testing level:" + (CurrentLevel - 2));
                levelPassed++;
            }

            arrowManager.Activate(CurrentLevel - 1);
        } else {
            timeManager.WriteTimeDisplay("Mission Complete");
            arrowManager.SetTextContent("Mission Complete. Thank you!");
            arrowManager.InstructionText.transform.position = new Vector3(0,1.3f,0);
            arrowManager.InstructionText.SetActive(true);
        }
    }
    //called by ArrowCollisionCheck, based on user confirmation on instruction avatar
    public void LoadLevel(){
        levelManager.SelectLevel(CurrentLevel);
        StartCoroutine(WaitInit());
    }
    //separate call to unload level, instead of PrepareLevel()
    public void UnloadLevel()
    {
        levelManager.LevelCheck();
    }
    public void ControlledInit()
    {
        logManager.WriteLevelInfo();

        logManager.Run();
        timeManager.Run();
        //spaceManager.Run();
        //targetManager.Run();
        LevelStartConfirmed = true; // level official start upon user confirmation
        
    }
    private IEnumerator WaitInit()
    {
        while (levelManager.loading)
        {
            yield return null;
        }


        //targetManager.Init();
        //spaceManager.Init();


        //start confirmation check

        if (CurrentLevel <= 2)
        {
            //if in tutorial and learning phase, no need for starting confirmation
            ControlledInit();
        } else
        {
            //Debug.Log("mapManager-Init");
            mapManager.Init();
        }
    }

    //call made by arrival collision check object, once target is reached, CompleteMaze() is called
    public void CompleteMaze(bool success, string name){
        timeManager.Complete(success, name);
    }
    //called in main UI, which will change the parameters of navi mode
    public void NaviModeSelect(int mode){
        CurrentMode = (NaviMode)mode;
        Debug.Log("Selected Current Mode = " + CurrentMode);

        //if tutorial
        if (CurrentMode == NaviMode.tutorial || CurrentMode == NaviMode.full){
            nextLevel = 1; // tutorial level = 1
            timeManager.SetTimeLimit(int.MaxValue); // timeCount goes infinite.
            PrepareLevel();
        }
        //if learning
        if (CurrentMode == NaviMode.learning){
            nextLevel = 2;
            timeManager.SetTimeLimit(int.MaxValue); // timeCount goes infinite.
            PrepareLevel();
        }
        //if training
        if (CurrentMode == NaviMode.testing){
            ChooseLevel();
            PrepareLevel(); //to do: randomize prepare level
        }
    }
    //a string can be provided to skip certain levels, use comma to separate
    public void SkipLevels(string levels)
    {
        int count = Int32.Parse(levels);
        for (int i = 0; i < count; i++)
        {
            candidates.Add(trialOrderInfo.sequence[i]);
            Debug.Log("Level Skipped " + trialOrderInfo.sequence[i]);
        }
        trialOrderInfo.pos = count;
        Debug.Log("Remaining " + (levelcount - testPhaseStartingIndex - candidates.Count));
    }
    // private void GetNextLevel(int min){
    //     //check if it's done
    //     if (candidates.Count == levelcount - min || CurrentMode == NaviMode.tutorial){
    //         missionComplete = true;
    //         ResetManagers(); // finishes
    //         Debug.Log("Mission Complete");
    //         return;
    //     }
    //     //check if its moving to learning phase
    //     if (CurrentMode == NaviMode.full && CurrentLevel == 1){
    //         nextLevel = 2;
    //         timeManager.SetTimeLimit(int.MaxValue);
    //         Debug.Log("Tutorial done. Move to next Learning phase");
    //         return;
    //     }
        
    //     //continue on with learning phase
    //     // if (CurrentLevel >= 2 && CurrentLevel < 5)
    //     // {
    //     //     nextLevel = CurrentLevel += 1;
    //     // }
    //     while (candidates.Count != levelcount - min){
    //         int randNum = UnityEngine.Random.Range(min, levelcount);
    //         //Debug.Log(randNum + " = generated");
    //         if (candidates.Add(randNum))
    //         {
    //             nextLevel = randNum + 1;
    //             Debug.Log("Level " + (nextLevel - 2) + " selected.");
    //             break;
    //         }
    //     }
    // }

    private void GetNextLevel(int min)
    {
        //check if it's done
        //if (candidates.Count == levelcount - min || CurrentMode == MazeMode.learning || CurrentMode == MazeMode.tutorial){
        if (candidates.Count == levelcount - min || CurrentMode == NaviMode.tutorial){
            missionComplete = true;
            ResetManagers(); // finishes
            //Debug.Log("Mission Complete");
            Application.Quit();
            return;
        }
        //check if its moving to learning phase
        if (CurrentMode == NaviMode.full && CurrentLevel == 1){
            nextLevel = 2;
            timeManager.SetTimeLimit(int.MaxValue);
            //Debug.Log("Tutorial done. Move to next Learning phase");
            return;
        }
        //continue on with learning phase
        while (candidates.Count != levelcount - min){
            if (candidates.Add(trialOrderInfo.getCurrentTrialNumber()))
            {
                nextLevel = trialOrderInfo.getCurrentTrialNumber();
                SaveThisRun();
                trialOrderInfo.pos++;
                Debug.Log("Level " + (nextLevel - 2) + " selected.");
                break;
            }
        }
    }
        private void GenerateRandomSequence(int size)
    {
        System.Random random = new System.Random();
        trialOrderInfo.sequence = new List<int>(Enumerable.Range(3, size).ToArray().OrderBy(x => random.Next()).ToArray());

        String sequneceString = "";
       foreach (int item in trialOrderInfo.sequence)
       {
           sequneceString += item + ",";
       }
        Debug.Log(sequneceString);

    }

    public void RestoreLastRun()
    {
        trialOrderInfo = ResumeLog.readTrialOrderInfoFromJSON(TrialOrderInfoFilename);
        for (int i = 0; i != trialOrderInfo.pos; ++i)
        {
            candidates.Add(trialOrderInfo.sequence[i]);
        }
        Debug.Log("restore");
    }

    private void SaveThisRun()
    {
        ResumeLog.writeTrialOrderInfoToJson(TrialOrderInfoFilename, trialOrderInfo);
    }

}
