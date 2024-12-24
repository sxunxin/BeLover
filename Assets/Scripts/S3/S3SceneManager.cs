using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;


public class S3SceneManager : MonoBehaviourPunCallbacks
{
    //플레이어가 스캔한 오브젝트 가져오기 & UI 
    public TextMeshProUGUI talkText;
    public GameObject scanObject;

    public GameObject talkPanel;
    public GameObject clearPanel;
    public TextMeshProUGUI clearUIText; // Clear UI의 텍스트 변경을 위해 추가
    public bool isAction;  //활성화 상태 판단 변수

    public TalkManager tm;
    public S3_1TalkManager talkManager;
    public int talkIndex;

    public static S3SceneManager Instance { get; private set; }

    public int TotalItemCount;

    public GameObject[] m2Portals;
    public GameObject M2_2Portal; // Unity 에디터에서 할당
    public GameObject HousePortalOn;
    public GameObject FinalPortal;

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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 중복 객체 삭제
        }
    }

    void Start()
    {
        StartCoroutine(firstCinema());
        TotalItemCount = GameObject.FindGameObjectsWithTag("candle").Length;
    }
    IEnumerator firstCinema()
    {
        firstPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        firstText.SetMsg(tm.S3Text[0]);

        yield return new WaitForSeconds(13f);

        firstPanel.SetActive(false);
        yield return new WaitForSeconds(2f);
        secondPanel.SetActive(true);

        secondText.SetMsg(tm.S3Text[1]);
        yield return new WaitForSeconds(7f);

        thirdText.SetMsg(tm.S3Text[2]);

        yield return new WaitForSeconds(20f);
        storyPanel.SetActive(false);
    }
    IEnumerator secondCinema()
    {
        yield return new WaitForSeconds(3f);
        bossPanel.SetActive(true);
        bossTalk.SetMsg(tm.S3Text[3]);
        yield return new WaitForSeconds(12f);
        bossPanel.SetActive(false);

        p2Panel.SetActive(true);
        p2Talk.SetMsg(tm.S3Text[4]);
        yield return new WaitForSeconds(5f);
        p2Panel.SetActive(false);

        bossPanel.SetActive(true);
        bossTalk.SetMsg(tm.S3Text[5]);
        yield return new WaitForSeconds(5f);
        bossPanel.SetActive(false);

        p1Panel.SetActive(true);
        p1Talk.SetMsg(tm.S3Text[6]);
        yield return new WaitForSeconds(5f);
        p1Panel.SetActive(false);

        p2Panel.SetActive(true);
        p2Talk.SetMsg(tm.S3Text[7]);
        yield return new WaitForSeconds(5f);
        p2Panel.SetActive(false);

        bossPanel.SetActive(true);
        bossTalk.SetMsg(tm.S3Text[8]);
        yield return new WaitForSeconds(5f);
        bossPanel.SetActive(false);

        p1Panel.SetActive(true);
        p1Talk.SetMsg(tm.S3Text[9]);
        yield return new WaitForSeconds(5f);
        p1Panel.SetActive(false);

        p2Panel.SetActive(true);
        p2Talk.SetMsg(tm.S3Text[10]);
        yield return new WaitForSeconds(5f);
        p2Panel.SetActive(false);

        bossPanel.SetActive(true);
        bossTalk.SetMsg(tm.S3Text[11]);
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
    public void CompareButtonAndRoad()
    {
        string player1Button = "None";
        string player2Road = "None";
        
        // 모든 플레이어의 Custom Properties를 가져옴
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Player1Button"))
            {
                player1Button = player.CustomProperties["Player1Button"].ToString();
            }

            if (player.CustomProperties.ContainsKey("Player2Road"))
            {
                player2Road = player.CustomProperties["Player2Road"].ToString();
            }

        }

        Debug.Log($"서버에서 받은 값: Player1Button={player1Button}, Player2Road={player2Road}");

        // 숫자 추출
        string buttonNumber = player1Button.Replace("Button", "");
        string roadNumber = player2Road.Replace("Road", "");

        GameObject player2 = GameObject.FindWithTag("player2");

        if (player2 != null)
        {
            PhotonView player2PhotonView = player2.GetComponent<PhotonView>();

            if (player2PhotonView != null)
            {
                if (buttonNumber == roadNumber)
                {
                    Debug.Log("정답입니다! Player 2의 속도를 증가시킵니다.");
                    player2PhotonView.RPC("SetSpeedRPC", RpcTarget.All, 1f); // PlayerScript의 photonView 사용
                }
                else
                {
                    Debug.Log("오답입니다! Player 2의 속도를 감소시킵니다.");
                    player2PhotonView.RPC("SetSpeedRPC", RpcTarget.All, 0.5f); // PlayerScript의 photonView 사용
                }
            }
            else
            {
                Debug.LogError("Player2의 PhotonView를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("Player2 오브젝트를 찾을 수 없습니다.");
        }
    }


    //M2 gimmick

    public void Action(GameObject scanObj)
    {
        if (isAction)
        {
            isAction = false;
        }
        else
        {
            isAction = true;
            scanObject = scanObj;
            S3ObjectData objData = scanObj.GetComponent<S3ObjectData>();
            Talk(objData.id, objData.isTomb, objData.SkullTrue);
        }
        talkPanel.SetActive(isAction);
    }

    //s3 m2 
    [PunRPC]
    public void ActivateM2_2Portal()
    {
        if (M2_2Portal != null)
        {
            M2_2Portal.SetActive(true);
            Debug.Log("M2_2Portal이 활성화되었습니다.");
        }
        else
        {
            Debug.LogWarning("M2_2Portal이 할당되지 않았습니다.");
        }
    }
    //퀴즈 출력 하면 될거 같은데
    public void Talk(int id, bool isTomb, bool skullTrue)
    {
        string talkData = talkManager.GetTalk(id, talkIndex);
        if (isTomb)
        {
            talkText.text = talkData;
        }
        else if (skullTrue)
        {
            talkText.text = talkData;
        }
        else
        {
            talkText.text = talkData;
        }
    }
    // 클리어 UI를 화면에 표시하고 2초 후에 사라지게 하는 메서드
    [PunRPC]
    public void ShowClearUI_RPC(int type)
    {
        if (clearPanel != null)
        {
            clearPanel.SetActive(true);

            // **조건에 따라 클리어 UI의 텍스트 변경**
            switch (type)
            {
                case 1:
                    clearUIText.text = "1st clear!";
                    break;
                case 2:
                    clearUIText.text = "2nd clear!";
                    m2Portals[0].SetActive(true);
                    m2Portals[1].SetActive(true);
                    break;
                default:
                    clearUIText.text = "3rd clear!";
                    // FinalPortal 및 House_PortalOn 활성화
                    photonView.RPC("ActivateFinalPortal", RpcTarget.All);
                    break;
            }

            Debug.Log($"Clear UI가 모든 클라이언트에 표시됩니다. 조건: {type}");
            StartCoroutine(HideClearUIAfterDelay(2f));
        }
        else
        {
            Debug.LogWarning("Clear UI 오브젝트가 설정되지 않았습니다.");

        }
    }
    // 2초 후에 클리어 UI를 숨기는 코루틴
    private IEnumerator HideClearUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
            Debug.Log("Clear UI가 사라졌습니다.");
        }
    }
    //s3 m3 
    public void CheckStageClear(int currentCandleCount)
    {
        Debug.Log($"현재 캔들 카운트: {currentCandleCount} / 총 아이템 수: {TotalItemCount}");

        // 만약 모든 캔들에 불이 켜졌다면
        if (currentCandleCount >= TotalItemCount)
        {
            Debug.Log("모든 캔들에 불이 켜졌습니다. 스테이지 클리어!");
            photonView.RPC("ShowClearUI_RPC", RpcTarget.All, 3);
        }
    }
    [PunRPC]
    public void ActivateFinalPortal()
    {
        if (HousePortalOn != null && FinalPortal != null)
        {
            HousePortalOn.SetActive(true);
            FinalPortal.SetActive(true);
            Debug.Log("House_PortalOn과 FinalPortal이 활성화되었습니다.");
        }
        else
        {
            Debug.LogWarning("House_PortalOn 또는 FinalPortal 오브젝트를 찾을 수 없습니다.");
        }
    }

}
