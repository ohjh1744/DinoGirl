using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RnDUintParser : MonoBehaviour
{
    // DataManager에서 가져올 DataLists를 받을 List제작
    List<Dictionary<string, string>> dictionary = new List<Dictionary<string, string>>();
    // 불러온 unit의 기초정보를 저장할 리스트
    List<RnDUnitBase> unitBases = new List<RnDUnitBase>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UnitDataLoadTest();
        }
    }

    private void UnitDataLoadTest()
    {
        dictionary = DataManager.Instance.DataLists[0];
        for (int i = 0; i < dictionary.Count; i++)
        {
            Debug.Log(dictionary[i]["Id"]);
            Debug.Log(dictionary[i]["Name"]);
            Debug.Log(dictionary[i]["Atk"]);
            Debug.Log(dictionary[i]["Def"]);
            Debug.Log(dictionary[i]["Hp"]);
            Debug.Log(dictionary[i]["HealHp"]);
            Debug.Log(dictionary[i]["Hit"]);
            Debug.Log(dictionary[i]["Critical"]);
            Debug.Log(dictionary[i]["HealCost"]);
            Debug.Log(dictionary[i]["Dodge"]);
            Debug.Log(dictionary[i]["Sight"]);
            Debug.Log(dictionary[i]["Type"]);
            Debug.Log(dictionary[i]["LevelUp"]);
        }
    }
}
