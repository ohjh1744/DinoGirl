using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeCastManager : MonoBehaviour
{
    private static TypeCastManager _instance;
    public static TypeCastManager Instance { get { return _instance; } set { _instance = value; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int TryParseInt(string value)
    {
        int parsedValue;
        if (int.TryParse(value, out parsedValue) == false)
        {
            Debug.Log($"{value} Parse실패!");
            return 0;
        }
        return parsedValue;
    }

    public float TryParseFloat(string value)
    {
        float parsedValue;
        if (float.TryParse(value, out parsedValue) == false)
        {
            Debug.Log($"{value} Parse실패!");
            return 0;
        }
        return parsedValue;
    }

    public bool TryParseBool(string value)
    {
        bool parsedValue;
        if (bool.TryParse(value, out parsedValue) == false)
        {
            Debug.Log($"{value} Parse실패!");
            return false;
        }
        return parsedValue;
    }
}
