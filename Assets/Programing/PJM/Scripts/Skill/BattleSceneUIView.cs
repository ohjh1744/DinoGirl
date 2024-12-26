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
    
    public bool isAutoOn; // 임시 자동체크 변수
    
    [SerializeField] private float[] _skillTimes = {3.0f, 6.0f}; // 임시 배치
    private int _maxSkillSlotNum = 2;
    private List<SkillSlot> _skillSlots = new List<SkillSlot>();
    public static event Action<int> OnSkillUsed;

    private void Awake()
    {
        Bind();
        InitializeSkillSlots();
    }

    private void Update()
    {
        UpdateCooldown();
    }

    private void InitializeSkillSlots()
    {
        for (int i = 1; i <= _maxSkillSlotNum; i++)
        {
            GameObject skillRoot = GetUI($"Skill{i}");
            if(skillRoot == null)
                break;

            // 스킬 슬롯에 UI 할당
            SkillSlot slot = new SkillSlot();
            slot.skillRoot = skillRoot;
            slot.skillButton = GetUI<Button>($"Skill{i}Button");
            slot.hideImage = GetUI<Image>($"HideImage{i}");
            slot.cooldownText = GetUI<TextMeshProUGUI>($"CooldownText{i}");

            int index = i - 1;
            if (index < _skillTimes.Length)
            {
                slot.skillTime = _skillTimes[index];
            }
            else
            {
                Debug.Log("skillTimes 배열을 벗어남");
            }
            
            // 전투 시작시 전체적으로 n초 동안은 스킬 사용 불가 // 임시
            slot.remainingTime = 2.0f;
            slot.isCooling = true;
            
            AddEvent($"Skill{i}Button",EventType.Click,(PointerEventData data) =>  OnSkillButtonTouched(index));
            
            _skillSlots.Add(slot);

        }
    }

    private void OnSkillButtonTouched(int skillIndex)
    {
        Debug.Log($"스킬버튼 {skillIndex} 터치됨");
        HideSkillSetting(skillIndex);
    }

    public void HideSkillSetting(int skillIndex)
    {
        // 잘못된 범위일경우 return
        if (skillIndex < 0 || skillIndex >= _skillSlots.Count)
        {
            Debug.Log("잘못된 스킬 인덱스");
            return;
        }
        _skillSlots[skillIndex].hideImage.gameObject.SetActive(true);
        //_skillSlots[skillIndex].skillRoot.SetActive(true);

        _skillSlots[skillIndex].remainingTime = _skillSlots[skillIndex].skillTime;
        _skillSlots[skillIndex].isCooling = true;
    }

    /// <summary>
    /// 모든 스킬 쿨타임 갱신
    /// </summary>
    private void UpdateCooldown()
    {
        foreach (SkillSlot slot in _skillSlots)
        {
            if(!slot.isCooling) // 쿨다운 중이 아닐경우는 넘김
                continue;
            
            slot.remainingTime -= Time.deltaTime;
            if (slot.remainingTime < 0)
            {
                slot.remainingTime = 0;
                slot.isCooling = false;
                slot.hideImage.gameObject.SetActive(false);
            }
            
            slot.cooldownText.text = slot.remainingTime.ToString("0");
            float ratio = slot.skillTime > 0 ? (slot.remainingTime / slot.skillTime) : 0;
            slot.hideImage.fillAmount = ratio;
        }
    }
    
}

