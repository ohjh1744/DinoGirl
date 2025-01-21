using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingVoice : MonoBehaviour
{   
    [SerializeField] AudioSource voiceSource;


    private void OnEnable()
    {
        StartCoroutine(delayingVoice());
    }

    IEnumerator delayingVoice() 
    {
        yield return new WaitForSeconds(0.3f);

        voiceSource.Play();
    }
}
