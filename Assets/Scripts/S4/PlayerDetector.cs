using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerDetector : MonoBehaviourPunCallbacks
{
    private GameObject childObject;

    void Start()
    {
        // 자신의 자식 오브젝트를 가져오기 (무조건 하나라고 가정)
        if (transform.childCount > 0)
        {
            childObject = transform.GetChild(0).gameObject;
            childObject.SetActive(false);
        }
        else
        {
            Debug.LogError("PlayerDetector: 자식 오브젝트가 없습니다.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.LogError("접촉하여 위치가 노출됩니다.");
        if (other.CompareTag("player1")) // Player1 태그와 충돌 확인
        {
            childObject.SetActive(true);
            //photonView.RPC("SetChildActive", RpcTarget.All, true); // 모든 클라이언트에서 활성화
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player1")) // Player1 태그가 나갔을 때 확인
        {
            childObject.SetActive(false);
            //photonView.RPC("SetChildActive", RpcTarget.All, false); // 모든 클라이언트에서 비활성화
        }
    }

    [PunRPC]
    void SetChildActive(bool isActive)
    {
        if (childObject != null)
        {
            childObject.SetActive(isActive);
        }
    }
}