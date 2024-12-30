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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Player2 ??? ?? ????? ?? ?? ??
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
}
