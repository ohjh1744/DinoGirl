using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : UIBInder
{
    [SerializeField] private SceneChanger _sceneChanger;

    //ButtonSound
    [SerializeField] private AudioClip _buttonClip;


    private void Awake()
    {
        BindAll();
    }

    private void OnEnable()
    {
        GetUI<Button>("SettingLogOutButton").onClick.AddListener(DoLogOut);
        GetUI<Button>("SettingGameExitButton").onClick.AddListener(ExitGame);

        //Sound
        GetUI<Button>("SettingLogOutButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("SettingGameExitButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("SettingExitButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
    }

    private void OnDisable()
    {
        GetUI<Button>("SettingLogOutButton").onClick.RemoveListener(DoLogOut);
        GetUI<Button>("SettingGameExitButton").onClick.RemoveListener(ExitGame);

        GetUI<Button>("SettingLogOutButton").onClick.RemoveListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("SettingGameExitButton").onClick.RemoveListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("SettingExitButton").onClick.RemoveListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
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
