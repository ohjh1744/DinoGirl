using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    private BaseUnitController _target;
    public BaseUnitController Target {get => _target; set => _target = value;}
    private Transform _targetPos;
    public Transform TargetPos { get => _targetPos; set => _targetPos = value; }
    [SerializeField] private float _speed;
    public float Speed { get => _speed; set => _speed = value; }
    private float _hitRange = 0.2f;

    private float _hitDamage;
    public float HitDamage { get => _hitDamage; set => _hitDamage = value; }
    [SerializeField] private GameObject _impactEffect;
    public GameObject ImpactEffect { get => _impactEffect; set => _impactEffect = value; }

    [SerializeField] AudioClip _impactSound;
    public AudioClip ImpactSound { get => _impactSound; private set => _impactSound = value; }

    [SerializeField] private bool _isRealAttack;

    
    //[SerializeField] private ProjectileSkill referenceSkill;
    private void OnEnable()
    {
        //Debug.Log("생성됨");
    }

    private void Update()
    {
        CheckNeedFlip(TargetPos);
        
        if (TargetPos == null || !TargetPos.gameObject.activeSelf || !Target.gameObject.activeSelf)
        {
            Destroy(gameObject);
            return;
        }
            
        transform.position = Vector2.MoveTowards(transform.position, TargetPos.position, Speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, TargetPos.position) <= _hitRange)
        {
            HitTarget();
        }
        
    }
    
    private void CheckNeedFlip(Transform target)
    {
        if ((transform.position.x < target.position.x && transform.localScale.x < 0) ||
            (transform.position.x > target.position.x && transform.localScale.x > 0))
        {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }
    }
    private void HitTarget()
    {
        if (Target == null || !Target.gameObject.activeSelf)
            return;

        if (_isRealAttack)
        {
            Target.UnitModel.TakeDamage(Mathf.RoundToInt(HitDamage));
        }
            
        SpawnImpactEffectAndDestroy();
        Destroy(gameObject);
    }

    private void SpawnImpactEffectAndDestroy()
    {
        if(ImpactEffect == null)
            return;
        
        GameObject particleObject = Instantiate(ImpactEffect, transform.position, Quaternion.identity);
        if (particleObject == null)
            return;
        
        if (particleObject.TryGetComponent<ParticleSystem>(out var particle))
        {
            PlaySkillSfx(ImpactSound);
            Destroy(particleObject, particle.main.duration + particle.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(particleObject);
        }
        
        //Destroy(gameObject);
    }
    
    private void PlaySkillSfx(AudioClip sound)
    {
        if(sound == null)
            return;
        SoundManager.Instance.PlaySFX(sound);
    }
}
