using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SignUpPanel : UIBInder
{
    private string _email;

    private string _password;

    private StringBuilder _sb = new StringBuilder();

    //ButtonSound
    [SerializeField] private AudioClip _buttonClip;

    private void Awake()
    {
        BindAll();
    }
    void Start()
    {
        GetUI<Button>("CreateButton").onClick.AddListener(CreateAccount);
        GetUI<Button>("SignUpExitButton").onClick.AddListener(ResetInputField);

        GetUI<Button>("SignUpExitButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("CreateButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("CreateWarningExitButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
    }


    private void CreateAccount()
    {
        Debug.Log("CreateAccount!!!");
        _email = GetUI<TMP_InputField>("SignUpEmailInputField").text;

        _password = GetUI<TMP_InputField>("SignUpPwInputField").text;

        BackendManager.Auth.CreateUserWithEmailAndPasswordAsync(_email, _password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
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
                    Debug.Log(errorCode.ToString());

                    switch (errorCode)
                    {
                        case AuthError.EmailAlreadyInUse:
                            SetTrueWarningPanel("이미 존재하는 계정입니다");
                            break;
                        case AuthError.InvalidEmail:
                            SetTrueWarningPanel("유효하지 않는 이메일입니다");
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
                        default:
                            SetTrueWarningPanel("Unknown Error. Try Again.");
                            break;
                    }
                }
                ResetInputField();
                return;
            }

            ResetInputField();
            gameObject.SetActive(false);
        });
    }


    private void SetTrueWarningPanel(string textName)
    {
        GetUI<Image>("CreateWarningPanel").gameObject.SetActive(true);
        _sb.Clear();
        _sb.Append(textName);
        GetUI<TextMeshProUGUI>("CreateWarningText").SetText(_sb);
    }

    private void ResetInputField()
    {
        GetUI<TMP_InputField>("SignUpEmailInputField").text = "";
        GetUI<TMP_InputField>("SignUpPwInputField").text = "";
    }
}
