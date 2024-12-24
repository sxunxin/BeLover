using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI; // **UI 이미지 처리를 위해 추가**
using TMPro;

public class S2SceneManager : MonoBehaviourPun
{
    TalkManager tm;
    public Camera mainCamera; // 연결된 카메라 (Camera.main 대신 사용)
    public TextMeshProUGUI warningText; // **TMP 경고 메시지 텍스트 연결**
    public Image displayImage; // **표시할 이미지 연결**
    public Sprite[] imageList; // **이미지 리스트 (5개 스프라이트 입력)**
    public GameObject prefabToShow; // **활성화할 Prefab**
    public int mirrorCount = 0; // 미러 카운트
    private int previousMirrorCount = 0; // 이전 미러 카운트를 저장
    private bool isShaking = false; // 흔들림 중인지 여부 확인

    public GameObject storyPanel;
    public GameObject firstPanel;
    public GameObject secondPanel;

    public TypeEffect firstText;
    public TypeEffect secondText;
    public TypeEffect thirdText;

    public GameObject endPanel;
    public GameObject p1Panel;
    public GameObject p2Panel;
    public GameObject bossPanel;

    public TypeEffect p1Talk;
    public TypeEffect p2Talk;
    public TypeEffect bossTalk;

    public Image ghostImage;
    public Image Buddhahood;


    private void Awake()
    {
        tm = FindObjectOfType<TalkManager>();
    }
    void Start()
    {
        if (mainCamera == null)
        {
            Debug.LogError("[S2SceneManager] mainCamera가 연결되지 않았습니다. 카메라를 연결하세요.");
        }

        if (warningText == null)
        {
            Debug.LogError("[S2SceneManager] warningText가 연결되지 않았습니다. TMP 오브젝트를 연결하세요.");
        }
        else
        {
            warningText.gameObject.SetActive(false); // 시작할 때 경고 메시지는 숨김
        }

        if (displayImage == null)
        {
            Debug.LogError("[S2SceneManager] displayImage가 연결되지 않았습니다. UI Image 오브젝트를 연결하세요.");
        }
        else
        {
            displayImage.preserveAspect = false; // **Preserve Aspect 비활성화**
        }

        if (imageList == null || imageList.Length == 0)
        {
            Debug.LogError("[S2SceneManager] imageList에 이미지가 설정되지 않았습니다.");
        }

        if (prefabToShow == null)
        {
            Debug.LogError("[S2SceneManager] prefabToShow가 연결되지 않았습니다. Prefab 오브젝트를 연결하세요.");
        }
        else
        {
            prefabToShow.SetActive(false); // **Prefab을 비활성화된 상태로 시작**
        }

        if (imageList.Length > 0)
        {
            displayImage.sprite = imageList[0];
            displayImage.rectTransform.sizeDelta = new Vector2(160, 180); // **이미지 크기 고정**
        }
        StartCoroutine(firstCinema());
    }

    void Update()
    {
        if (mirrorCount > previousMirrorCount)
        {
            Debug.Log($"[S2SceneManager] mirrorCount가 {previousMirrorCount}에서 {mirrorCount}로 증가했습니다.");

            // **mirrorCount가 5가 아닐 때만 카메라 흔들림 발생**
            if (mirrorCount != 5)
            {
                StartCoroutine(CameraShake(3f));
            }

            StartCoroutine(ShowWarningMessage(warningText, "플레이어의 방향키가 랜덤으로 재설정됩니다", Color.red));
            ChangeImage(mirrorCount);

            if (mirrorCount == 5)
            {
                // **Prefab 활성화 및 카메라 이동**
                StartCoroutine(ShowPrefabAndSwitchCamera());
            }

            previousMirrorCount = mirrorCount;
        }
    }

    IEnumerator CameraShake(float duration)
    {
        if (isShaking) yield break;
        isShaking = true;

        if (mainCamera == null)
        {
            Debug.LogError("[S2SceneManager] mainCamera가 연결되지 않았습니다. 흔들림을 중단합니다.");
            yield break;
        }

        float elapsed = 0f;
        Matrix4x4 originalProjectionMat = mainCamera.projectionMatrix;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Matrix4x4 pMat = originalProjectionMat;

            pMat.m01 += Mathf.Sin(Time.time * 25f) * 0.2f;
            pMat.m10 += Mathf.Sin(Time.time * 30f) * 0.2f;

            pMat.m00 += Mathf.Sin(Time.time * 20f) * 0.2f;
            pMat.m11 += Mathf.Sin(Time.time * 20f) * 0.2f;

            mainCamera.projectionMatrix = pMat;

            yield return null;
        }

