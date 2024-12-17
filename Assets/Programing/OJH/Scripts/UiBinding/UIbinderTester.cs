using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIbinderTester : UIBInder
{

    void Awake()
    {
        BindAll();
    }

    private void Start()
    {
        GetUI<Text>("TestText").text = "10";
        AddEvent("TestText", EventType.Click, Click);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetUI<Text>("TestText").text = "100";
        }
    }

    public void Click(PointerEventData eventData)
    {
        Debug.Log("Hiiiiiiiiiii");
    }
}