using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GachaManager : MonoBehaviour
{
    // UI 요소
    public Image scriptBg;          // 스크립트 배경 이미지
    public TMP_Text scriptText;     // 스크립트 텍스트
    public AudioSource audioSource; // 사운드 재생을 위한 AudioSource
    public AudioClip scriptSd;      // 스크립트 출력 시 사운드
    public AudioClip pingSd;        // 별이 나타날 때 재생되는 ping 사운드
    public AudioClip getSd;         // 'get' 사운드 클립
    public AudioClip showSd;        // 'show' 사운드 클립
    public AudioClip endSd;         // 'end' 사운드 클립

    public Image star1;             // 별 이미지 1
    public Image star2;             // 별 이미지 2
    public Image star3;             // 별 이미지 3
    public Image star4;             // 별 이미지 4
    public Image star5;             // 별 이미지 5

    public Image background1;       // 배경 이미지
    public Image dinosaur1;         // 공룡 이미지
    public Image character1;        // 캐릭터 이미지

    public TMP_Text name1;          // 캐릭터 이름 텍스트1
    public TMP_Text name2;          // 캐릭터 이름 텍스트2

    private Sequence animationSequence; // DOTween의 애니메이션 시퀀스를 관리하는 변수

    // OnEnable: 스크립트가 활성화될 때 실행
    private void OnEnable()
    {
        MakeSequence(); // DOTween 시퀀스를 생성
        animationSequence.Play();   // 생성된 시퀀스를 바로 실행
    }

    // OnDisable: 스크립트가 비활성화될 때 실행
    private void OnDisable()
    {
        Destroy(gameObject); // 게임 오브젝트를 제거하여 메모리 관리
    }

    // Start: 게임 시작 시 호출
    void Start()
    {
        InitializeUI(); // 모든 UI 요소를 초기화 (비활성화 상태로 설정)
    }

    // InitializeUI: UI 요소를 초기 상태로 설정
    private void InitializeUI()
    {
        // 스크립트 배경과 텍스트를 투명 상태로 초기화
        scriptBg.color = new Color(scriptBg.color.r, scriptBg.color.g, scriptBg.color.b, 0);
        scriptText.alpha = 0;

        // 별 이미지를 투명 상태로 초기화
        star1.color = new Color(star1.color.r, star1.color.g, star1.color.b, 0);
        star2.color = new Color(star2.color.r, star2.color.g, star2.color.b, 0);
        star3.color = new Color(star3.color.r, star3.color.g, star3.color.b, 0);
        star4.color = new Color(star4.color.r, star4.color.g, star4.color.b, 0);
        star5.color = new Color(star5.color.r, star5.color.g, star5.color.b, 0);

        // 배경, 공룡, 캐릭터 이미지를 투명 상태로 초기화
        background1.color = new Color(background1.color.r, background1.color.g, background1.color.b, 0);
        dinosaur1.color = new Color(dinosaur1.color.r, dinosaur1.color.g, dinosaur1.color.b, 0);
        character1.color = new Color(character1.color.r, character1.color.g, character1.color.b, 0);

        // 캐릭터 이름 텍스트를 투명 상태로 초기화
        name1.alpha = 0;
        name2.alpha = 0;
    }

    // MakeSequence: DOTween 시퀀스를 생성
    private void MakeSequence()
    {
        // 시퀀스 초기화
        animationSequence = DOTween.Sequence();
        animationSequence.Pause(); // 시퀀스를 즉시 실행하지 않고 대기 상태로 설정

        // 1. 스크립트 배경과 텍스트를 페이드 인
        animationSequence.AppendCallback(() => PlaySound(getSd));    // 'get' 사운드 재생
        animationSequence.Append(scriptBg.DOFade(1.0f, 1f));   // 배경 페이드 인
        animationSequence.Join(scriptText.DOFade(1.0f, 1f));   // 텍스트를 배경과 동시에 페이드 인
        animationSequence.AppendCallback(() => PlaySound(scriptSd)); // 페이드 인이 끝난 후 사운드 재생

        // 2. 타이핑 효과 시작
        animationSequence.AppendCallback(() =>
        {
            StartCoroutine(TypingEffect(scriptText, "백악기사단의 트리케라톱스, 트리샤야. 오늘부터 내가 너의 방패가 되어줄게", 0.05f));
        });

        // 3. 텍스트를 6초 동안 유지
        animationSequence.AppendInterval(6f);

        // 4. 스크립트 텍스트를 비활성화
        animationSequence.AppendInterval(0.5f); // 약간의 딜레이 추가
        animationSequence.AppendCallback(() => scriptText.DOFade(0.0f, 0.5f));

        // 5. 별 애니메이션 추가 (1초 안에 5개의 별이 순차적으로 나타남)
        float starDuration = 1f;             // 총 별 애니메이션 시간
        float starInterval = starDuration / 5f; // 별 하나가 나오는 시간 간격

        AppendStarAnimation(star1, starInterval);
        AppendStarAnimation(star2, starInterval);
        AppendStarAnimation(star3, starInterval);
        AppendStarAnimation(star4, starInterval);
        AppendStarAnimation(star5, starInterval);

        // 6. 배경 이미지를 페이드 인
        animationSequence.Join(background1.DOFade(1.0f, 1f)); // 배경 이미지를 페이드 인

        // 7. 공룡과 캐릭터 이미지를 동시에 페이드 인
        animationSequence.AppendCallback(() => PlaySound(showSd)); // 'show' 사운드 재생
        animationSequence.Append(dinosaur1.DOFade(1.0f, 1f));
        animationSequence.Join(character1.DOFade(1.0f, 1f));

        // 8. 캐릭터 이름 텍스트를 페이드 인
        animationSequence.AppendCallback(() => PlaySound(endSd)); // 'end' 사운드 재생
        animationSequence.Append(name1.DOFade(1.0f, 1f));
        animationSequence.Join(name2.DOFade(1.0f, 1f));
    }

    // AppendStarAnimation: 별 애니메이션 추가
    private void AppendStarAnimation(Image star, float interval)
    {
        animationSequence.Append(star.rectTransform.DORotate(new Vector3(0, 0, 360), 0.2f, RotateMode.FastBeyond360)); // 별 회전
        animationSequence.Join(star.DOFade(1.0f, 0.2f)); // 별 페이드 인
        animationSequence.AppendCallback(() => PlaySound(pingSd)); // 별이 나타날 때 사운드 재생
        animationSequence.AppendInterval(interval); // 다음 별 애니메이션 전까지 대기
    }

    // TypingEffect: 타이핑 효과 구현
    private IEnumerator TypingEffect(TMP_Text textComponent, string fullText, float typingSpeed)
    {
        textComponent.text = ""; // 텍스트 초기화
        textComponent.alpha = 1; // 텍스트 표시
        foreach (char letter in fullText) // 각 문자를 순차적으로 출력
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed); // 문자 간 출력 속도 조절
        }
    }

    // PlaySound: 사운드 재생
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null) // AudioSource와 AudioClip이 설정되어 있는 경우
        {
            audioSource.PlayOneShot(clip); // 지정된 클립 재생
        }
    }
}
