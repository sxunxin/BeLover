using System.Collections;
using UnityEngine;
using Photon.Pun;

public class EnemyAI : MonoBehaviourPun
{
    public float speed = 0.8f; // ?? ??
    public float speedIncreaseRate = 0.05f; // 1??? ??? ??
    public float defaultSpeed = 0.8f; // ??? ? ??
    private bool isStopped = false; // ?? ??
    private Rigidbody2D rb; // Rigidbody2D ????
    private Vector2 moveDirection; // ?? ??
    private Transform targetPlayer; // ?? ????
    private SpriteRenderer spriteRenderer;

    public bool isEnd = false;
    public GameObject EndHouse;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Player2 ??? ?? ????? ?? ?? ?
        GameObject playerObject = GameObject.FindWithTag("player2");
        if (playerObject != null)
        {
            targetPlayer = playerObject.transform;
        }

        StartCoroutine(IncreaseSpeedOverTime());
    }

    void Update()
    {
        if (isStopped || targetPlayer == null) return;

        // Player2? ??? ??? ??
        Vector2 targetPosition = targetPlayer.position;
        moveDirection = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = moveDirection * speed;

        // Player2? ????? ?? ??
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private IEnumerator IncreaseSpeedOverTime()
    {
        while (true)
        {
            if (!isStopped)
            {
                speed += speedIncreaseRate; // ?? ??
            }
            yield return new WaitForSeconds(1f); // 1? ??
        }
    }

    [PunRPC]
    public void TriggerStopAndResetSpeed()
    {
        StartCoroutine(StopAndResetSpeed());

        GameObject mirrorEffectManagerObject = GameObject.Find("MirrorEffectManager");
        MirrorEffectManager mirrorEffectManager = mirrorEffectManagerObject.GetComponent<MirrorEffectManager>();
        mirrorEffectManager.TriggerMirrorEffect();
    }

    private IEnumerator StopAndResetSpeed()
    {
        isStopped = true;
        rb.velocity = Vector2.zero; // ?? 0?? ??
        speed = defaultSpeed; // ?? ???
        yield return new WaitForSeconds(4f);
        isStopped = false;
    }

    [PunRPC]
    public void TriggerStopAndMoveToPosition()
    {
        StartCoroutine(MoveToPosition(new Vector3(-37.17f, -17.16f, 0f), 3f));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        isStopped = true; // 이동 중지
        rb.velocity = Vector2.zero; // 현재 속도 0으로 설정

        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        // 부드럽게 목표 위치로 이동
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 정확히 목표 위치에 도달
        transform.position = targetPosition;

        // 자식 오브젝트 비활성화 및 투명화 효과 시작
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        StartCoroutine(FadeAndDeactivate());
    }

    private IEnumerator FadeAndDeactivate()
    {
        float duration = 5f; // 5초 동안 투명하게
        float elapsedTime = 0f;

        // Gradually fade out
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            // Set sprite transparency
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }

            yield return null;
        }

        // 완전히 비활성화
        isEnd = true;
        EndHouse.SetActive(true);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MainScene");
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EndTrigger"))
        {
            photonView.RPC("TriggerStopAndMoveToPosition", RpcTarget.All);
        }
    }
}
