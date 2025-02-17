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

    //�ȷο� ���� �ð�
    [SerializeField] private int _resetFollowTime;

    //�ȷο� origin ��
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
        Debug.Log("�α��� ����");

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
                            SetTrueWarningPanel("��ȿ�� �̸��� ������ �ƴմϴ�");
                            break;
                        case AuthError.UserNotFound:
                            SetTrueWarningPanel("�������� �ʴ� �̸����Դϴ�");
                            break;
                        case AuthError.MissingEmail:
                            SetTrueWarningPanel("�̸����� �ۼ����ּ���");
                            break;
                        case AuthError.MissingPassword:
                            SetTrueWarningPanel("��й�ȣ�� �ۼ����ּ���");
                            break;
                        case AuthError.WeakPassword:
                            SetTrueWarningPanel("��й�ȣ�� ����� �ۼ����ּ���");
                            break;
                        case AuthError.WrongPassword:
                            SetTrueWarningPanel("�߸��� ��й�ȣ �Դϴ�");
                            break;
                        default:
                            SetTrueWarningPanel("Unknown Error. Try Again.");
                            break;
                    }
                }
                else
                {
                    Debug.LogError("FirebaseException�� �ƴ� �ٸ� ���ܰ� �߻��߽��ϴ�: " + exception?.ToString());
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
        Debug.Log("�α���!!!!");
        FirebaseUser user = BackendManager.Instance.Auth.CurrentUser;
        if (user == null)
        {
            return;
        }

        //// ù �α��� �� �̸��� ���� �� �г��� ���� ����
        //if (user.IsEmailVerified == false)
        //{
        //    //TODO : �̸��� ���� ����
        //    GetUI<VerifyPanel>("VerifyPanel").namePanel = GetUI("NamePanel").gameObject;
        //    GetUI<VerifyPanel>("VerifyPanel").gameObject.SetActive(true);

        //}
        if (user.DisplayName == "")  // �������ϰ� �̸� �������� ��쵵 ���.
        {
            //TODO : �г��� ���� ����
            GetUI("NamePanel").SetActive(true);
        }
        else
        {
            //TODO :  �κ������ �񵿱� �� ��ȯ 
            _sceneChanger.ChangeScene("Lobby_OJH");

            //ToDo: DB���� PlayerData �ҷ����� 
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