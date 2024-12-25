using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class BattleSceneUIView : UIBInder
{
    private Image[] _hideSkillButtonImages;
    private TextMeshProUGUI[] _hideSkillTexts;
    private Image[] _hideSkillImages;
    private bool[] _isHideSkills;
    private float[] _skillTimes;
    private float[] _getSkillTimes;

    public static event Action<int> OnSkillUsed;
    
    
    private Button _autoButton;
    private TMP_Text _autoText;
    
    

    public bool isAutoOn;

    private void Awake()
    {
        BindAll();
        _hideSkillButtonImages[0] = GetUI<Image>("HideImage1");
        _hideSkillButtonImages[1] = GetUI<Image>("HideImage2");
        
        _hideSkillTexts[0] = GetUI<TextMeshProUGUI>("CooldownText1");
        _hideSkillTexts[1] = GetUI<TextMeshProUGUI>("CooldownText2");
        
        _hideSkillImages[0] = GetUI<Image>("HideImage1");
        _hideSkillImages[1] = GetUI<Image>("HideImage2");
        
        AddEvent("HideImage1",EventType.Click, (PointerEventData data) => OnSkillButtonTouched(0));
        AddEvent("HideImage1",EventType.Click, (PointerEventData data) => OnSkillButtonTouched(1));

    }

    
    void Start()
    {
        
        _autoButton = GetUI<Button>("AutoButton");
        _autoText = GetUI<TMP_Text>("AutoText");
        _autoButton.onClick.AddListener(ToggleAuto);
    }

    void Update()
    {
        
    }
    
    
    private void OnSkillButtonTouched(int skillIndex)
    {
        Debug.Log($"{skillIndex} 번째 스킬버튼 클릭됨");
    }

    // 스킬 인덱스 스킬 쿨타임 UI 활성화
    public void HideSkillSetting(int skillIndex)
    {
        _hideSkillImages[skillIndex].gameObject.SetActive(true);
        _getSkillTimes[skillIndex] = _skillTimes[skillIndex];
        _isHideSkills[skillIndex] = true;
    }

    public void HideSkillCheck()
    {
        
    }

    public void ToggleAuto()
    {
        isAutoOn = !isAutoOn;
        _autoText.text = $"Auto : {isAutoOn}";
        Debug.Log($" 현재 오토 상태 : {isAutoOn}");
    }
}
