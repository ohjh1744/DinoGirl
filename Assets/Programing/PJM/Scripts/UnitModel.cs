using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

[System.Flags]
public enum CrowdControls
{
    None = 0,
    Taunt = 1 << 0,
    Stun = 1 << 1,
}
public class UnitModel : MonoBehaviour
{
    //private Coroutine _crowdControlRoutine;
    private BaseUnitController _ccCaster;
    public BaseUnitController CcCaster { get { return _ccCaster; } private set { _ccCaster = value; } }
    private CrowdControls _curCc;
    public CrowdControls CurCc { get => _curCc; set => _curCc = value; }
    


    public event Action<int> OnHpChanged;
    public event Action OnTaunted;
    public event Action OnStun;
    public event Action OnDeath;
    public event Action<int> OnHealed;
    public event Action<int> OnDamaged; 
    
    [SerializeField] private int _maxHp;
    public int MaxHp {get => _maxHp; set => _maxHp = value; }
    [SerializeField] private int _hp;

    public int Hp
    {
        get => _hp;
        set
        {
            int oldValue = _hp;
            if (_hp != value) //&& oldValue != 0)
            {
                _hp = Mathf.Clamp(value, 0, MaxHp);
                OnHpChanged?.Invoke(_hp);
                if (_hp < oldValue)
                    OnDamaged?.Invoke(oldValue - _hp);
                if (_hp > oldValue)
                    OnHealed?.Invoke(_hp - oldValue);
                if (_hp <= 0)
                    Die();
            }
        }
    }
    
    //private Dictionary<CrowdControls, float> _ccDurations = new Dictionary<CrowdControls, float>();
    
    /*private bool _isTaunted = false;
    public bool IsTaunted { get => _isTaunted; set => _isTaunted = value; }*/

    [SerializeField] private float _coolDownAcc = 1.0f;
    public float CoolDownAcc { get => _coolDownAcc; set => _coolDownAcc = value; }
    
    [SerializeField] private int _attackPoint;
    public int AttackPoint { get => _attackPoint;  set => _attackPoint = value; }
    [SerializeField] private int _defensePoint;
    public int DefensePoint { get => _defensePoint; set => _defensePoint = value; }
    [SerializeField] private float _moveSpeed;
    public float Movespeed { get => _moveSpeed; set => _moveSpeed = value; }
    [SerializeField] private float _attackRange;
    public float AttackRange { get => _attackRange; set => _attackRange = value; }

    [SerializeField] private bool _isPriorityTargetFar;
    public bool IsPriorityTargetFar { get => _isPriorityTargetFar;  set => _isPriorityTargetFar = value; }

    private void Awake()
    {
        //임시
        Debug.Log("HP 초기화 시작");
        Hp = MaxHp;
    }

    private void OnEnable()
    {
        
    }

    private void Start()
    {
       
    }

    public void TakeDamage(int damage)
    {
        if (damage == 0)
            return;
        
        if (Hp <= 0)
        {
            Debug.LogWarning("이미 hp가 0입니다.");
            return;
        }

        int calcDamage = damage - DefensePoint;
        
        if (calcDamage <= 0)
            calcDamage = 1;
        
        Hp -= calcDamage;
        
        //Debug.Log($"데미지 : {damage} 받음.");
    }

    public void TakeCrowdControl(CrowdControls crowdControl, float duration, BaseUnitController caster)
    {
        if(crowdControl == CrowdControls.None)
            return;
        
        switch (crowdControl)
        {
            case CrowdControls.Taunt:
                CurCc |= CrowdControls.Taunt;
                CcCaster = caster;
                Debug.Log($"{caster.name}에게 도발당함");
                OnTaunted?.Invoke();
                break;
            case CrowdControls.Stun:
                CurCc |= CrowdControls.Stun;
                OnStun?.Invoke();
                break;
        }
        
        StartCoroutine(RunningCrowdControlRoutine(crowdControl, duration));
    }

    public void TakeHeal(int heal)
    {
        if (Hp <= 0)
        {
            Debug.LogWarning("이미 hp가 0입니다.");
            return;
        }

        int calcHeal = heal; // 치유 감소가 있을경우 추가 로직 구현
        if (Hp + heal >= MaxHp)
        {
            calcHeal = MaxHp - Hp;
        }
        
        Hp += calcHeal;
        
        Debug.Log($"힐 : {heal} 받음. 현재 hp : {Hp}/{MaxHp}");
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 죽음");
        
        OnDeath?.Invoke();
        
        //gameObject.SetActive(false);
    }

    private IEnumerator RunningCrowdControlRoutine(CrowdControls crowdControl, float duration)
    {
        yield return new WaitForSeconds(duration);
        CurCc &= ~crowdControl;
        Debug.Log("bbbbbb");
        Debug.Log($"{gameObject.name}에게 있던 {crowdControl}효과 해제");
        CcCaster = null;

        // 필요할 경우 군중제어 해제시 이벤트 호출을 위한 Switch문
        switch (crowdControl)
        {
            case CrowdControls.Taunt:
                break;
            case CrowdControls.Stun:
                break;
        }
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
        StopAllCoroutines();
    }

    private void UnsubscribeEvents()
    {
        OnHpChanged = null;
        OnDeath = null;
        OnHealed = null;
        OnDamaged = null;
        
    }
}
