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
    // 글로벌 쿨타임 // 현재 임시
    [SerializeField] private float _globalCooldown = 0.5f;
    private float _globalCooldownTimer;
    //public bool isAutoOn; // 임시 자동체크 변수, 임시 배틀매니저에 들어가야함

    [SerializeField] private GameObject _skillSlotPrefab;
    public GameObject SkillSlotPrefab { get => _skillSlotPrefab; set => _skillSlotPrefab = value; }
    
    
    //[SerializeField] private float[] _skillTimes = {3.0f, 6.0f}; // 임시 배치
    private int _maxSkillUINum = 10;
    private List<SkillSlot> _skillSlots;
    public List<SkillSlot> SkillSlots { get => _skillSlots; set => _skillSlots = value; }
    //public static event Action<int> OnSkillUsed;

    private void Awake()
    {
        //Bind();
    }

    private void Start()
    {
        InitializeSkillSlots();
        
    }

    private void Update()
    {
        if(TempBattleContext.Instance.isGamePaused)
            return;
        
        UpdateCooldown();
        //Debug.Log($"{TempBattleContext.Instance.players[0].CoolTimeCounter}");
        //Debug.Log($"{TempBattleContext.Instance.players[0].CoolTimeCounter} | {SkillSlots[0].remainingTime}");
    }



    private void InitializeSkillSlots()
    {
        
        if (TempBattleContext.Instance == null)
        {
            Debug.LogError("TempBattleContext 인스턴스가 없다.");
            return;
        }
        if (TempBattleContext.Instance.players.Count <= 0)
        {
            Debug.LogWarning("플레이어가 배틀씬에 없다.");
            return;
        }

        
        SkillSlots = new List<SkillSlot>();
        int slotIndex = 0;

        foreach (var player in TempBattleContext.Instance.players)
        {
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
            //SkillSlot slot = new SkillSlot();
            /*skillSlot.skillRoot = skillRoot;
            skillSlot.skillButton = GetComponent<Button>();//GetUI<Button>($"Skill{slotIndex}Button");
            skillSlot.hideImage = GetUI<Image>($"HideImage{slotIndex}");
            skillSlot.cooldownText = GetUI<TextMeshProUGUI>($"CooldownText{slotIndex}");*/

            //skillSlot.SkillRoot = GetUI($"Skill{slotIndex}");
            // 스킬 데이터 할당
            skillSlot.SkillOwner = player;
            skillSlot.SkillData = skillSlot.SkillOwner.UniqueSkill;
            skillSlot.SkillIcon.sprite = skillSlot.SkillData.SkillIcon;
            
            //skillSlot.SkillIcon = player.UniqueSkill.SkillIcon;
            /*slot.skillTime = player.UniqueSkill.Cooltime;
            slot.remainingTime = player.CoolTimeCounter;
            slot.isCooling = false;*/
            //skillSlot.SkillIcon = player.UniqueSkill.SkillIcon;
            //Image iconImage = GetUI<Image>($"Skill{slotIndex}Icon");
            
            //if(iconImage != null)
            
            
            /*// 전투 시작시 전체적으로 n초 동안은 스킬 사용 불가 // 임시
            slot.remainingTime = 2.0f;
            slot.isCooling = true;*/

            var index = slotIndex;
            skillSlot.SkillButton.onClick.AddListener(() => OnSkillButtonTouched(index));
            
            //AddEvent($"Skill{slotIndex}Button",EventType.Click, _ => OnSkillButtonTouched(index));
            
            // 각 플레이어가 죽었을 때 버튼에도 처리가 필요함 ex) 비활성화
            player.UnitModel.OnDeath += skillSlot.HandleDeath;
            //player.UnitModel.OnDeath += HandleDestroyed(slotIndex);
            
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
        
        Debug.Log($"스킬버튼 {slotIndex} 터치됨");
        slot.SkillOwner.SkillInputed = true;
        //HideSkillSetting(slotIndex);
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
            float remainingTime = slot.SkillOwner.CoolTimeCounter;
            float totalTime = slot.SkillData != null ? slot.SkillData.Cooltime : 0f;

            /*if (slot.skillData != null)
            {
                totalTime = slot.skillData.Cooltime;
            }
            else
            {
                totalTime = 0f;
            }*/
            
            slot.CooldownText.text = remainingTime.ToString("0.0");
            
            float ratio = (totalTime > 0f) ? (remainingTime / totalTime) : 0f;
            slot.HideImage.fillAmount = ratio;
            
            bool isCooling = remainingTime > 0f;
            slot.HideImage.gameObject.SetActive(isCooling);
            
            
            
            /*if(!slot.isCooling) // 쿨다운 중이 아닐경우는 넘김
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
            slot.hideImage.fillAmount = ratio;*/
        }
    }

    /*private void CheckAuto()
    {
        if (TempBattleContext.Instance.isAutoOn)
        {
            for (int i = 0; i < _maxSkillUINum; i++)
            {
                // 이렇게 하면안됨.. 쿨타임 무시하고 스킬을 계속 쓰게 될거라
                OnSkillButtonTouched(i);
            }
                
        }
    }*/
    
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

