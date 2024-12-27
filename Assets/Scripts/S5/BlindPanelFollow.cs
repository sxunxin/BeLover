using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlindPanelFollow : MonoBehaviour
{
    private Transform player2Transform;

    private Vector3 originalScale;
    public Vector2 offset = new Vector2(0.18f, -0.15f);

    void Start()
    {
        originalScale = transform.localScale;
        GameObject player2 = GameObject.FindWithTag("player2");
        if (player2 != null)
        {
            player2Transform = player2.transform;
        }
        else
        {
            Debug.LogError("Player2 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        if (player2Transform != null)
        {
            // Player2의 위치를 따라다니도록 설정
            transform.position = player2Transform.position + (Vector3)offset;
        }
    }


    public IEnumerator ScaleBlindEffect()
    {
        float scaleMultiplier = 4f; 
        float duration = 0.9f;
        float durationEnding = 4f;
        float holdTime = 0.5f;

        // 목표 크기 설정
        Vector3 targetScale = originalScale * scaleMultiplier;

        // BlindPanel 점차 커지기
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 정확히 목표 크기로 설정
        transform.localScale = targetScale;

        // 잠시 대기
        yield return new WaitForSeconds(holdTime);

        // BlindPanel 점차 원래 크기로 줄이기
        elapsedTime = 0f;
        while (elapsedTime < durationEnding)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / durationEnding);
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 정확히 원래 크기로 복구
        transform.localScale = originalScale;
    }
}
