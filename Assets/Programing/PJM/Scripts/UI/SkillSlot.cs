using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : UIBInder
{
    private GameObject _skillRoot;
    public GameObject SkillRoot { get => _skillRoot; set => _skillRoot = value; }
    private Button _skillButton;
    public Button SkillButton { get => _skillButton; set => _skillButton = value; }
    private Image _hideImage;
    public Image HideImage { get => _hideImage; set => _hideImage = value; }
    private TextMeshProUGUI _cooldownText;
    public TextMeshProUGUI CooldownText { get => _cooldownText; set => _cooldownText = value; }
    private Image _skillIcon;
    public Image SkillIcon { get => _skillIcon; set => _skillIcon = value; }

    /*public float skillTime; // 스킬의 쿨타임
    public float remainingTime; // 남은시간
    public bool isCooling; */

    public PlayableBaseUnitController SkillOwner { get; set; }

    public Skill SkillData { get; set; }

    private void Awake()
    {
        Bind();
        SkillRoot = GetUI("SkillRoot");
        SkillButton = GetUI<Button>("SkillButton");
        HideImage = GetUI<Image>("HideImage");
        CooldownText = GetUI<TextMeshProUGUI>("CooldownText");
        SkillIcon = GetUI<Image>("SkillIcon");
        
        
        /*SpriteRenderer tempRenderer = GetUI<SpriteRenderer>("SkillIcon");
        if (tempRenderer != null)
        {
            SkillIcon = tempRenderer.sprite;
        }
        else
        {
            Debug.LogWarning("스프라이트 없음");
        }*/
            
        //SkillIcon = GetUI<Sprite>("SkillIcon");
        
        /*skillRoot = GameObject.Find("SkillRoot");
        skillButton = GameObject.Find("SkillButton").GetComponent<Button>();
        hideImage = GameObject.Find("HideImage").GetComponent<Image>();
        cooldownText = GameObject.Find("CooldownText").GetComponent<TextMeshProUGUI>();
        skillIcon = GameObject.Find("SkillIcon").GetComponent<Image>().sprite;*/
    }

    public void HandleDeath()
    {
       gameObject.SetActive(false);
    }

}
