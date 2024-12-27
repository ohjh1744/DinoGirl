using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] private IdleReward idleReward;

    private void Awake()
    {
    }

    private void OnDisable()
    {
        idleReward.SaveExitTime();
    }
}