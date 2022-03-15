using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using System.Text.RegularExpressions;

//Author: Carol He, 2022; Fredrick Jin, 2022;
//For questions: carol.hcxy@gmail.com
public class LevelGenerator : EditorWindow {

 public class TrialInfo {
        public int trialId;
        public int sceneId;
        public float perspectiveShift;
        public float relativeDirection;

        public float travelDistance;

        public float travelDirection;

        public override string ToString() {
            return $"{trialId},{sceneId},{perspectiveShift},{relativeDirection},{travelDistance},{travelDirection}";
        }
    }

    private string tableFilePath = "Assets/Resources/TrialList.csv";
    private string targetDirectoryPath = "Assets/Scenes/TrialsV2";
    private int numberOfEntries = 0;
    private string prompt;

    [MenuItem("Window/LevelGenerator")]
    static void Init() {
        LevelGenerator window = (LevelGenerator)EditorWindow.GetWindowWithRect(typeof(LevelGenerator), new Rect(0, 0, 250, 150));
        window.Show();
    }

    void load() {
    }
    void Start() {

    }

    private void OnGUI() {
        prompt = $"{numberOfEntries} entries loaded";

        EditorGUILayout.LabelField(prompt);
        EditorGUILayout.LabelField("Table file path:");
        tableFilePath = EditorGUILayout.TextArea(tableFilePath);
        EditorGUILayout.LabelField("Target scene generating directory path:");
        targetDirectoryPath = EditorGUILayout.TextArea(targetDirectoryPath);

        if (GUILayout.Button("Generate trial scenes")) {
            GenerateScenes();
        }
    }

    private async void GenerateScenes() {
        List<TrialInfo> infos;
        readTrialsFromCSV(out infos, new StreamReader(tableFilePath).ReadToEnd());
        numberOfEntries = infos.Count;

        Scene baseTrialScene = EditorSceneManager.GetSceneByName("Base");
        GameObject[] gameObjects = baseTrialScene.GetRootGameObjects();

        foreach (TrialInfo info in infos) {
            foreach (GameObject gameObject in gameObjects) {
                if (gameObject.name == "FinishConfirmation") {
                    ArrivalCollisionCheck arrival = gameObject.AddComponent<ArrivalCollisionCheck>();
                    string trialEndName = "Level"+info.trialId.ToString();
                    arrival.ArrivalPointName = trialEndName;
                    //Debug.Log(arrival.ArrivalPointName);
                    arrival.CorrectThreshold = 0.7f;
                }

                if (gameObject.tag == "UserPlane") {
                    Vector3 temp = gameObject.transform.rotation.eulerAngles;
                    temp.y = info.perspectiveShift;
                    gameObject.transform.rotation = Quaternion.Euler(temp);
                }

                if (gameObject.tag == "TargetPlane"){
                    Vector3 temp = gameObject.transform.rotation.eulerAngles;
                    temp.y = info.travelDirection;
                    gameObject.transform.rotation = Quaternion.Euler(temp);

                    GameObject targetObject = gameObject.transform.GetChild(0).gameObject;
                    Vector3 tempTarget = targetObject.transform.localPosition;
                    tempTarget.z = info.travelDistance;
                    targetObject.transform.localPosition = tempTarget;
                    //Debug.Log("Parent: "+ gameObject.transform.position.z.ToString()+"Child: "+targetObject.transform.localPosition.z.ToString());
                }
            }

            string savePath = $"{targetDirectoryPath}/Trial{info.trialId}.unity";
            EditorSceneManager.SaveScene(baseTrialScene, savePath, true);
            resetGameObjects(gameObjects);
        }


    }
    private void resetGameObjects(GameObject[] gameObjects) {
        foreach (GameObject gameObject in gameObjects) {
            if (gameObject.name == "FinishConfirmation") {
                DestroyImmediate(gameObject.GetComponent<ArrivalCollisionCheck>());
            }
            
        }
    }

     public static void readTrialsFromCSV(out List<TrialInfo> infos, string text) {
        infos = new List<TrialInfo>();
        bool firstLine = true;
        foreach (string line in Regex.Split(text, "\r\n|\r|\n")) {
            if (firstLine) {
                firstLine = false;
                continue;
            }

            string[] entries = line.Split(',');
            if (entries.Length == 6) {
                TrialInfo info = new TrialInfo {
                    trialId = int.Parse(entries[0]),
                    sceneId = int.Parse(entries[1]),
                    perspectiveShift = float.Parse(entries[2]),
                    relativeDirection = float.Parse(entries[3]),
                    travelDistance = float.Parse(entries[4]),
                    travelDirection = float.Parse(entries[5])
                };

                infos.Add(info);
            }

        }
     }
    
}