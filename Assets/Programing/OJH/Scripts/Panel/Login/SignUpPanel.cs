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

    private void Awake()
    {
        BindAll();
    }
    void Start()
    {
        GetUI<Button>("CreateButton").onClick.AddListener(CreateAccount);
        GetUI<Button>("SignUpExitButton").onClick.AddListener(ResetInputField);
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
                            SetTrueWarningPanel("EmailAlreadyInUse");
                            break;
                        case AuthError.InvalidEmail:
                            SetTrueWarningPanel("InvalidEmail");
                            break;
                        case AuthError.MissingEmail:
                            SetTrueWarningPanel("MissingEmail");
                            break;
                        case AuthError.MissingPassword:
                            SetTrueWarningPanel("MissingPassword");
                            break;
                        case AuthError.WeakPassword:
                            SetTrueWarningPanel("WeakPassword");
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
        GetUI<TextMeshProUGUI>("WarningText").SetText(_sb);
    }

    private void ResetInputField()
    {
        GetUI<TMP_InputField>("SignUpEmailInputField").text = "";
        GetUI<TMP_InputField>("SignUpPwInputField").text = "";
    }
}
