using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : UIBInder
{
    [SerializeField] private SceneChanger _sceneChanger;

    private void Awake()
    {
        BindAll();
    }

    private void OnEnable()
    {
        GetUI<Button>("SettingLogOutButton").onClick.AddListener(DoLogOut);
        GetUI<Button>("SettingGameExitButton").onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        GetUI<Button>("SettingLogOutButton").onClick.RemoveListener(DoLogOut);
        GetUI<Button>("SettingGameExitButton").onClick.RemoveListener(ExitGame);
    }

    private void DoLogOut()
    {
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("Login_OJH");
    }

    private void ExitGame()
    {
        _sceneChanger.QuitGame();
    }

    public void Test(string s)
    {

    }
    
    
}
