using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class MobileManager : MonoBehaviour
{
    private Canvas cv; // 상위 Canvas를 자동으로 가져옴
    private RectTransform cvRectTransform; // Canvas의 RectTransform
    private RectTransform thisRectTransform; // MobileManager 오브젝트의 RectTransform

    private PlayerScript localPlayerScript; // 로컬 플레이어 스크립트
    private string currentSceneName;

    void Start()
    {
        // 상위 Canvas 가져오기
        cv = GetComponentInParent<Canvas>();
        if (cv != null)
        {
            cvRectTransform = cv.GetComponent<RectTransform>();
            thisRectTransform = GetComponent<RectTransform>();

            if (cvRectTransform != null && thisRectTransform != null)
            {
                SyncRectTransform();
            }
            else
            {
                Debug.LogWarning("RectTransform을 가져오지 못했습니다.");
            }
        }
        else
        {
            Debug.LogWarning("상위 Canvas를 찾을 수 없습니다.");
        }

        // 로컬 플레이어 오브젝트 찾기
        foreach (var player in FindObjectsOfType<PlayerScript>())
        {
            if (player.photonView.IsMine)
            {
                localPlayerScript = player;
                Debug.Log("로컬 플레이어 스크립트 연결 성공!");
                break;
            }
        }

        currentSceneName = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        // 앵커를 항상 중앙으로 고정 (Anchor Preset의 Center와 동일)
        if (thisRectTransform != null)
        {
            thisRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            thisRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            thisRectTransform.pivot = new Vector2(0.5f, 0.5f);
            thisRectTransform.anchoredPosition = Vector2.zero; // 중앙 위치 유지
        }
    }

    private void SyncRectTransform()
    {
        // 크기 및 피벗 속성을 동기화
        thisRectTransform.sizeDelta = cvRectTransform.sizeDelta;
        thisRectTransform.pivot = cvRectTransform.pivot;
        thisRectTransform.anchoredPosition = cvRectTransform.anchoredPosition;

        Debug.Log("RectTransform이 Canvas와 동기화되었습니다.");
    }

    public void ButtonDown(string type)
    {
        if (localPlayerScript != null)
        {
            localPlayerScript.ButtonDown(type, currentSceneName); // PlayerScript의 ButtonDown 호출
        }
        else
        {
            Debug.LogWarning("로컬 플레이어 스크립트가 연결되지 않았습니다.");
        }
    }

    public void ButtonUp(string type)
    {
        if (localPlayerScript != null)
        {
            localPlayerScript.ButtonUp(type, currentSceneName); // PlayerScript의 ButtonUp 호출
        }
        else
        {
            Debug.LogWarning("로컬 플레이어 스크립트가 연결되지 않았습니다.");
        }
    }

    public void InteractionButtonDown()
    {
        if (localPlayerScript != null)
        {
            localPlayerScript.SetInteractState(true, currentSceneName); // isInteract를 1로 설정
        }
        else
        {
            Debug.LogWarning("로컬 플레이어 스크립트가 연결되지 않았습니다.");
        }
    }

    public void InteractionButtonUp()
    {
        if (localPlayerScript != null)
        {
            localPlayerScript.SetInteractState(false, currentSceneName); // isInteract를 0으로 설정
        }
        else
        {
            Debug.LogWarning("로컬 플레이어 스크립트가 연결되지 않았습니다.");
        }
    }
}
