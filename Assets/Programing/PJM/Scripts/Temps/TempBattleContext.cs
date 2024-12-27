using System.Collections.Generic;
using UnityEngine;

public class TempBattleContext : MonoBehaviour
{
    public static TempBattleContext Instance { get; private set; }

    public List<PlayableUnitController> players = new List<PlayableUnitController>();

    public bool isAutoOn {get; set; }
    public bool isGamePaused {get; set; }
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    private void InitializeAuto()
    {
        
    }
    
    public void ToggleAuto()
    {
        isAutoOn = !isAutoOn;
        Debug.Log($"Auto : {isAutoOn}");
    }
}