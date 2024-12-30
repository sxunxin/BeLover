using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MirrorEffectManager : MonoBehaviour
{
    public GameObject mirrorImage; // 거울 이미지
    public GameObject flashPanel; // 화면 깜박임 패널

    private RectTransform mirrorRect; // 거울 이미지 RectTransform
    private Vector3 initialPosition; // 거울의 초기 위치
    private Vector3 targetPosition = Vector3.zero; // 거울의 목표 위치 (화면 중앙)
    private Vector3 initialScale; // 거울의 초기 크기
    private Vector3 targetScale = new Vector3(3f, 3f, 3f); // 거울의 목표 크기 (더 커지도록 설정)

    private Image flashImage; // 플래시 패널의 이미지 컴포넌트

    void Start()
    {
        // RectTransform 및 초기값 설정
        mirrorRect = mirrorImage.GetComponent<RectTransform>();
        initialPosition = new Vector3(0, -400, 0); // 화면 아래 위치
        initialScale = new Vector3(0.1f, 0.1f, 1f); // 작게 시작
        mirrorRect.localPosition = initialPosition;
        mirrorRect.localScale = initialScale;

        flashImage = flashPanel.GetComponent<Image>();
        flashImage.color = new Color(1, 1, 1, 0); // 투명하게 시작
        flashPanel.SetActive(false); // 플래시 패널 비활성화
        mirrorImage.SetActive(false); // 거울 이미지 비활성화
    }

    public void TriggerMirrorEffect()
    {
        StartCoroutine(MirrorEffectSequence());
    }

    private IEnumerator MirrorEffectSequence()
    {
        // 1. 거울 이미지 활성화 및 애니메이션 시작
        mirrorImage.SetActive(true);
        float elapsedTime = 0f;

        // 거울 이동 및 크기 확대
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.5f;

            // 위치와 크기를 보간
            mirrorRect.localPosition = Vector3.Lerp(initialPosition, targetPosition, t);
            mirrorRect.localScale = Vector3.Lerp(initialScale, new Vector3(3f, 3f, 3f), t);

            yield return null;
        }

        // 2. 화면 깜박임 효과
        flashPanel.SetActive(true);
        elapsedTime = 0f;
        while (elapsedTime < 0.2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.2f;

            // Alpha 값 변화로 깜박임 효과
            float alpha = Mathf.Sin(t * Mathf.PI); // 부드럽게 깜박임
            flashImage.color = new Color(1, 1, 1, alpha);

            yield return null;
        }

        flashPanel.SetActive(false); // 깜박임 종료

        // 3. 거울 잠시 유지
        yield return new WaitForSeconds(0.2f);

        // 4. 거울 이미지 비활성화
        mirrorImage.SetActive(false);
    }
}
