using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot
{
    public GameObject skillRoot;
    public Button skillButton;
    public Image hideImage;
    public TextMeshProUGUI cooldownText;
    public Sprite skillIcon;

    /*public float skillTime; // 스킬의 쿨타임
    public float remainingTime; // 남은시간
    public bool isCooling; */
    
    public PlayableBaseUnitController skillOwner;
    public Skill skillData;

    public void HandleDeath()
    {
       skillRoot.SetActive(false);
    }

}
