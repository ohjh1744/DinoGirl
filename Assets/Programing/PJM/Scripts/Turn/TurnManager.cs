using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public bool IsAutoBattle { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void ToggleAutoBattle()
    {
        IsAutoBattle = !IsAutoBattle;
    }
}
