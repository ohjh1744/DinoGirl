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

    private FirebaseApp _app;

    public FirebaseApp App { get { return Instance._app; } }

    private FirebaseAuth _auth;
    public  FirebaseAuth Auth { get { return Instance._auth; } }

    private FirebaseDatabase _database;

    public  FirebaseDatabase Database { get { return Instance._database; } }

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

                _app = FirebaseApp.DefaultInstance;
                _auth = FirebaseAuth.DefaultInstance;
                _database = FirebaseDatabase.DefaultInstance;
                _database.SetPersistenceEnabled(false);
                Debug.Log("Firebase dependencies check success");

            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");

                _app = null;
                _auth = null;
                _database = null;
            }
        });
    }

}