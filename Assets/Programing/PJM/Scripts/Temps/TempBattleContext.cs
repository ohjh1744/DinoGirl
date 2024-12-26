using System.Collections.Generic;
using UnityEngine;

public class TempBattleContext : MonoBehaviour
{
    public static TempBattleContext Instance { get; private set; }

    public List<PlayableUnitController> players;

    public bool isAutoOn {get; set; }
    public bool isGamePaused {get; set; }
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}