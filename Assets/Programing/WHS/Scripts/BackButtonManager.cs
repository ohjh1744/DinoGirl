using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class BackButtonManager : MonoBehaviour
{
    public static BackButtonManager Instance { get; private set; }

    private Stack<Action> _backActions = new Stack<Action>();

    private SceneChanger _sceneChanger;

    public int BackActionCount => _backActions.Count;
    [SerializeField] private int _backActionCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
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
        // ESC버튼 (폰에서 뒤로가기 기능)
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleBackButton();
        }

        _backActionCount = _backActions.Count;
    }

    private void HandleBackButton()
    {
        // backAction 스택이 있으면 한개 지우기
        if (_backActions.Count > 0)
        {
            _backActions.Pop().Invoke();
        }
        // Lobby에선 게임종료
        else if (SceneManager.GetActiveScene().name == "Lobby_OJH")
        {
            Application.Quit();
        }
        // 다른 씬에선 로비로 이동
        else if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            _sceneChanger.CanChangeSceen = true;
            _sceneChanger.ChangeScene("Lobby_OJH");
        }
    }

    // UI창을 열 때 backAction 스택 추가하기
    public void AddBackAction(Action action)
    {
        _backActions.Push(action);
    }

    private void ShowExitConfirmation()
    {
        // 앱 종료 확인 팝업 출력
        Debug.Log("게임을 종료하시렵니까? Application.Quit(); ");
    }
}