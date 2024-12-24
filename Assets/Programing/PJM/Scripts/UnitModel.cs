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
    
    [SerializeField] private int _hp;
    public int Hp { get => _hp; set { if (_hp != value) { _hp = value; OnHPChanged?.Invoke(_hp); } } }
    [SerializeField] private int _attackPoint;
    public int AttackPoint { get => _attackPoint; private set => _attackPoint = value; }
    [SerializeField] private int _defensePoint;
    public int DefensePoint { get => _defensePoint; private set => _defensePoint = value; }
    [SerializeField] private float _moveSpeed;
    public float Movespeed { get => _moveSpeed; set => _moveSpeed = value; }
    [SerializeField] private float _attackRange;
    public float AttackRange { get => _attackRange; set => _attackRange = value; }


}
