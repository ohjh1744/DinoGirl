using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private GameObject[] _setFalsePanels;

    [SerializeField] private GameObject _loadingPanel;

    [SerializeField] private Slider _loadingBar;

    [SerializeField] private TextMeshProUGUI _loadingText;

    [SerializeField] private float _loadingTime;


    private Coroutine _loadingRoutine;

    private bool _canChangeSceen;

    public bool CanChangeSceen { get { return _canChangeSceen; } set { _canChangeSceen = value; } }

    private StringBuilder _sb = new StringBuilder();


    private void Awake()
    {
        _canChangeSceen = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1;
        // setfalse해야하는 Panel이 있다면 false해주기.
        foreach(GameObject panel in _setFalsePanels)
        {
            if(panel != null)
            {
                panel.SetActive(false);
            }
        }

        // loading Panel UI 켜주기
        _loadingPanel.SetActive(true);

        if (_loadingRoutine != null)
        {
            return;
        }
        _loadingRoutine = StartCoroutine(LoadingRoutine(sceneName));
    }

    IEnumerator LoadingRoutine(string sceneName)
    {
        AsyncOperation oper = SceneManager.LoadSceneAsync(sceneName);

        oper.allowSceneActivation = false;

        _loadingBar.gameObject.SetActive(true);

        _loadingText.gameObject.SetActive(true);

        while (oper.isDone == false)
        {
            if (oper.progress < 0.9f)
            {
                Debug.Log($"loading = {oper.progress}");
            }
            else
            {
                break;
            }
            yield return null;
        }


        //Fake Loading
        float time = 0f;
        while (time < _loadingTime || _canChangeSceen == false)
        {
            time += Time.deltaTime;
            _loadingBar.value = time / _loadingTime;
            _sb.Clear();
            _sb.Append("Loading ");
            int percent = Mathf.FloorToInt(_loadingBar.value * 100);
            if (percent == 100)
            {
                _sb.Append(99);
            }
            else
            {
                _sb.Append(percent);
            }
            _sb.Append("%");
            _loadingText.SetText(_sb);
            yield return null;
        }

        Debug.Log("loading Success");
        oper.allowSceneActivation = true;
    }


}