using UnityEngine;
using Photon.Pun;

public class MobileManager : MonoBehaviour
{
    private PlayerScript localPlayerScript; // 로컬 플레이어 스크립트

    void Start()
    {
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

        if (localPlayerScript == null)
        {
            Debug.LogError("로컬 플레이어 스크립트를 찾을 수 없습니다.");
        }
    }

    public void ButtonDown(string type)
    {
        if (localPlayerScript != null)
        {
            localPlayerScript.ButtonDown(type); // PlayerScript의 ButtonDown 호출
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
            localPlayerScript.ButtonUp(type); // PlayerScript의 ButtonUp 호출
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
            localPlayerScript.SetInteractState(true); // isInteract를 1로 설정
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
            localPlayerScript.SetInteractState(false); // isInteract를 0으로 설정
        }
        else
        {
            Debug.LogWarning("로컬 플레이어 스크립트가 연결되지 않았습니다.");
        }
    }
}
