using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class RnDSaveData : MonoBehaviour
{
    // Database의 가장 맨 위에 위치한 정보 => Firebase에서 링크 부분
    DatabaseReference root;
    DatabaseReference stages;

    // 미리 string 으로 경로를 저장하여 사용도 가능
    // ex. public const string DataPath = "Stages";
    //     stages = root.Child(DataPath);

    private void Update()
    {
        root = BackendManager.Database.RootReference;
        // 기본 데이터의 자식 중 Stages를 가져오기
        stages = root.Child("Stages");
        if (Input.GetKeyDown(KeyCode.X))
        {
            RnDChangeDatabase(50);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            RnDChangeDatabase(150);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="test"></param>
    private void RnDChangeDatabase(bool test)
    {
        DatabaseReference testChange = stages.Child("0/isCleared");
        // 데이터 한번 쓰기
        testChange.SetValueAsync(test);
    }
    private void RnDChangeDatabase(int test)
    {
        DatabaseReference testChange = stages.Child("0/timeLimit");
        // 데이터 한번 쓰기
        testChange.SetValueAsync(test);
    }
}
