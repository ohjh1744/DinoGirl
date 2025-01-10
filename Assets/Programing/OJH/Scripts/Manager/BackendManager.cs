using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BackendManager : MonoBehaviour
{
    public static BackendManager Instance { get; private set; }

    private FirebaseApp app;

    public static FirebaseApp App { get { return Instance.app; } }

    private FirebaseAuth auth;
    public static FirebaseAuth Auth { get { return Instance.auth; } }

    private FirebaseDatabase database;

    public static FirebaseDatabase Database { get { return Instance.database; } }

    private void Awake()
    {
        CreateSingleton();
    }

    private void Start()
    {
        CheckDependency();
    }

    private void CreateSingleton()
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

    //호환성 여부 체크
    private void CheckDependency()
    {
        // checkandfixDependenciesasync가 요청, continuewithonmainTHread가 반응.
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            //결과가 사용가능하면
            if (task.Result == DependencyStatus.Available)
            {

                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance;
                database.SetPersistenceEnabled(false);
                Debug.Log("Firebase dependencies check success");

            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");

                app = null;
                auth = null;
                database = null;
            }
        });
    }

}