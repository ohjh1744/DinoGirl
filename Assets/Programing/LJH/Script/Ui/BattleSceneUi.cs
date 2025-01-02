using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class BattleSceneUi : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject winUi;
    [SerializeField] private GameObject loseUi;



    [SerializeField] private float time;
    [SerializeField] private float curTime;

    private int minute;
    private int second;

    private void OnEnable()
    {
        time = BattleSceneManager.Instance._timeLimit;
        //time = 120f;

        StartCoroutine(startTimer());
    }

    IEnumerator startTimer() 
    {
        curTime = time;
        while (curTime > 0)
        {
            curTime -= Time.deltaTime;
            minute = (int)curTime / 60;
            second = (int)curTime % 60;
            timerText.text = minute.ToString("00") + ":" + second.ToString("00");
            yield return null;

            if (curTime <= 0)
            {
                Debug.Log("시간 종료");
                curTime = 0;
                yield break;
            }
        }
    }
    public void WinorLose() 
    {   
        
        Debug.Log("테스트");
        if (BattleSceneManager.Instance.myUnits.All(item => item.gameObject.activeInHierarchy ==false)) // list의 내용이 전부 false면 
        {
            // 패배
            Debug.Log("패배");
            BattleSceneManager.Instance.curBattleState = BattleSceneManager.BattleState.Lose;
        }
        else if (BattleSceneManager.Instance.enemyUnits.All(item => item.gameObject.activeInHierarchy == false)) 
        {
            // 승리
            Debug.Log("승리");
            BattleSceneManager.Instance.curBattleState = BattleSceneManager.BattleState.Win;
        }
        else if (curTime <= 0)
        {
            // 시간제한 패배
            Debug.Log("시간제한 패배");
            BattleSceneManager.Instance.curBattleState = BattleSceneManager.BattleState.Lose;
        }

    }
    public void inToDbData() 
    {
        // 승리시 획득 아이템 + 승리 결과(스테이지 클리어 여부 삽입) , 추후에 레이드 클리어 결과 삽입도 생각
        // db 수정 + playerdatamanager 수정해야함
        Debug.Log("데이터 삽입");
        

    }
    public void goToLobby() 
    {   
        inToDbData();
        // 승리시 획득 아이템 + 승리 결과(스테이지 클리어 여부 삽입)

    }


}
