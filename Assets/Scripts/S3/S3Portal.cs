using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Photon 네임스페이스 추가

public class S3Portal : MonoBehaviourPun
{
    S3SceneManager S3sm;
    public GameObject targetSpawnPoint;
    public GameObject pairedPortal; // 연결된 포탈을 참조합니다.
    public GameObject finalPortal;

    void Awake()
    {
        S3sm = FindObjectOfType<S3SceneManager>();
    }

    public void OnPlayerEnter(GameObject player)
    {
        if (gameObject.name == "FinalPortal")
        {
            Debug.Log("FinalPortal에 플레이어가 진입했습니다.");
            if (PhotonNetwork.IsMasterClient)
            {
                S3sm.photonView.RPC("ShowEndPanel_RPC", RpcTarget.All); // 모든 클라이언트에서 패널 활성화
            }

            return; // FinalPortal은 더 이상 로직이 필요 없음
        }
        if (targetSpawnPoint != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.position = targetSpawnPoint.transform.position;
                Debug.Log($"Player moved to {targetSpawnPoint.transform.position}");
                // M1_2 포탈이 작동하면 M1_1 포탈을 활성화합니다.
                if (gameObject.name == "M1_2Portal" && pairedPortal != null)
                {
                    photonView.RPC("ActivatePairedPortal", RpcTarget.All, pairedPortal.name);
                    Debug.Log("M1_1 포탈이 활성화되었습니다.");
                }
                // M2_2 포탈이 작동하면 M2_1 포탈을 활성화합니다.
                else if (gameObject.name == "M2_2Portal" && pairedPortal != null)
                {
                    photonView.RPC("ActivatePairedPortal", RpcTarget.All, pairedPortal.name);
                    Debug.Log("M2_1 포탈이 활성화되었습니다.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Target spawn point is not assigned!");
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (finalPortal != null)
            {
                Debug.Log("FinalPortal에 플레이어가 진입했습니다.");
                OnPlayerEnter(collision.gameObject);
            }
            else
            {
                if (targetSpawnPoint != null)
                {
                    Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        // 플레이어 이동
                        rb.position = targetSpawnPoint.transform.position;
                        Debug.Log($"Player moved to {targetSpawnPoint.transform.position}");
                        // M1_2 포탈이 작동하면 M1_1 포탈을 활성화합니다.
                        if (gameObject.name == "M1_2Portal" && pairedPortal != null)
                        {
                            photonView.RPC("ActivatePairedPortal", RpcTarget.All, pairedPortal.name);
                            Debug.Log("M1_1 포탈이 활성화되었습니다.");
                        }
                        // M2_2 포탈이 작동하면 M2_1 포탈을 활성화합니다.
                        else if (gameObject.name == "M2_2Portal" && pairedPortal != null)
                        {
                            photonView.RPC("ActivatePairedPortal", RpcTarget.All, pairedPortal.name);
                            Debug.Log("M2_1 포탈이 활성화되었습니다.");
                        }
                    }

                    // 카메라 이동
                    S3Camera cameraFollow = Camera.main.GetComponent<S3Camera>();
                    if (cameraFollow != null)
                    {
                        cameraFollow.SnapToTarget(); // 즉시 이동
                    }
                }
                else
                {
                    Debug.LogWarning("Target spawn point is not assigned!");
                }
            }
        }
    }
    //모든 클라이언트가 실행할 포탈 활성화 메서드
    [PunRPC]
    public void ActivatePairedPortal(string portalName)
    {
        GameObject portalToActivate = pairedPortal;
        if (portalToActivate != null)
        {
            portalToActivate.SetActive(true);
            Debug.Log($"{portalName} 포탈이 활성화되었습니다.");
        }
        else
        {
            Debug.LogWarning($"{portalName} 포탈을 찾을 수 없습니다.");
        }
    }
}
