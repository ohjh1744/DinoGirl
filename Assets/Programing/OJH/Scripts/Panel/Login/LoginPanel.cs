using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class LoginPanel : UIBInder
{
    private string _email;

    private string _password;

    private StringBuilder _sb = new StringBuilder();

    [SerializeField] private SceneChanger _sceneChanger;

    //팔로우 리셋 시간
    [SerializeField] private int _resetFollowTime;

    //팔로우 origin 값
    [SerializeField] private int _originFollowTime;

    //ButtonSound
    [SerializeField] private AudioClip _buttonClip;

    //Bgm
    [SerializeField] private AudioClip _bgmClip;

    private void Awake()
    {
        BindAll();
    }
    void Start()
    {
        _sceneChanger.CanChangeSceen = false;
        GetUI<Button>("LoginButton").onClick.AddListener(Login);
        GetUI<Button>("SignUpButton").onClick.AddListener(ResetInputField);
        GetUI<Button>("LoginExitButton").onClick.AddListener(_sceneChanger.QuitGame);

        //Sound
        SoundManager.Instance.SetLoopBGM(true);
        SoundManager.Instance.PlayeBGM(_bgmClip);
        GetUI<Button>("LoginButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("SignUpButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("LoginExitButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("LoginWarningExitButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
    }

    private void Login()
    {
        Debug.Log("로그인 성공");

        _email = GetUI<TMP_InputField>("LoginEmailInputField").text;

        _password = GetUI<TMP_InputField>("LoginPwInputField").text;

        BackendManager.Instance.Auth.SignInWithEmailAndPasswordAsync(_email, _password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                ResetInputField();
                return;
            }

            if (task.IsFaulted)
            {
                Exception exception = task.Exception.InnerException;

                FirebaseException firebaseException = exception as FirebaseException;

                if (firebaseException != null)
                {
                    AuthError errorCode = (AuthError)firebaseException.ErrorCode;
                    Debug.Log(errorCode);

                    switch (errorCode)
                    {
                        case AuthError.InvalidEmail:
                            SetTrueWarningPanel("유효한 이메일 형식이 아닙니다");
                            break;
                        case AuthError.UserNotFound:
                            SetTrueWarningPanel("존재하지 않는 이메일입니다");
                            break;
                        case AuthError.MissingEmail:
                            SetTrueWarningPanel("이메일을 작성해주세요");
                            break;
                        case AuthError.MissingPassword:
                            SetTrueWarningPanel("비밀번호를 작성해주세요");
                            break;
                        case AuthError.WeakPassword:
                            SetTrueWarningPanel("비밀번호를 제대로 작성해주세요");
                            break;
                        case AuthError.WrongPassword:
                            SetTrueWarningPanel("잘못된 비밀번호 입니다");
                            break;
                        default:
                            SetTrueWarningPanel("Unknown Error. Try Again.");
                            break;
                    }
                }
                else
                {
                    Debug.LogError("FirebaseException이 아닌 다른 예외가 발생했습니다: " + exception?.ToString());
                }

                ResetInputField();
                return;
            }

            AuthResult result = task.Result;
            Debug.Log($"User signed in successfully: {result.User.DisplayName} ({result.User.UserId})");
            CheckUserInfo();
            ResetInputField();
        });
    }

    private void CheckUserInfo()
    {
        Debug.Log("로그인!!!!");
        FirebaseUser user = BackendManager.Instance.Auth.CurrentUser;
        if (user == null)
        {
            return;
        }

        //// 첫 로그인 시 이메일 인증 및 닉네임 설정 진행
        //if (user.IsEmailVerified == false)
        //{
        //    //TODO : 이메일 인증 진행
        //    GetUI<VerifyPanel>("VerifyPanel").namePanel = GetUI("NamePanel").gameObject;
        //    GetUI<VerifyPanel>("VerifyPanel").gameObject.SetActive(true);

        //}
        if (user.DisplayName == "")  // 인증만하고 이름 설정안한 경우도 고려.
        {
            //TODO : 닉네임 설정 진행
            GetUI("NamePanel").SetActive(true);
        }
        else
        {
            //TODO :  로비씬으로 비동기 씬 전환 
            _sceneChanger.ChangeScene("Lobby_OJH");

            //ToDo: DB에서 PlayerData 불러오기 
            GetPlayerData();

        }
    }

    private void GetPlayerData()
    {
        FirebaseUser user = BackendManager.Instance.Auth.CurrentUser;

        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("UserData").Child(user.UserId);

        Debug.Log(root);

        root.KeepSynced(true);

        root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapShot = task.Result;

            string json = snapShot.GetRawJsonValue();
            Debug.Log(json);
            PlayerDataManager.Instance.PlayerData = JsonUtility.FromJson<PlayerData>(json);

            _sceneChanger.CanChangeSceen = true;
        });
    }

    private void SetTrueWarningPanel(string textName)
    {
        GetUI<Image>("LoginWarningPanel").gameObject.SetActive(true);
        _sb.Clear();
        _sb.Append(textName);
        GetUI<TextMeshProUGUI>("LoginWarningText").SetText(_sb);
    }

    private void ResetInputField()
    {
        GetUI<TMP_InputField>("LoginEmailInputField").text = "";
        GetUI<TMP_InputField>("LoginPwInputField").text = "";
    }
}