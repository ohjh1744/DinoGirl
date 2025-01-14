using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GridContext : MonoBehaviour
{
    // 버프 형태별로 저장해놓고 id로 csv 에서 버프 이름 찾아서 적용 ? 
    // 배틀씬 매니저에있는 배치정보를 사용할것 
    // ui 에 보여주는건 따로?
    // 놓았을 때 주변 그리드 색이 바뀌게 

    private Vector2Int[] Diagonal_2 = { new Vector2Int(-1, -1), new Vector2Int(1, -1) };
    private Vector2Int[] Type_D = { new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1) };
    private Vector2Int[] Diagonal_1 = { new Vector2Int(-1, 1) };
    private Vector2Int[] Back_1 = { new Vector2Int(0, -1) };
    private Vector2Int[] T_Spin = { new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };
    private Vector2Int[] Front_3 = { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1) };
    private Vector2Int[] Back_2 = { new Vector2Int(-1, 0), new Vector2Int(-1, 1) };

    private Vector2Int[] Front_T = { new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(0, 1) };
    private Vector2Int[] Side_UpDown = { new Vector2Int(0, -1), new Vector2Int(0, 1)};
    private Vector2Int[] Side_FrontBack = { new Vector2Int(0, -1), new Vector2Int(0, 1)};
    private Vector2Int[] Front_L = { new Vector2Int(-1, 1), new Vector2Int(0, 1)};


    [SerializeField] Image[] myGrids;
    [SerializeField] Color[] highLightColors;
    [SerializeField] TMP_Text unitCount;
    private bool mybuff;

    private void OnEnable()
    {
        unitCount.text = "0/5";

    }
  


    public void UpdateBuffsList(int num) 
    {   
        for (int i = 1; i < BattleSceneManager.Instance.inGridObject.Length; i++) 
        {
            if (BattleSceneManager.Instance.inGridObject[i] != null)
            {
                BattleSceneManager.Instance.inGridObject[i].GetComponent<UnitStat>().buffs.Clear();
                BattleSceneManager.Instance.ClearBuffs();
            }
        }
        for (int i = 1; i < BattleSceneManager.Instance.inGridObject.Length; i++)
        {
            if (BattleSceneManager.Instance.inGridObject[i] != null)
            {
                if (i == num) 
                {
                    mybuff = true;
                }
                showGridArea(i, BattleSceneManager.Instance.inGridObject[i].GetComponent<UnitStat>().Id);
                
            }
            mybuff = false;
        }

        BattleSceneManager.Instance.inGridObjectCount = BattleSceneManager.Instance.inGridObject.Count(inGridObject => inGridObject != null);
        unitCount.text = BattleSceneManager.Instance.inGridObjectCount.ToString() + "/5";
        //BattlePower.text = "전투력 : " + "";
    }
    public void showGridArea(int gridnum, int id) // 이벤트로 실행됨
    { 
        string girdshapeName = CsvDataManager.Instance.DataLists[0][id]["Grid"];
        selectGridBuff(girdshapeName, gridnum - 1, id); //그리드 모양(이름), 현재 위치 , 캐릭터 아이디 
    }
    public void selectGridBuff(string name, int curPos, int id) // 이름 csv 기준 그리드 버프 이름  curPos 는 0~8 기준 현재 위치 -4 한 값을 넣을 것                                                     
    {
        switch (name)
        {
            case "Diagonal_2":
                applyGridBuff(Diagonal_2, curPos, id);
                break;
            case "Type_D":
                applyGridBuff(Type_D, curPos, id);
                break;
            case "T_Spin":
                applyGridBuff(T_Spin, curPos, id);
                break;
            case "Back_1":
                applyGridBuff(Back_1, curPos, id);
                break;
            case "Diagonal_1":
                applyGridBuff(Diagonal_1, curPos, id);
                break;
            case "Front_3":
                applyGridBuff(Front_3, curPos, id);
                break;
            case "Back_2":
                applyGridBuff(Back_2, curPos, id);
                break;
            case "Front_T":
                applyGridBuff(Front_T, curPos, id);
                break;
            case "Side_UpDown":
                applyGridBuff(Side_UpDown, curPos, id);
                break;
            case "Side_FrontBack":
                applyGridBuff(Side_FrontBack, curPos, id);
                break;
            case "Front_L":
                applyGridBuff(Front_L, curPos, id);
                break;
        }
    }
    public void applyGridBuff(Vector2Int[] gridList, int curPos, int id) // 현재 위치를 2차원으로 변경하기, 그리드에 포함된지 확인하기
    {
        Vector2Int Pos = ConvertTo2D(curPos);
        for (int i = 0; i < gridList.Length; i++)
        {
            int x = Pos.x + gridList[i].x;
            int y = Pos.y + gridList[i].y;
            if (isInIndex(x, y) == true)
            {
                int index = ConvertTo1D(x, y);
                if (mybuff== true) 
                {
                    StartCoroutine(highLightingColor(index));
                }
                if (BattleSceneManager.Instance.inGridObject[index + 1] != null)
                {
                    buffeffect(id, index + 1);
                }

            }
        }
    }
    private void buffeffect(int id, int index)  // 1 체력 2 공격력 3 방어력
    {
        int statid = int.Parse(CsvDataManager.Instance.DataLists[0][id]["StatID"]);
        int increase = int.Parse(CsvDataManager.Instance.DataLists[0][id]["PercentIncrease"]);
        Vector3Int item = new Vector3Int(id,statid, increase);
        BattleSceneManager.Instance.inGridObject[index].GetComponent<UnitStat>().buffs.Add(item);
        BattleSceneManager.Instance.inGridObject[index].GetComponent<CharSlot>().buffCount = BattleSceneManager.Instance.inGridObject[index].GetComponent<UnitStat>().buffs.Count;
        //BattleSceneManager.Instance.inGridObject[index].GetComponent<CharSlot>().onBuffDatas();
    }
    Vector2Int ConvertTo2D(int index)
    {
        int row = index / 3;
        int column = index % 3;
        return new Vector2Int(row, column);
    }
    int ConvertTo1D(int row, int column)
    {
        return row * 3 + column;
    }
    IEnumerator highLightingColor(int index)
    {
        myGrids[index].color = highLightColors[0];
        yield return new WaitForSeconds(0.3f);
        myGrids[index].color = Color.white;
    }
    private bool isInIndex(int row, int column)
    {
        if (row >= 0 && row < 3 && column >= 0 && column < 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
