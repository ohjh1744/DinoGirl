using System.Collections.Generic;
using UnityEngine;

public class RnDStageParser : MonoBehaviour
{
    // DataManager에서 가져올 DataLists를 받을 List제작
    List<Dictionary<string, string>> dictionary = new List<Dictionary<string, string>>();

    // 원하는 데이터를 형전환 후 저장할 리스트(Stage 자료형)
    List<RnDStageCSV> stages = new List<RnDStageCSV>();
    private void Update()
    {
        // 자료 테스트 용
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DataLoadTest();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            DataShow();
        }
    }

    private void DataLoadTest()
    {
        // DataManager.cs에서 파싱된 데이터 불러오기
        dictionary = RnDDataManager.Instance.DataLists[0];
        // [index][원하는 항목 제목 string]
        for(int i = 0; i < dictionary.Count; i++)
        {
            Debug.Log(dictionary[i]["Id"]);
            Debug.Log(dictionary[i]["StageName"]);
            Debug.Log(dictionary[i]["TimeLimit"]);
            Debug.Log(dictionary[i]["MonsterCount"]);
            //Debug.Log(dictionary[i]["MonsterPos"]);

            RnDStageCSV stage = new RnDStageCSV();
            // 각 stage의 변수들의 자료형에 맞추어 변환 후 저장
            stage.id = int.Parse(dictionary[i]["Id"]);
            stage.stageName = dictionary[i]["StageName"];
            stage.timeLimit = int.Parse(dictionary[i]["TimeLimit"]);
            stage.monsterCount = int.Parse(dictionary[i]["MonsterCount"]);
            //stage.monsterPos = int.Parse(dictionary[i]["MonsterPos"]);
            stages.Add(stage); // 완성된 stage를 stages 리스트에 저장
        }
    }
    private void DataShow()
    {
        for (int i = 0; i < stages.Count; i++)
        {
            Debug.Log($"{i} 번째");
            Debug.Log(stages[i].id);
            Debug.Log(stages[i].stageName);
            Debug.Log(stages[i].timeLimit);
            Debug.Log(stages[i].monsterCount);
            //Debug.Log(stages[i].monsterPos);
        }
    }
}
