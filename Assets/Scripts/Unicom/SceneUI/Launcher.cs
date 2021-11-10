using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Author: Mengyu Chen, 2018; Carol He, 2021
//For questions: mengyuchenmat@gmail.com
public class Launcher : MonoBehaviour
{
    //key control
	[SerializeField] KeyCode newGameKey = KeyCode.N;

    //scene loading
    public int levelCount;
    int loadedLevelBuildIndex;

    void Start()
    {
        if (Application.isEditor) {
			for (int i = 0; i < SceneManager.sceneCount; i++) {
				Scene loadedScene = SceneManager.GetSceneAt(i);
                //make sure level contains word Level
				if (loadedScene.name.Contains("Level")) {
					SceneManager.SetActiveScene(loadedScene);
					loadedLevelBuildIndex = loadedScene.buildIndex;
					return;
				}
			}
        }

        BeginNewGame();
		StartCoroutine(LoadLevel(1));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(newGameKey)){
			BeginNewGame();
			StartCoroutine(LoadLevel(loadedLevelBuildIndex));
		} else {
			for (int i = 1; i <= levelCount; i++) {
				if (Input.GetKeyDown(KeyCode.Alpha0 + i)) {
					StartCoroutine(LoadLevel(i));
					return;
				}
			}
		}
    }

    IEnumerator LoadLevel (int levelBuildIndex) {
		enabled = false; // <-- this is the Unity Behavior enable bool
		if (loadedLevelBuildIndex > 0) {
			yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
		}
		//async loading for multiple frames, can also show a loading screen at this point
		yield return SceneManager.LoadSceneAsync(
			levelBuildIndex, LoadSceneMode.Additive
		);
		SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
		loadedLevelBuildIndex = levelBuildIndex;
		enabled = true;
	}
	void BeginNewGame () {
		
	}
}
