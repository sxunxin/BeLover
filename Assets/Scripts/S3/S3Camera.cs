using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S3Camera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target; // 추적할 대상 (Player)
    public float smoothSpeed = 0.125f; // 이동 속도
    public Vector3 offset; // 카메라 오프셋

    private Vector3 targetPosition;

    void Start()
    {
        // 자동으로 Player를 찾고 카메라의 타겟으로 설정
        if (target == null)
        {
            TryFindPlayer();
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            // 타겟 위치에 오프셋 추가
            targetPosition = target.position + offset;

            // Lerp를 사용하여 부드럽게 이동
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    public void SnapToTarget()
    {
        // 타겟 위치로 즉시 이동
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    // Player 오브젝트를 찾는 함수
    public void TryFindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            Debug.Log("플레이어를 찾았습니다: " + target.name);
        }
        else
        {
            Debug.LogWarning("Player를 찾을 수 없습니다. 나중에 다시 시도합니다.");
            Invoke(nameof(TryFindPlayer), 1f); // 1초 후 다시 시도
        }
    }

    // Player를 수동으로 설정할 때 호출
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log("타겟이 수동으로 설정되었습니다: " + target.name);
    }
}
