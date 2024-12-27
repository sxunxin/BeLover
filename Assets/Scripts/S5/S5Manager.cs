using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class S5Manager : MonoBehaviourPun
{
    private HashSet<string> processedBridges = new HashSet<string>();

    [Header("UI Elements")]
    public Image candleImage; // 촛불 이미지
    public Image mirrorImage; // 거울 이미지
    public Image bridgeImage; // 분리 이미지

    public TMP_Text candleText;
    public TMP_Text mirrorText;
    public TMP_Text bridgeText;

    public void ExecuteRPC(string bridgeName)
    {
        if (processedBridges.Contains(bridgeName))
        {
            Debug.Log($"'{bridgeName}'은 이미 처리되었습니다.");
            return;
        }

        processedBridges.Add(bridgeName);

        // PunRPC 실행
        photonView.RPC("BridgeGeneratorRPC", RpcTarget.All, bridgeName);
    }

    [PunRPC]
    public void BridgeGeneratorRPC(string bridgeName)
    {
        Debug.Log($"BridgeGeneratorRPC 호출됨. 이름: {bridgeName}");

        PlayerScript localPlayer = FindLocalPlayer();
        if (localPlayer != null && localPlayer.bridge > 0)
        {
            localPlayer.bridge--; // 다리 개수 감소
            UpdateBridgeUI(localPlayer.bridge); // UI 업데이트
            Debug.Log($"다리 사용: 남은 다리 수 {localPlayer.bridge}");
        }

        // BridgeX와 BridgeRealX 오브젝트 찾기
        GameObject bridge = GameObject.Find(bridgeName);
        GameObject bridgeReal = GameObject.Find(bridgeName.Replace("Bridge", "BridgeReal"));

        if (bridge != null)
        {
            StartCoroutine(MoveObject(bridge, new Vector3(0, -2f, 0))); // -y 방향으로 이동
        }
        else
        {
            Debug.Log($"'{bridgeName}' 오브젝트를 찾을 수 없습니다.");
        }

        if (bridgeReal != null)
        {
            StartCoroutine(MoveObject(bridgeReal, new Vector3(0, -2f, 0))); // x 방향으로 이동
        }
        else
        {
            Debug.Log($"'{bridgeName.Replace("Bridge", "BridgeReal")}' 오브젝트를 찾을 수 없습니다.");
        }
    }

    private IEnumerator MoveObject(GameObject targetObject, Vector3 offset)
    {
        // 이동 시작과 끝 위치 설정
        Vector3 startPosition = targetObject.transform.position;
        Vector3 endPosition = startPosition + offset;
        float duration = 3f; // 이동 시간
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            targetObject.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 이동 완료 후 정확히 도착 지점으로 설정
        targetObject.transform.position = endPosition;

        if (targetObject.name.StartsWith("Bridge") && !targetObject.name.Contains("Real"))
        {
            targetObject.SetActive(false);
        }

    }

    public void ShowImage(string imageType)
    {
        // 지정된 이미지를 활성화
        switch (imageType)
        {
            case "Candle":
                candleImage.gameObject.SetActive(true);
                break;
            case "Mirror":
                mirrorImage.gameObject.SetActive(true);
                bridgeImage.gameObject.SetActive(false);
                break;
            case "Bridge":
                bridgeImage.gameObject.SetActive(true);
                mirrorImage.gameObject.SetActive(false);
                break;
            default:
                mirrorImage.gameObject.SetActive(false);
                bridgeImage.gameObject.SetActive(false);
                candleImage.gameObject.SetActive(false);
                break;
        }
    }

    private PlayerScript FindLocalPlayer()
    {
        foreach (var player in FindObjectsOfType<PlayerScript>())
        {
            if (player.photonView.IsMine)
            {
                return player;
            }
        }
        Debug.LogError("로컬 플레이어를 찾을 수 없습니다.");
        return null;
    }

    public void UpdateCandleUI(int candleCount)
    {
        if (candleText != null)
        {
            candleText.text = candleCount.ToString(); 
        }
        else
        {
            Debug.LogWarning("CandleText가 설정되지 않았습니다.");
        }
    }

    public void UpdateMirrorUI(int mirrorCount)
    {
        if (mirrorText != null)
        {
            mirrorText.text = mirrorCount.ToString(); 
        }
        else
        {
            Debug.LogWarning("MirrorText 설정되지 않았습니다.");
        }
    }

    public void UpdateBridgeUI(int bridgeCount)
    {
        if (bridgeText != null)
        {
            bridgeText.text = bridgeCount.ToString(); 
        }
        else
        {
            Debug.LogWarning("BridgeText가 설정되지 않았습니다.");
        }
    }
}
