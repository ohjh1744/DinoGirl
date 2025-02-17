using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class VerifyPanel : MonoBehaviour
{
    private GameObject _namePanel;

    public GameObject namePanel { get { return _namePanel; } set { _namePanel = value; } }

    private Coroutine checkVerifyRoutine;

    [SerializeField] private float _checkVerifyTime;

    WaitForSeconds _checkVerifySeconds;

    private void OnEnable()
    {
        _checkVerifySeconds = new WaitForSeconds(_checkVerifyTime);
        SendVerifyMail();
    }

    private void OnDisable()
    {
        if (checkVerifyRoutine != null)
        {
            StopCoroutine(checkVerifyRoutine);
        }
    }

    //�̸��� ����
    private void SendVerifyMail()
    {
        FirebaseUser user = BackendManager.Instance.Auth.CurrentUser;
        user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendEmailVerificationAsync was canceled.");
                gameObject.SetActive(false);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                gameObject.SetActive(false);
                return;
            }

            Debug.Log("Email sent successfully.");
            checkVerifyRoutine = StartCoroutine(CheckVerifyRoutine());
        });
    }

    IEnumerator CheckVerifyRoutine()
    {
        while (true)
        {
            BackendManager.Instance.Auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("ReloadAsync was canceled");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"ReloadAsync encountered an error: {task.Exception.Message}");
                    return;
                }

                //���ε��ؾ� ������ ��.
                if (BackendManager.Instance.Auth.CurrentUser.IsEmailVerified == true)
                {
                    Debug.Log("���� Ȯ��aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                    // ����Panel�� �Ѿ�Դٴ� ���� ù �α����̶�� ���̹Ƿ�, �̸� �������� �ڿ������� �Ѿ��
                    _namePanel.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log("���� �ȵ�aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                }

            });

            //���� Ȯ�� 
            yield return _checkVerifySeconds;
        }
    }


}
