using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class S5Manager : MonoBehaviourPun
{
    [PunRPC]
    public void BridgeGeneratorRPC(string objectName)
    {
        // 이름으로 오브젝트 찾기
        GameObject targetObject = GameObject.Find(objectName);

        if (targetObject != null)
        {
            // 오브젝트를 움직이는 동작 구현
            StartCoroutine(MoveObject(targetObject));
        }
        else
        {
            Debug.LogError($"오브젝트 '{objectName}'를 찾을 수 없습니다.");
        }
    }

    private IEnumerator MoveObject(GameObject targetObject)
    {
        // 시작 위치와 끝 위치 설정 (-y 방향으로 이동)
        Vector3 startPosition = targetObject.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, -2f, 0); // 아래로 2 유닛 이동
        float duration = 1f; // 이동 시간
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Lerp를 사용하여 부드럽게 이동
            targetObject.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 이동 완료 후 정확히 도착 지점으로 설정
        targetObject.transform.position = endPosition;
    }
}
