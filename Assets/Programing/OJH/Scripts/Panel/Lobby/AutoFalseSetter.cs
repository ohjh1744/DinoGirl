using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFalseSetter : MonoBehaviour
{
    [SerializeField] private float _falseTime;

    private float _currentTime;

    private void OnEnable()
    {
        _currentTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        _currentTime += Time.deltaTime;
        if(_currentTime > _falseTime)
        {
            gameObject.SetActive(false);
        }
    }

    public void ResetCurrentTime()
    {
        _currentTime = 0f;
    }


}
