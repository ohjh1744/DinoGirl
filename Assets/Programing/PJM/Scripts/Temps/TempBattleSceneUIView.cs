using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TempBattleSceneUIView : UIBInder
{
    private void Awake()
    {
        Bind();
    }

    private void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        
        AddEvent("PauseButton", EventType.Click, (PointerEventData data) => ToggleTimeScale());
        AddEvent("AutoButton", EventType.Click, (PointerEventData data) => ToggleAuto());
    }
    private void Update()
    {
        
    }
    public void ToggleTimeScale()
    {
        if (TempBattleContext.Instance.isGamePaused)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
        
        TempBattleContext.Instance.isGamePaused = !TempBattleContext.Instance.isGamePaused;
        GetUI<TMP_Text>("PauseText").text = TempBattleContext.Instance.isGamePaused ? " Pause : ON" : "Pause : OFF";
    }
    
    public void ToggleAuto()
    {
        TempBattleContext.Instance.isAutoOn = !TempBattleContext.Instance.isAutoOn;
        GetUI<TMP_Text>("AutoText").text = TempBattleContext.Instance.isAutoOn ? " Auto : ON" : "Auto : OFF";
        Debug.Log($"Auto : {TempBattleContext.Instance.isAutoOn}");
    }

}
