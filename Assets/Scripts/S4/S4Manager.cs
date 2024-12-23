using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class S4Manager : MonoBehaviourPunCallbacks
{
    private int currentButtonIndex = 1; // 플레이어가 밟아야 할 버튼의 순서
    public GameObject door;
    public GameObject house1;
    public GameObject goal;
    public GameObject blind;
    public GameObject mapLight;
    public float lightMoveSpeed = 5f;
    public Vector3[] lightPositions;

    private ResetStatue resetStatue;
    private Btn[] Btns;
    private bool isRight = true;


    private void Start()
    {
        // 모든 버튼 스크립트 가져오기
        Btns = FindObjectsOfType<Btn>();
        resetStatue = FindObjectOfType<ResetStatue>();

        mapLight.transform.position = lightPositions[1];
    }

    private void Update()
    {
        // ======================= 마지막에 지울 것 ! =======================
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log($"키보드 0번 키 입력: 자동으로 {currentButtonIndex}번 버튼을 누릅니다.");
            OnButtonPressed(currentButtonIndex); // 현재 눌러야 할 버튼을 누른 것으로 처리
        }
        // ==============================================================
    }

    public int OnButtonPressed(int buttonID)
    {
        if (!isRight)
        {
            return 2;
        }

        if (buttonID == currentButtonIndex) // 올바른 순서로 밟았을 때
        {
            Debug.Log($"Button {buttonID} Correct!");

            currentButtonIndex++;
            UpdateBlindSize();
            photonView.RPC("MoveLightBezier", RpcTarget.All, mapLight.transform.position, lightPositions[currentButtonIndex]);

            if (currentButtonIndex > 20) // 모든 버튼을 밟았을 때
            {
                Debug.Log("Door Opened!");
                photonView.RPC("OpenDoor", RpcTarget.All);
                EnlargeBlind();
            }
            return 1;
        }
        else if (buttonID > currentButtonIndex)
        {
            isRight = false;
            Debug.Log($"Button {buttonID} Incorrect!");
            currentButtonIndex = 0; // 버튼 순서
            UpdateBlindSize();
            photonView.RPC("MoveLightBezier", RpcTarget.All, mapLight.transform.position, lightPositions[currentButtonIndex]);
            return 0;
        }
        return 2;
    }

    private void UpdateBlindSize()
    {
        // Blind의 크기를 (1.2, 1.2)에서 (2.5, 2.5)로 선형 변화
        Vector3 startSize = new Vector3(1.2f, 1.2f, 1.2f);
        Vector3 endSize = new Vector3(2.5f, 2.5f, 2.5f);

        // currentButtonIndex에 따른 보간 인덱스 t (1일 때 0, 19일 때 1)
        float t = (currentButtonIndex - 1) / 18.0f;
        t = Mathf.Clamp01(t);

        // 부드러운 곡선 보간 (Ease-in, Ease-out)
        t = t * t * (3f - 2f * t); // SmoothStep

        // Blind의 크기를 변경
        blind.transform.localScale = Vector3.Lerp(startSize, endSize, t);
    }

    public void ResetButtons()
    {
        Debug.Log("Resetting Buttons!");
        currentButtonIndex = 1;
        isRight = true;
        photonView.RPC("MoveLightBezier", RpcTarget.All, mapLight.transform.position, lightPositions[currentButtonIndex]);
        foreach (Btn button in Btns)
        {
            button.ResetButton();
        }
        resetStatue.TriggerResetEffect();
    }

    [PunRPC]
    public void MoveLightBezier(Vector3 startPosition, Vector3 targetPosition)
    {
        StopAllCoroutines(); // 기존 코루틴 중지
        StartCoroutine(MoveLightBezierCoroutine(mapLight.transform, startPosition, targetPosition));
    }

    private IEnumerator MoveLightBezierCoroutine(Transform lightTransform, Vector3 startPosition, Vector3 targetPosition)
    {
        float t = 0f; // 곡선 진행도 (0~1)
        float maxHeight = 1f; // 곡선의 최대 높이
        Vector3 controlPoint = (startPosition + targetPosition) / 2 + Vector3.up * maxHeight; // 곡선 제어점

        while (t < 1)
        {
            t += Time.deltaTime * lightMoveSpeed; // 진행 속도
            t = Mathf.Clamp01(t); // 0~1 사이로 제한

            // Bezier 곡선 계산
            Vector3 pointOnCurve =
                Mathf.Pow(1 - t, 2) * startPosition +
                2 * (1 - t) * t * controlPoint +
                Mathf.Pow(t, 2) * targetPosition;

            lightTransform.position = pointOnCurve;

            yield return null;
        }

        // 정확히 목표 위치로 설정
        lightTransform.position = targetPosition;
    }


    [PunRPC]
    public void OpenDoor()
    {
        Debug.Log("All players: Door Opened!");

        door.SetActive(false);
        house1.SetActive(true);
        goal.SetActive(true);
    }

    public void EnlargeBlind()
    {
        StopAllCoroutines(); // 모든 기존 코루틴 중지
        StartCoroutine(EnlargeBlindCoroutine()); // Blind 크기 변경 시작
    }

    private IEnumerator EnlargeBlindCoroutine()
    {
        Vector3 startScale = blind.transform.localScale; // 현재 크기
        Vector3 targetScale = new Vector3(6f, 6f, 6f); // 목표 크기
        float duration = 1.5f; // 크기 변화에 걸리는 시간
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration); // 0 ~ 1
            blind.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        // 정확히 목표 크기로 설정
        blind.transform.localScale = targetScale;
    }

}