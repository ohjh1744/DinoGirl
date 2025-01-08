using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class OJHTest : MonoBehaviour
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
        //foreach(Dictionary<string, string> field in stageDic)
        //{
        //    if (field["Id"] == "0")
        //    {
        //        a = int.Parse(field["TimeLimit"]);
        //        Debug.Log(a);
        //    }
        //}
        
        
    }
}
