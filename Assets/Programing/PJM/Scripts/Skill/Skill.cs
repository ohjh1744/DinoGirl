using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class Skill : ScriptableObject
{
    public enum SkillState
    {
        None,
        Charging,   // 준비 동작
        Firing,     // 실제 데미지를 주는 중
        Done        // 종료
    }
    
    [SerializeField] private string _skillName;
    public string SkillName {get => _skillName; set => _skillName = value; }
    [SerializeField] private bool _targetAll;
    public bool TargetAll {get => _targetAll; protected set => _targetAll = value; }
    // TargetAll이 true면 아래 skillrange와 maxTargetingNum은 숨기고싶다
    
    [SerializeField] private float _skillRangeRadius;
    public float SkillRangeRadius {get => _skillRangeRadius; protected set => _skillRangeRadius = value; }


    [SerializeField] private int _maxTargetingNum;
    public int MaxTargetingNum {get => _maxTargetingNum; protected set => _maxTargetingNum = value; }

    [SerializeField] private bool _isPriorityTargetFar;
    public bool IsPriorityTargetFar {get => _isPriorityTargetFar; protected set => _isPriorityTargetFar = value; }
    
    [SerializeField] private float skillRatio;
    public float SkillRatio {get => skillRatio; protected set => skillRatio = value; }
    [SerializeField] private float cooltime;
    public float Cooltime {get => cooltime; protected set => cooltime = value; }
    [SerializeField] private CrowdControls crowdControl = CrowdControls.None;
    protected CrowdControls CrowdControl {get => crowdControl;}
    [SerializeField] float ccDuration;

    protected float CcDuration
    {
        get
        {
            if (crowdControl == CrowdControls.None)
                return 0f;
            return ccDuration;
        }
    }
    
    [SerializeField] private Sprite _skillIcon;
    public Sprite SkillIcon {get => _skillIcon;set => _skillIcon = value; }
    
    [SerializeField] private GameObject _vfxToTarget;
    public GameObject VFXToTarget {get => _vfxToTarget;}
    
    [SerializeField] private GameObject _vfxToMine;
    public GameObject VFXToMine {get => _vfxToMine;}
    
    [SerializeField] private GameObject _vfxToMuzzle;
    public GameObject VFXToMuzzle {get => _vfxToMuzzle;}
    
    [SerializeField] private AudioClip _skillStartSound;
    public AudioClip SkillStartSound {get => _skillStartSound;}
    
    //[SerializeField] private AudioClip _skillOngoingSound;
    //public AudioClip SkillOngoingSound {get => _skillOngoingSound;}
    
    [SerializeField] private AudioClip _skillEndSound;
    public AudioClip SkillEndSound {get => _skillEndSound;}
    

    [SerializeField] protected SkillParameter skillParameterNumber;
    public SkillParameter SkillParameterNumber {get => skillParameterNumber;}

    // 타겟 설정
    public abstract BaseNode.ENodeState SetTargets(BaseUnitController caster, List<BaseUnitController> targets);

    // 스킬 실행
    public abstract BaseNode.ENodeState Perform(BaseUnitController caster, List<BaseUnitController> targets);
    public SequenceNode CreateSkillBTree(BaseUnitController caster,List<BaseUnitController> targets)
    {
        return new SequenceNode
        (
            new List<BaseNode>()
            {
                //new ConditionNode(() => CheckRange(caster)),
                new ActionNode(() => SetTargets(caster,targets)), 
                new ActionNode(() => Perform(caster, targets))
            }
        );
    }
    
    /*public SequenceNode CreateSkillBTree(BaseUnitController caster,List<BaseUnitController> targets, bool needRemainTimeChecker)
    {
        switch (needRemainTimeChecker)
        {
            case true:
                return new SequenceNode
                (
                    new List<BaseNode>()
                    {
                        new ConditionNode(CheckRemainingTime),
                        new ActionNode(() => SetTargets(caster,targets)), 
                        new ActionNode(() => Perform(caster, targets))
                    }
                );
            case false:
                return new SequenceNode
                (
                    new List<BaseNode>()
                    {
                        new ActionNode(() => SetTargets(caster,targets)), 
                        new ActionNode(() => Perform(caster, targets))
                    }
                );
        }
    }*/

    public BaseNode CreatePerformNode(BaseUnitController caster,List<BaseUnitController> targets)
    {
        return new ActionNode(() => Perform(caster, targets));
    }

    protected void SpawnAllVFXs(BaseUnitController caster, BaseUnitController target)
    {
        if(_vfxToMine != null)
            SpawnVFX(caster.transform, caster.transform, VFXToMine);
        if(_vfxToMuzzle != null)
            SpawnVFX(caster.transform, caster.MuzzlePoint, VFXToMuzzle);
        if(_vfxToTarget != null)
            SpawnVFX(caster.transform, target.CenterPosition, VFXToTarget);
    }
    protected void SpawnVFX(Transform casterPos ,Transform targetTransform, GameObject effectPrefab)
    {
        if(effectPrefab == null || casterPos == null)
            return;
        
        
        GameObject particleObject = Instantiate(effectPrefab, targetTransform.position, Quaternion.identity);
        
        if (particleObject == null)
            return;
        // Todo : 
        // localScale 방향 조정 필요
        Vector3 newScale = new Vector3(particleObject.transform.localScale.x * Mathf.Sign(casterPos.localScale.x), particleObject.transform.localScale.y, particleObject.transform.localScale.z);
        particleObject.transform.localScale = newScale;
        var particle = particleObject.GetComponentInChildren<ParticleSystem>();
        if (particle != null)
        {
            Destroy(particleObject, particle.main.duration + particle.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(particleObject);
        }
        
        /*if (particleObject.TryGetComponent<ParticleSystem>(out var particleSystem))
        {
            Destroy(particleObject, particleSystem.main.duration + particleSystem.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(particleObject);
        }*/
    }
    
    protected bool GetBoolSkillParameter(BaseUnitController caster)
    {
        int skillParameterHash = caster.UnitViewer.SkillParameterHash[(int)SkillParameterNumber];
        return caster.UnitViewer.UnitAnimator.GetBool(skillParameterHash);
    }

    protected void SetBoolSkillParameter(BaseUnitController caster,bool value)
    {
        int skillParameterHash = caster.UnitViewer.SkillParameterHash[(int)SkillParameterNumber];
        caster.UnitViewer.UnitAnimator.SetBool(skillParameterHash, value);
    }

    private bool CheckRemainingTime()
    {
        if (BattleSceneManager.Instance.RemainTime > 60.0f)
        {
            Debug.Log("남은시간 60초 + ");
            return false;
        }
           
        Debug.Log("남은시간 60초 - ");
        return true;
    }

    protected void PlaySkillSfx(AudioClip soundClip)
    {
        if(soundClip == null)
            return;
        SoundManager.Instance.PlaySFX(soundClip);
    }

    protected void ResetTargets(List<BaseUnitController> targets)
    {
        targets.Clear();
    }
}

