using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public static class DOTweenTMPExtensions
{
    // TextMeshPro 텍스트의 내용을 애니메이션으로 변경하는 확장 메서드
    public static Tweener DOText(this TMP_Text target, string endValue, float duration, bool richTextEnabled = true, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
    {
        int startValueLength = target.text.Length; // 시작 텍스트 길이 (사용하지 않는 경우 제거 가능)
        return DOTween.To(() => target.text, x => target.text = x, endValue, duration)
            .SetOptions(richTextEnabled, scrambleMode, scrambleChars)
            .SetTarget(target);
    }
}

public class DOTweenManager : MonoBehaviour
{
    // UI 이미지들 (애니메이션을 적용할 대상)
    public Image testImage1;
    public Image testImage2;
    public Image testImage3;
    public Image testImage4;
    public Image testImage5;

    public RectTransform testRect1;

    // DOTween 시퀀스
    public Sequence testSequence;

    // 사운드 추가를 위해 필요한 변수
    public AudioSource audioSource; // 사운드 재생을 위한 AudioSource
    public AudioClip clip1;         // 첫 번째 사운드 클립
    public AudioClip clip2;         // 두 번째 사운드 클립

    // 게임 시작 시 호출되는 메서드
    void Start()
    {
        MakeSequence(); // 시퀀스 초기화
        DOVirtual.DelayedCall(2f, () => testSequence.Play()); // 2초 후 자동으로 시퀀스를 실행
    }

    // 시퀀스를 생성하고 애니메이션 설정
    public void MakeSequence()
    {
        // DOTween 시퀀스 초기화 및 정지 상태로 설정
        testSequence = DOTween.Sequence();
        testSequence.Pause();

        // 5초의 대기 시간 추가
        testSequence.AppendInterval(5f);

        // 이미지 페이드 아웃 (0.5초)
        testSequence.Join(testImage5.DOFade(0.0f, 0.5f));

        // 이미지들의 위치 이동 애니메이션 설정
        // 각 이미지를 지정된 위치로 이동 (3초 동안)
        testSequence.Join(testImage2.rectTransform.DOAnchorPos(new Vector2(0, 0), 3));
        testSequence.Join(testImage3.rectTransform.DOAnchorPos(new Vector2(0, 0), 3));
        testSequence.Join(testImage4.rectTransform.DOAnchorPos(new Vector2(0, 0), 3));

        // 0.5초의 대기 시간 추가
        testSequence.AppendInterval(0.5f);

        // 상어 그림 이동 (1초 동안)
        testSequence.Join(testImage1.rectTransform.DOAnchorPos(new Vector2(0, 0), 1));

        // 0.1초의 대기 시간 추가
        testSequence.AppendInterval(0.1f);

        // 상어 그림 Shake 애니메이션 추가 (0.5초 동안, 강도는 10, 진동 횟수는 10)
        testSequence.Append(testImage1.rectTransform.DOShakeAnchorPos(0.5f, 10f, 10, 90, false, true));

        // 사운드 재생 애니메이션 추가
        // 첫 번째 사운드 클립 재생 (애니메이션 시작 시)
        testSequence.Insert(0f, DOVirtual.DelayedCall(0f, () => PlaySound(clip1)));

        // 두 번째 사운드 클립 재생 (이미지들이 이동하는 3초 동안)
        testSequence.Insert(5.5f, DOVirtual.DelayedCall(0f, () => PlaySound(clip2)));
    }

    // 사운드 재생 메서드
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // 시퀀스를 재생하는 메서드 (외부에서 호출 가능)
    public void PlaySequence()
    {
        testSequence.Play();
    }
}
