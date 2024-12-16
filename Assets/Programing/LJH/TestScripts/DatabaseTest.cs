using Firebase.Database;
using Firebase.Extensions;
using Google.MiniJSON;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;





public class DatabaseTest : MonoBehaviour
{
    [SerializeField] GameObject monster;
    [SerializeField] GameObject player;

    [SerializeField] Transform[] myPoss;
    [SerializeField] Transform[] enemyPoss;

    private void OnEnable()
    {
    }


    private StageData stageData;
    
   
    private void getDbData() 
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("Stages").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogWarning("값 가져오기 취소됨");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"값 가져오기 실패함 : {task.Exception.Message}");
                    return;
                }

                DataSnapshot snapshot = task.Result;

                Debug.Log($"Found {snapshot.GetRawJsonValue()}");
                Debug.Log("가져오기 성공");
            });



    }
    public void TestgetStageData(int stageNum) 
    {   
        string num = (stageNum-1).ToString(); // 입력할때 1적은 값을 입력하는게 나아보이긴 함 

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("Stages").Child(num).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogWarning("값 가져오기 취소됨");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"값 가져오기 실패함 : {task.Exception.Message}");
                    return;
                }

                DataSnapshot snapshot = task.Result;
                Debug.Log($"Found {snapshot.GetRawJsonValue()}");
                if (snapshot.Exists)
                {
                    foreach (var childSnapshot in snapshot.Children)
                    {

                        string result = childSnapshot.GetRawJsonValue();
                        Debug.Log(result);
                        
                        //stageData = JsonUtility.FromJson<StageData>(snapshot.GetRawJsonValue());
                        
                    }
                }
               

                //Debug.Log($"Found {snapshot.GetRawJsonValue()}");
                //Debug.Log("가져오기 성공");
 
                //Debug.Log($"{stageData.stageName}");
                //Debug.Log($"{stageData.timeLimit}");
                //Debug.Log($"{stageData.isClear}");
            });
    }
    public void getSt(int num)  // 예시 세팅
    {
        if (num == 1)
        {
            int[] ints = {1,2,3};
            StartStage(3, ints, 3, ints);
        }
        else if (num == 2) 
        {
            int[] ints = { 4, 7, 8 };
            StartStage(3, ints, 3, ints);
        }
        else if (num == 3) 
        {
            int[] ints = { 1, 6, 3 };
            StartStage(3, ints, 3, ints);
        }

    }
    // 스테이지씬에 미리 세팅되어야 하는 것
    // 배경 , 전투씬 ui , 캐릭터와 몬스터가 배치될 위치, 배경음  
    public void StartStage(int monsterCount,int[] monsterpos,int playerCount, int[] myPos)
    {
        for (int i = 0; i < monsterCount; i++) 
        {
            Instantiate(monster, enemyPoss[monsterpos[i]].position, Quaternion.identity);
        }
        for (int i = 0; i < playerCount; i++) 
        {
            Instantiate(player, myPoss[myPos[i]].position, Quaternion.identity);
        }
    }

    public void StageClear() 
    {

    }
}
