using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOnOff : MonoBehaviour
{
    public int buttonID; // 버튼 고유 ID (1부터 20까지)
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color pressedColor = new Color(0.5f, 0.5f, 0.5f);

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // 원래 색 저장
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player1")) // 플레이어가 밟았을 때
        {
            Debug.Log($"Player stepped on Button {buttonID}");
            spriteRenderer.color = pressedColor;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("player1"))
        {
            spriteRenderer.color = originalColor; // 원래 색으로 복원
        }
    }
}
