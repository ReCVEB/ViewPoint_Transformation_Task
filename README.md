# Viewpoint Transformation Task
last update: 02/15/2022  
beta version author: [Mengyu Chen](https://github.com/mengyuchen/BirdViewNavigation)  
Updated version: Carol He (contact_info: carol.hcxy@gmail.com), Scott Matsubara, Rongfei Jin

## Background Info:
This task needs to be run in an ambulatory virtual reality setting with at leat 7 by 7 meters physical open space. 

It aims to test participants' ***spatial perspective taking ability***. Spatial perspective taking is an essential cognitive ability that enables people to imagine how an object or scene would appear from a perspective different from their current physical viewpoint.

The data collected by this task has been published. The journal article is available [online](https://www.frontiersin.org/articles/10.3389/frvir.2022.971502/full). Raw data and analysis scripts is in another [GitHub Repo](https://github.com/CarolHeChuanxiuyue/HumanViewpointTransformationAbility)

**To run this task, please use the exe files in the [executation file folder](https://github.com/ReCVEB/ViewPoint_Transformation_Task/tree/main/Task%20Execution%20Files).**
- [VRIntroduction](https://github.com/ReCVEB/ViewPoint_Transformation_Task/tree/main/Task%20Execution%20Files/VRIntroduction) is a small VR space irrelavent to the task so that participants can get familiar with the vr system and control before doing the task
- [SOT32](https://github.com/ReCVEB/ViewPoint_Transformation_Task/tree/main/Task%20Execution%20Files/SOT32) is the 32-item version of Computerized Spatial Orientation Task running on desktop
- [*iVTT_Experimenter_Protocol.docx*](https://github.com/ReCVEB/ViewPoint_Transformation_Task/blob/main/Task%20Execution%20Files/iVTT_Experimenter_Protocol%20Script.docx) documents the scripts experimenters used to give instructions.


## Unity Model and C# Code 

**To modify the task or check the unity code, please check the folder [_iVTT-UnityProgram-02152022_](https://github.com/ReCVEB/ViewPoint_Transformation_Task/tree/main/iVTT-UnityProgram-02152022)**

### Software Requirements
1. Unity 2018 4.0f1 LTS
You can download here: https://unity3d.com/get-unity/download/archive
2. Git LFS
Make sure you have Git LFS: https://git-lfs.github.com/

### Beta Version Reference 
Refer to [*Code_Structure_BetaVersion_Mengyu_2019.pdf*](https://github.com/ReCVEB/ViewPoint_Transformation_Task/blob/main/Code_Structure_BetaVersion_Mengyu_2019.pdf)

### Git Tips:

#### Merge Conflicts
```shell
git checkout --ours folderA/filename.xxx
git checkout --theirs folderB/filename.yyy
```

#### Revert all your changes
```shell
git stash
```