        mainCamera.projectionMatrix = originalProjectionMat;
        isShaking = false;
    }

    IEnumerator ShowWarningMessage(TextMeshProUGUI text, string message, Color color)
    {
        if (warningText == null)
        {
            Debug.LogError("[S2SceneManager] warningText가 연결되지 않았습니다.");
            yield break;
        }

        text.text = message;
        text.color = color;

        for (int i = 0; i < 3; i++)
        {
            warningText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            warningText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }

        warningText.gameObject.SetActive(false);
    }

    void ChangeImage(int index)
    {
        if (imageList == null || imageList.Length == 0) return;

        if (index < imageList.Length)
        {
            displayImage.sprite = imageList[index];
            displayImage.rectTransform.sizeDelta = new Vector2(160, 180);
        }
    }

    IEnumerator ShowPrefabAndSwitchCamera()
    {
        if (prefabToShow == null || mainCamera == null)
        {
            Debug.LogError("[S2SceneManager] prefabToShow 또는 mainCamera가 연결되지 않았습니다.");
            yield break;
        }

        Debug.Log("[S2SceneManager] Prefab 활성화 및 카메라 전환");

        // **Prefab 활성화**
        prefabToShow.SetActive(true);

        // **현재 카메라의 위치와 회전 저장**
        Vector3 originalCameraPosition = mainCamera.transform.position;
        Quaternion originalCameraRotation = mainCamera.transform.rotation;

        // **Prefab 위치로 카메라 이동**
        Vector3 targetPosition = prefabToShow.transform.position + new Vector3(0, 0, -10);
        float transitionTime = 2f; // **카메라 전환에 걸리는 시간**
        float elapsedTime = 0f;

        // **카메라를 Prefab 위치로 이동 (2초 동안 부드럽게 전환)**
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            mainCamera.transform.position = Vector3.Lerp(originalCameraPosition, targetPosition, elapsedTime / transitionTime);
            mainCamera.transform.rotation = Quaternion.Slerp(originalCameraRotation, Quaternion.LookRotation(prefabToShow.transform.position - mainCamera.transform.position), elapsedTime / transitionTime);
            yield return null;
        }

        // **Prefab을 비춘 후 원래 위치로 복귀**
        elapsedTime = -2f;
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            mainCamera.transform.position = Vector3.Lerp(targetPosition, originalCameraPosition, elapsedTime / transitionTime);
            mainCamera.transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(prefabToShow.transform.position - mainCamera.transform.position), originalCameraRotation, elapsedTime / transitionTime);
            yield return null;
        }

        Debug.Log("[S2SceneManager] Prefab 비활성화 및 카메라 복원 완료");
    }
    IEnumerator firstCinema()
    {
        firstPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        firstText.SetMsg(tm.S2Text[0]);

        yield return new WaitForSeconds(12f);

        firstPanel.SetActive(false);
        yield return new WaitForSeconds(2f);
        secondPanel.SetActive(true);

        secondText.SetMsg(tm.S2Text[1]);
        yield return new WaitForSeconds(7f);

        thirdText.SetMsg(tm.S2Text[2]);

        yield return new WaitForSeconds(20f);
        storyPanel.SetActive(false);
    }
    IEnumerator secondCinema()
    {
        yield return new WaitForSeconds(3f);
        p1Panel.SetActive(true);
        p1Talk.SetMsg(tm.S2Text[3]);
        yield return new WaitForSeconds(5f);
        p1Panel.SetActive(false);

        bossPanel.SetActive(true);
        bossTalk.SetMsg(tm.S2Text[4]);
        yield return new WaitForSeconds(5f);
        bossPanel.SetActive(false);

        p2Panel.SetActive(true);
        p2Talk.SetMsg(tm.S2Text[5]);
        yield return new WaitForSeconds(5f);
        p2Panel.SetActive(false);

        bossPanel.SetActive(true);
        bossTalk.SetMsg(tm.S2Text[6]);
        yield return new WaitForSeconds(5f);
        bossPanel.SetActive(false);

        p2Panel.SetActive(true);
        p2Talk.SetMsg(tm.S2Text[7]);
        yield return new WaitForSeconds(5f);
        p2Panel.SetActive(false);

        p1Panel.SetActive(true);
        p1Talk.SetMsg(tm.S2Text[8]);
        yield return new WaitForSeconds(5f);
        p1Panel.SetActive(false);

        bossPanel.SetActive(true);
        bossTalk.SetMsg(tm.S2Text[9]);
        yield return new WaitForSeconds(5f);
        bossPanel.SetActive(false);

        p2Panel.SetActive(true);
        p2Talk.SetMsg(tm.S2Text[10]);
        yield return new WaitForSeconds(5f);
        p2Panel.SetActive(false);

        bossPanel.SetActive(true);
        bossTalk.SetMsg(tm.S2Text[11]);
        yield return new WaitForSeconds(5f);

        bossTalk.SetMsg(tm.S2Text[12]);
        yield return new WaitForSeconds(15f);

        bossPanel.SetActive(false);

        yield return new WaitForSeconds(3f);
        ghostImage.gameObject.SetActive(true); // ghostImage 활성화
        yield return new WaitForSeconds(3f); // 3초 대기 후 크기와 투명도 변경 시작

        StartCoroutine(tm.ImagePadeOut(ghostImage, Buddhahood));
    }

    [PunRPC]
    public void ShowEndPanel_RPC()
    {
        if (endPanel != null)
        {
            endPanel.SetActive(true);
            StartCoroutine(secondCinema());
        }
        else
        {
            Debug.LogError("endPanel이 연결되지 않았습니다.");
        }
    }
}
