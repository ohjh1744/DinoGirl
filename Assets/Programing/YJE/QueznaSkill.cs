using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QueznaSkill : MonoBehaviour
{
    GameObject target;
    public void DoSkill(List<GameObject> enemyList, float damgae)
    {
        Debug.Log("스킬실행");
        List<float> enemy = new List<float>();
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemy.Add(Vector2.Distance(gameObject.transform.position, enemyList[i].transform.position));
        }

        int index = enemy.IndexOf(enemy.Min());
        target = enemyList[index];
    }
}
