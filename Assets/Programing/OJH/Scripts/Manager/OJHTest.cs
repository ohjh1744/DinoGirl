using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class OJHTest : MonoBehaviour
{
    [SerializeField] private int a;

    //string이기 때문에 추후에 int나 float형등 형변환 필요.
    private List<Dictionary<string, string>> stageDic;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Test();
        }
    }

    private void Test()
    {
        stageDic = DataManager.Instance.DataLists[(int)E_CsvData.Stage];

        // 시트의 첫번째 행의 TileLimit 값 150
        Debug.Log(stageDic[0]["TimeLimit"]);

        //포문을 통해 값을 찾는 방법
        foreach(Dictionary<string, string> field in stageDic)
        {
            if (field["Id"] == "0")
            {
                a = int.Parse(field["TimeLimit"]);
                Debug.Log(a);
            }
        }
        
        
    }
}
