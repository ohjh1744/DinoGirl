using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBarController : MonoBehaviour
{
    private UnitModel _unit; // 이벤트 구독용
    private Slider _healthSlider; // 체력바 슬라이더
    [SerializeField] private Transform _target; // 체력바가 따라다닐 타겟
    public Transform Target { get { return _target; } set { _target = value; } }
    public Vector3 barTransformOffset; // 체력바 위치 조정용 오프셋
    private Vector3 _screenPosition;
    private Camera _mainCam;
    

    private void Start()
    {
        _mainCam = Camera.main;
        SetSliderValues();
        SubscribeEvents();
        _unit = GetComponentInParent<UnitModel>();
        _healthSlider = GetComponentInParent<Slider>();
    }

    private void LateUpdate()
    {
        if(Target != null && Target.gameObject.activeSelf) 
        {
            _screenPosition = _mainCam.WorldToScreenPoint(Target.position + barTransformOffset);
            transform.position = _screenPosition;
        }
    }

    private void SubscribeEvents()
    {
        _unit.OnHPChanged += UpdateHealthBar;
        _unit.OnDeath += HandleDeath;
    }

    private void SetSliderValues()
    {
        _healthSlider.maxValue = _unit.MaxHp;
        _healthSlider.minValue = 0;
    }
    private void UpdateHealthBar(int changedHpValue)
    {
        _healthSlider.value = changedHpValue;
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }
}
