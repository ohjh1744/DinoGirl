using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Parameter
{
    Idle, Run ,Attack, Skill, Size
}

public class UnitView : MonoBehaviour
{

    public BaseUnitController unit;
    private Animator _unitAnimator;
    public Animator UnitAnimator { get => _unitAnimator;}

    private int[] _parameterHash;

    public int[] ParameterHash { get => _parameterHash; private set => _parameterHash = value; }
    //public AnimatorStateInfo stateInfo;

    private int[] _animationHash = new int[]
    {
        Animator.StringToHash("IdleState"),
        Animator.StringToHash("Attacking"),
    };



    private void Awake()
    {
        ParameterHash = new int[(int)Parameter.Size];
        ParameterHash[(int)Parameter.Idle] = Animator.StringToHash("Idle");
        ParameterHash[(int)Parameter.Run] = Animator.StringToHash("Run");
        ParameterHash[(int)Parameter.Attack] = Animator.StringToHash("Attack");
        ParameterHash[(int)Parameter.Skill] = Animator.StringToHash("Skill");
    }

    private void Start()
    {
        unit = GetComponent<BaseUnitController>();
        _unitAnimator = GetComponentInChildren<Animator>();
    }
    
    public bool IsAnimationRunning(string stateName)
    {
        /*if (UnitAnimator.IsInTransition(0))
        {
            Debug.Log($"트랜지션 중, 상태: {stateName}");
            return true; 
            // 트랜지션 중일 때도 실행 중으로 간주
        }*/
        
        if (UnitAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            var normalizedTime = UnitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            return normalizedTime != 0 && normalizedTime < 1.0f;
        }
        return false;
    }

    /*public void PlayAnimation(int animationHash)
    {
        if (animationHash >= 0 && animationHash < animationHash.Length)
        {
            unitAnimator.Play(_animationHash[animationHash],0,0);
        }
        else
        {
            Debug.LogError("애니메이션 해시 범위 오류");
        }
    }

    public bool IsAnimationFinished()
    {
        stateInfo = unitAnimator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 1.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }*/


}
