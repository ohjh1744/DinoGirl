using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform _target;
    public Transform Target { get => _target; set => _target = value; }
    [SerializeField] private float _speed;
    public float Speed { get => _speed; set => _speed = value; }
    private float _hitRange = 0.2f;

    private void OnEnable()
    {
        Debug.Log("생성됨");
    }

    private void Update()
    {
        if (Target == null || !Target.gameObject.activeSelf)
        {
            Destroy(gameObject);
            return;
        }
            
        transform.position = Vector2.MoveTowards(transform.position, Target.position, Speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, Target.position) <= _hitRange)
        {
            HitTarget();
        }
        
    }
    private void HitTarget()
    {
        // 명중처리를 할것인지 단순 보이기용인지 결정해야함
        Destroy(gameObject);
    }
}
