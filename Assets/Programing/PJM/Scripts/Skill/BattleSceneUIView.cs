using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class BattleSceneUIView : UIBInder
{
    [SerializeField] private Button autoButton;
    [SerializeField] private TMP_Text autoText;

    public bool isAutoOn;
    // Start is called before the first frame update
    void Start()
    {
        BindAll();
        autoButton = GetUI<Button>("AutoButton");
        autoText = GetUI<TMP_Text>("AutoText");
        autoButton.onClick.AddListener(ToggleAuto);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleAuto()
    {
        isAutoOn = !isAutoOn;
        autoText.text = $"Auto : {isAutoOn}";
        Debug.Log($" 현재 오토 상태 : {isAutoOn}");
    }
}
