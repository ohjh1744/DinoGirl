using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllButtonListener : MonoBehaviour
{
    [SerializeField] private AudioClip _buttonClip;

    private void Start()
    {
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            button.onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        }
    }
}
