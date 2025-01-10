using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class BackButtonManager : MonoBehaviour
{
    public static BackButtonManager Instance { get; private set; }

    private Stack<GameObject> _openPanels = new Stack<GameObject>();

    private SceneChanger _sceneChanger;

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
    }

    private void HandleBackButton()
    {
        // 열린 패널이 있으면 닫기
        if (_openPanels.Count > 0)
        {
            CloseTopPanel();
        }
        // Lobby에선 게임종료
        else if (SceneManager.GetActiveScene().name == "Lobby_OJH")
        {
            // 종료 팝업 UI 출력해서 종료 누르면 quit
            // Application.Quit();
            Debug.Log("게임 종료");
        }
        // 다른 씬에선 로비로 이동
        else if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            _sceneChanger.CanChangeSceen = true;
            _sceneChanger.ChangeScene("Lobby_OJH");
        }
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        _openPanels.Push(panel);
    }

    private void CloseTopPanel()
    {
        if (_openPanels.Count > 0)
        {
            GameObject panel = _openPanels.Pop();
            panel.SetActive(false);
        }
    }

    public int OpenPanelCount => _openPanels.Count;
}