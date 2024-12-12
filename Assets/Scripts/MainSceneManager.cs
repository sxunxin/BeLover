using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    GameManager gm;
    NetworkManager nm;
    public GameObject storyPanel;
    public Button StartBtn;

    public GameObject[] mainMission;

    void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        nm = FindObjectOfType<NetworkManager>();

        // 게임 재실행 시 항상 storyPanel을 활성화
        storyPanel.SetActive(true);

        // 필요하면 PlayerPrefs 초기화
        PlayerPrefs.SetInt("StoryPanelHidden", 0);
    }
    void Update()
    {
        if(gm.mainSceneEnterCount >= 2)
        {
            storyPanel.SetActive(false);
            if(gm.mainSceneEnterCount == 2 && gm.isMission1Clear == true && gm.isMission2Clear == false && gm.isMission3Clear == false)
            {
                mainMission[0].tag = "MainMission";
            }
            else
                mainMission[0].tag = "Untagged";

            if (gm.mainSceneEnterCount == 3 && gm.isMission1Clear == true && gm.isMission2Clear == true && gm.isMission3Clear == false)
            {
                mainMission[1].tag = "MainMission";
            }
            else
                mainMission[1].tag = "Untagged";

            if(gm.mainSceneEnterCount == 4 && gm.isMission1Clear == true && gm.isMission2Clear == true && gm.isMission3Clear == true)
            {
                mainMission[2].tag = "MainMission";
            }
            else
                mainMission[2].tag = "Untagged";
        }
    }
    public void SpawnChar()
    {
        StartBtn.gameObject.SetActive(false);

        // storyPanel 숨김 상태 저장
        PlayerPrefs.SetInt("StoryPanelHidden", 1);

        // Spawn 전에 ViewID 관리
        nm.Spawn();

        // 버튼을 누른 플레이어 정보 업데이트
        UpdatePlayerReadyStatus();

        // 두 플레이어가 준비 상태인지 확인
        StartCoroutine(CheckBothPlayersReady());
    }

    void UpdatePlayerReadyStatus()
    {
        // 현재 플레이어가 준비 상태임을 설정
        var props = new ExitGames.Client.Photon.Hashtable
        {
            { "IsReady", true }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    IEnumerator CheckBothPlayersReady()
    {
        while (true)
        {
            // 모든 플레이어의 CustomProperties 확인
            bool allReady = true;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("IsReady"))
                {
                    allReady &= (bool)player.CustomProperties["IsReady"];
                }
                else
                {
                    allReady = false;
                }
            }

            // 두 플레이어가 모두 준비 상태면 Scene1으로 이동
            if (allReady)
            {
                yield return new WaitForSeconds(3f); // 3초 대기
                nm.StartScene1(); // Scene1으로 이동
                yield break; // Coroutine 종료
            }

            yield return null; // 다음 프레임까지 대기
        }
    }
}
