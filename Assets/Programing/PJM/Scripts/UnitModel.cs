using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class UnitModel : MonoBehaviour
{
    public event Action<int> OnHPChanged;
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
            if (_hp != value)
            {
                _hp = Mathf.Clamp(value, 0, MaxHp);
                OnHPChanged?.Invoke(_hp);
                if (_hp < oldValue)
                    OnDamaged?.Invoke(oldValue - _hp);
                if (_hp > oldValue)
                    OnHealed?.Invoke(_hp - oldValue);
                if (_hp <= 0)
                    Die();
            }
        }
    }
    [SerializeField] private int _attackPoint;
    public int AttackPoint { get => _attackPoint; private set => _attackPoint = value; }
    [SerializeField] private int _defensePoint;
    public int DefensePoint { get => _defensePoint; private set => _defensePoint = value; }
    [SerializeField] private float _moveSpeed;
    public float Movespeed { get => _moveSpeed; set => _moveSpeed = value; }
    [SerializeField] private float _attackRange;
    public float AttackRange { get => _attackRange; set => _attackRange = value; }

    public void TakeDamage(int damage)
    {
        if (Hp <= 0)
        {
            
            Debug.LogWarning("이미 hp가 0입니다.");
            return;
        }
        Hp -= damage;
        Debug.Log($"{damage} 받음. 현재 hp : {Hp}/{MaxHp}");
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 죽음");
        OnDeath?.Invoke();
    }
}
