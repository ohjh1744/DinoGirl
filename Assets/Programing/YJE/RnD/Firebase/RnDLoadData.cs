using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class RnDLoadData : MonoBehaviour
{
    // Database의 가장 맨 위에 위치한 정보 => Firebase에서 링크 부분
    DatabaseReference root;
    DatabaseReference stages;

    private void Update()
    {
        root = BackendManager.Database.RootReference;
        // 기본 데이터의 자식 중 Stages를 가져오기
        stages = root.Child("Stages");
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            RnDSettingDate(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RnDSettingDate(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RnDSettingDate(2);
        }
    }

    /// <summary>
    /// 기본 데이터 불러오기 방식
    /// </summary>
    private void RnDFirebaseLoad()
    {
        DatabaseReference testCleared = stages.Child("0").Child("stageName");
        testCleared.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            // 가져오기 실패 시 에러
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            // DataSnapshot으로 결과를 받기 => key, Value값으로
            DataSnapshot snapshot = task.Result;
            Debug.Log(snapshot.Value.ToString());
        });
    }

    private void RnDSettingDate(int id)
    {
        RnDStage rnDStage = new RnDStage();

        DatabaseReference getID = stages.Child($"{id}");

        getID.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted) // 데이터 가져오기에 실패한 경우
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            // 데이터를 DataSnapshot으로 결과 가져오기
            DataSnapshot snapshot = task.Result;

            // 각 결과를 string으로 가져와 알맞은 자료형으로 변환 - 저장
            string result = snapshot.Child("isCleared").Value.ToString();
            rnDStage.isCleared = bool.Parse(result);
            rnDStage.name = snapshot.Child("stageName").Value.ToString();
            result = snapshot.Child("timeLimit").Value.ToString();
            rnDStage.timeLimit = int.Parse(result);

            Debug.Log(rnDStage.isCleared);
            Debug.Log(rnDStage.name);
            Debug.Log(rnDStage.timeLimit);
        });

        /*DatabaseReference testCleared = stages.Child($"{id}").Child("isCleared");
        DatabaseReference teststageName = stages.Child($"{id}").Child("stageName");
        DatabaseReference testTimeLimit = stages.Child($"{id}").Child("timeLimit");

        testCleared.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            // 가져오기 실패 시 에러
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            // DataSnapshot으로 결과를 받기 => key, Value값으로
            DataSnapshot snapshot = task.Result;
            string result = snapshot.Value.ToString();
            bool isCleared = bool.TryParse(result, out isCleared);
            RnDStage.isCleared = isCleared;
            Debug.Log(RnDStage.isCleared);
        });
        teststageName.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            // 가져오기 실패 시 에러
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            // DataSnapshot으로 결과를 받기 => key, Value값으로
            DataSnapshot snapshot = task.Result;
            string result = snapshot.Value.ToString();
            RnDStage.name = result;
            Debug.Log(RnDStage.name);

        });
        testTimeLimit.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            // 가져오기 실패 시 에러
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            // DataSnapshot으로 결과를 받기 => key, Value값으로
            DataSnapshot snapshot = task.Result;
            string result = snapshot.Value.ToString();
            int testTimeLimit = int.Parse(result);
            RnDStage.timeLimit = testTimeLimit;
            Debug.Log(RnDStage.timeLimit);
        });*/
    }
}
