using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class SkillUIView : UIBInder
{
   
    private float _globalCooldownTimer;

    [SerializeField] private GameObject _skillSlotPrefab;
    public GameObject SkillSlotPrefab { get => _skillSlotPrefab; set => _skillSlotPrefab = value; }
    private int _maxSkillUINum = 10;
    private List<SkillSlot> _skillSlots;
    public List<SkillSlot> SkillSlots { get => _skillSlots; set => _skillSlots = value; }

    private void Awake()
    {
        //Bind();
    }

    private void OnEnable()
    {
        Spawner.OnSpawnCompleted += InitializeSkillSlots;
    }

    private void Start()
    {
    }

    private void Update()
    {
        if(BattleSceneManager.Instance.isGamePaused)
            return;

        if(BattleSceneManager.Instance.curBattleState == BattleSceneManager.BattleState.Battle)
            UpdateCooldown();
    }

    private void OnDisable()
    {
        Spawner.OnSpawnCompleted -= InitializeSkillSlots;
    }


    private void InitializeSkillSlots()
    {
        if (BattleSceneManager.Instance == null)
        {
            Debug.LogError("TempBattleContext 인스턴스가 없다.");
            return;
        }
        if (BattleSceneManager.Instance.myUnits.Count <= 0)
        {
            Debug.LogWarning("플레이어가 배틀씬에 없다.");
            return;
        }
        
        SkillSlots = new List<SkillSlot>();
        int slotIndex = 0;

        foreach (var player in BattleSceneManager.Instance.myUnits)
        {
            if(player == null || !player.gameObject.activeSelf)
                continue;
            
            if (player.UniqueSkill == null)
            {
                Debug.Log($"{player.name}에 스킬이 없습니다.");
                continue;
            }

            if (slotIndex >= _maxSkillUINum)
                break;
            
            GameObject slot = Instantiate(SkillSlotPrefab, transform);
            //slot.name = $"Skill{slotIndex}";
            SkillSlot skillSlot = slot.GetComponent<SkillSlot>();
            
            //GameObject skillRoot = GetUI($"Skill{slotIndex}");
            if (skillSlot == null)
            {
                Debug.Log($"Skill{slotIndex} UI 찾을 수 없음");
                break;
            }
            
            // 스킬 슬롯에 UI 할당
            // 스킬 데이터 할당
            skillSlot.SkillOwner = player;
            skillSlot.SkillData = skillSlot.SkillOwner.UniqueSkill;
            skillSlot.SkillIcon.sprite = skillSlot.SkillData.SkillIcon;
            
            var index = slotIndex;
            skillSlot.SkillButton.onClick.AddListener(() => OnSkillButtonTouched(index));
            
            // 각 플레이어가 죽었을 때 버튼에도 처리가 필요함 ex) 비활성화
            player.UnitModel.OnDeath += skillSlot.HandleDeath;
            SkillSlots.Add(skillSlot);
            slotIndex++;
            
        }
    }

    private void OnSkillButtonTouched(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SkillSlots.Count)
        {
            Debug.Log("스킬 슬롯 리스트에 없는 스킬슬롯 입니다.");
        }
        
        // 못누르게 해놨지만 혹시 모르니 추가
        SkillSlot slot = SkillSlots[slotIndex];
        if (slot.SkillOwner.CoolTimeCounter > 0f)
        {
            Debug.Log($"{slotIndex} 스킬이 쿨타임중입니다.");
            return;
        }
        
        slot.SkillOwner.SkillInputed = true;
    }

    private void HandleDestroyed(int index)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 모든 스킬 쿨타임 갱신
    /// </summary>
    private void UpdateCooldown()
    {
        foreach (SkillSlot slot in SkillSlots)
        {
            if(slot == null || slot.SkillOwner == null)
                Debug.LogWarning("dddddddd");
            
            float remainingTime = slot.SkillOwner.CoolTimeCounter;
            float totalTime = slot.SkillData != null ? slot.SkillData.Cooltime : 0f;

            slot.CooldownText.text = remainingTime.ToString("0.0");
            
            float ratio = (totalTime > 0f) ? (remainingTime / totalTime) : 0f;
            slot.HideImage.fillAmount = ratio;
            
            bool isCooling = remainingTime > 0f;
            slot.HideImage.gameObject.SetActive(isCooling);
        }
    }
    public void HideSkillSetting(int slotIndex)
    {
        /*// 잘못된 범위일경우 return
        if (slotIndex < 0 || slotIndex >= SkillSlots.Count)
        {
            Debug.Log("잘못된 스킬 슬롯 인덱스");
            return;
        }
        SkillSlots[slotIndex].hideImage.gameObject.SetActive(true);

        SkillSlots[slotIndex].remainingTime = SkillSlots[slotIndex].skillTime;
        SkillSlots[slotIndex].isCooling = true;*/
    }

}

