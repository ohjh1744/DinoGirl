using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class SkillController : MonoBehaviour
{
    public GameObject[] hideSkillButtons;
    public GameObject[] textPros;
    public TextMeshProUGUI[] hideSkillTimeTexts;
    public Image[] hideSkillImages;
    private bool[] _isHideSkills = {false, false};
    private float[] _skillTimes = {3, 6};
    private float[] _getSkillTime = {0, 0};

    public UnitController tempUnit;

    public static event Action<int> OnSkillUsed;
    
    private void Update()
    {
        HideSkillCheck();
    }

    public void HideSkillSetting(int skillIndex)
    {
        hideSkillButtons[skillIndex].SetActive(true);
        _getSkillTime[skillIndex] = _skillTimes[skillIndex];
        _isHideSkills[skillIndex] = true;
    }

    public void OnSkillButtonTouched()
    {
        tempUnit.UseSkill();
    }

    /*public void OnSkillButtonTouched(int skillIndex)
    {
        OnSkillUsed?.Invoke(skillIndex);
    }*/

    private void HideSkillCheck()
    {
        for(int i = 0; i < hideSkillButtons.Length; i++)
        {
            if (_isHideSkills[i])
                StartCoroutine(SkillTimeCheck(i));
        }
    }
    
    

    IEnumerator SkillTimeCheck(int skillIndex)
    {
        yield return null;
        if (_getSkillTime[skillIndex] > 0)
        {
            _getSkillTime[skillIndex] -= Time.deltaTime;

            if (_getSkillTime[skillIndex] < 0)
            {
                _getSkillTime[skillIndex] = 0;
                _isHideSkills[skillIndex] = false;
                hideSkillButtons[skillIndex].SetActive(false);
            }
            hideSkillTimeTexts[skillIndex].text = _getSkillTime[skillIndex].ToString("0");
            float time = _getSkillTime[skillIndex] / _skillTimes[skillIndex];
            hideSkillImages[skillIndex].fillAmount = time;
        }
    }
}
