using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class BackButtonManager : MonoBehaviour
{
    public static BackButtonManager Instance { get; private set; }

    private Stack<Action> backActions = new Stack<Action>();

    private SceneChanger _sceneChanger;

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

    private void Start()
    {
        _sceneChanger = FindObjectOfType<SceneChanger>();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleBackButton();
        }
    }

    private void HandleBackButton()
    {
        if (backActions.Count > 0)
        {
            backActions.Pop().Invoke();
        }
        else if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            _sceneChanger.CanChangeSceen = true;
            _sceneChanger.ChangeScene("Lobby_OJH");
        }
        else
        {
            ShowExitConfirmation();
        }
    }

    public void AddBackAction(Action action)
    {
        backActions.Push(action);
    }

    private void ShowExitConfirmation()
    {
        // 앱 종료 확인 팝업 출력
        Debug.Log("게임을 종료하시렵니까? Application.Quit(); ");
    }
}