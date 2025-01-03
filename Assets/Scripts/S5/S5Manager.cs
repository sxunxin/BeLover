using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class S5Manager : MonoBehaviourPun
{
    private HashSet<string> processedBridges = new HashSet<string>();
    private int correctAnswer;
    private int correctAnswer2;

    [Header("UI Elements")]
    public Image candleImage; // 촛불 이미지
    public Image mirrorImage; // 거울 이미지
    public Image bridgeImage; // 분리 이미지
    public Image mirrorNeedImage; // 거울 엔딩 이미지;
    public Image bridgeNeedImage; // 분리 엔딩 이미지;
    public Image candleNeedImage; // 촛불 엔딩 이미지;

    public TMP_Text candleText;
    public TMP_Text mirrorText;
    public TMP_Text bridgeText;

    public Sprite correctSprite;   // 정답 이미지

    [Header("BridgeGenerator Objects")]
    public GameObject BridgeGenerator0;
    public GameObject BridgeGenerator1;
    public GameObject BridgeGenerator2;
    public GameObject BridgeGenerator3;
    public GameObject BridgeGenerator4;
    public GameObject BridgeGenerator5;
    public GameObject BridgeGenerator6;
    public GameObject BridgeGenerator7;

    [Header("Ending Objects")]
    public GameObject[] endCandles;
    public GameObject mirrorNeed; // 비활성화할 mirrorNeed
    public GameObject ENDMirror; // 활성화할 ENDMirror
    public GameObject bridgeNeed; 
    public GameObject ENDBridge;
    public GameObject EndZone;

    public bool EndCandleCnt = false;
    public bool isEndMirror = false;
    public bool isEndBridge = false;
    public bool canEnd = false;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip candleSound;
    public AudioClip mirrorSound;
    public AudioClip bridgeSound;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트가 정답을 설정
            correctAnswer = Random.Range(0, 3);
            correctAnswer2 = Random.Range(3, 8);
            Debug.Log($"정답은 Bridge{correctAnswer}입니다.");
            Debug.Log($"정답2는 Bridge{correctAnswer2}입니다.");

            // 모든 클라이언트에 정답을 공유
            photonView.RPC("SetCorrectAnswer", RpcTarget.All, correctAnswer, correctAnswer2);
        }
    }

    void Update()
    {
        if (EndCandleCnt && isEndMirror && isEndBridge)
        {
            canEnd = true;
            EndZone.SetActive(true);
            Debug.Log("CAN END!!!");
        }
    }

    public void EndCandle()
    {
        photonView.RPC("EndCandleRPC", RpcTarget.All);
    }

    [PunRPC]
    public void EndCandleRPC()
    {
        EndCandleCnt = true;
    }

    public void ProcessEndMirror()
    {
        // mirrorNeed 비활성화
        if (mirrorNeed != null)
        {
            mirrorNeed.SetActive(false);
            Debug.Log("mirrorNeed 비활성화 완료");
        }
        else
        {
            Debug.LogWarning("mirrorNeed가 설정되지 않았습니다.");
        }

        // ENDMirror 활성화
        if (ENDMirror != null)
        {
            ENDMirror.SetActive(true);
            Debug.Log("ENDMirror 활성화 완료");
        }
        else
        {
            Debug.LogWarning("ENDMirror가 설정되지 않았습니다.");
        }
    }

    private void ChangeCorrectBridgeImage(int correctAnswer)
    {
        GameObject correctBridge = null;

        // 정답 브릿지를 가져옴
        switch (correctAnswer)
        {
            case 0:
                correctBridge = BridgeGenerator0;
                break;
            case 1:
                correctBridge = BridgeGenerator1;
                break;
            case 2:
                correctBridge = BridgeGenerator2;
                break;
            case 3:
                correctBridge = BridgeGenerator3;
                break;
            case 4:
                correctBridge = BridgeGenerator4;
                break;
            case 5:
                correctBridge = BridgeGenerator5;
                break;
            case 6:
                correctBridge = BridgeGenerator6;
                break;
            case 7:
                correctBridge = BridgeGenerator7;
                break;

        }

        if (correctBridge != null)
        {
            SpriteRenderer spriteRenderer = correctBridge.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = correctSprite; // 정답 이미지로 변경
                Debug.Log($"{correctBridge.name}의 이미지가 정답 이미지로 변경되었습니다.");
            }
            else
            {
                Debug.LogWarning($"{correctBridge.name}에 SpriteRenderer가 없습니다.");
            }
        }
    }


    [PunRPC]
    public void SetCorrectAnswer(int answer, int answer2)
    {
        correctAnswer = answer;
        correctAnswer2 = answer2;
        Debug.Log($"클라이언트에서 수신한 정답: Bridge{correctAnswer}");
        Debug.Log($"클라이언트에서 수신한 정답2: Bridge{correctAnswer2}");

        ChangeCorrectBridgeImage(correctAnswer);
        ChangeCorrectBridgeImage(correctAnswer2);
    }

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

    public void ExecuteRPCEndBridge()
    {
        photonView.RPC("ENDBridgeRPC", RpcTarget.All);
    }

    public void ExecuteRPCEndMirror()
    {
        photonView.RPC("ENDMirrorRPC", RpcTarget.All);
    }

    [PunRPC]
    public void ENDBridgeRPC()
    {
        PlayerScript localPlayer = FindLocalPlayer();
        if (localPlayer != null && localPlayer.bridge > 0)
        {
            bridgeNeed.SetActive(false);
            ENDBridge.SetActive(true);
            isEndBridge = true;
            localPlayer.bridge--; // 다리 개수 감소
            UpdateBridgeUI(localPlayer.bridge); // UI 업데이트

            StartCoroutine(DelayedSetIsEndP1ingFalse(localPlayer));

        }
    }

    [PunRPC]
    public void ENDMirrorRPC()
    {
        PlayerScript localPlayer = FindLocalPlayer();
        if (localPlayer != null && localPlayer.bridge > 0)
        {
            mirrorNeed.SetActive(false);
            ENDMirror.SetActive(true);
            isEndMirror = true;
            localPlayer.mirror--; // 다리 개수 감소
            UpdateMirrorUI(localPlayer.mirror); // UI 업데이트

            StartCoroutine(DelayedSetIsEndP1ingFalse(localPlayer));

        }
    }

    private IEnumerator DelayedSetIsEndP1ingFalse(PlayerScript player)
    {
        yield return new WaitForSeconds(1f); // 1초 대기
        player.isEndP1ing = false; // 값 변경
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

        bool isCorrect1 = bridgeName == $"Bridge{correctAnswer}";
        bool isCorrect2 = bridgeName == $"Bridge{correctAnswer2}";
        int bridgeNumber = int.Parse(bridgeName.Replace("Bridge", ""));

        if (bridge != null)
        {
            float moveDistance;
            if (isCorrect1 || isCorrect2)
            {
                moveDistance = (bridgeNumber >= 3) ? -1.6f : -2f; 
            }
            else
            {
                moveDistance = (bridgeNumber >= 3) ? -0.5f : -1.2f;
            }
            StartCoroutine(MoveObject(bridge, moveDistance, isCorrect1 || isCorrect2));
        }
        else
        {
            Debug.Log($"'{bridgeName}' 오브젝트를 찾을 수 없습니다.");
        }

        if (bridgeReal != null)
        {
            float moveDistance;
            if (isCorrect1 || isCorrect2)
            {
                moveDistance = (bridgeNumber >= 3) ? -1.8f : -3f;
            }
            else
            {
                moveDistance = (bridgeNumber >= 3) ? -1.1f : -1.6f;
            }
            StartCoroutine(MoveObject(bridgeReal, moveDistance, false));
        }
        else
        {
            Debug.Log($"'{bridgeName.Replace("Bridge", "BridgeReal")}' 오브젝트를 찾을 수 없습니다.");
        }
    }

    private IEnumerator MoveObject(GameObject targetObject, float distance, bool isCorrect)
    {
        // 이동 시작 지점
        Vector3 startPosition = targetObject.transform.position;

        // 이동 방향은 물체의 로컬 forward 방향을 기준으로 설정
        Vector3 direction = targetObject.transform.up; // 현재 물체의 "위쪽 방향"을 기준
        Vector3 endPosition = startPosition + direction * distance; // 이동 거리만큼 이동

        float duration = 3f; // 이동 시간
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 선형 보간으로 이동
            targetObject.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 이동 완료 후 정확히 도착 지점으로 설정
        targetObject.transform.position = endPosition;

        if (isCorrect)
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
                candleNeedImage.gameObject.SetActive(false);
                break;
            case "candleNeed":
                candleNeedImage.gameObject.SetActive(true);
                candleImage.gameObject.SetActive(false);
                break;
            case "Mirror":
                mirrorImage.gameObject.SetActive(true);
                bridgeImage.gameObject.SetActive(false);
                mirrorNeedImage.gameObject.SetActive(false);
                bridgeNeedImage.gameObject.SetActive(false);
                break;
            case "Bridge":
                bridgeImage.gameObject.SetActive(true);
                mirrorImage.gameObject.SetActive(false);
                mirrorNeedImage.gameObject.SetActive(false);
                bridgeNeedImage.gameObject.SetActive(false);
                break;
            case "mirrorNeed":
                mirrorNeedImage.gameObject.SetActive(true);
                bridgeNeedImage.gameObject.SetActive(false);
                mirrorImage.gameObject.SetActive(false);
                bridgeImage.gameObject.SetActive(false);
                break;
            case "bridgeNeed":
                bridgeNeedImage.gameObject.SetActive(true);
                mirrorNeedImage.gameObject.SetActive(false);
                mirrorImage.gameObject.SetActive(false);
                bridgeImage.gameObject.SetActive(false);
                break;
            default:
                mirrorImage.gameObject.SetActive(false);
                bridgeImage.gameObject.SetActive(false);
                candleImage.gameObject.SetActive(false);
                mirrorNeedImage.gameObject.SetActive(false);
                bridgeNeedImage.gameObject.SetActive(false);
                candleNeedImage.gameObject.SetActive(false);
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
        audioSource.PlayOneShot(candleSound);
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
        audioSource.PlayOneShot(mirrorSound);
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
        audioSource.PlayOneShot(bridgeSound);
        if (bridgeText != null)
        {
            bridgeText.text = bridgeCount.ToString(); 
        }
        else
        {
            Debug.LogWarning("BridgeText가 설정되지 않았습니다.");
        }
    }

    public void ProcessEndCandle(int cnt)
    {
        // ENDCandle 처리
        GameObject targetObject = endCandles[cnt - 1]; // 배열은 0부터 시작하므로 cnt-1
        if (targetObject != null)
        {
            targetObject.SetActive(true); // 오브젝트 활성화
            Debug.Log($"ENDCandle {cnt} activated.");
        }


        // candleNeed 처리
        string candleNeedName = $"candleNeed{cnt}";
        GameObject candleNeedObject = GameObject.Find(candleNeedName);
        if (candleNeedObject != null)
        {
            candleNeedObject.SetActive(false);
            Debug.Log($"Object {candleNeedName} set to inactive.");
        }
        else
        {
            Debug.LogWarning($"Object {candleNeedName} not found!");
        }
    }
}
