using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GachaManager : MonoBehaviour
{
    // UI 요소 선언
    public Image scriptBg;          // 스크립트 배경 이미지
    public TMP_Text scriptText;     // 스크립트 텍스트 (미리 입력된 텍스트를 타이핑 효과로 출력)
    public AudioSource audioSource; // 사운드 재생을 위한 AudioSource
    public AudioClip scriptSd;      // 스크립트 출력 시 재생되는 사운드
    public AudioClip pingSd;        // 별 애니메이션에서 재생되는 효과음
    public AudioClip getSd;         // "get" 이벤트 사운드 클립
    public AudioClip showSd;        // "show" 이벤트 사운드 클립
    public AudioClip endSd;         // "end" 이벤트 사운드 클립

    public Image star1;             // 별 이미지 1
    public Image star2;             // 별 이미지 2
    public Image star3;             // 별 이미지 3
    public Image star4;             // 별 이미지 4
    public Image star5;             // 별 이미지 5

    public Image background1;       // 배경 이미지
    public Image dinosaur1;         // 공룡 이미지
    public Image character1;        // 캐릭터 이미지

    public TMP_Text name1;          // 캐릭터 이름 텍스트 1
    public TMP_Text name2;          // 캐릭터 이름 텍스트 2

    private Sequence animationSequence; // DOTween 애니메이션 시퀀스를 관리하는 변수

    // OnEnable: 스크립트가 활성화될 때 실행
    private void OnEnable()
    {
        MakeSequence(); // DOTween 시퀀스 생성
        animationSequence.Play();   // 생성된 시퀀스 실행
    }

    // OnDisable: 스크립트가 비활성화될 때 실행
    private void OnDisable()
    {
        Destroy(gameObject); // 게임 오브젝트 제거 (메모리 관리)
    }

    // Start: 게임 시작 시 호출
    private void Start()
    {
        InitializeUI(); // UI 초기화 (모든 요소를 비활성화 상태로 설정)
    }

    // InitializeUI: UI 초기화
    private void InitializeUI()
    {
        // 스크립트 배경을 투명 상태로 초기화
        scriptBg.color = new Color(scriptBg.color.r, scriptBg.color.g, scriptBg.color.b, 0);

        // 스크립트 텍스트를 투명 상태로 초기화
        scriptText.alpha = 0;

        // 모든 별 이미지를 투명 상태로 초기화
        InitializeImage(star1);
        InitializeImage(star2);
        InitializeImage(star3);
        InitializeImage(star4);
        InitializeImage(star5);

        // 배경, 공룡, 캐릭터 이미지를 투명 상태로 초기화
        InitializeImage(background1);
        InitializeImage(dinosaur1);
        InitializeImage(character1);

        // 캐릭터 이름 텍스트를 투명 상태로 초기화
        name1.alpha = 0;
        name2.alpha = 0;
    }

    // InitializeImage: 이미지를 투명 상태로 설정
    private void InitializeImage(Image img)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
    }

    // MakeSequence: DOTween 애니메이션 시퀀스 생성
    private void MakeSequence()
    {
        animationSequence = DOTween.Sequence(); // 시퀀스 초기화
        animationSequence.Pause();             // 바로 실행하지 않고 대기 상태로 설정

        // 1. 스크립트 배경과 텍스트 페이드 인
        // animationSequence.AppendCallback(() => PlaySound(getSd));  // "get" 사운드 재생
        animationSequence.Append(scriptBg.DOFade(1.0f, 1f));       // 배경 페이드 인
        animationSequence.AppendCallback(() =>
        {
            scriptText.alpha = 1; // 타이핑 시작 시 텍스트 활성화
            PlaySound(scriptSd);  // 스크립트 사운드 재생
            StartCoroutine(TypingEffect(scriptText, 0.05f)); // 타이핑 효과 실행
        });

        // 2. 텍스트를 8초 동안 유지
        animationSequence.AppendInterval(8f);

        // 4. 별 애니메이션 추가
        AppendStarAnimation(star1, 0.2f);
        AppendStarAnimation(star2, 0.2f);
        AppendStarAnimation(star3, 0.2f);
        AppendStarAnimation(star4, 0.2f);
        AppendStarAnimation(star5, 0.2f);

        // 5. 배경 페이드 인
        animationSequence.Join(background1.DOFade(1.0f, 1f));

        // 6. 공룡과 캐릭터 이미지 페이드 인
        animationSequence.AppendCallback(() => PlaySound(showSd)); // "show" 사운드 재생
        animationSequence.Append(dinosaur1.DOFade(1.0f, 1f));
        animationSequence.Join(character1.DOFade(1.0f, 1f));

        // 7. 캐릭터 이름 텍스트 페이드 인
        animationSequence.AppendCallback(() => PlaySound(endSd)); // "end" 사운드 재생
        animationSequence.Append(name1.DOFade(1.0f, 1f));
        animationSequence.Join(name2.DOFade(1.0f, 1f));

    }

    // AppendStarAnimation: 별 애니메이션 추가
    private void AppendStarAnimation(Image star, float duration)
    {
        animationSequence.Append(star.rectTransform.DORotate(new Vector3(0, 0, 360), 0.2f, RotateMode.FastBeyond360)); // 별 회전
        animationSequence.Join(star.DOFade(1.0f, duration)); // 별 페이드 인
        animationSequence.AppendCallback(() => PlaySound(pingSd)); // 사운드 재생
        animationSequence.AppendInterval(duration); // 간격 설정
    }

    // TypingEffect: 텍스트 타이핑 효과
    private IEnumerator TypingEffect(TMP_Text textComponent, float typingSpeed)
    {
        animationSequence.AppendCallback(() => PlaySound(getSd));  // "get" 사운드 재생
        string fullText = textComponent.text; // 미리 입력된 텍스트 가져오기
        textComponent.text = "";             // 텍스트 초기화

        // 각 문자를 순차적으로 출력
        foreach (char letter in fullText)
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed); // 출력 속도 조절
        }
        yield return new WaitForSeconds(5f);

        // 3. 텍스트 비활성화
        animationSequence.Append(scriptText.DOFade(0.0f, 0.5f));
    }

    // PlaySound: 사운드 재생
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null) // AudioSource와 AudioClip이 존재할 경우
        {
            audioSource.PlayOneShot(clip); // 효과음 재생
        }
    }
}
