using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeEffect : MonoBehaviour
{
    public float CharPerSeconds; // 타이핑 속도
    public AudioClip typingSound; // 타이핑 소리 파일
    private AudioSource audioSource; // 오디오 소스를 제어할 변수

    string targetMsg; // 출력할 전체 메시지
    TextMeshProUGUI msgText; // 텍스트 출력용 TextMeshProUGUI

    int index; // 현재 출력 중인 문자 인덱스

    public TextMeshProUGUI MsgText => msgText; // 외부에서 접근할 수 있는 프로퍼티 추가

    private void Awake()
    {
        msgText = GetComponent<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource가 할당되지 않았습니다. TypeEffect 오브젝트에 AudioSource 컴포넌트를 추가하세요.");
        }

        if (typingSound != null)
        {
            audioSource.clip = typingSound;
        }
    }

    public void SetMsg(string msg)
    {
        targetMsg = msg;
        EffectStart();
    }

    void EffectStart()
    {
        msgText.text = ""; // 텍스트 초기화
        index = 0;

        if (audioSource != null && typingSound != null)
        {
            audioSource.clip = typingSound; // 오디오 클립 할당
            audioSource.loop = true; // 반복 재생
            audioSource.Play(); // 소리 재생
        }

        Invoke("Effecting", 1 / CharPerSeconds);
    }

    void Effecting()
    {
        if (msgText.text == targetMsg)
        {
            EffectEnd();
            return;
        }

        msgText.text += targetMsg[index]; // 한 글자 추가
        index++;

        Invoke("Effecting", 1 / CharPerSeconds); // 다음 글자 출력
    }

    void EffectEnd()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.loop = false; // 루프 해제
            audioSource.Stop(); // 소리 중지
        }
    }
}
