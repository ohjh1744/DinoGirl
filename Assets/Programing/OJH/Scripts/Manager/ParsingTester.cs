using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ParsingTester : MonoBehaviour
{
    [SerializeField] private int a;

    //string이기 때문에 추후에 int나 float형등 형변환 필요.
    private Dictionary<int, Dictionary<string, string>> stageDic;



    [ContextMenu("DebugTest")]
    public void Test()
    {
       stageDic = CsvDataManager.Instance.DataLists[(int)E_CsvData.Stat];

        Debug.Log(TypeCastManager.Instance.TryParseInt(stageDic[2]["4"]));

        //포문을 통해 값을 찾는 방법
        foreach (var outerKeyValue in stageDic)
        {
            // 외부 딕셔너리의 키 (int)
            Debug.Log("Outer Key: " + outerKeyValue.Key);

            // 내부 딕셔너리 (Dictionary<string, string>) 출력
            foreach (var innerKeyValue in outerKeyValue.Value)
            {
                // 내부 딕셔너리의 키와 값 (string, string)
                Debug.Log("  Inner Key: " + innerKeyValue.Key + ", Inner Value: " + innerKeyValue.Value);
            }
        }


    }
}
