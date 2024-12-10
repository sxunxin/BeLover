using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image backgroundImage; // 배경 이미지를 연결합니다.
    public float moveDuration = 2f; // 좌우로 움직이는 데 걸리는 시간
    public float moveSpeed = 0.1f; // 배경의 이동 속도
    public float scaleDuration = 2f; // 확대 및 축소에 걸리는 시간
    public Vector2 minMaxScale = new Vector2(0.8f, 1.2f); // 최소/최대 스케일 설정

    [Range(0f, 1f)]
    public float minOffsetX = 0f; // 텍스처 이동의 최소 x 범위 (0 ~ 1)
    [Range(0f, 1f)]
    public float maxOffsetX = 0.2f; // 텍스처 이동의 최대 x 범위 (0 ~ 1)

    void Start()
    {
        if (backgroundImage == null)
        {
            Debug.LogError("Background Image를 연결해야 합니다.");
            return;
        }

        // Image의 Material을 인스턴스화하여 독립적으로 조작할 수 있도록 함
        backgroundImage.material = new Material(backgroundImage.material);

        // 반복 애니메이션 시작
        StartCoroutine(AnimateBackground());
    }

    IEnumerator AnimateBackground()
    {
        while (true)
        {
            // 1. 확대
            yield return StartCoroutine(ScaleImage(minMaxScale.y, scaleDuration * 2));

            // 2. 좌로 이동
            yield return StartCoroutine(MoveBackground(Vector2.left, moveDuration));

            // 3. 우로 이동
            yield return StartCoroutine(MoveBackground(Vector2.right, moveDuration * 3)); // 더 긴 시간으로 오른쪽 이동

            // 4. 좌로 다시 이동 (원래 위치로)
            yield return StartCoroutine(MoveBackground(Vector2.left, moveDuration * 3));

            // 5. 원래 크기로 복귀 (1.0으로 복귀)
            yield return StartCoroutine(ScaleImage(1.0f, scaleDuration * 2));
        }
    }

    IEnumerator ScaleImage(float targetScale, float duration)
    {
        float time = 0f;
        Vector3 initialScale = backgroundImage.rectTransform.localScale;
        Vector3 targetScaleVector = new Vector3(targetScale, targetScale, 1f);

        while (time < duration)
        {
            time += Time.deltaTime;
            backgroundImage.rectTransform.localScale = Vector3.Lerp(initialScale, targetScaleVector, time / duration);
            yield return null;
        }

        backgroundImage.rectTransform.localScale = targetScaleVector; // 정확히 맞추기 위해서 마지막에 적용
    }

    IEnumerator MoveBackground(Vector2 direction, float duration)
    {
        float time = 0f;
        Vector2 initialOffset = backgroundImage.material.mainTextureOffset;

        while (time < duration)
        {
            time += Time.deltaTime;
            // mainTextureOffset을 변경하여 이미지의 소스 부분을 이동시킵니다.
            Vector2 newOffset = initialOffset + direction * (moveSpeed * time / duration);

            // 새로운 x 오프셋을 범위 내로 제한합니다.
            newOffset.x = Mathf.Clamp(newOffset.x, minOffsetX, maxOffsetX);

            // 오프셋 적용
            backgroundImage.material.mainTextureOffset = newOffset;
            yield return null;
        }

        // 오프셋을 최종적으로 다시 클램핑하여 정확히 맞춤
        Vector2 finalOffset = initialOffset + direction * moveSpeed;
        finalOffset.x = Mathf.Clamp(finalOffset.x, minOffsetX, maxOffsetX);
        backgroundImage.material.mainTextureOffset = finalOffset;
    }
}
