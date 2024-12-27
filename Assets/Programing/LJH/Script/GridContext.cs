using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridContext : MonoBehaviour
{
    // 버프 형태별로 저장해놓고 id로 csv 에서 버프 이름 찾아서 적용 ? 
    // 배틀씬 매니저에있는 배치정보를 사용할것 
    // ui 에 보여주는건 따로?
    // 놓았을 때 주변 그리드 색이 바뀌게 
   
    // 기준 : 0 ~ 8 기준으로 4를 중앙으로 두고 사용 
    private int[] Diagonal_2 = {-4,2};
    private int[] Type_D = {-3,-2,1,3,4};
    private int[] Bak_1 = {-1};
    private int[] T_Spine = { -3, -1, 1 };
    private int[] Diagonal_1 = {-2};
    private int[] Front_3 = {-2,1,4};
    private int[] Back_2 = {-1,2};

    


    
    public void selectGridBuff(string name,int curPos) // 이름 csv 기준 그리드 버프 이름  curPos 는 0~8 기준 현재 위치 -4 한 값을 넣을 것
                                                       //  ex 현재 위치 0  -4  = -4 
                                                       //               7  -4  = 3 
    {   
        
        switch (name)
        {
            case "Diagonal_2":
                applyGridBuff(curPos);
                    break;
            case "Type_D":

                    break;
            case "T_Spine":

                    break;
        }
    }
    public void applyGridBuff(int curPos) 
    {
        for (int i = 0; i < 9; i++) { }
    }

    private bool isInIndex(int[] PosList, int index)  // 배열 범위 안에 인덱스가 포함인지 검사 , 유닛 위치 배열
    {
        if (index >= 0 && PosList.Length > index) 
        {
            return true;  
        }
        else
            return false;

    }
}
