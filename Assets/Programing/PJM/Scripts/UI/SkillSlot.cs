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
    }

    public void HandleDeath()
    {
       gameObject.SetActive(false);
    }

}
