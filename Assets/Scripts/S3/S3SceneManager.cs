using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class S3SceneManager : MonoBehaviour
{
    //플레이어가 스캔한 오브젝트 가져오기 & UI 
    public TextMeshProUGUI talkText;
    public GameObject scanObject;

    public GameObject talkPanel;
    public bool isAction; //활성화 상태 판단 변수

    public S3_1TalkManager talkManager;
    public int talkIndex;

    public static S3SceneManager Instance { get; private set; }


    public List<int> selectedOrder = new List<int>(); // 선택된 묘비 순서
    private int[] correctOrder = { 1002, 1005, 1004, 1001, 1003 }; // 정답 순서

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 정답 매핑 테이블
    private readonly Dictionary<string, string> correctPairs = new Dictionary<string, string>
    {
        { "Button1", "Road1" },
        { "Button2", "Road2" },
        { "Button3", "Road3" }
    };

    public static string p1ObjectName = "None";
    public static string p2ObjectName = "None";

    public void SetP1ObjectName(string objectName)
    {
        p1ObjectName = objectName;
        CheckMatch();
    }

    public void SetP2ObjectName(string objectName)
    {
        p2ObjectName = objectName;
        CheckMatch();
    }

    private void CheckMatch()
    {
        // Button과 Road 이름이 모두 설정된 경우
        if (p1ObjectName != "None" && p2ObjectName != "None")
        {
            // 매핑 테이블에서 정답 확인
            if (correctPairs.TryGetValue(p1ObjectName, out string correctRoad) && correctRoad == p2ObjectName)
            {
                Debug.Log($"정답! {p1ObjectName} ↔ {p2ObjectName} 매칭 성공!");
                OnCorrectMatch();
            }
            else
            {
                Debug.Log($"오답! {p1ObjectName} ↔ {p2ObjectName} 매칭 실패!");
                OnIncorrectMatch();
            }

            // 매칭 이름 초기화
            p1ObjectName = "None";
            p2ObjectName = "None";
        }
    }

    private void OnCorrectMatch()
    {
        Debug.Log("정답에 대한 보상을 줍니다. 속도가 증가합니다.");
        PlayerScript[] players = FindObjectsOfType<PlayerScript>();
        foreach (var player in players)
        {
            if (player.CompareTag("player2")) // Player 2만 속도 변경
            {
                player.SetSpeed(1f);
                Debug.Log("Player 2의 속도가 2로 설정되었습니다.");
            }
        }
    }

    private void OnIncorrectMatch()
    {
        Debug.Log("오답에 대한 페널티를 줍니다. 속도가 감소합니다.");
        PlayerScript[] players = FindObjectsOfType<PlayerScript>();
        foreach (var player in players)
        {
            if (player.CompareTag("player2")) // Player 2만 속도 변경
            {
                player.SetSpeed(0.5f);
                Debug.Log("Player 2의 속도가 0.5로 설정되었습니다.");
            }
        }
    }


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
            Talk(objData.id, objData.isTomb);


            // 묘비인 경우 선택된 순서에 추가
            if (objData.isTomb)
            {
                selectedOrder.Add(objData.id);

                // 선택된 순서 검사
                CheckOrder();
            }
        }

        talkPanel.SetActive(isAction);
    }

    void CheckOrder()
    {
        //정답은 2번 > 5번 > 4번 > 1번 > 3번
        // 순서가 틀렸다면 초기화
        if (!IsCorrectSequence())
        {
            Debug.Log("잘못된 순서입니다! 다시 시도하세요.");
            selectedOrder.Clear();
        }
        else if (selectedOrder.Count == correctOrder.Length)
        {
            Debug.Log("퀘스트 완료! 문이 열립니다.");
            // 여기에서 다음 스텝(예: 문 열기)을 호출
        }
    }

    bool IsCorrectSequence()
    {
        for (int i = 0; i < selectedOrder.Count; i++)
        {
            if (selectedOrder[i] != correctOrder[i])
            {
                return false;
            }
        }
        return true;
    }

    void Talk(int id, bool isTomb)
    {
        string talkData = talkManager.GetTalk(id, talkIndex);
        if (talkData == null)
        {
            talkIndex = 0;
            isAction = false;
            return;
        }
        talkText.text = talkData;

    }
}
