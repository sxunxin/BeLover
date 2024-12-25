using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform[] checkpoints; // 이동할 체크포인트 배열
    public float speed = 0.8f; // 기본 속도
    public float speedIncreaseRate = 0.05f; // 1초당 속도 증가량
    public float defaultSpeed = 0.8f; // 초기 속도
    private int currentCheckpointIndex = 0; // 현재 이동 중인 체크포인트 인덱스
    private bool isStopped = false; // 정지 상태 확인
    private Rigidbody2D rb; // Rigidbody2D 컴포넌트
    private Vector2 moveDirection; // 이동 방향

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (checkpoints.Length > 0)
        {
            SetNextCheckpoint();
        }

        StartCoroutine(IncreaseSpeedOverTime());
    }

    void Update()
    {
        if (isStopped || checkpoints.Length == 0) return;

        // 체크포인트 방향으로 이동
        Vector2 targetPosition = checkpoints[currentCheckpointIndex].position;
        moveDirection = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = moveDirection * speed;

        // 체크포인트 도달 확인
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            AdvanceToNextCheckpoint();
        }
    }

    private void AdvanceToNextCheckpoint()
    {
        currentCheckpointIndex++;
        if (currentCheckpointIndex >= checkpoints.Length)
        {
            // 마지막 체크포인트에 도달하면 이동 정지
            rb.velocity = Vector2.zero;
            enabled = false; // 스크립트 비활성화
            return;
        }
        SetNextCheckpoint();
    }

    private void SetNextCheckpoint()
    {
        if (checkpoints.Length > 0)
        {
            moveDirection = (checkpoints[currentCheckpointIndex].position - transform.position).normalized;
        }
    }

    private IEnumerator IncreaseSpeedOverTime()
    {
        while (true)
        {
            if (!isStopped)
            {
                speed += speedIncreaseRate; // 속도 증가
            }
            yield return new WaitForSeconds(1f); // 1초 대기
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MirrorAttack"))
        {
            StartCoroutine(StopAndResetSpeed());
        }
    }

    private IEnumerator StopAndResetSpeed()
    {
        isStopped = true;
        rb.velocity = Vector2.zero; // 정지
        speed = defaultSpeed; // 속도 초기화
        yield return new WaitForSeconds(3f); // 3초 대기
        isStopped = false;
    }
}
