/*using System.Collections.Generic;
using UnityEngine;

public class TempBattleContext : MonoBehaviour
{
    //public static TempBattleContext Instance { get; private set; }

    //public List<PlayableBaseUnitController> players = new List<PlayableBaseUnitController>();
   // public List<BaseUnitController> enemies = new List<BaseUnitController>();
    
    public bool IsAutoOn {get; set; }
    public bool IsGamePaused {get; set; }
    

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
        IsAutoOn = !IsAutoOn;
        Debug.Log($"Auto : {IsAutoOn}");
    }
}*/